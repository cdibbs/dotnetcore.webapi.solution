using System;
using Data;
using API.Exceptions;
using API.Models;
using API.Models.InputModels;
using API.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace API.Tests.Validators
{
    [TestClass]
    public class UserRoleValidatorTest
    {
        public UserRoleValidator Validator { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Validator = new UserRoleValidator();
        }

        [TestMethod, TestCategory("Validator_UserRole")]
        public void Validate_NoThrowsWhenValid()
        {
            var i = new UserRoleInputModel()
            {
                Id=1,
                UserId=2,
                RoleId=3
            };

            Validator.Validate(i);
        }

        [TestMethod, TestCategory("Validator_UserRole"),
        ExpectedException(typeof(NotImplementedException))]
        public void Validate_ThrowsWhenInvalid_InputNull()
        {
            Validator.Validate(null);
        }

        [TestMethod, TestCategory("Validator_UserRole"),
        ExpectedException(typeof(InvalidInputException))]
        public void Validate_ThrowsWhenInvalid_NoId_Null()
        {
            var i = new UserRoleInputModel();

            Validator.Validate(i);
        }

        [TestMethod, TestCategory("Validator_UserRole"),
        ExpectedException(typeof(InvalidInputException))]
        public void Validate_ThrowsWhenInvalid_UserId_Invalid()
        {
            var i = new UserRoleInputModel()
            {
                Id = 1,
                UserId = -2,
                RoleId = 3
            };

            Validator.Validate(i);
        }

        [TestMethod, TestCategory("Validator_UserRole"),
        ExpectedException(typeof(InvalidInputException))]
        public void Validate_ThrowsWhenInvalid_RoleId_Invalid()
        {
            var i = new UserRoleInputModel()
            {
                Id = 1,
                UserId = 2,
                RoleId = -3
            };

            Validator.Validate(i);
        }


    }
}
