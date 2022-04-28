using Aimo.Domain.SwipeHistories;
using FluentValidation;

namespace Aimo.Application.SwipeHistories;

public partial class SwipeHistoryDtoValidator : Validator<SwipeHistoryDto>
{
    public SwipeHistoryDtoValidator()
    {
        RuleFor(d => d.CardId).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.UserId).NotEmpty().WithMessage(L["Validation.Required"]);
       // RuleFor(d => d.SwipeType).mu().WithMessage(L["Validation.Required"]);
       
    }
}

public partial class SwipeGroupDtoValidator : Validator<SwipeGroupDto>
{
    public SwipeGroupDtoValidator()
    {
        RuleFor(d => d.AgeFrom).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.AgeTo).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.UserId).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.GroupType).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.Gender).NotEmpty().WithMessage(L["Validation.Required"]);
       
    }
}
public partial class SwipeGroupInterestDtoValidator : Validator<SwipeGroupInterestDto>
{
    public SwipeGroupInterestDtoValidator()
    {
        RuleFor(d => d.SwipeGroupId).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.InterestId).NotEmpty().WithMessage(L["Validation.Required"]);
    }
}