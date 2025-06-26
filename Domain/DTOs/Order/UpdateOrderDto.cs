using Domain.DTOs.OrderItem;

namespace Domain.DTOs.Order;

public class UpdateOrderDto
{
    public int Id { get; set; }

    public List<UpdateOrderItemDto> Items { get; set; } = new();

    public string Status { get; set; } = null!; // "Pending", "Paid", "Shipped", "Cancelled"
}