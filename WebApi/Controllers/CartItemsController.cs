using System.Security.Claims;
using Domain.Constants;
using Domain.DTOs.CartItem;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartItemsController(ICartItemService cartItemService) : ControllerBase
{
    // получить все элементы корзины пользователя (текущего)
    [HttpGet]
    [Authorize]
    public async Task<Response<List<CartItemDto>>> GetAllByUserAsync()
    {
        // var userId = User.Identity?.Name;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return new Response<List<CartItemDto>>(System.Net.HttpStatusCode.Unauthorized, "User not authorized");

        return await cartItemService.GetAllByUserIdAsync(userId);
    }

    // получить элемент корзины по id (у пользователя)
    [HttpGet("{id}")]
    [Authorize]
    public async Task<Response<CartItemDto>> GetByIdAsync(int id)
    {
        return await cartItemService.GetByIdAsync(id);
    }

    // добавить товар в корзину
    [HttpPost]
    [Authorize]
    public async Task<Response<CartItemDto>> CreateAsync([FromBody] CreateCartItemDto dto)
    {
        // var userId = User.Identity?.Name;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return new Response<CartItemDto>(System.Net.HttpStatusCode.Unauthorized, "User not authorized");

        return await cartItemService.CreateAsync(dto, userId);
    }

    // обновить количество товара в корзине
    [HttpPut("{id}")]
    [Authorize]
    public async Task<Response<CartItemDto>> UpdateAsync(int id, [FromBody] UpdateCartItemDto dto)
    {
        // var userId = User.Identity?.Name;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return new Response<CartItemDto>(System.Net.HttpStatusCode.Unauthorized, "User not authorized");

        return await cartItemService.UpdateAsync(id, dto, userId);
    }

    // удалить товар из корзины
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<Response<string>> DeleteAsync(int id)
    {
        // var userId = User.Identity?.Name;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return new Response<string>(System.Net.HttpStatusCode.Unauthorized, "User not authorized");

        return await cartItemService.DeleteAsync(id, userId);
    }
}
