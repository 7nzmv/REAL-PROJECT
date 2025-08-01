using System.Security.Claims;
using Domain.Constants;
using Domain.DTOs.Review;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    // создание отзыва
    [HttpPost]
    [Authorize]
    public async Task<Response<ReviewDto>> CreateAsync(CreateReviewDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return await reviewService.CreateAsync(userId!, dto);
    }

    // получить отзывы по конкретному продукту
    [HttpGet("product/{productId}")]
    public async Task<Response<List<ReviewDto>>> GetByProductIdAsync(int productId)
    {
        return await reviewService.GetByProductIdAsync(productId);
    }

    // обновить отзыв (только владелец)
    [HttpPut("{id}")]
    [Authorize]
    public async Task<Response<ReviewDto>> UpdateAsync(int id, UpdateReviewDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return await reviewService.UpdateAsync(id, userId!, dto);
    }

    // получить все отзывы (только админ)
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<List<ReviewDto>>> GetAllAsync()
    {
        return await reviewService.GetAllAsync();
    }

    // удалить отзыв (владелец или админ)
    [HttpDelete("{id}")]
    [Authorize] // проверка прав делается в сервисе
    public async Task<Response<string>> DeleteAsync(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return await reviewService.DeleteAsync(id, userId!);
    }
}
