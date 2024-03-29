﻿using Aimo.Domain.Users;
using FluentValidation;

namespace Aimo.Application.Users
{
    public partial class PictureDtoCollectionValidator : Validator<IEnumerable<UserPictureDto>>
    {
        public PictureDtoCollectionValidator()
        {
            RuleFor(d => d)
                .Must(v => !v.GroupBy(g => g.PictureType).Where(x=>x.Key != PictureType.Media)
                    .Any(x => x.Count() > 1)).WithMessage(L["Validation.AllowedOnlyOnePicturePerType"]);
        }
    }
    

    public partial class UserDtoValidator : Validator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(d => d.Name).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(250);
            RuleFor(d => d.Email).NotEmpty().WithMessage(L["Validation.Required"]);
            RuleFor(d => d.DateOfBirth).NotEmpty().WithMessage(L["Validation.Required"]);
            RuleFor(d => d.Gender).NotEmpty().WithMessage(L["Validation.Required"]);
            RuleFor(d => d.UploadedPictures).SetValidator(new PictureDtoCollectionValidator());
        }
    }
}