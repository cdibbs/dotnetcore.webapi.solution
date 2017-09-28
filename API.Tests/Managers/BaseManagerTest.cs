using API.Authorization;
using API.Exceptions;
using API.Managers;
using API.Models;
using API.Models.InputModels;
using API.Models.ViewModels;
using API.SpecificationProviders;
using API.Validators;
using AutoMapper;
using Data;
using Data.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace API.Tests
{
    [TestClass]
    public class BaseManagerTest
    {
        public BaseManager<User> Manager { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Manager = new BaseManager<User>(
                Mock.Of<IRepository>(),
                Mock.Of<IMapper>(),
                Mock.Of<IValidator<User>>(),
                Mock.Of<ILogger>(),
                Mock.Of<IAuthManager<User, long>>(),
                Mock.Of<IUserSpecificationProvider>()
            );
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Filter_UsesSpecAndPageParams()
        {
            // Setup
            ISpecification<User> actualSpec = null;
            int actualPage = 0, actualPageSize = 0;
            var expectedSpec = Specification<User>.All();
            int expectedPage = 1, expectedPageSize = 10;
            var rm = Mock.Get(Manager.Repo)
                .Setup(m => m.Page(It.IsAny<ISpecification<User>>(),
                        It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ISortFactory<User, long>>(), false, false));
            Mock.Get(Manager.Auth)
                .Setup(a => a.GenerateFilterGet())
                .Returns(Specification<User>.All());
            var list = new List<User>().OrderBy(c => c.Created);
            rm.Returns(list);
            rm.Callback<
                ISpecification<User>,
                int, int,
                ISortFactory<User, long>,
                bool, bool, 
                Expression<Func<User, object>>[]>(
                (s, p, ps, a, b, b1, i) => { actualSpec = s; actualPage = p; actualPageSize = ps; });
            rm.Verifiable();

            // Test
            Manager.Filter(expectedSpec, expectedPage, expectedPageSize, new SortSpecification[0]);

            // Verify
            Assert.AreEqual(expectedSpec, actualSpec);
            Assert.AreEqual(expectedPage, actualPage);
            Assert.AreEqual(expectedPageSize, actualPageSize);
        }

        [TestMethod, TestCategory("BaseManager"),
            ExpectedException(typeof(AuthorizationException))]
        public void Filter_Authorized()
        {
            // Setup
            var rm = Mock.Get(Manager.Auth)
                .Setup(m => m.AuthorizeGet()).Throws<AuthorizationException>();

            // Test
            Manager.Filter(null, 0, 0, new SortSpecification[0]);
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Filter_SortsAsRequested()
        {
            //throw new NotImplementedException();
            // TODO If we need it. Repo method default is descending by created.
        }

        [TestMethod, TestCategory("BaseManager"),
            ExpectedException(typeof(AuthorizationException))]
        public void Add_Authorized()
        {
            // Setup
            var rm = Mock.Get(Manager.Auth)
                .Setup(m => m.AuthorizeAdd()).Throws<AuthorizationException>();

            // Test
            Manager.Add(null);
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Add_UsesInput_Saves()
        {
            // Setup
            var input = new UserInputModel();
            var mapperOutput = new User();
            var mm = Mock.Get(Manager.Mapper);
            mm.Setup(m => m.Map<User>(It.Is<UserInputModel>(i => i == input)))
                .Returns(mapperOutput)
                .Verifiable();
            var rm = Mock.Get(Manager.Repo);
            rm.Setup(m => m.AddEntity(It.Is<User>(o => o == mapperOutput)))
                .Verifiable();
            rm.Setup(m => m.Save(false))
                .Verifiable();

            // Test
            Manager.Add(input);

            // Verify
            mm.Verify();
            rm.Verify();
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Add_ValidatesInput()
        {
            // Setup
            Mock.Get(Manager.Validator)
                .Setup(m => m.Validate(It.IsAny<UserInputModel>()))
                .Verifiable();

            // Test
            Manager.Add(new UserInputModel());

            // Verify
            Mock.Get(Manager.Validator).Verify();
        }

        [TestMethod, TestCategory("BaseManager"),
            ExpectedException(typeof(InvalidInputException))]
        public void Add_InvalidInput_ThrowsAndPreventsSave()
        {
            // Setup
            Mock.Get(Manager.Validator)
                .Setup(m => m.Validate(It.IsAny<UserInputModel>()))
                .Throws<InvalidInputException>();

            Mock.Get(Manager.Repo)
                .Setup(m => m.Save(false))
                .Throws<Exception>();

            // Test
            Manager.Add(new UserInputModel());
        }

        [TestMethod, TestCategory("BaseManager"),
            ExpectedException(typeof(AuthorizationException))]
        public void Get_Authorized()
        {
            // Setup
            var rm = Mock.Get(Manager.Auth)
                .Setup(m => m.AuthorizeGet()).Throws<AuthorizationException>();

            // Test
            Manager.Filter(null, 0, 0, new SortSpecification[0]);
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Get_UsesId_ReturnsMapped()
        {
            // Setup
            long input = 314;
            var repoOutput = new User();
            var mapperOutput = new UserViewModel();
            var specOutput = Specification<User>.Start(c => true);
            Manager.Specs = Mock.Of<IUserSpecificationProvider>(
                sp => sp.ById<User>(It.Is<long>(i => i == input)) == specOutput);
            Mock.Get(Manager.Auth)
                .Setup(a => a.GenerateFilterGet())
                .Returns(Specification<User>.All());
            var rm = Mock.Get(Manager.Repo);
            rm.Setup(m => m.FindOne(
                It.Is<ISpecification<User>>(o => o == specOutput),
                It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(repoOutput)
                .Verifiable();
            var mm = Mock.Get(Manager.Mapper);
            mm.Setup(m => m.Map<IViewModel<User>>(It.Is<User>(i => i == repoOutput)))
                .Returns(mapperOutput)
                .Verifiable();

            // Test
            Manager.Get(input);

            // Verify
            mm.Verify();
            rm.Verify();
            Mock.Get(Manager.Specs).Verify();
        }

        [TestMethod, TestCategory("BaseManager"),
            ExpectedException(typeof(AuthorizationException))]
        public void Update_Authorized()
        {
            // Setup
            var rm = Mock.Get(Manager.Auth)
                .Setup(m => m.AuthorizeUpdate()).Throws<AuthorizationException>();

            // Test
            Manager.Update(null);
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Get_AuthFiltered()
        {
            // Setup
            var filter = Specification<User>.Start(c => true);
            var s = Mock.Of<ISpecification<User>>();
            Mock.Get(s)
                .Setup(sp => sp.And(It.Is<ISpecification<User>>(ss => ss == filter)))
                .Verifiable();
            Mock.Get(Manager.Specs)
                .Setup(sp => sp.ById<User>(It.IsAny<long>()))
                .Returns(s);
            var rm = Mock.Get(Manager.Auth);
            rm.Setup(m => m.GenerateFilterGet())
                .Returns(filter)
                .Verifiable();
            var mrepo = Mock.Get(Manager.Repo);
            mrepo.Setup(m => m.FindOne(
                    It.IsAny<ISpecification<User>>(), false, false))
                    .Verifiable();

            // Test
            Manager.Get(1);

            // Check
            mrepo.Verify();
            rm.Verify();
            Mock.Get(s).Verify();
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Filter_AuthFiltered()
        {
            // Setup
            var filter = Specification<User>.Start(c => true);
            var s = Mock.Of<ISpecification<User>>();
            Mock.Get(s)
                .Setup(sp => sp.And(It.Is<ISpecification<User>>(f => f == filter)))
                .Verifiable();
            var rm = Mock.Get(Manager.Auth);
            rm.Setup(m => m.GenerateFilterGet())
                .Returns(filter)
                .Verifiable();
            var mrepo = Mock.Get(Manager.Repo);
            mrepo.Setup(m => m.Page(
                    It.IsAny<ISpecification<User>>(), 0, 0, It.IsAny<ISortFactory<User, long>>(), false,false))
                    .Returns(new List<User>().OrderBy(c => c.Id))
                    .Verifiable();

            // Test
            Manager.Filter(s, 0, 0, new SortSpecification[0]);

            // Check
            mrepo.Verify();
            rm.Verify();
            Mock.Get(s).Verify();
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Update_AuthFiltered()
        {
            // Setup
            var filter = Specification<User>.Start(c => true);
            var rm = Mock.Get(Manager.Auth);
            var s = Mock.Of<ISpecification<User>>();
            Mock.Get(s)
                .Setup(sp => sp.And(It.Is<ISpecification<User>>(f => f == filter)))
                .Verifiable();
            Manager.Specs = Mock.Of<IUserSpecificationProvider>(
                sp => sp.ById<User>(It.IsAny<long>()) == s);

            rm.Setup(m => m.GenerateFilterUpdate())
                .Returns(filter)
                .Verifiable();
            var mrepo = Mock.Get(Manager.Repo);
            mrepo.Setup(m => m.FindOne(
                    It.IsAny<ISpecification<User>>(), true, false))
                    .Verifiable();

            // Test
            Manager.Update(new BaseInputModel<User, long>());

            // Check
            mrepo.Verify();
            rm.Verify();
            Mock.Get(s).Verify();
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Deleted_AuthFiltered()
        {
            // Setup
            var filter = Specification<User>.Start(c => true);
            var s = Mock.Of<ISpecification<User>>();
            Mock.Get(s)
                .Setup(sp => sp.And(It.Is<ISpecification<User>>(f => f == filter)))
                .Verifiable();
            var sm = Mock.Get(Manager.Specs);
            sm.Setup(a => a.ById<User>(It.IsAny<long>())).Returns(s);

            var rm = Mock.Get(Manager.Auth);
            rm.Setup(m => m.GenerateFilterDelete())
                .Returns(filter)
                .Verifiable();
            var mrepo = Mock.Get(Manager.Repo);
            mrepo.Setup(m => m.Delete(
                    It.Is<ISpecification<User>>(sp => sp == s), true))
                    .Verifiable();

            // Test
            Manager.Delete(1);

            // Check
            rm.Verify();
            Mock.Get(s).Verify();
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Update_UsesInput_Saves()
        {
            // Setup
            var input = new UserInputModel() { Id = 1 };
            var repoOutput = new User() { Id = 1};
            var s = Mock.Of<ISpecification<User>>();
            Mock.Get(s)
                .Setup(sp => sp.And(It.IsAny<ISpecification<User>>()))
                .Verifiable();
            Manager.Specs = Mock.Of<IUserSpecificationProvider>(
                sp => sp.ById<User>(It.IsAny<long>()) == s);
            var mm = Mock.Get(Manager.Mapper);
            mm.Setup(m => m.Map(
                It.Is<BaseInputModel<User, long>>(i => i == input), 
                It.Is<User>(o => o == repoOutput)))
                .Verifiable();
            var rm = Mock.Get(Manager.Repo);
            rm.Setup(m => m.FindOne(
                It.IsAny<ISpecification<User>>(),
                It.Is<bool>(o => o), It.IsAny<bool>()))
                .Returns(repoOutput)
                .Verifiable();
            rm.Setup(m => m.Save(false))
                .Verifiable();

            // Test
            Manager.Update(input);

            // Verify
            mm.Verify();
            rm.Verify();
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Update_ValidatesInput()
        {
            // Setup
            Mock.Get(Manager.Validator)
                .Setup(m => m.Validate(It.IsAny<UserInputModel>()))
                .Verifiable();
            var s = Mock.Of<ISpecification<User>>();
            Manager.Specs = Mock.Of<IUserSpecificationProvider>(
                sp => sp.ById<User>(It.IsAny<long>()) == s);

            // Test
            Manager.Update(new UserInputModel());

            // Verify
            Mock.Get(Manager.Validator).Verify();
        }

        [TestMethod, TestCategory("BaseManager"),
            ExpectedException(typeof(InvalidInputException))]
        public void Update_InvalidInput_ThrowsAndPreventsSave()
        {
            // Setup
            Mock.Get(Manager.Validator)
                .Setup(m => m.Validate(It.IsAny<UserInputModel>()))
                .Throws<InvalidInputException>();

            Mock.Get(Manager.Repo)
                .Setup(m => m.Save(false))
                .Throws<Exception>();

            // Test
            Manager.Update(new UserInputModel());
        }

        [TestMethod, TestCategory("BaseManager"),
            ExpectedException(typeof(AuthorizationException))]
        public void Delete_Authorized()
        {
            // Setup
            var rm = Mock.Get(Manager.Auth)
                .Setup(m => m.AuthorizeDelete()).Throws<AuthorizationException>();

            // Test
            Manager.Delete(null);
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Delete_UsesId_ReturnsMapped()
        {
            // Setup
            long input = 314;
            var repoOutput = new User();
            var mapperOutput = new UserViewModel();
            var specOutput = Specification<User>.Start(c => true);
            Manager.Specs = Mock.Of<IUserSpecificationProvider>(
                sp => sp.ById<User>(It.Is<long>(i => i == input)) == specOutput);
            var am = Mock.Get(Manager.Auth);
            am.Setup(a => a.GenerateFilterDelete()).Returns(specOutput);
            var rm = Mock.Get(Manager.Repo);
            rm.Setup(m => m.Delete(
                    It.Is<ISpecification<User>>(o => o == specOutput), true))
                .Returns(repoOutput)
                .Verifiable();
            Mock.Get(Manager.Mapper)
                .Setup(m => m.Map<IViewModel<User>>(It.IsAny<User>()))
                .Returns(mapperOutput);

            // Test
            var t = Manager.Delete(input);

            // Verify
            rm.Verify();
            Assert.IsNotNull(t);
            Mock.Get(Manager.Specs).Verify();
        }

        [TestMethod, TestCategory("BaseManager")]
        public void Delete_Saves() // save is required, given its a soft delete.
        {
            // Setup
            var order = new List<string>();
            var rm = Mock.Get(Manager.Repo);
            rm.Setup(m => m.Delete(
                    It.IsAny<ISpecification<User>>(), true))
                .Callback<ISpecification<User>, bool, Expression<Func<User, object>>[]>
                    ((c, b ,e) => order.Add("delete"));
            var am = Mock.Get(Manager.Auth);
            am.Setup(a => a.GenerateFilterDelete()).Returns(Specification<User>.All());
            var sm = Mock.Get(Manager.Specs);
            sm.Setup(a => a.ById<User>(It.IsAny<long>())).Returns(Specification<User>.All());
            rm.Setup(m => m.Save(false))
                .Callback((bool b) => order.Add("save"));

            // Test
            Manager.Delete(1);

            // Verify
            Assert.AreEqual("delete", order.First());
            Assert.AreEqual("save", order.Last());
        }
    }
}
