using Domain.DTOs.OrderItem;

namespace Domain.DTOs.Order;

public class CreateOrderDto
{
    public List<CreateOrderItemDto> Items { get; set; } = new();

}

