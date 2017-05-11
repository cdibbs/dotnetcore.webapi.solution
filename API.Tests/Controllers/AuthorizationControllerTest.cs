using API.Controllers;
using API.Models;
using API.SpecificationProviders;
using AutoMapper;
using Castle.Core.Internal;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Specifications;
using System;
using System.Linq.Expressions;

namespace API.Tests
{
    [TestClass]
    public class AuthorizationControllerTest {
        private Mock<ILogger> _logger;
        private AuthorizationController _controller;
        private const string AccessKey = "some_random_key";

        [TestInitialize]
        public void Init() {
            _logger = new Mock<ILogger>();
            _controller = new AuthorizationController(
                AccessKey,
                _logger.Object,
                Mock.Of<IRepository>(),
                Mock.Of<IMapper>(),
                Mock.Of<IUserSpecificationProvider>()
            );
        }

        [TestMethod, TestCategory("Controller_Authorization")]
        public void Get_WithInvalidKeys_ReturnsUnauthorized() {
            var result = _controller.Get(string.Empty, string.Empty);

            Assert.IsInstanceOfType(result,typeof(UnauthorizedResult));
        }

        [TestMethod, TestCategory("Controller_Authorization")]
        public void Get_WhenKeyIsNotSet_ReturnsDetails() {
            _controller.Key = string.Empty;
            var result = _controller.Get("myid", string.Empty) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod, TestCategory("Controller_Authorization")]
        public void Get_WithValidKeyAndUsername_ReturnsDetails()
        {
            // Setup
            var myUser = "myid";
            var dbUser = new User();
            var mapperOutput = new AuthorizationDetails() { Username = "imdifferent!"};
            var spec = Specification<User>.Start(u => true);
            _controller.Specs = Mock.Of<IUserSpecificationProvider>(
                s => s.ByUsername<User>(It.Is<string>(u => u == myUser)) == spec);
            var m = Mock.Get(_controller.Repo);
            m.Setup(r => r.FindOne(It.Is<ISpecification<User>>(s => s == spec), false, false, 
                It.IsAny<Expression<Func<User, object>>>()))
                .Returns(dbUser);
            var mm = Mock.Get(_controller.Mapper);
            mm.Setup(o => o.Map<AuthorizationDetails>(It.Is<User>(u => u == dbUser)))
                .Returns(mapperOutput);

            var result = _controller.Get(myUser, AccessKey) as OkObjectResult;

            Mock.Get(_controller.Specs).Verify();
            m.Verify();
            mm.Verify();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("imdifferent!", (result.Value as AuthorizationDetails).Username);
        }

        [TestMethod, TestCategory("Controller_Authorization")]
        public void Get_WithValidKeyInvalidUsername_ReturnsEmptyAuth()
        {
            var m = Mock.Get(_controller.Repo);
            m.Setup(r => r.FindOne(It.IsAny<ISpecification<User>>(), false, false))
                .Returns<User>(null);
            var result = _controller.Get("myid", AccessKey) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.IsTrue((result.Value as AuthorizationDetails).Groups.IsNullOrEmpty());
            Assert.AreEqual("myid", (result.Value as AuthorizationDetails).Username);
        }
    }
}
