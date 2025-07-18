using Domain.DTOs.Statistics;
using Domain.Entities;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class StatisticsService(
    IBaseRepository<Order, int> orderRepo,
    IBaseRepository<OrderItem, int> orderItemRepo,
    IBaseRepository<Product, int> productRepo
) : IStatisticsService
{
    public async Task<Response<SalesStatisticsDto>> GetSalesStatisticsAsync(SalesStatisticsFilter filter)
    {
        var allOrders = await orderRepo.GetAllAsync();
        var allItems = await orderItemRepo.GetAllAsync();

        // Отфильтровываем заказы по дате
        var filteredOrders = allOrders
            .Where(o => o.CreatedAt >= filter.StartDate && o.CreatedAt <= filter.EndDate)
            .ToList();

        var orderIds = filteredOrders.Select(o => o.Id).ToHashSet();

        // Берём только те OrderItem, которые относятся к этим заказам
        var filteredItems = allItems
            .Where(oi => orderIds.Contains(oi.OrderId))
            .ToList();

        var totalRevenue = filteredItems.Sum(i => i.UnitPrice * i.Quantity);
        var totalItems = filteredItems.Sum(i => i.Quantity);
        var totalOrders = filteredOrders.Count;

        var result = new SalesStatisticsDto
        {
            TotalOrders = totalOrders,
            TotalItemsSold = totalItems,
            TotalRevenue = totalRevenue
        };

        return new Response<SalesStatisticsDto>(result);
    }

    public async Task<List<TopProductDto>> GetTopSellingProductsAsync(int top = 5)
    {
        var orderItems = await orderItemRepo.GetAllAsync();

        var topProducts = orderItems
            .GroupBy(oi => oi.ProductId)
            .Select(group => new
            {
                ProductId = group.Key,
                QuantitySold = group.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(top)
            .ToList();

        var products = await productRepo.GetAllAsync();

        var result = topProducts
            .Join(products,
                tp => tp.ProductId,
                p => p.Id,
                (tp, p) => new TopProductDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    QuantitySold = tp.QuantitySold
                })
            .ToList();

        return result;
    }

}
