namespace Domain.DTOs.Product;

public class CreateProductDto
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? OldPrice { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsNew { get; set; }

    public bool IsPromo { get; set; }

    public int CategoryId { get; set; }
}
