using Aimo.Domain.Issues;
using FluentValidation;

namespace Aimo.Application.Issues;

public partial class IssueDtoValidator : Validator<IssueDto>
{
    public IssueDtoValidator()
    {
        RuleFor(d => d.Title).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(250);
        RuleFor(d => d.Description).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.ExceptedBehaviour).NotEmpty().WithMessage(L["Validation.Required"]);
    }
}