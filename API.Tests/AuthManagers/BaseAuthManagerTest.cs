using API.Authorization;
using API.Exceptions;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace API.Tests.AuthManagers
{
    [TestClass]
    public class BaseAuthManagerTest
    {
        public BaseAuthManager<User, long> AuthMock(string[] roles)
        {
            var user = Mock.Of<IPrincipal>(u => u.Identity.Name == "unittest");
            Mock.Get(user)
                .Setup(u => u.IsInRole(It.Is<string>(s => roles.Contains(s))))
                .Returns(true);
            Mock.Get(user)
                .Setup(u => u.IsInRole(It.Is<string>(s => ! roles.Contains(s))))
                .Returns(false);
            var AuthM = new BaseAuthManager<User, long>(
                user: user,
                logger: Mock.Of<ILogger>()
            );
            return AuthM;
        }

        [TestMethod, TestCategory("Authorization")]
        public void MayProps_ValidUser_Ok()
        {
            Assert.IsTrue(AuthMock(new[] {"ValidUser"}).MayGet);
            Assert.IsTrue(AuthMock(new[] {"ValidUser"}).MayAdd);
            Assert.IsTrue(AuthMock(new[] {"ValidUser"}).MayUpdate);
            Assert.IsTrue(AuthMock(new[] {"ValidUser"}).MayDelete);
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
        public void AuthorizeGet_May_NoThrows()
        {
            AuthMock(new[] {"ValidUser"}).AuthorizeGet();
            AuthMock(new[] {"ValidUser"}).AuthorizeAdd();
            AuthMock(new[] {"ValidUser"}).AuthorizeUpdate();
            AuthMock(new[] {"ValidUser"}).AuthorizeDelete();
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
        public void GenerateFilterGet_MayFilter_True()
        {
            var f = AuthMock(new[] {"ValidUser"}).GenerateFilterGet();
            Assert.AreEqual(1, new List<User>()
            {
                new User()
            }.Where(f.AsExpression().Compile()).Count());
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterUpdate_MayFilter_True()
        {
            var f = AuthMock(new[] { "ValidUser" }).GenerateFilterUpdate();
            Assert.AreEqual(1, new List<User>()
            {
                new User()
            }.Where(f.AsExpression().Compile()).Count());
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterDelete_MayFilter_True()
        {
            var f = AuthMock(new[] { "ValidUser" }).GenerateFilterDelete();
            Assert.AreEqual(1, new List<User>()
            {
                new User()
            }.Where(f.AsExpression().Compile()).Count());
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterGet_MayNotFilter_None()
        {
            var f = AuthMock(new string[0]).GenerateFilterGet();
            Assert.AreEqual(0, new List<User>()
            {
                new User()
            }.Where(f.AsExpression().Compile()).Count());
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterUpdate_MayNotFilter_None()
        {
            var f = AuthMock(new string[0]).GenerateFilterUpdate();
            Assert.AreEqual(0, new List<User>()
            {
                new User()
            }.Where(f.AsExpression().Compile()).Count());
        }

        [TestMethod, TestCategory("Authorization")]
        public void GenerateFilterDelete_MayNotFilter_None()
        {
            var f = AuthMock(new string[0]).GenerateFilterDelete();
            Assert.AreEqual(0, new List<User>()
            {
                new User()
            }.Where(f.AsExpression().Compile()).Count());
        }
    }
}
