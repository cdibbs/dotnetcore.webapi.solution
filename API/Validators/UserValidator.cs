using API.Exceptions;
using API.Models.InputModels;
using Data;
using System;

namespace API.Validators
{
    public class UserValidator : IValidator<User>
    {
        public void Validate(IInputModel<User, long> input)
        {
            var i = input as UserInputModel;
            if (i == null)
                throw new NotImplementedException($"Validation is not implemented.");

            if (String.IsNullOrWhiteSpace(i.Username))
                throw new InvalidInputException("Username cannot be empty.");
        }
    }
}