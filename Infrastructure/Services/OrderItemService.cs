using System.Net;
using AutoMapper;
using Domain.DTOs.OrderItem;
using Domain.Entities;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

public class OrderItemService(
    IBaseRepositoryWithInclude<OrderItem, int> orderItemRepository,
    IBaseRepository<Product, int> productRepository,
    IBaseRepository<Order, int> orderRepository,
    IMapper mapper,
    ILogger<OrderItemService> logger,
    IRedisCacheService redisCacheService
) : IOrderItemService
{

    private const string CacheKey = "myapp_OrderItems";
    public async Task<Response<List<OrderItemDto>>> GetAllAsync(int orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return new Response<List<OrderItemDto>>(HttpStatusCode.NotFound, "Order not found");

        var items = await orderItemRepository.GetAllIncludingAsync(i => i.Product);
        var filteredItems = items.Where(i => i.OrderId == orderId).ToList();

        var dtos = filteredItems.Select(i =>
        {
            var dto = mapper.Map<OrderItemDto>(i);
            dto.ProductName = i.Product.Name;
            return dto;
        }).ToList();

        return new Response<List<OrderItemDto>>(dtos);
    }

    public async Task<Response<OrderItemDto>> GetByIdAsync(int id)
    {
        var item = await orderItemRepository.GetByIdIncludingAsync(id, x => x.Product);
        if (item == null)
            return new Response<OrderItemDto>(HttpStatusCode.NotFound, "Order item not found");

        var dto = mapper.Map<OrderItemDto>(item);
        dto.ProductName = item.Product.Name;

        return new Response<OrderItemDto>(dto);
    }



    public async Task<Response<OrderItemDto>> CreateAsync(CreateOrderItemDto dto, int orderId, string userId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return new Response<OrderItemDto>(HttpStatusCode.NotFound, "Order not found");

        if (order.UserId != userId)
            return new Response<OrderItemDto>(HttpStatusCode.Forbidden, "Access denied");

        var product = await productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            return new Response<OrderItemDto>(HttpStatusCode.BadRequest, "Product not found");

        var orderItem = new OrderItem
        {
            OrderId = orderId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = product.Price
        };

        var result = await orderItemRepository.AddAsync(orderItem);
        if (result == 0)
            return new Response<OrderItemDto>(HttpStatusCode.BadRequest, "Failed to add order item");

        var mapped = mapper.Map<OrderItemDto>(orderItem);
        mapped.ProductName = product.Name;

        await redisCacheService.RemoveData(CacheKey); // Очистить кэш заказа

        return new Response<OrderItemDto>(mapped);
    }

    public async Task<Response<OrderItemDto>> UpdateAsync(int id, UpdateOrderItemDto updateDto, string userId)
    {
        var orderItem = await orderItemRepository.GetByIdAsync(id);
        if (orderItem == null)
            return new Response<OrderItemDto>(HttpStatusCode.NotFound, "Order item not found");

        var order = await orderRepository.GetByIdAsync(orderItem.OrderId);
        if (order == null)
            return new Response<OrderItemDto>(HttpStatusCode.BadRequest, "Order not found");

        if (order.UserId != userId)
            return new Response<OrderItemDto>(HttpStatusCode.Forbidden, "Access denied");

        // теперь можно обновить позицию
        orderItem.Quantity = updateDto.Quantity;

        var result = await orderItemRepository.UpdateAsync(orderItem);
        if (result == 0)
            return new Response<OrderItemDto>(HttpStatusCode.BadRequest, "Failed to update order item");

        await redisCacheService.RemoveData(CacheKey);

        var dto = mapper.Map<OrderItemDto>(orderItem);
        return new Response<OrderItemDto>(dto);
    }

    public async Task<Response<string>> DeleteAsync(int id, string userId)
    {
        var orderItem = await orderItemRepository.GetByIdAsync(id);
        if (orderItem == null)
            return new Response<string>(HttpStatusCode.NotFound, "Order item not found");

        var order = await orderRepository.GetByIdAsync(orderItem.OrderId);
        if (order == null)
            return new Response<string>(HttpStatusCode.BadRequest, "Order not found");

        if (order.UserId != userId)
            return new Response<string>(HttpStatusCode.Forbidden, "Access denied");

        var result = await orderItemRepository.DeleteAsync(orderItem);
        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to delete order item");

        await redisCacheService.RemoveData(CacheKey);

        return new Response<string>("Order item deleted successfully");
    }

}

