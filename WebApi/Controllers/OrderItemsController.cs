using Domain.Constants;
using Domain.DTOs.OrderItem;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController(IOrderItemService orderItemService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<List<OrderItemDto>>> GetAllAsync([FromQuery] int orderId)
    {
        return await orderItemService.GetAllAsync(orderId);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<Response<OrderItemDto>> GetByIdAsync(int id)
    {
        return await orderItemService.GetByIdAsync(id);
    }

    [HttpPost("{orderId}")]
    [Authorize]
    public async Task<Response<OrderItemDto>> CreateAsync(int orderId, [FromBody] CreateOrderItemDto createDto)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
            return new Response<OrderItemDto>(System.Net.HttpStatusCode.Unauthorized, "User not authorized");

        return await orderItemService.CreateAsync(createDto, orderId, userId);
    }


    [HttpPut("{id}")]
    [Authorize]
    public async Task<Response<OrderItemDto>> UpdateAsync(int id, [FromBody] UpdateOrderItemDto updateDto)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
            return new Response<OrderItemDto>(System.Net.HttpStatusCode.Unauthorized, "User not authorized");

        return await orderItemService.UpdateAsync(id, updateDto, userId);
    }


    [HttpDelete("{id}")]
    [Authorize]
    public async Task<Response<string>> DeleteAsync(int id)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
            return new Response<string>(System.Net.HttpStatusCode.Unauthorized, "User not authorized");

        return await orderItemService.DeleteAsync(id, userId);
    }
}
