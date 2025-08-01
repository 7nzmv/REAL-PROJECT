namespace Domain.DTOs.Review;

public class CreateReviewDto
{
    public int ProductId { get; set; }
    public string Comment { get; set; } = null!;
    public int Rating { get; set; }
}
