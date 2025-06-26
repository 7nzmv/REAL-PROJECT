using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Order 
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalPrice { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;  // enum: Pending, Paid, Shipped, Cancelled

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
