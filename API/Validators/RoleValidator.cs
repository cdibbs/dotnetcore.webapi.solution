using System;
using Data;
using API.Models.InputModels;

namespace API.Validators
{
    public class RoleValidator : IValidator<Role>
    {
        public void Validate(IInputModel<Role, long> input)
        {
                throw new NotImplementedException($"Validation is not implemented (ATOW creation of roles was not implemented).");
        }
    }
}