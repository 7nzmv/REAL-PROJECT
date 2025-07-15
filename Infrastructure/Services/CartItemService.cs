using System.Net;
using AutoMapper;
using Domain.DTOs.CartItem;
using Domain.Entities;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class CartItemService(
    IBaseRepositoryWithInclude<CartItem, int> cartItemRepository,
    IBaseRepository<Product, int> productRepository,
    IMapper mapper,
    ILogger<CartItemService> logger,
    IRedisCacheService redisCacheService
) : ICartItemService
{
    private const string CacheKeyPrefix = "myapp_CartItems_";

    public async Task<Response<List<CartItemDto>>> GetAllByUserIdAsync(string userId)
    {
        var cacheKey = CacheKeyPrefix + userId;
        var cachedItems = await redisCacheService.GetData<List<CartItemDto>>(cacheKey);
        if (cachedItems != null)
        {
            logger.LogInformation($"Returning cached cart items for user {userId}");
            return new Response<List<CartItemDto>>(cachedItems);
        }

        var items = await cartItemRepository.GetAllIncludingAsync(x => x.Product);
        var userItems = items.Where(ci => ci.UserId == userId).ToList();

        var mapped = userItems.Select(ci =>
        {
            var dto = mapper.Map<CartItemDto>(ci);
            dto.ProductName = ci.Product.Name;
            return dto;
        }).ToList();

        await redisCacheService.SetData(cacheKey, mapped, 10);

        return new Response<List<CartItemDto>>(mapped);
    }

    public async Task<Response<CartItemDto>> GetByIdAsync(int id)
    {
        var item = await cartItemRepository.GetByIdIncludingAsync(id, x => x.Product);
        if (item == null)
        {
            return new Response<CartItemDto>(HttpStatusCode.NotFound, "Cart item not found");
        }

        var dto = mapper.Map<CartItemDto>(item);
        dto.ProductName = item.Product.Name;

        return new Response<CartItemDto>(dto);
    }

    public async Task<Response<CartItemDto>> CreateAsync(CreateCartItemDto dto, string userId)
    {
        var product = await productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
        {
            return new Response<CartItemDto>(HttpStatusCode.BadRequest, "Product not found");
        }

        var existingItems = await cartItemRepository.GetAllAsync();
        var existingItem = existingItems.FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == dto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            var updateResult = await cartItemRepository.UpdateAsync(existingItem);
            if (updateResult == 0)
                return new Response<CartItemDto>(HttpStatusCode.BadRequest, "Failed to update existing cart item");

            await redisCacheService.RemoveData(CacheKeyPrefix + userId);
            return new Response<CartItemDto>(mapper.Map<CartItemDto>(existingItem));
        }

        var cartItem = new CartItem
        {
            UserId = userId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
        };

        var result = await cartItemRepository.AddAsync(cartItem);
        if (result == 0)
            return new Response<CartItemDto>(HttpStatusCode.BadRequest, "Failed to add cart item");

        await redisCacheService.RemoveData(CacheKeyPrefix + userId);

        var createdDto = mapper.Map<CartItemDto>(cartItem);
        createdDto.ProductName = product.Name;

        return new Response<CartItemDto>(createdDto);
    }

    public async Task<Response<CartItemDto>> UpdateAsync(int id, UpdateCartItemDto dto, string userId)
    {
        var item = await cartItemRepository.GetByIdIncludingAsync(id, x => x.Product);
        if (item == null || item.UserId != userId)
        {
            return new Response<CartItemDto>(HttpStatusCode.NotFound, "Cart item not found or access denied");
        }

        item.Quantity = dto.Quantity;

        var updateResult = await cartItemRepository.UpdateAsync(item);
        if (updateResult == 0)
            return new Response<CartItemDto>(HttpStatusCode.BadRequest, "Failed to update cart item");

        await redisCacheService.RemoveData(CacheKeyPrefix + userId);

        var updatedDto = mapper.Map<CartItemDto>(item);
        updatedDto.ProductName = item.Product.Name;

        return new Response<CartItemDto>(updatedDto);
    }

    public async Task<Response<string>> DeleteAsync(int id, string userId)
    {
        var item = await cartItemRepository.GetByIdAsync(id);
        if (item == null || item.UserId != userId)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Cart item not found or access denied");
        }

        var result = await cartItemRepository.DeleteAsync(item);
        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to delete cart item");

        await redisCacheService.RemoveData(CacheKeyPrefix + userId);

        return new Response<string>("Cart item deleted successfully");
    }
}
