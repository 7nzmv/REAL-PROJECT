using Domain.DTOs.OrderItem;

namespace Domain.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = null!;  // enum как строка

    public List<OrderItemDto> Items { get; set; } = new();
}
