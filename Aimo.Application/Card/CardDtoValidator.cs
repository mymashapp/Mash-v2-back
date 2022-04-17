using Aimo.Domain.Card;
using FluentValidation;

namespace Aimo.Application.Card;

public partial class CardDtoValidator : Validator<CardDto>
{
    public CardDtoValidator()
    {
        RuleFor(d => d.EventName).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(250);
        RuleFor(d => d.latitude).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.longitude).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.DateUTC).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.Address).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.Zip).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.CardType).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.CategoryId).NotEmpty().WithMessage(L["Validation.Required"]);
    }
}