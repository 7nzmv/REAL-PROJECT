using System.Net;
using AutoMapper;
using Domain.DTOs.Product;
using Domain.Entities;
using Domain.Filtres;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ProductService(IBaseRepository<Product, int> productRepository, IMapper mapper, ILogger<ProductService> logger, IRedisCacheService redisCacheService) : IProductService
{
    public async Task<Response<List<ProductDto>>> GetAllAsync(ProductFilter filter)
    {
        const string cacheKey = "Products";
        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);
        var productsInCache = await redisCacheService.GetData<List<ProductDto>>(cacheKey);

        if (productsInCache == null)
        {
            var Product = await productRepository.GetAllAsync();
            productsInCache = Product.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                OldPrice = p.OldPrice,
                ImageUrl = p.ImageUrl,
                IsNew = p.IsNew,
                IsPromo = p.IsPromo,
                CreatedAt = p.CreatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name
            }).ToList();

            await redisCacheService.SetData(cacheKey, productsInCache, 10);
        }

        if (!string.IsNullOrEmpty(filter.Name))
        {
            productsInCache = productsInCache
                .Where(p => p.Name == filter.Name)
                .ToList();
        }

        if (filter.From.HasValue)
        {
            productsInCache = productsInCache
                .Where(p => p.CreatedAt >= filter.From.Value)
                .ToList();
        }

        if (filter.To.HasValue)
        {
            productsInCache = productsInCache
                .Where(p => p.CreatedAt <= filter.To.Value)
                .ToList();
        }
        var mapped = mapper.Map<List<ProductDto>>(productsInCache);

        var totalRecords = mapped.Count;

        var data = mapped.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize).ToList();

        return new PagedResponse<List<ProductDto>>(data, validFilter.PageNumber, validFilter.PageSize, totalRecords);
    }

    public async Task<Response<ProductDto>> GetByIdAsync(int id)
    {
        logger.LogInformation("GetByIdAsync called with id: {Id}", id);
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            logger.LogWarning("Product not found with id: {Id}", id);
            return new Response<ProductDto>(HttpStatusCode.BadRequest, "Product not found");
        }

        var dto = mapper.Map<ProductDto>(product);

        return new Response<ProductDto>(dto);
    }

    public async Task<Response<ProductDto>> CreateAsync(CreateProductDto productDto)
    {

        logger.LogInformation("CreateAsync called with: {@Request}", productDto);
        var product = mapper.Map<Product>(productDto);

        var result = await productRepository.AddAsync(product);

        var dto = mapper.Map<ProductDto>(product);

        await redisCacheService.RemoveData("Products");

        return result == 0
            ? new Response<ProductDto>(HttpStatusCode.BadRequest, "Product not created")
            : new Response<ProductDto>(dto);
    }

    public async Task<Response<ProductDto>> UpdateAsync(int id, UpdateProductDto productDto)
    {
        logger.LogInformation("Update Product with ID: {Id}, {@Request}", id, productDto);
        var existingProduct = await productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            logger.LogWarning("Product with ID {Id} not found for update", id);
            return new Response<ProductDto>(HttpStatusCode.BadRequest, "Product not found");
        }

        existingProduct.Name = productDto.Name;
        existingProduct.Description = productDto.Description;
        existingProduct.Price = productDto.Price;
        existingProduct.OldPrice = productDto.OldPrice;
        existingProduct.ImageUrl = productDto.ImageUrl;
        existingProduct.IsNew = productDto.IsNew;
        existingProduct.IsPromo = productDto.IsPromo;
        existingProduct.CategoryId = productDto.CategoryId;

        var result = await productRepository.UpdateAsync(existingProduct);


        var dto = mapper.Map<ProductDto>(existingProduct);

        await redisCacheService.RemoveData("Products");

        return result == 0
            ? new Response<ProductDto>(HttpStatusCode.BadRequest, "Product not updated")
            : new Response<ProductDto>(dto);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        logger.LogInformation("Trying to delete Product with ID: {Id}", id);
        var product = await productRepository.GetByIdAsync(id);

        if (product == null)
        {
            logger.LogWarning("Product with ID {Id} not found for delete", id);
            return new Response<string>(HttpStatusCode.NotFound, "Product not found");
        }

        var result = await productRepository.DeleteAsync(product);

        await redisCacheService.RemoveData("Products");

        return result == 0
            ? new Response<string>(HttpStatusCode.BadRequest, "Product not deleted")
            : new Response<string>("Product deleted successfully");
    }
}
