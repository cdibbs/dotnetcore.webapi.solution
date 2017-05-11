using API.Authorization;
using Data.Repositories.ReadOnly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using LinqKit;

namespace API.Tests.AuthManagers
{
    [TestClass]
    public class PopulationAuthManagerTest
    {
        public List<V_Population> AllPeople = new List<V_Population>()
        {
            new V_Population()
        };

        public PopulationAuthManager AuthMock(string[] roles)
        {
            var user = Mock.Of<IPrincipal>(u => u.Identity.Name == "me");
            Mock.Get(user)
                .Setup(u => u.IsInRole(It.Is<string>(s => roles.Contains(s))))
                .Returns(true);
            Mock.Get(user)
                .Setup(u => u.IsInRole(It.Is<string>(s => !roles.Contains(s))))
                .Returns(false);
            var AuthM = new PopulationAuthManager()
            {
                User = user,
                Logger = Mock.Of<ILogger>()
            };
            return AuthM;
        }

        [TestMethod, TestCategory("Authorization")]
        public void MayWrite_Never()
        {
            // It is read-only, after all :-)
            foreach (string role in new[] {"Admin" }) {
                Assert.IsFalse(AuthMock(new[] {role}).MayAdd);
                Assert.IsFalse(AuthMock(new[] {role}).MayUpdate);
                Assert.IsFalse(AuthMock(new[] {role}).MayDelete);
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void MayGet_WhenAtLeastAdmin()
        {
            Assert.IsTrue(AuthMock(new[] { "Admin" }).MayGet);
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterGet_MinPerms_DeniedAll()
        {
            foreach (string role in new[] { "ValidUser", "Intake", "Reviewer" })
            {
                var f = AuthMock(new[] {role}).GenerateFilterGet();
                Assert.AreEqual(0, AllPeople.Where(f.AsExpression().Compile()).Count());
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterGet_NeedPerms_Allowed()
        {

            foreach (string role in new[] { "Admin" })
            {
                var f = AuthMock(new[] { role }).GenerateFilterGet();
                Assert.AreEqual(1, AllPeople.Where(f.AsExpression().Compile()).Count());
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterUpdate_DeniedAll()
        {
            foreach (string role in new[] { "ValidUser", "Admin" })
            {
                var f = AuthMock(new[] { role }).GenerateFilterDelete();
                Assert.AreEqual(0, AllPeople.Where(f.AsExpression().Compile()).Count());
            }
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterDelete_DeniedAll()
        {
            foreach (string role in new[] { "ValidUser", "Admin" })
            {
                var f = AuthMock(new[] { role }).GenerateFilterUpdate();
                Assert.AreEqual(0, AllPeople.Where(f.AsExpression().Compile()).Count());
            }
        }
    }
}
