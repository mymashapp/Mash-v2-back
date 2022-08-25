using Aimo.Domain.Users.Entities;
using FluentValidation;

namespace Aimo.Application.Users;

public partial class UserLocationDtoValidator : Validator<UserLocationDto>
{
    public UserLocationDtoValidator()
    {
        RuleFor(d => d.UserId).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.Longitude).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.Latitude).NotEmpty().WithMessage(L["Validation.Required"]);
    }
}