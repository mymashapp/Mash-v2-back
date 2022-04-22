using Aimo.Application.Categories;
using Aimo.Domain.Categories;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

public class CategoryController : ApiBaseController
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCategory()
    {
        return Ok(await _categoryService.GetAllCategory());
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        return Result(await _categoryService.CreateAsync(dto));
    }
    [HttpGet]
    public async Task<IActionResult> EditCard(int id)
    {
        return Result(await _categoryService.GetByIdAsync(id));
    }
    [HttpPost]
    public async Task<IActionResult> Update(CategoryDto dto)
    {
        return Result(await _categoryService.UpdateAsync(dto));
    }
}