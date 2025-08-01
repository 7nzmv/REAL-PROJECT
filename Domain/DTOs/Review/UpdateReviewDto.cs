namespace Domain.DTOs.Review;

public class UpdateReviewDto
{
    public string Comment { get; set; } = null!;
    public int Rating { get; set; }
}
