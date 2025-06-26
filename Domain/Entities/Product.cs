using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100), MinLength(2)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string Description { get; set; } = null!;

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    [Range(0.01, 1000000)]
    public decimal? OldPrice { get; set; }              // старая цена, если была скидка
    public string ImageUrl { get; set; } = null!;
    public bool IsNew { get; set; } = false;
    public bool IsPromo { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
