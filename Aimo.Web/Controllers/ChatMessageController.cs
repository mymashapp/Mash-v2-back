using Aimo.Application.Chats;
using Aimo.Domain.Chats;
using Aimo.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Aimo.Web.Controllers;

public class ChatMessageController : ApiBaseController
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IUserContext _userContext;
    private readonly IChatMessageService _chatMessageService;

    public ChatMessageController(IHubContext<ChatHub> hubContext,IUserContext userContext, IChatMessageService chatMessageService)
    {
        _hubContext = hubContext;
        _userContext = userContext;
        _chatMessageService = chatMessageService;
    }

    /*[HttpGet]
    public async Task<IActionResult> GetChatMessages(int chatId)
    {
        var data =JsonConvert.SerializeObject(await _chatMessageService.GetMessagesByChatId(chatId),new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        return new JsonResult(data);
    }*/


    [HttpGet("{id:int}")]
    public async Task<ActionResult> Get(int id)
    {
        var message = await _chatMessageService.GetByIdAsync(id);
        return Ok(message);
    } 
    [HttpGet("{userId:int}")]
    public async Task<ActionResult> GetChatMemberInfoByUserId(int userId)
    {
        var message = await _chatMessageService.GetChatMemberInfoByUserId(userId);
        return Ok(message);
    }
    [HttpGet("{chatId:int}")]
    public async Task<ActionResult> GetChat(int chatId)
    {
        var message = await _chatMessageService.GetChatByIdAsync(chatId);
        return Ok(message);
    }
    [HttpGet("{userId:int}")]
    public async Task<ActionResult> GetChatList(int userId)
    {
        var chatList = await _chatMessageService.GetChatListWithUser(userId);
        return Ok(chatList);
    }

    [HttpGet("{chatId:int}")]
    public async Task<IActionResult> GetMessages(int chatId,int userId)
    {
       //var userId = (await _userContext.GetCurrentUserAsync())?.Id??0;
       //var userId = 0;
        var dto = await _chatMessageService.GetMessagesByChatId(chatId,userId);

        return Ok(dto);
    } 
    
    [HttpGet]
    public async Task<IActionResult> GetPrivateMessages(int senderId,int receiverId)
    {
       //var userId = (await _userContext.GetCurrentUserAsync())?.Id??0;
        var dto = await _chatMessageService.GetPrivateMessages(senderId,receiverId);

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> Create(ChatMessageDto dto)
    {
        var msg = (await _chatMessageService.CreateAsync(dto)) as Result<ChatMessageDto>;

        await _hubContext.Clients.Group(dto.ChatId.ToString()).SendAsync("newMessage", dto);

        //return Created(msg);
        return CreatedAtAction(nameof(Get), new { id = msg?.Data.Id }, dto);
    }
    
    [HttpPost]
    public async Task<ActionResult> SendPrivateMessage(PrivateMessageDto dto)
    {
        
        var msg = (await _chatMessageService.SendPrivateMessages(dto)) as Result<ChatMessageDto>;

        await _hubContext.Clients.Client(dto.ReceiverUserId.ToString()).SendAsync("newMessage", dto);
       
        return CreatedAtAction(nameof(Get), new { id = msg?.Data.Id }, dto);
    }

    [HttpDelete("{chatId}")]
    public async Task<IActionResult> Delete(int chatId)
    {
        return Result(await _chatMessageService.DeleteChatAsync(chatId));
        //return ();
    }
}