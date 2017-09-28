using API.Authorization;
using API.Managers;
using API.SpecificationProviders;
using AutoMapper;
using Data.Repositories.ReadOnly;
using Data.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using Data.Models;

namespace API.Tests.Managers
{
    [TestClass]
    public class ReadOnlyManagerTest
    {
        public PopulationManager Manager { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Manager = new PopulationManager(
                Mock.Of<IReadOnlyRepository<string>>(),
                Mock.Of<IMapper>(),
                Mock.Of<ILogger>(),
                Mock.Of<IAuthManager<V_MyView, string>>(),
                Mock.Of<IPopulationSpecificationProvider>()
            );
        }

        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public void Add_Throws() => Manager.Add(null);

        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public void Delete_Id_Throws() => Manager.Delete("1");

        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public void Delete_Spec_Throws() => Manager.Delete("");

        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public void Update_Throws() => Manager.Update(null);

        [TestMethod, TestCategory("BaseManager")]
        public void Get_AuthFiltered()
        {
            // Setup
            var filter = Specification<V_MyView>.Start(c => true);
            var s = Mock.Of<ISpecification<V_MyView>>();
            Mock.Get(s)
                .Setup(sp => sp.And(It.Is<ISpecification<V_MyView>>(ss => ss == filter)))
                .Verifiable();
            Mock.Get(Manager.Specs)
                .Setup(sp => sp.ByUserId<V_MyView>(It.IsAny<string>()))
                .Returns(s);

            var rm = Mock.Get(Manager.Auth);
            rm.Setup(m => m.GenerateFilterGet())
                .Returns(filter)
                .Verifiable();
            var mrepo = Mock.Get(Manager.Repo);
            mrepo.Setup(m => m.FindOne(
                    It.IsAny<ISpecification<V_MyView>>(),
                    Manager.GetIncludes))
                    .Verifiable();

            // Test
            Manager.Get("hawkid");

            // Check
            mrepo.Verify();
            rm.Verify();
            Mock.Get(s).Verify();
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Filter_AuthFiltered()
        {
            // Setup
            var filter = Specification<V_MyView>.Start(c => true);
            var s = Mock.Of<ISpecification<V_MyView>>();
            Mock.Get(s)
                .Setup(sp => sp.And(It.Is<ISpecification<V_MyView>>(ss => ss == filter)))
                .Verifiable();

            var rm = Mock.Get(Manager.Auth);
            rm.Setup(m => m.GenerateFilterGet())
                .Returns(filter)
                .Verifiable();
            var mrepo = Mock.Get(Manager.Repo);
            mrepo.Setup(m => m.Page(
                    It.IsAny<ISpecification<V_MyView>>(), It.IsAny<ISortFactory<V_MyView, string>>(), 0, 0))
                    .Returns(new List<V_MyView>().AsQueryable().OrderBy(l => l.Id))
                    .Verifiable();

            // Test
            Manager.Filter(s, 0, 0, new SortSpecification[0]);

            // Check
            mrepo.Verify();
            rm.Verify();
            Mock.Get(s).Verify();
        }
    }
}
