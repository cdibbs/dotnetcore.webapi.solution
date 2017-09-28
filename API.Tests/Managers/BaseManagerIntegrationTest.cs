using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using API.Authorization;
using Data;
using API.Exceptions;
using API.Managers;
using API.Mapping;
using API.Models;
using API.Models.InputModels;
using API.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Specifications;
using API.SpecificationProviders;

namespace API.Tests
{
    /// <summary>
    /// These tests stem from a deep and profound distrust of generics.
    /// They ask "do non-base properties get copied around, also?" :-)
    /// If not, fail.
    /// </summary>
    [TestClass]
    public class BaseManagerIntegrationTest
    {
        public BaseManager<User> Manager { get; set; }
        public MapperConfiguration MapperConfig { get; set; }
        public IMapper Mapper { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            MapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new APIMappingProfile());
            });
            Mapper = MapperConfig.CreateMapper();

            Manager = new UserManager(
                Mock.Of<IRepository>(),
                Mapper,
                Mock.Of<IValidator<User>>(),
                Mock.Of<ILogger>(),
                Mock.Of<IAuthManager<User, long>>(),
                Mock.Of<IUserSpecificationProvider>()
            );
        }

        [TestMethod]
        public void Add_HandlesDerivedProperties()
        {
            User r = null;
            var rm = Mock.Get(Manager.Repo);
            rm.Setup(m => m.AddEntity(It.IsAny<User>()))
                .Callback((User b) => { r = b; });
            var i = new UserInputModel()
            {
                First = "Chris"
            };

            Manager.Add(i);

            Assert.AreEqual(i.First, r.First);
        }

        [TestMethod]
        public void Update_HandlesDerivedProperties()
        {
            User r = new User() { First = "Mine" };
            var s = Mock.Of<ISpecification<User>>();
            Mock.Get(s)
                .Setup(sp => sp.And(It.IsAny<ISpecification<User>>()))
                .Verifiable();
            Manager.Specs = Mock.Of<IUserSpecificationProvider>(
                sp => sp.ById<User>(It.IsAny<long>()) == s);

            var rm = Mock.Get(Manager.Repo);
            rm.Setup(m => m.FindOne(
                    It.IsAny<ISpecification<User>>(),
                    true, false))
                .Returns(r);
            var i = new UserInputModel()
            {
                First = "Chris was here"
            };

            Manager.Update(i);

            Assert.AreEqual(i.First, r.First);
        }
    }
}
