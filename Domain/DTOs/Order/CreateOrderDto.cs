using Domain.DTOs.OrderItem;

namespace Domain.DTOs.Order;

public class CreateOrderDto
{
    public string UserId { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = new();

}

