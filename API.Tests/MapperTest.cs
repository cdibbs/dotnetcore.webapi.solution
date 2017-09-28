using System;
using API.Mapping;
using API.Models;
using API.Models.InputModels;
using API.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using Data;
using Data.Models;
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
    }
}
