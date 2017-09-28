using API.Controllers;
using API.Managers;
using API.Mapping;
using API.Models;
using API.Models.FilterModels;
using API.Models.InputModels;
using API.SpecificationProviders;
using AutoMapper;
using Data.Repositories.ReadOnly;
using Data.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Specifications;
using System;
using Data.Models;

namespace API.Tests
{
    [TestClass]
    public class PopulationControllerTest : BaseControllerTest<PopulationController, V_MyView, string>
    {
        [TestInitialize]
        public void Setup()
        {
            MapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new APIMappingProfile());
            });
        }

        [TestMethod, TestCategory("Controller_User")]
        public void Get_Filter_PassesParamsResults()
        {
            // Setup
            var result = new PersonViewModel[0];
            var ctrl = GetController(Mock.Of<IPopulationSpecificationProvider>());
            Mock.Get(ctrl.Manager)
                .Setup(c => c.Filter(It.IsAny<ISpecification<V_MyView>>(), 
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<SortSpecification[]>()))
                .Returns(result);

            // Test
            var actualResult = ctrl.Get(new PopulationFilterModel() {Page = 3, PageSize = 14});

            // Examine
            Mock.Get(ctrl.Manager)
                .Verify(c => c.Filter(It.IsAny<ISpecification<V_MyView>>(),
                    It.Is<int>(i => i == 3), It.Is<int>(i => i == 14), It.IsAny<SortSpecification[]>()),
                    Times.Once);
            Assert.AreEqual(result, actualResult);
        }
    }
}
