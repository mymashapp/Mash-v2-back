using Aimo.Core.Specifications;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;

namespace Aimo.Application.Chats;

internal partial class ChatMessageService:IChatMessageService
{
    private readonly IRepository<ChatMessage> _chatMessageRepository;
    private readonly ChatMessageDtoValidator _chatMessageDtoValidator;

    public ChatMessageService(IRepository<ChatMessage> chatMessageRepository,ChatMessageDtoValidator chatMessageDtoValidator)
    {
        _chatMessageRepository = chatMessageRepository;
        _chatMessageDtoValidator = chatMessageDtoValidator;
    }
    
    
    public async Task<IdNameDto[]> GetAllChatMessages()
    {
        return (await _chatMessageRepository.FindAsync(x => true)).Map<IdNameDto[]>();
    }
    
     public async ResultTask GetByIdAsync(int id)
    {
        var result = Result.Create(new ChatMessageDto());
        var entity = await _chatMessageRepository.GetByIdAsync(id);
        return entity is not null
            ? result.SetData(entity.Map<ChatMessageDto>()).Success()
            : result.Failure(ResultMessage.NotFound);
    }

    public async ResultTask CreateAsync(ChatMessageDto dto)
    {
        var result = await _chatMessageDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var entity = dto.Map<ChatMessage>();
            await _chatMessageRepository.AddAsync(entity);
            var affected = await _chatMessageRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    public async ResultTask UpdateAsync(ChatMessageDto dto)
    {
        var result = await _chatMessageDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;
        try
        {
            var entity = await _chatMessageRepository.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity is null)
                return result.Failure(ResultMessage.NotFound);

            dto.MapTo(entity);

            _chatMessageRepository.Update(entity);
            await _chatMessageRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto)).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }


    public async ResultTask DeleteAsync(params int[] ids)
    {
        var result = Result.Create(false);
        try
        {
            var entity = await _chatMessageRepository.FindBySpecAsync(new ByIdsSpec<ChatMessage>(ids));

            if (!entity.IsNullOrEmpty())
                return result.Failure(ResultMessage.NotFound);
            _chatMessageRepository.RemoveBulk(entity);
            var affected = await _chatMessageRepository.CommitAsync();
            return result.SetData(affected > 0, affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

}

public partial interface IChatMessageService
{
   
    Task<IdNameDto[]> GetAllChatMessages();
    ResultTask GetByIdAsync(int id);
    ResultTask CreateAsync(ChatMessageDto dto);
    ResultTask UpdateAsync(ChatMessageDto viewDto);
}