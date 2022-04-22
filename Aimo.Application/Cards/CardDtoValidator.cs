using Aimo.Domain.Cards;
using FluentValidation;

namespace Aimo.Application.Cards;

public partial class CardDtoValidator : Validator<CardDto>
{
    public CardDtoValidator()
    {
        RuleFor(d => d.Name).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(250);
        RuleFor(d => d.Latitude).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.Longitude).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.DateUtc).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.Address).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.Zip).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.CardType).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.CategoryId).NotEmpty().WithMessage(L["Validation.Required"]);
    }
}