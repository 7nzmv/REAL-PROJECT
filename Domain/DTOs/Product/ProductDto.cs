namespace Domain.DTOs.Product;

public class ProductDto
{
    public int Id { get; set; }

    public string NameRu { get; set; } = null!;
    public string NameTg { get; set; } = null!;
    public string? DescriptionRu { get; set; }
    public string? DescriptionTg { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? OldPrice { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsNew { get; set; }

    public bool IsPromo { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int StockQuantity { get; set; }
    public double AverageRating { get; set; }

}
