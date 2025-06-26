using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class ProductRepository(DataContext context) : IBaseRepository<Product, int>
{
    public async Task<int> AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public async Task<int> DeleteAsync(Product product)
    {
        context.Remove(product);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public Task<IQueryable<Product>> GetAllAsync()
    {
        var products = context.Products.AsQueryable();
        return Task.FromResult(products);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        return product;
    }

    public async Task<int> UpdateAsync(Product product)
    {
        context.Update(product);
        var result = await context.SaveChangesAsync();
        return result;
    }

}

