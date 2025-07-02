using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class OrderRepository(DataContext context): IBaseRepository<Order, int>
{
    public async Task<int> AddAsync(Order order)
    {
        await context.Orders.AddAsync(order);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public async Task<int> DeleteAsync(Order order)
    {
        context.Remove(order);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public Task<IQueryable<Order>> GetAllAsync()
    {
        var orders = context.Orders.AsQueryable();
        return Task.FromResult(orders);
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        var order = await context.Orders.FindAsync(id);
        return order;
    }

    public async Task<int> UpdateAsync(Order order)
    {
        context.Update(order);
        var result = await context.SaveChangesAsync();
        return result;
    }

}


