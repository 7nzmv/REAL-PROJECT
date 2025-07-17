using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;           // например: Спальня
    public string? ImageUrl { get; set; }

    public int? ParentCategoryId { get; set; }          // для вложенных категорий (например, "Кровати" внутри "Спальни")

    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
