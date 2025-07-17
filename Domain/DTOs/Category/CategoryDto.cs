namespace Domain.DTOs.Category;

public class CategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentCategoryId { get; set; }

    public string? ParentCategoryName { get; set; }
    public string? ImageUrl { get; set; }


    public List<CategoryDto> SubCategories { get; set; } = new();

    public int ProductCount { get; set; }  // если надо показать количество товаров
}
