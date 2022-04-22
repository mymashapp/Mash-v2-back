using Aimo.Application.Cards;
using Aimo.Data.Infrastructure.Yelp;
using Aimo.Domain.Cards;
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
        return Result(await _cardService.CreateAsync(dto));
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
    public async Task<IActionResult> Delete(int id)
    {
        return Result(await _cardService.DeleteAsync(id));
    }
}