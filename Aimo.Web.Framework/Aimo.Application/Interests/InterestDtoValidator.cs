using Aimo.Domain.Interests;
using FluentValidation;

namespace Aimo.Application.Interests;

public partial class InterestDtoValidator : Validator<InterestDto>
{
    public InterestDtoValidator()
    {
        RuleFor(d => d.Name).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(250);
        RuleFor(d => d.DisplayOrder).NotEmpty().WithMessage(L["Validation.Required"]);
       
    }
}