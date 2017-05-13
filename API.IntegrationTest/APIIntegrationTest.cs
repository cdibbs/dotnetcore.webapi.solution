using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;

namespace CorTrack.IntegrationTest
{
    [TestClass]
    [DeploymentItem("EntityFramework.SqlServer.dll")]
    [DeploymentItem("Snapshots/Database.dacpac", "./")]
    public class APIIntegrationTest
    {
        public IntegTestDBDeployer testDeploy { get; set; }
        public DateTime Today { get; set; } = new DateTime(2015, 3, 14);
        public static IKernel Kernel { get; set; }
        public static IMapper Mapper { get; set; }
        public static string myHawkId = "myhawkid";

        [TestInitialize]
        public void TestSetup()
        {
            // The unfortunate part is that we also rely on MAUI, which is Oracle. We cannot (yet)
            // do this for Oracle DBs.
            var myRoles = new[] {"Admin", "ValidUser"};
            var userObj = Mock.Of<IPrincipal>(ip => ip.Identity == setupMockId());
            var mailUtilityObj = Mock.Of<IMailUtility>();

            Mock.Get(userObj)
                .Setup(u => u.IsInRole(It.Is<string>(r => myRoles.Contains(r))))
                .Returns(true);
            Mock.Get(userObj)
                .Setup(u => u.IsInRole(It.Is<string>(r => ! myRoles.Contains(r))))
                .Returns(false);
            Kernel = new StandardKernel(new ApiModule("API Integration Test")
            {
                RequestIP = "127.0.0.1"
            });

            Kernel.Rebind<IMailUtility>().ToConstant(mailUtilityObj);
            Kernel.Rebind<IPrincipal>().ToConstant(userObj);
            Mapper = Kernel.Get<IMapper>();
            testDeploy = new IntegTestDBDeployer()
            {
                DacFilesPath = @"Database.dacpac",
                DbName = "FakeDB_API_E2E",
                DeploymentConnStr = ConfigurationManager.ConnectionStrings["WritableDataContext"].ToString(),
                MasterConnStr = ConfigurationManager.ConnectionStrings["MasterDataContext"].ToString(),
            };
            testDeploy.CheckConnectionStringsAllSafe();
            testDeploy.DropTestDatabaseAndConnectionsIfExists();
            testDeploy.DeployTestDatabase();
            initializeFakeData();
        }

        [TestCleanup]
        public void TestTeardown()
        {
            // Might as well leave the DB in place. It drops on redeploy, anyway. :-)
        }

        protected static ClaimsIdentity setupMockId()
        {
            var claim = new Claim("test", myHawkId);
            var mockIdentity =
                Mock.Of<ClaimsIdentity>(ci => ci.FindFirst(It.IsAny<string>()) == claim);
            Mock.Get(mockIdentity).Setup(i => i.IsAuthenticated).Returns(true);
            Mock.Get(mockIdentity).Setup(i => i.Name).Returns(myHawkId);
            return mockIdentity;
        }

        private class TestStartup : Startup
        {
            public override void Configuration(IAppBuilder app)
            {
                app.Use(async (ctx, next) =>
                {
                    ctx.Request.User = Mock.Of<IPrincipal>(ip => ip.Identity == setupMockId());
                    await next();
                });
                base.Configuration(app);
            }

            public override IKernel CreateKernel()
            {
                // Use ours, instead.
                return Kernel;
            }

            public override void SetupAppCycleLogging(IAppBuilder app)
            {
                // No.
            }

            public override void HelpPages(IAppBuilder app, HttpConfiguration config)
            {
                // Do nothing. Don't bind help pages.
            }

            public override void ConfigureAuth(IAppBuilder app)
            {
                // Nada
            }

            public override void ConfigErrorHandling(HttpConfiguration httpconfig)
            {
                httpconfig.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void EndpointsBasicallyWork()
        {
            using (var server = TestServer.Create<TestStartup>())
            {
                using (var client = new HttpClient(server.Handler))
                {
                    UserEndpoint(client);
                    DnaEndpoint(client);
                    UserRoleEndpoint(client); // auth??
                }
            }
        }

        public void DnaEndpoint(HttpClient client)
        {
            var endpointBase = "http://testserver/idw";
            var users = EndpointFetch<IdwViewModel[]>(client, $"{endpointBase}?hawkid=noone&Type=2");
            Assert.IsTrue(users == null || ! users.Any()); 

            users = EndpointFetch<IdwViewModel[]>(client, $"{endpointBase}?hawkid=jbond&Type=2");
            Assert.AreEqual("jbond", users.First().HawkId);

            users = EndpointFetch<IdwViewModel[]>(client, $"{endpointBase}?page=0&pagesize=1");
            Assert.AreEqual("jbnemesis", users.First().HawkId);
        }

        public void UserEndpoint(HttpClient client)
        {
            var endpointBase = "http://testserver/user";
            var uvm = new UserInputModel()
            {
                HawkId = "one" //duplicate - should fail.
            };
            var response = EndpointPostFail(client, endpointBase, uvm);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            uvm.HawkId = "four";
            EndpointPost(client, endpointBase, uvm);

            var user = EndpointFetch<User>(client, $"{endpointBase}?id=2");
            Assert.IsNull(user); // IsDeleted

            var users = EndpointFetch<User[]>(client, $"{endpointBase}?filter.Type=2&filter.HawkId=four");
            user = users[0];
            Assert.AreEqual("four", user.HawkId);

            var userInput = Mapper.Map<UserInputModel>(user);
            userInput.Id = user.Id;
            userInput.HawkId = "whoa";
            EndpointPut(client, $"{endpointBase}?", userInput);

            user = EndpointFetch<User>(client, $"{endpointBase}?id=" + user.Id);
            Assert.AreEqual("whoa", user.HawkId);

            // delete
            user = EndpointFetch<User>(client, $"{endpointBase}?id=1");
            Assert.IsNotNull(user);
            EndpointDelete<User>(client, $"{endpointBase}?id=1");
            user = EndpointFetch<User>(client, $"{endpointBase}?id=1");
            Assert.IsNull(user);

            //var users = EndpointFetch<User>(client, endpointBase);
            //Assert.AreEqual(1, users.Length);
        }

        public void UserRoleEndpoint(HttpClient client)
        {
            var endpointBase = "http://testserver/userrole";
            var uri = new UserRoleInputModel()
            {
                UserId = 1,
                RoleId=1 //duplicate - should fail.
            };
            var response = EndpointPostFail(client, endpointBase, uri);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

            uri.UserId = 1;
            uri.RoleId = 3;
            EndpointPost(client, endpointBase, uri);  //should succeed

            var userrole = EndpointFetch<UserRole[]>(client, $"{endpointBase}?keywords=admin"); 
        }

        public T EndpointFetch<T>(HttpClient client, string url) where T: class
        {
            var task = client.GetAsync(url);
            task.Wait(TimeSpan.FromSeconds(30));
            var response = task.Result;

            if (response.IsSuccessStatusCode)
            {
                var task2 = response.Content.ReadAsStringAsync();
                var entityTask = response.Content.ReadAsAsync<T>();
                entityTask.Wait(TimeSpan.FromSeconds(5));
                var entity = entityTask.Result;
                return entity;
            }

            var result = response.Content.ReadAsStringAsync().Result;
            try
            {
                Assert.Fail($"API did not return success: {response?.StatusCode}. {result.ToString()}");
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public byte[] EndpointFetchRaw<T>(HttpClient client, string url) where T : class
        {
            var task = client.GetAsync(url);
            task.Wait(TimeSpan.FromSeconds(30));
            var response = task.Result;

            if (response.IsSuccessStatusCode)
            {
                var entityTask = response.Content.ReadAsByteArrayAsync();
                entityTask.Wait(TimeSpan.FromSeconds(5));
                var entity = entityTask.Result;
                return entity;
            }

            var result = response.Content.ReadAsStringAsync().Result;
            try
            {
                Assert.Fail($"API did not return success: {response?.StatusCode}. {result.ToString()}");
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public T[] EndpointSearch<T>(HttpClient client, string url, string[] keywords) where T : class
        {
            var filter = new FilterModel() {Keywords = keywords, Page = 0, PageSize = 2};
            var req = new HttpRequestMessage();
            req.Method = new HttpMethod("SEARCH");
            req.RequestUri = new Uri(url);
            req.Content = new StringContent(JsonConvert.SerializeObject(filter));
            var task = client.SendAsync(req);
            task.Wait(TimeSpan.FromSeconds(30));
            var response = task.Result;

            if (response.IsSuccessStatusCode)
            {
                var task2 = response.Content.ReadAsStringAsync();
                var entityTask = response.Content.ReadAsAsync<T[]>();
                entityTask.Wait(TimeSpan.FromSeconds(5));
                var entity = entityTask.Result;
                return entity;
            }

            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Fail($"API did not return success: {response?.StatusCode}. {result.ToString()}");
            return null;
        }

        public T EndpointPost<T>(HttpClient client, string url, T entity) where T : class
        {
            var task = client.PostAsJsonAsync(url, entity);
            task.Wait(TimeSpan.FromSeconds(30));
            var response = task.Result;

            if (response.IsSuccessStatusCode)
            {
                var entityTask = response.Content.ReadAsAsync<T>();
                entityTask.Wait(TimeSpan.FromSeconds(5));
                return entityTask.Result;
            }

            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Fail($"API did not return success: {response?.StatusCode}. {result.ToString()}");
            return null;
        }

        /* Here for reference. Useful for testing multipart uploads.
        public U[] EndpointPostMultipart<T, U>(HttpClient client, string url, T entity)
            where T : DocumentInputModel
            where U : DocumentViewModel
        {
            using (
                var content =
                    new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                content.Add(new StreamContent(new MemoryStream(entity.DocumentFile.ContentBytes)), "\"1\"", entity.Filename);
                var task = client.PostAsync(url, content);
                task.Wait(TimeSpan.FromSeconds(30));
                var response = task.Result;

                if (response.IsSuccessStatusCode)
                {
                    var entityTask = response.Content.ReadAsAsync<U[]>();
                    entityTask.Wait(TimeSpan.FromSeconds(5));
                    return entityTask.Result;
                }

                var result = response.Content.ReadAsStringAsync().Result;
                Assert.Fail($"API did not return success: {response?.StatusCode}. {result.ToString()}");
            }
            return null;
        } */

        public T[] EndpointPostArray<T>(HttpClient client, string url, T[] entity) where T : class
        {
            var task = client.PostAsJsonAsync(url, entity);
            task.Wait(TimeSpan.FromSeconds(30));
            var response = task.Result;

            if (response.IsSuccessStatusCode)
            {
                var entityTask = response.Content.ReadAsAsync<T[]>();
                entityTask.Wait(TimeSpan.FromSeconds(5));
                return entityTask.Result;
            }

            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Fail($"API did not return success: {response?.StatusCode}. {result.ToString()}");
            return null;
        }

        public T EndpointPut<T>(HttpClient client, string url, T entity) where T : class
        {
            var task = client.PutAsJsonAsync(url, entity);
            task.Wait(TimeSpan.FromSeconds(30));
            var response = task.Result;

            if (response.IsSuccessStatusCode)
            {
                var entityTask = response.Content.ReadAsAsync<T>();
                entityTask.Wait(TimeSpan.FromSeconds(5));
                return entityTask.Result;
            }

            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Fail($"API did not return success: {response?.StatusCode}. {result.ToString()}");
            return null;
        }

        public T EndpointDelete<T>(HttpClient client, string url) where T : class
        {
            var task = client.DeleteAsync(url);
            task.Wait(TimeSpan.FromSeconds(30));
            var response = task.Result;

            if (response.IsSuccessStatusCode)
            {
                var entityTask = response.Content.ReadAsAsync<T>();
                entityTask.Wait(TimeSpan.FromSeconds(5));
                return entityTask.Result;
            }

            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Fail($"API did not return success: {response?.StatusCode}. {result.ToString()}");
            return null;
        }

        public HttpResponseMessage EndpointPostFail<T>(HttpClient client, string url, T entity) where T : class
        {
            var task = client.PostAsJsonAsync(url, entity);
            task.Wait(TimeSpan.FromSeconds(30));
            var response = task.Result;

            return response;
        }

        private void initializeFakeData()
        {
            using (var dc = new DataContext(Kernel.Get<IPrincipal>(), Mock.Of<ILogger>()))
            {
                new List<User>()
                {
                    new User()
                    {
                        HawkId = "one",
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                    },
                    new User()
                    {
                        HawkId = "two",
                        IsDeleted = true,
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now
                    },
                    new User()
                    {
                        HawkId = "three",
                        IsDeleted = false,
                        Email = "li_yan_richard@hotmail.com",
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now
                    }
                }.ForEach(u => dc.Users.Add(u));

                new List<Role>()
                {
                    new Role()
                    {
                        Id = 1,
                        RoleName = "Nobody",
                        Description = "I can do nothing. You have my account frozen.",
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        LastUpdatedBy = 1,
                        IsDeleted = false
                    },
                    new Role()
                    {
                        Id = 2,
                        RoleName = "User",
                        Description = "La-tee-daa I am a user, look at me!",
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        LastUpdatedBy = 1,
                        IsDeleted = false
                    },
                    new Role()
                    {
                        Id = 3,
                        RoleName = "UserAdmin",
                        Description = "I don't quite have superpowers, but the grass is always greener!",
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        LastUpdatedBy = 1,
                        IsDeleted = false
                    },
                    new Role()
                    {
                        Id = 4,
                        RoleName = "Admin",
                        Description = "I am superman! Wheee!!! *BONK*",
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        LastUpdatedBy = 1,
                        IsDeleted = false
                    }
                }.ForEach(u => dc.Roles.Add(u));

                try
                {
                    dc.Save();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                new List<UserRole>()
                {
                    new UserRole()
                    {
                        Id = 1,
                        UserId = 1,
                        RoleId = 1,
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        LastUpdatedBy = 1,
                        IsDeleted = false
                    },
                    new UserRole()
                    {
                        Id = 2,
                        UserId = 1,
                        RoleId = 2,
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        LastUpdatedBy = 1,
                        IsDeleted = false
                    },
                    new UserRole()
                    {
                        Id = 3,
                        UserId = 2,
                        RoleId = 3,
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        LastUpdatedBy = 1,
                        IsDeleted = false
                    },
                    new UserRole()
                    {
                        Id = 4,
                        UserId = 3,
                        RoleId = 3,
                        Created = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        LastUpdatedBy = 1,
                        IsDeleted = false
                    }
                }.ForEach(ur => dc.UserRoles.Add(ur));

                try
                {
                    dc.Save();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            using (var dc = new DNADataContext())
            {
                // We don't wanna play with live IDW, so fake the view.
                new List<V_Population>()
                {
                    new V_Population()
                    {
                        UniversityId = "007",
                        HawkId = "jbond",
                    },
                    new V_Population()
                    {
                        UniversityId = "700",
                        HawkId = "jbnemesis",
                    }
                }.ForEach(u => dc.Population.Add(u));

                try
                {
                    dc.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
