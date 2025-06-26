using Domain.DTOs.OrderItem;

namespace Domain.DTOs.Order;

public class CreateOrderDto
{
    public List<OrderItemDto> Items { get; set; } = new();

}

