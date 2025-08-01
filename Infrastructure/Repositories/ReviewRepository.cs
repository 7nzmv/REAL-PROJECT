using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class ReviewRepository(DataContext context) : IBaseRepository<Review, int>
{
    public async Task<int> AddAsync(Review review)
    {
        await context.Reviews.AddAsync(review);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public async Task<int> DeleteAsync(Review review)
    {
        context.Reviews.Remove(review);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public Task<IQueryable<Review>> GetAllAsync()
    {
        var reviews = context.Reviews.AsQueryable();
        return Task.FromResult(reviews);
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        var review = await context.Reviews.FindAsync(id);
        return review;
    }

    public async Task<int> UpdateAsync(Review review)
    {
        context.Reviews.Update(review);
        var result = await context.SaveChangesAsync();
        return result;
    }
}
