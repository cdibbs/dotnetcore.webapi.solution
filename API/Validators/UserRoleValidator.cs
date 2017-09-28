using API.Exceptions;
using API.Models.InputModels;
using Data;
using System;

namespace API.Validators
{
    public class UserRoleValidator : IValidator<UserRole>
    {
        public void Validate(IInputModel<UserRole, long> input)
        {
            var i = input as UserRoleInputModel;
            if (i == null)
                throw new NotImplementedException($"Validation is not implemented.");

            if (i.UserId <= 0)
                throw new InvalidInputException("UserId is invalid.");

            if (i.RoleId <= 0)
                throw new InvalidInputException("RoleId is invalid.");
        }

    }
}