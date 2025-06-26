using System.Net;
using AutoMapper;
using Domain.DTOs.Category;
using Domain.Entities;
using Domain.Filtres;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class CategoryService(IBaseRepository<Category, int> categoryRepository, IMapper mapper, ILogger<ProductService> logger, IRedisCacheService redisCacheService) : ICategoryService
{
     public async Task<Response<List<CategoryDto>>> GetAllAsync(CategoryFilter filter)
    {
        const string cacheKey = "Categories";
        var categoriesInCache = await redisCacheService.GetData<List<CategoryDto>>(cacheKey);

        if (categoriesInCache == null)
        {
            var categories = await categoryRepository.GetAllAsync();
            categoriesInCache = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null,
                ProductCount = c.Products.Count,
                SubCategories = c.SubCategories.Select(sc => new CategoryDto
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    ParentCategoryId = sc.ParentCategoryId
                }).ToList()
            }).ToList();

            await redisCacheService.SetData(cacheKey, categoriesInCache, 10);
        }

        if (!string.IsNullOrEmpty(filter.Name))
        {
            categoriesInCache = categoriesInCache
                .Where(c => c.Name == filter.Name)
                .ToList();
        }

        var data = mapper.Map<List<CategoryDto>>(categoriesInCache);

        return new Response<List<CategoryDto>>(data);
    }

    public async Task<Response<CategoryDto>> GetByIdAsync(int id)
    {
        logger.LogInformation("GetByIdAsync called with id: {Id}", id);
        var category = await categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            logger.LogWarning("Category with ID {Id} not found", id);
            return new Response<CategoryDto>(HttpStatusCode.BadRequest, "Category not found");
        }

        var dto = mapper.Map<CategoryDto>(category);

        return new Response<CategoryDto>(dto);
    }

    public async Task<Response<CategoryDto>> CreateAsync(CreateCategoryDto dto)
    {
        logger.LogInformation("CreateAsync called with: {@Request}", dto);
        var category = mapper.Map<Category>(dto);

        var result = await categoryRepository.AddAsync(category);

        var mapped = mapper.Map<CategoryDto>(category);

        await redisCacheService.RemoveData("Categories");

        return result == 0
            ? new Response<CategoryDto>(HttpStatusCode.BadRequest, "Category not created")
            : new Response<CategoryDto>(mapped);
    }

    public async Task<Response<CategoryDto>> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        logger.LogInformation("UpdateAsync called with ID: {Id}, {@Request}", id, dto);
        var existing = await categoryRepository.GetByIdAsync(id);
        if (existing == null)
        {
            logger.LogWarning("Category with ID {Id} not found for update", id);
            return new Response<CategoryDto>(HttpStatusCode.BadRequest, "Category not found");
        }

        existing.Name = dto.Name;
        existing.ParentCategoryId = dto.ParentCategoryId;

        var result = await categoryRepository.UpdateAsync(existing);

        var mapped = mapper.Map<CategoryDto>(existing);

        await redisCacheService.RemoveData("Categories");

        return result == 0
            ? new Response<CategoryDto>(HttpStatusCode.BadRequest, "Category not updated")
            : new Response<CategoryDto>(mapped);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        logger.LogInformation("Trying to delete Category with ID: {Id}", id);
        var category = await categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            logger.LogWarning("Category with ID {Id} not found for delete", id);
            return new Response<string>(HttpStatusCode.NotFound, "Category not found");
        }

        var result = await categoryRepository.DeleteAsync(category);

        await redisCacheService.RemoveData("Categories");

        return result == 0
            ? new Response<string>(HttpStatusCode.BadRequest, "Category not deleted")
            : new Response<string>("Category deleted successfully");
    }
}

