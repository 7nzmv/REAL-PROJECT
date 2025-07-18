using Domain.Constants;
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
public class ProductsController(IProductService productService, IWebHostEnvironment environment) : ControllerBase
{

    [HttpGet]
    public async Task<Response<List<ProductDto>>> GetAllAsync([FromQuery] ProductFilter filter)
    {
        return await productService.GetAllAsync(filter);
    }


    [HttpGet("{id}")]
    public async Task<Response<ProductDto>> GetByIdAsync(int id)
    {
        return await productService.GetByIdAsync(id);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<ProductDto>> UpdateAsync(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        return await productService.UpdateAsync(id, updateProductDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> DeleteAsync(int id)
    {
        return await productService.DeleteAsync(id);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<ProductDto>> CreateAsync(CreateProductDto productDto)
    {
        return await productService.CreateAsync(productDto);
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageDto dto, [FromServices] IWebHostEnvironment environment)
    {
        var file = dto.File;

        if (file == null || file.Length == 0)
            return BadRequest("Файл не выбран");

        var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = Path.GetFileName(file.FileName).Replace(" ", "_");
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{Uri.EscapeDataString(fileName)}";

        return Ok(new { url = imageUrl });
    }


}

