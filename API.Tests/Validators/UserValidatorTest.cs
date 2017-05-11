using API.Exceptions;
using API.Models.InputModels;
using API.Validators;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace API.Tests.Validators
{
    [TestClass]
    public class UserValidatorTest
    {
        public UserValidator Validator { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Validator = new UserValidator();
        }

        [TestMethod, TestCategory("Validator_User")]
        public void Validate_NoThrowsWhenValid()
        {
            var i = new UserInputModel()
            {
                Username = "myuser"
            };

            Validator.Validate(i);
        }

        [TestMethod, TestCategory("Validator_User"),
        ExpectedException(typeof(NotImplementedException))]
        public void Validate_ThrowsWhenInvalid_InputNull()
        {
            Validator.Validate(null);
        }

        [TestMethod, TestCategory("Validator_User"),
        ExpectedException(typeof(InvalidInputException))]
        public void Validate_ThrowsWhenInvalid_NoUsername_Null()
        {
            var i = new UserInputModel();

            Validator.Validate(i);
        }

        [TestMethod, TestCategory("Validator_User"),
        ExpectedException(typeof(InvalidInputException))]
        public void Validate_ThrowsWhenInvalid_NoUsername_Empty()
        {
            var i = new UserInputModel()
            {
                Username=""
            };

            Validator.Validate(i);
        }
    }
}
