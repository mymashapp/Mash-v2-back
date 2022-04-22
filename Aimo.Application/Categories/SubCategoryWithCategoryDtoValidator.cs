using Aimo.Domain.Categories;
using FluentValidation;

namespace Aimo.Application.Categories;

public partial class SubCategoryWithCategoryDtoValidator : Validator<SubCategoryWithCategoryDto>
{
    public SubCategoryWithCategoryDtoValidator()
    {
        RuleFor(d => d.Alias).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(250);
        RuleFor(d => d.Title).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(250);
        RuleFor(d => d.CategoryId).NotNull().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.DisplayOrder).NotEmpty().WithMessage(L["Validation.Required"]);
       
    }
}