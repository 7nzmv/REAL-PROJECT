using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class CategoryRepository(DataContext context) : IBaseRepository<Category, int>
{
    public async Task<int> AddAsync(Category category)
    {
        await context.Categories.AddAsync(category);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public async Task<int> DeleteAsync(Category category)
    {
        context.Remove(category);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public Task<IQueryable<Category>> GetAllAsync()
    {
        var categories = context.Categories.AsQueryable();
        return Task.FromResult(categories);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var category = await context.Categories.FindAsync(id);
        return category;
    }

    public async Task<int> UpdateAsync(Category category)
    {
        context.Update(category);
        var result = await context.SaveChangesAsync();
        return result;
    }


}
