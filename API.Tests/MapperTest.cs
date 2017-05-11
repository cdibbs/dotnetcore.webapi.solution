using System;
using API.Mapping;
using API.Models;
using API.Models.InputModels;
using API.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using Data;
using Data.Repositories.ReadOnly;

namespace API.Tests
{
    /// The intention, here, is to catch basic mapping regression errors.
    /// After an update to the mapper definitions, its very common to have
    /// mismatched types, missed properties, etc.
    [TestClass]
    public class MapperTest
    {
        public MapperConfiguration MapperConfig { get; set; }
        public IMapper Mapper { get; set; }
        public DateTime Now => new DateTime(2016, 3, 14);

        [TestInitialize]
        public void Initialize()
        {
            MapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new APIMappingProfile());
            });
            Mapper = MapperConfig.CreateMapper();
        }

        [TestMethod, TestCategory("Mapper")]
        public void User_MapsWithoutError()
        {
            var min = new User()
            {
                Id = 314,
                Username="uiowaHawk",
                Created = Now,
                LastUpdated = Now.AddDays(4),
                LastUpdatedBy = 3,
                IsDeleted = false
            };
            var mout = Mapper.Map<UserViewModel>(min);

            //As this evolves, update, possibly assert on properties.
            Assert.AreEqual(min.Id, mout.Id);
            Assert.AreEqual(min.Username, mout.Username);

            var iout = Mapper.Map<IViewModel<User>>(min);

        }

        [TestMethod, TestCategory("Mapper")]
        public void UserInput_MapsWithoutError()
        {
            var min = new UserInputModel()
            {
                Username = "uiowaHawkeye",
            };
            var mout = Mapper.Map<User>(min);

            // TODO: As this evolves, update, possibly assert on properties.
            Assert.AreEqual(min.Username, mout.Username);
        }

        [TestMethod, TestCategory("Mapper")]
        public void V_Population_ViewModel_MapsWithoutError()
        {
            var min = new V_Population()
            {
                Username = "one", UserId = "000001",
                First = "Seven", Middle = "Of", Last = "Nine",
                Email = "we@borg.com",
                Title = "Mind of the Hive, Hive Mind"
            };
            var mout = (PersonViewModel)Mapper.Map<IViewModel<V_Population, string>>(min);

            // TODO: As this evolves, update, possibly assert on properties.
            Assert.AreEqual(min.Username, mout.Username);
            Assert.AreEqual(min.UserId, mout.Id);
            Assert.AreEqual(min.First, mout.First);
            Assert.AreEqual(min.Middle, mout.Middle);
            Assert.AreEqual(min.Last, mout.Last);
            Assert.AreEqual(min.Email, mout.Email);
            Assert.AreEqual(min.Title, mout.Title);
        }
    }
}
