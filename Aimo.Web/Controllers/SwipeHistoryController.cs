using Aimo.Application.Cards;
using Aimo.Application.Chats;
using Aimo.Application.SwipeHistories;
using Aimo.Domain.Chats;
using Aimo.Domain.SwipeHistories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Aimo.Web.Controllers;

public class SwipeHistoryController : ApiBaseController
{
    private readonly ISwipeHistoryService _swipeHistoryService;
    private readonly ICardService _cardService;
    private readonly IHubContext<ChatHub> _hubContext;

    public SwipeHistoryController(ISwipeHistoryService swipeHistoryService,ICardService cardService, IHubContext<ChatHub> hubContext)
    {
        _swipeHistoryService = swipeHistoryService;
        _cardService = cardService;
        _hubContext = hubContext;
    }
    
    
    /*[HttpGet]
    public async Task<IActionResult> GetAllSwipeHistory()
    {
        return Ok(await _swipeHistoryService.GetAllSwipeHistory());
    }*/
    [HttpPost]
    public async Task<IActionResult> Create(SwipeHistoryDto dto)
    {
        var result = await _swipeHistoryService.CreateAsync(dto);
        if (result.IsSucceeded)
        {
            var cardName = await _cardService.GetCardNameByIdAsync(dto.CardId);
            if (result.AdditionalData is not Result<ChatDto> additionalData) 
                return Result(result);
           
            foreach (var sendNotificationUserId in additionalData.AdditionalData)
            {
                //await _hubContext.Clients.Group(sendNotificationUserId.ToString()).SendAsync("ReceiveNotification", "your profile is match with another user");
                await ClientProxyExtensions.SendAsync(_hubContext.Clients.Group(sendNotificationUserId.ToString()),
                    "ReceiveNotification", $"Your event {cardName} found a match.” ");
            }
        }
        return Result(result);
    }
    /*[HttpGet]
    public async Task<IActionResult> EditCard(int id)
    {
        return Result(await _swipeHistoryService.GetByIdAsync(id));
    }
    [HttpPost]
    public async Task<IActionResult> Update(SwipeHistoryDto dto)
    {
        return Result(await _swipeHistoryService.UpdateAsync(dto));
    }*/

}