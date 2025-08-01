using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Comment { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // связи
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string UserId { get; set; } = null!;
}
