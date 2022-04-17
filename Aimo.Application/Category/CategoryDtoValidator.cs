using Aimo.Domain.Category;
using FluentValidation;

namespace Aimo.Application.Category;

public partial class CategoryDtoValidator : Validator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(d => d.Name).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(250);
        RuleFor(d => d.DisplayOrder).NotEmpty().WithMessage(L["Validation.Required"]);
       
    }
}

