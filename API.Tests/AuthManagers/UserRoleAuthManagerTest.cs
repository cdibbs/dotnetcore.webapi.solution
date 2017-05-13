using API.Authorization;
using API.Exceptions;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Specifications;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace API.Tests.AuthManagers
{
    [TestClass]
    public class UserRoleAuthManagerTest
    {
        public const string CurrentUserUsername = "me";
        public List<UserRole> All = new List<UserRole>()
        {
            new UserRole()
        };

        public UserRoleAuthManager AuthMock(string[] roles, string name = CurrentUserUsername)
        {
            var user = Mock.Of<IPrincipal>(u => u.Identity.Name == name);
            Mock.Get(user)
                .Setup(u => u.IsInRole(It.Is<string>(s => roles.Contains(s))))
                .Returns(true);
            Mock.Get(user)
                .Setup(u => u.IsInRole(It.Is<string>(s => !roles.Contains(s))))
                .Returns(false);
            var AuthM = new UserRoleAuthManager(
                user: user,
                logger: Mock.Of<ILogger>()
            );
            return AuthM;
        }

        [TestMethod, TestCategory("Authorization")]
        public void MayProps_Ok()
        {
            foreach (string role in new[] {"Admin" })
            {
                Assert.IsTrue(AuthMock(new[] {role}).MayGet);
                //nope, see ValidUserOnly_False:
                Assert.IsTrue(AuthMock(new[] { role }).MayAdd);
                Assert.IsTrue(AuthMock(new[] { role }).MayUpdate);
                Assert.IsTrue(AuthMock(new[] { role }).MayDelete);
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void MayProps_NotValidUser_Fail()
        {
            Assert.IsFalse(AuthMock(new string[0]).MayGet);
            Assert.IsFalse(AuthMock(new string[0]).MayAdd);
            Assert.IsFalse(AuthMock(new string[0]).MayUpdate);
            Assert.IsFalse(AuthMock(new string[0]).MayDelete);
        }

        [TestMethod, TestCategory("Authorization")]
        public void MayAdd_LowPerms_False()
        {
            foreach (string role in new [] { "Reviewer", "Intake", "ValidUser", "Bogus" })
            {
                Assert.IsFalse(AuthMock(new[] {role}).MayAdd);
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void AuthorizeGet_May_NoThrows()
        {
            AuthMock(new[] { "Admin" }).AuthorizeGet();
            AuthMock(new[] { "Admin" }).AuthorizeAdd();
            AuthMock(new[] { "Admin" }).AuthorizeUpdate();
            AuthMock(new[] { "Admin" }).AuthorizeDelete();
        }

        [TestMethod, TestCategory("Authorization")]
        [ExpectedException(typeof(AuthorizationException))]
        public void AuthorizeGet_NotMay_Throws()
            => AuthMock(new string[0]).AuthorizeGet();

        [TestMethod, TestCategory("Authorization")]
        [ExpectedException(typeof(AuthorizationException))]
        public void AuthorizeAdd_NotMay_Throws()
            => AuthMock(new string[0]).AuthorizeAdd();

        [TestMethod, TestCategory("Authorization")]
        [ExpectedException(typeof(AuthorizationException))]
        public void AuthorizeUpdate_NotMay_Throws()
            => AuthMock(new string[0]).AuthorizeUpdate();

        [TestMethod, TestCategory("Authorization")]
        [ExpectedException(typeof(AuthorizationException))]
        public void AuthorizeDelete_NotMay_Throws()
            => AuthMock(new string[0]).AuthorizeDelete();

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterGet_MinPerms_DeniedAll()
        {
            var f = AuthMock(new[] { "ValidUser" }).GenerateFilterGet();
            Assert.AreEqual(0, All.Where(f.AsExpression().Compile()).Count());
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterGet_Admin_DeniedNone()
        {
            var f = AuthMock(new[] { "Admin" }).GenerateFilterGet();
            Assert.AreEqual(1, All.Where(f.AsExpression().Compile()).Count());
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterGet_All_AllowSelf()
        {
            foreach (string role in new[] { "Admin", "Reviewer", "Intake", "Admin", "ValidUser" })
            {
                var f = AuthMock(new[] { role }).GenerateFilterGet();
                var list = new List<UserRole>()
                {
                    new UserRole()
                    {
                        Id = 1,
                        User = new User() {
                            Username = CurrentUserUsername
                        }
                    }
                };
                var results = list.Where(f.AsExpression().Compile());
                Assert.AreEqual(1, results.First().Id);
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterGet_NoUserBadRecord_ReturnsEmpty()
        {
            foreach (string role in new[] { "ValidUser" })
            {
                var f = AuthMock(new[] { role }).GenerateFilterGet();
                var list = new List<UserRole>()
                {
                    new UserRole()
                    {
                        Id = 1,
                        RoleId = 1,
                        UserId = 0,
                        User = null
                    }
                };
                var results = list.Where(f.AsExpression().Compile());
                Assert.IsFalse(results.Any());
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterGet_CannotGetAll_NoUserBadRecord_DisallowAll()
        {
            foreach (var role in new[] { "ValidUser" })
            {
                var f = AuthMock(new[] { role }, null).GenerateFilterGet();
                var list = new List<UserRole>()
                {
                    new UserRole()
                    {
                        Id = 1,
                        RoleId = 1,
                        UserId = 0,
                        User = null
                    }
                };
                var results = list.Where(f.AsExpression().Compile());
                Assert.IsFalse(results.Any());
                list.First().User = new User();
                Assert.IsFalse(results.Any());
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterGet_UsernameNull_DisallowAll()
        {
            foreach (string role in new[] { "ValidUser" }) {
                var amock = AuthMock(new[] { role }, null);
                amock.User = null;
                var f = amock.GenerateFilterGet();
                var list = new List<UserRole>()
                {
                    new UserRole()
                    {
                        Id = 1,
                        RoleId = 1,
                        UserId = 1,
                        User = new User() { Username = null }
                    }
                };
                var results = list.Where(f.AsExpression().Compile());
                Assert.IsFalse(results.Any());
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterUpdate_SameAsGet()
        {
            var filter = Specification<User>.All();
            var m = new Mock<UserAuthManager>(Mock.Of<IPrincipal>(), Mock.Of<ILogger>());
            m.Setup(c => c.GenerateFilterUpdate()).CallBase();
            m.Setup(c => c.GenerateFilterGet())
                .Returns(filter)
                .Verifiable();

            var f = m.Object.GenerateFilterUpdate();

            m.Verify();
            Assert.AreEqual(filter, f);
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterDelete_NeverSelf()
        {
            foreach (string role in new[] { "Admin", "ValidUser" })
            {
                var f = AuthMock(new[] { role }).GenerateFilterDelete();
                var list = new List<UserRole>()
                    {
                        new UserRole()
                        {
                            Id = 1,
                            User = new User() {
                                Username = CurrentUserUsername
                            }
                        }
                    };
                var results = list.Where(f.AsExpression().Compile());
                Assert.AreEqual(0, results.Count());
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterDelete_Admin_AllOthers()
        {
            foreach (string role in new[] { "Admin" })
            {
                var f = AuthMock(new[] { role }).GenerateFilterDelete();
                var list = new List<UserRole>()
                {
                    new UserRole()
                    {
                        Id = 1,
                        User = new User() {
                            Username = "other"
                        }
                    }
                };
                var results = list.Where(f.AsExpression().Compile());
                Assert.AreEqual(1, results.Count());
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterDelete_LowPerms_None()
        {
            foreach (string role in new[] { "Reviewer", "Intake", "ValidUser", "Bogus" }) {
                var f = AuthMock(new[] { role }).GenerateFilterDelete();
                var list = new List<UserRole>()
                {
                    new UserRole()
                    {
                        Id = 1,
                    }
                };
                var results = list.Where(f.AsExpression().Compile());
                Assert.AreEqual(0, results.Count());
            }
        }
    }
}
