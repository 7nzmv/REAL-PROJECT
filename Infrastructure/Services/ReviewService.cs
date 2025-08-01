using System.Net;
using AutoMapper;
using Domain.DTOs.Review;
using Domain.Entities;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class ReviewService(
    IBaseRepository<Review, int> reviewRepository,
    IBaseRepository<Product, int> productRepository,
    IMapper mapper,
    UserManager<IdentityUser> userManager
) : IReviewService
{
    public async Task<Response<List<ReviewDto>>> GetAllAsync()
    {
        var reviews = await reviewRepository.GetAllAsync();
        var mapped = mapper.Map<List<ReviewDto>>(reviews.ToList());

        return new Response<List<ReviewDto>>(mapped);
    }

    public async Task<Response<ReviewDto>> CreateAsync(string userId, CreateReviewDto dto)
    {
        var review = mapper.Map<Review>(dto);
        review.UserId = userId;

        var result = await reviewRepository.AddAsync(review);

        if (result == 0)
            return new Response<ReviewDto>(HttpStatusCode.BadRequest, "Review not created");

        // пересчёт рейтинга
        await RecalculateProductRatingAsync(review.ProductId);

        return new Response<ReviewDto>(mapper.Map<ReviewDto>(review));
    }

    public async Task<Response<List<ReviewDto>>> GetByProductIdAsync(int productId)
    {
        var reviews = await reviewRepository.GetAllAsync();
        var filtered = reviews.Where(r => r.ProductId == productId).ToList();

        return new Response<List<ReviewDto>>(mapper.Map<List<ReviewDto>>(filtered));
    }

    public async Task<Response<ReviewDto>> UpdateAsync(int id, string userId, UpdateReviewDto dto)
    {
        var review = await reviewRepository.GetByIdAsync(id);
        if (review == null)
            return new Response<ReviewDto>(HttpStatusCode.NotFound, "Review not found");

        if (review.UserId != userId)
            return new Response<ReviewDto>(HttpStatusCode.Forbidden, "You can update only your own reviews");

        review.Comment = dto.Comment;
        review.Rating = dto.Rating;

        var result = await reviewRepository.UpdateAsync(review);

        if (result == 0)
            return new Response<ReviewDto>(HttpStatusCode.BadRequest, "Review not updated");

        // пересчёт рейтинга
        await RecalculateProductRatingAsync(review.ProductId);

        return new Response<ReviewDto>(mapper.Map<ReviewDto>(review));
    }

    public async Task<Response<string>> DeleteAsync(int id, string userId)
    {
        var review = await reviewRepository.GetByIdAsync(id);
        if (review == null)
            return new Response<string>(HttpStatusCode.NotFound, "Отзыв не найден");

        // проверка: владелец или админ
        var user = await userManager.FindByIdAsync(userId);
        var roles = await userManager.GetRolesAsync(user);

        if (review.UserId != userId && !roles.Contains("Admin"))
            return new Response<string>(HttpStatusCode.Forbidden, "Нет доступа");

        var result = await reviewRepository.DeleteAsync(review);

        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Ошибка при удалении");

        // пересчёт рейтинга
        await RecalculateProductRatingAsync(review.ProductId);

        return new Response<string>("Отзыв удалён");
    }

    private async Task RecalculateProductRatingAsync(int productId)
    {
        var allReviews = await reviewRepository.GetAllAsync();
        var productReviews = allReviews.Where(r => r.ProductId == productId).ToList();

        double average = productReviews.Any() ? productReviews.Average(r => r.Rating) : 0;

        var product = await productRepository.GetByIdAsync(productId);
        if (product != null)
        {
            product.AverageRating = average;
            await productRepository.UpdateAsync(product);
        }
    }
}
