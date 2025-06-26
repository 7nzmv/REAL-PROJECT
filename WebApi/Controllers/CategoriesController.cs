using Domain.DTOs.Category;
using Domain.DTOs.Product;
using Domain.Filtres;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{

    [HttpGet]
    // [Authorize(Roles = Roles.Admin)]
    public async Task<Response<List<CategoryDto>>> GetAllAsync([FromQuery] CategoryFilter filter)
    {
        return await categoryService.GetAllAsync(filter);
    }


    [HttpGet("{id}")]
    // [Authorize(Roles = Roles.Admin)]
    public async Task<Response<CategoryDto>> GetByIdAsync(int id)
    {
        return await categoryService.GetByIdAsync(id);
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = Roles.Admin)]
    public async Task<Response<CategoryDto>> UpdateAsync(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        return await categoryService.UpdateAsync(id, updateCategoryDto);
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> DeleteAsync(int id)
    {
        return await categoryService.DeleteAsync(id);
    }

    [HttpPost]
    // [Authorize(Roles = Roles.Admin)]
    public async Task<Response<CategoryDto>> CreateAsync(CreateCategoryDto categoryDto)
    {
        return await categoryService.CreateAsync(categoryDto);
    }

}


