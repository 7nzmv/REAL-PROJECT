using System.Linq.Expressions;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CartItemRepository(DataContext context) : IBaseRepositoryWithInclude<CartItem, int>
{
    public async Task<int> AddAsync(CartItem cartItem)
    {
        await context.CartItems.AddAsync(cartItem);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public async Task<int> DeleteAsync(CartItem cartItem)
    {
        context.Remove(cartItem);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public Task<IQueryable<CartItem>> GetAllAsync()
    {
        var cartItems = context.CartItems.AsQueryable();
        return Task.FromResult(cartItems);
    }

    public async Task<List<CartItem>> GetAllIncludingAsync(params Expression<Func<CartItem, object>>[] includeProperties)
    {
        IQueryable<CartItem> query = context.CartItems;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.ToListAsync();
    }


    public async Task<CartItem?> GetByIdAsync(int id)
    {
        var cartItem = await context.CartItems.FindAsync(id);
        return cartItem;
    }

    public async Task<CartItem?> GetByIdIncludingAsync(int id, params Expression<Func<CartItem, object>>[] includeProperties)
    {
        IQueryable<CartItem> query = context.CartItems;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync(o => o.Id == id);
    }


    public async Task<int> UpdateAsync(CartItem cartItem)
    {
        context.Update(cartItem);
        var result = await context.SaveChangesAsync();
        return result;
    }


}

