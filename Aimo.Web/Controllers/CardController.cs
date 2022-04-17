using Aimo.Application.Card;
using Aimo.Domain.Card;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

public class CardController : ApiBaseController
{
    private readonly ICardService _cardService;

    public CardController(ICardService cardService)
    {
        _cardService = cardService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CardDto dto)
    {
        return Result(await _cardService.Create(dto));
    }
    [HttpGet]
    public async Task<IActionResult> EditCard(int id)
    {
        return Result(await _cardService.GetById(id));
    }
    [HttpPost]
    public async Task<IActionResult> Update(CardDto dto)
    {
        return Result(await _cardService.Update(dto));
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        return Result(await _cardService.Delete(id));
    }
}