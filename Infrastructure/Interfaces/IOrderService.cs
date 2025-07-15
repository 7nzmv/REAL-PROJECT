using Domain.DTOs.Order;
using Domain.Filtres;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IOrderService
{
    Task<Response<List<OrderDto>>> GetAllAsync(OrderFilter filter);
    Task<Response<OrderDto>> GetByIdAsync(int id);
    Task<Response<OrderDto>> CreateAsync(CreateOrderDto createOrderDto, string userId);
    Task<Response<OrderDto>> UpdateAsync(int id, UpdateOrderDto updateOrderDto, string userId);
    Task<Response<string>> DeleteAsync(int id, string userId);
}
