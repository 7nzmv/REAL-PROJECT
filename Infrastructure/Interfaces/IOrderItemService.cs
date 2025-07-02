using Domain.DTOs.OrderItem;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IOrderItemService
{
    Task<Response<List<OrderItemDto>>> GetAllAsync(int orderId);
    Task<Response<OrderItemDto>> GetByIdAsync(int id);
    Task<Response<OrderItemDto>> CreateAsync(CreateOrderItemDto dto, int orderId, string userId);
    Task<Response<OrderItemDto>> UpdateAsync(int id, UpdateOrderItemDto dto, string userId);
    Task<Response<string>> DeleteAsync(int id, string userId);
}
