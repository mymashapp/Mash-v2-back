using Aimo.Application.Interests;
using Aimo.Domain.Interests;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

public class InterestController : ApiBaseController
{
    private readonly IInterestService _interestService;

    public InterestController(IInterestService interestService)
    {
        _interestService = interestService;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetAllInterests()
    {
        return Ok(await _interestService.GetAllInterests());
    }
    [HttpPost]
    public async Task<IActionResult> Create(InterestDto dto)
    {
        return Result(await _interestService.CreateAsync(dto));
    }
    [HttpGet]
    public async Task<IActionResult> EditCard(int id)
    {
        return Result(await _interestService.GetByIdAsync(id));
    }
    [HttpPost]
    public async Task<IActionResult> Update(InterestDto dto)
    {
        return Result(await _interestService.UpdateAsync(dto));
    }
}