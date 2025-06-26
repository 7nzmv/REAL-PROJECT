namespace Domain.Entities;

public class CartItem
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;         // от Identity

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; } = 1;
}
