using Aimo.Application.Cards;
using Aimo.Application.SwipeHistories;
using Aimo.Domain.Cards;
using Aimo.Domain.Chats;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

public class CardController : ApiBaseController
{
    private readonly ICardService _cardService;
    private readonly ISwipeHistoryService _swipeHistoryService;

    public CardController(ICardService cardService,ISwipeHistoryService swipeHistoryService)
    {
        _cardService = cardService;
        _swipeHistoryService = swipeHistoryService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CardDto dto)
    {
        var result = await _cardService.CreateAsync(dto);
        //add card RightSwipe
        await _swipeHistoryService.CardRightSwipe(result.Data);
        return Result(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        return Result(await _cardService.GetByIdAsync(id));
    }
    [HttpPost]
    public async Task<IActionResult> GetCards(CardSearchDto dto, CancellationToken ct = default)
    {
        return Result(await _cardService.GetCardsAsync(dto,ct));
    }
    [HttpPost]
    public async Task<IActionResult> Update(CardDto dto)
    {
        return Result(await _cardService.UpdateAsync(dto));
    }
    
    [HttpPost]
    public async Task<IActionResult> ReMatch(ReMatchDto dto)
    {
        return Result(await _swipeHistoryService.ReMatchUser(dto));
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        return Result(await _cardService.DeleteAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> ReportCard(int cardId)
    {
        return Result(await _cardService.ReportCardAsync(cardId));
    }
}