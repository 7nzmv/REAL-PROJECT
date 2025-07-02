using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class OrderItemRepository(DataContext context) : IBaseRepository<OrderItem, int>
{
    public async Task<int> AddAsync(OrderItem orderItem)
    {
        await context.OrderItems.AddAsync(orderItem);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public async Task<int> DeleteAsync(OrderItem orderItem)
    {
        context.Remove(orderItem);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public Task<IQueryable<OrderItem>> GetAllAsync()
    {
        var orderItems = context.OrderItems.AsQueryable();
        return Task.FromResult(orderItems);
    }

    public async Task<OrderItem?> GetByIdAsync(int id)
    {
        var orderItem = await context.OrderItems.FindAsync(id);
        return orderItem;
    }

    public async Task<int> UpdateAsync(OrderItem orderItem)
    {
        context.Update(orderItem);
        var result = await context.SaveChangesAsync();
        return result;
    }

}

