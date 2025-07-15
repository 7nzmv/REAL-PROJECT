using System.Net;
using AutoMapper;
using Domain.DTOs.Order;
using Domain.Entities;
using Domain.Enums;
using Domain.Filtres;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class OrderService(
    IBaseRepository<Order, int> orderRepository,
    IBaseRepository<Product, int> productRepository,
    IMapper mapper,
    ILogger<OrderService> logger,
    IRedisCacheService redisCacheService
) : IOrderService
{
    private const string CacheKey = "myapp_Orders";

    public async Task<Response<List<OrderDto>>> GetAllAsync(OrderFilter filter)
    {
        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);
        var ordersInCache = await redisCacheService.GetData<List<OrderDto>>(CacheKey);

        if (ordersInCache == null)
        {
            var orders = await orderRepository.GetAllAsync();
            ordersInCache = orders.Select(o => mapper.Map<OrderDto>(o)).ToList();
            await redisCacheService.SetData(CacheKey, ordersInCache, 10);
        }

        if (!string.IsNullOrEmpty(filter.Status))
        {
            ordersInCache = ordersInCache
                .Where(o => o.Status.Equals(filter.Status, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (filter.From.HasValue)
        {
            ordersInCache = ordersInCache
                .Where(o => o.CreatedAt >= filter.From.Value)
                .ToList();
        }

        if (filter.To.HasValue)
        {
            ordersInCache = ordersInCache
                .Where(o => o.CreatedAt <= filter.To.Value)
                .ToList();
        }

        var totalRecords = ordersInCache.Count;

        var data = ordersInCache
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToList();

        return new PagedResponse<List<OrderDto>>(data, validFilter.PageNumber, validFilter.PageSize, totalRecords);
    }

    public async Task<Response<OrderDto>> GetByIdAsync(int id)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return new Response<OrderDto>(HttpStatusCode.NotFound, "Order not found");
        }

        var dto = mapper.Map<OrderDto>(order);
        return new Response<OrderDto>(dto);
    }

    public async Task<Response<OrderDto>> CreateAsync(CreateOrderDto createOrderDto, string userId)
    {
        logger.LogInformation("CreateAsync called with: {@Request}", createOrderDto);

        var order = new Order
        {
            // UserId = createOrderDto.UserId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        var allProducts = await productRepository.GetAllAsync();
        var items = new List<OrderItem>();

        foreach (var itemDto in createOrderDto.Items)
        {
            var product = allProducts.FirstOrDefault(p => p.Id == itemDto.ProductId);
            if (product == null)
                return new Response<OrderDto>(HttpStatusCode.BadRequest, $"Product with ID {itemDto.ProductId} not found");

            items.Add(new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });
        }

        order.Items = items;
        order.TotalPrice = items.Sum(x => x.Quantity * x.UnitPrice);

        var result = await orderRepository.AddAsync(order);

        await redisCacheService.RemoveData(CacheKey);

        var mapped = mapper.Map<OrderDto>(order);

        return result == 0
            ? new Response<OrderDto>(HttpStatusCode.BadRequest, "Order not created")
            : new Response<OrderDto>(mapped);
    }

    public async Task<Response<OrderDto>> UpdateAsync(int id, UpdateOrderDto updateOrderDto, string userId)
    {
        var existingOrder = await orderRepository.GetByIdAsync(id);
        if (existingOrder == null)
            return new Response<OrderDto>(HttpStatusCode.NotFound, "Order not found");

        // Проверяем, что заказ принадлежит пользователю
        if (existingOrder.UserId != userId)
            return new Response<OrderDto>(HttpStatusCode.Forbidden, "You do not have permission to update this order");

        existingOrder.Status = Enum.TryParse<OrderStatus>(updateOrderDto.Status, true, out var status)
            ? status
            : existingOrder.Status;

        var allProducts = await productRepository.GetAllAsync();

        var updatedItems = new List<OrderItem>();
        foreach (var itemDto in updateOrderDto.Items)
        {
            var product = allProducts.FirstOrDefault(p => p.Id == itemDto.ProductId);
            if (product == null)
                return new Response<OrderDto>(HttpStatusCode.BadRequest, $"Product with ID {itemDto.ProductId} not found");

            updatedItems.Add(new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });
        }

        existingOrder.Items = updatedItems;
        existingOrder.TotalPrice = updatedItems.Sum(i => i.UnitPrice * i.Quantity);

        var result = await orderRepository.UpdateAsync(existingOrder);

        await redisCacheService.RemoveData(CacheKey);

        return result == 0
            ? new Response<OrderDto>(HttpStatusCode.BadRequest, "Order not updated")
            : new Response<OrderDto>(mapper.Map<OrderDto>(existingOrder));
    }


    public async Task<Response<string>> DeleteAsync(int id, string userId)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null)
            return new Response<string>(HttpStatusCode.NotFound, "Order not found");

        // Проверяем, что заказ принадлежит пользователю
        if (order.UserId != userId)
            return new Response<string>(HttpStatusCode.Forbidden, "You do not have permission to delete this order");

        var result = await orderRepository.DeleteAsync(order);

        await redisCacheService.RemoveData(CacheKey);

        return result == 0
            ? new Response<string>(HttpStatusCode.BadRequest, "Order not deleted")
            : new Response<string>("Order deleted successfully");
    }
}
