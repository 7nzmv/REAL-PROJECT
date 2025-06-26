namespace Domain.DTOs.Category;

public class CreateCategoryDto
{
    public string Name { get; set; } = null!;

    public int? ParentCategoryId { get; set; }
}
