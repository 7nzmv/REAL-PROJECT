using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class CartItemRepository(DataContext context) : IBaseRepository<CartItem, int>
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

    public async Task<CartItem?> GetByIdAsync(int id)
    {
        var cartItem = await context.CartItems.FindAsync(id);
        return cartItem;
    }

    public async Task<int> UpdateAsync(CartItem cartItem)
    {
        context.Update(cartItem);
        var result = await context.SaveChangesAsync();
        return result;
    }


}

