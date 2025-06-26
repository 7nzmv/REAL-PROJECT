namespace Domain.DTOs.CartItem;

public class UpdateCartItemDto
{
    public int Id { get; set; }  // id позиции в корзине

    public int Quantity { get; set; }
}
