using Aimo.Domain.Users;
using FluentValidation;

namespace Aimo.Application.Users
{
    public partial class UserDtoValidator:  Validator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(d => d.Name).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(250);
            RuleFor(d => d.Email).NotEmpty().WithMessage(L["Validation.Required"]);
            RuleFor(d => d.DateOfBirth).NotEmpty().WithMessage(L["Validation.Required"]);
            RuleFor(d => d.Gender).NotEmpty().WithMessage(L["Validation.Required"]);
        }
    }
}
