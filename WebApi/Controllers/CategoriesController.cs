using Domain.Constants;
using Domain.DTOs.Category;
using Domain.DTOs.Product;
using Domain.Filtres;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService, IWebHostEnvironment environment) : ControllerBase
{

    [HttpGet]
    public async Task<Response<List<CategoryDto>>> GetAllAsync([FromQuery] CategoryFilter filter)
    {
        return await categoryService.GetAllAsync(filter);
    }


    [HttpGet("{id}")]
    public async Task<Response<CategoryDto>> GetByIdAsync(int id)
    {
        return await categoryService.GetByIdAsync(id);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<CategoryDto>> UpdateAsync(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        return await categoryService.UpdateAsync(id, updateCategoryDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> DeleteAsync(int id)
    {
        return await categoryService.DeleteAsync(id);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<CategoryDto>> CreateAsync(CreateCategoryDto categoryDto)
    {
        return await categoryService.CreateAsync(categoryDto);
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageDto dto)
    {
        var file = dto.File;

        if (file == null || file.Length == 0)
            return BadRequest("Файл не выбран");

        // получаем путь к wwwroot/uploads
        var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");

        // создаём папку, если её нет
        Directory.CreateDirectory(uploadsFolder);

        // формируем безопасное имя файла (убираем пробелы и запрещённые символы)
        var fileName = Path.GetFileName(file.FileName).Replace(" ", "_");

        var filePath = Path.Combine(uploadsFolder, fileName);

        // сохраняем файл
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // формируем url для доступа к файлу
        var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

        return Ok(new { url = imageUrl });
    }


}


