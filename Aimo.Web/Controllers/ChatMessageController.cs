using Aimo.Application.Chats;
using Aimo.Application.Interests;
using Aimo.Domain.Chats;
using Aimo.Domain.Interests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Aimo.Web.Controllers;

public class ChatMessageController : ApiBaseController
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IChatMessageService _chatMessageService;

    public ChatMessageController(IHubContext<ChatHub> hubContext, IChatMessageService chatMessageService)
    {
        _hubContext = hubContext;
        _chatMessageService = chatMessageService;
    }
    
    
    
    
    [Route("send")]                                           //path looks like this: https://localhost:44379/api/chat/send
    [HttpPost]
    public IActionResult SendRequest([FromBody] ChatMessageDto msg)
    {
        _hubContext.Clients.All.SendAsync("ReceiveOne", msg.UserName, msg.Text);
        return Ok();
    }
    
    /*[HttpGet]
    public async Task<IActionResult> GetAllInterests()
    {
        return Ok(await _chatMessageService.GetAllChatMessages());
    }
    [HttpPost]
    public async Task<IActionResult> Create(ChatMessageDto dto)
    {
        return Result(await _chatMessageService.CreateAsync(dto));
    }
    [HttpGet]
    public async Task<IActionResult> GetChat(int id)
    {
        return Result(await _chatMessageService.GetByIdAsync(id));
    }
    [HttpPost]
    public async Task<IActionResult> Update(ChatMessageDto dto)
    {
        return Result(await _chatMessageService.UpdateAsync(dto));
    }*/
}