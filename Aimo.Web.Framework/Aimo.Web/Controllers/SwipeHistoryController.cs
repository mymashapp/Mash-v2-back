using Aimo.Application.SwipeHistories;
using Aimo.Domain.SwipeHistories;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

public class SwipeHistoryController : ApiBaseController
{
    private readonly ISwipeHistoryService _swipeHistoryService;

    public SwipeHistoryController(ISwipeHistoryService swipeHistoryService)
    {
        _swipeHistoryService = swipeHistoryService;
    }
    
    
    /*[HttpGet]
    public async Task<IActionResult> GetAllSwipeHistory()
    {
        return Ok(await _swipeHistoryService.GetAllSwipeHistory());
    }*/
    [HttpPost]
    public async Task<IActionResult> Create(SwipeHistoryDto dto)
    {
        return Result(await _swipeHistoryService.CreateAsync(dto));
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