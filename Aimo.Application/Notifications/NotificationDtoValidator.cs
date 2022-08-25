//#TODO: Fix Auto Generated Code According to your requirements
//# Auto generated file : To prevent generation add [DontGenerate] to domain class #//

using Aimo.Domain.Notifications;
using FluentValidation;

namespace Aimo.Application.Notifications;
public partial class NotificationDtoValidator : Validator<NotificationDto>
{
    #region ctor
    public NotificationDtoValidator()
    {
		RuleFor(d => d.UserId).NotEmpty().WithMessage(L["Validation.Required"]);
		RuleFor(d => d.NotificationTypeId).NotEmpty().WithMessage(L["Validation.Required"]);
		RuleFor(d => d.Message).NotEmpty().WithMessage(L["Validation.Required"]).MaximumLength(400);

    }
    #endregion
}