using System.Net;
using Domain.Constants;
using Domain.DTOs.Order;
using Domain.Filtres;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<List<OrderDto>>> GetAllAsync([FromQuery] OrderFilter filter)
    {
        return await orderService.GetAllAsync(filter);
    }


    [HttpGet("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<OrderDto>> GetByIdAsync(int id)
    {
        return await orderService.GetByIdAsync(id);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<Response<OrderDto>> UpdateAsync(int id, [FromBody] UpdateOrderDto updateOrderDto)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
            return new Response<OrderDto>(HttpStatusCode.Unauthorized, "User not authorized");

        return await orderService.UpdateAsync(id, updateOrderDto, userId);
    }


    [HttpDelete("{id}")]
    [Authorize]
    public async Task<Response<string>> DeleteAsync(int id)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
            return new Response<string>(HttpStatusCode.Unauthorized, "User not authorized");

        return await orderService.DeleteAsync(id, userId);
    }

    [HttpPost]
    [Authorize]
    public async Task<Response<OrderDto>> CreateAsync(CreateOrderDto orderDto)
    {
        return await orderService.CreateAsync(orderDto);
    }

}


