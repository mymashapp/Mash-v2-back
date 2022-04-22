using Aimo.Domain.Categories;
using FluentValidation;

namespace Aimo.Application.Categories;

public partial class CategoryDtoValidator : Validator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(d => d.Name).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(250);
        RuleFor(d => d.DisplayOrder).NotEmpty().WithMessage(L["Validation.Required"]);
       
    }
}