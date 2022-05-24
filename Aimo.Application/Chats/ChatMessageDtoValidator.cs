using Aimo.Domain.Chats;
using FluentValidation;

namespace Aimo.Application.Chats;

public partial class ChatMessageDtoValidator:Validator<ChatMessageDto>
{
    public ChatMessageDtoValidator()
    {
        RuleFor(d => d.Text).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.SenderUserId).NotEmpty().WithMessage(L["Validation.Required"]);
        RuleFor(d => d.ChatId).NotEmpty().WithMessage(L["Validation.Required"]);
    }
}