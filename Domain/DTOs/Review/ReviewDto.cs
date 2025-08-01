namespace Domain.DTOs.Review;

public class ReviewDto
{
    public int Id { get; set; }
    public string Comment { get; set; } = null!;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = null!; 
}
