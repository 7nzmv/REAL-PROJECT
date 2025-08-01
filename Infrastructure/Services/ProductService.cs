using System.Net;
using AutoMapper;
using Domain.DTOs.Product;
using Domain.Entities;
using Domain.Filtres;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ProductService(
    IBaseRepository<Product, int> productRepository,
    IMapper mapper,
    ILogger<ProductService> logger,
    IRedisCacheService redisCacheService
) : IProductService
{
    private const string CacheKey = "myapp_Products";

    public async Task<Response<List<ProductDto>>> GetAllAsync(ProductFilter filter)
    {
        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);
        var productsInCache = await redisCacheService.GetData<List<ProductDto>>(CacheKey);

        if (productsInCache == null)
        {
            var products = await productRepository.GetAllAsync();

            productsInCache = products.Select(p => new ProductDto
            {
                Id = p.Id,
                NameRu = p.NameRu,
                NameTg = p.NameTg,
                DescriptionRu = p.DescriptionRu,
                DescriptionTg = p.DescriptionTg,
                Price = p.Price,
                OldPrice = p.OldPrice,
                ImageUrl = p.ImageUrl,
                IsNew = p.IsNew,
                IsPromo = p.IsPromo,
                CreatedAt = p.CreatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                StockQuantity = p.StockQuantity,
                AverageRating = p.AverageRating
            }).ToList();

            await redisCacheService.SetData(CacheKey, productsInCache, 10);
        }

        // фильтрация
        if (!string.IsNullOrEmpty(filter.Name))
            productsInCache = productsInCache.Where(p => p.NameRu.Contains(filter.Name) || p.NameTg.Contains(filter.Name)).ToList();

        if (filter.From.HasValue)
            productsInCache = productsInCache.Where(p => p.CreatedAt >= filter.From.Value).ToList();

        if (filter.To.HasValue)
            productsInCache = productsInCache.Where(p => p.CreatedAt <= filter.To.Value).ToList();

        var totalRecords = productsInCache.Count;

        var data = productsInCache.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                                  .Take(validFilter.PageSize)
                                  .ToList();

        return new PagedResponse<List<ProductDto>>(data, validFilter.PageNumber, validFilter.PageSize, totalRecords);
    }

    public async Task<Response<ProductDto>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
            return new Response<ProductDto>(HttpStatusCode.NotFound, "Product not found");

        var dto = mapper.Map<ProductDto>(product);
        return new Response<ProductDto>(dto);
    }

    public async Task<Response<ProductDto>> CreateAsync(CreateProductDto productDto)
    {
        var product = mapper.Map<Product>(productDto);
        product.AverageRating = 0;

        var result = await productRepository.AddAsync(product);

        await redisCacheService.RemoveData(CacheKey);

        var dto = mapper.Map<ProductDto>(product);

        return result == 0
            ? new Response<ProductDto>(HttpStatusCode.BadRequest, "Product not created")
            : new Response<ProductDto>(dto);
    }

    public async Task<Response<ProductDto>> UpdateAsync(int id, UpdateProductDto productDto)
    {
        var existingProduct = await productRepository.GetByIdAsync(id);
        if (existingProduct == null)
            return new Response<ProductDto>(HttpStatusCode.NotFound, "Product not found");

        // обновляем все поля
        existingProduct.NameRu = productDto.NameRu;
        existingProduct.NameTg = productDto.NameTg;
        existingProduct.DescriptionRu = productDto.DescriptionRu;
        existingProduct.DescriptionTg = productDto.DescriptionTg;
        existingProduct.Price = productDto.Price;
        existingProduct.OldPrice = productDto.OldPrice;
        existingProduct.ImageUrl = productDto.ImageUrl;
        existingProduct.IsNew = productDto.IsNew;
        existingProduct.IsPromo = productDto.IsPromo;
        existingProduct.CategoryId = productDto.CategoryId;
        existingProduct.StockQuantity = productDto.StockQuantity;

        var result = await productRepository.UpdateAsync(existingProduct);

        await redisCacheService.RemoveData(CacheKey);

        var dto = mapper.Map<ProductDto>(existingProduct);

        return result == 0
            ? new Response<ProductDto>(HttpStatusCode.BadRequest, "Product not updated")
            : new Response<ProductDto>(dto);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
            return new Response<string>(HttpStatusCode.NotFound, "Product not found");

        var result = await productRepository.DeleteAsync(product);

        await redisCacheService.RemoveData(CacheKey);

        return result == 0
            ? new Response<string>(HttpStatusCode.BadRequest, "Product not deleted")
            : new Response<string>("Product deleted");
    }
}
