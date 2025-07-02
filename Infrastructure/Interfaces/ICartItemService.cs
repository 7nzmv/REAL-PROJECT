using Domain.DTOs.CartItem;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface ICartItemService
{
    Task<Response<List<CartItemDto>>> GetAllByUserIdAsync(string userId);
    Task<Response<CartItemDto>> GetByIdAsync(int id);
    Task<Response<CartItemDto>> CreateAsync(CreateCartItemDto dto, string userId);
    Task<Response<CartItemDto>> UpdateAsync(int id, UpdateCartItemDto dto, string userId);
    Task<Response<string>> DeleteAsync(int id, string userId);
}
