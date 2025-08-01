using Domain.DTOs.Review;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IReviewService
{
    Task<Response<List<ReviewDto>>> GetAllAsync();
    Task<Response<ReviewDto>> CreateAsync(string userId, CreateReviewDto dto);
    Task<Response<List<ReviewDto>>> GetByProductIdAsync(int productId);
    Task<Response<ReviewDto>> UpdateAsync(int id, string userId, UpdateReviewDto dto);
    Task<Response<string>> DeleteAsync(int id, string userId);

}
