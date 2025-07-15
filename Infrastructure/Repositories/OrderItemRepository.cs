using System.Linq.Expressions;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderItemRepository(DataContext context) : IBaseRepositoryWithInclude<OrderItem, int>
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

    public async Task<List<OrderItem>> GetAllIncludingAsync(params Expression<Func<OrderItem, object>>[] includeProperties)
    {
        IQueryable<OrderItem> query = context.OrderItems;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.ToListAsync();
    }


    public async Task<OrderItem?> GetByIdIncludingAsync(int id, params Expression<Func<OrderItem, object>>[] includeProperties)
    {
        IQueryable<OrderItem> query = context.OrderItems;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(o => o.Id == id);
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

