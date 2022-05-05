using Aimo.Domain.Users;
using FluentValidation;

namespace Aimo.Application.Users;

public partial class UserPictureDtoValidator : Validator<UserPictureDto>
{
    public UserPictureDtoValidator()
    {
        RuleFor(d => d.UserId).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.PictureType).Must(x=>x == PictureType.Media).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.PictureUrl).NotEmpty().WithMessage(L["Validation.Required"]);
    }
}