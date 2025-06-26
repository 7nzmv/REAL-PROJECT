namespace Domain.Responses;

public class ValidFilter(int pageNumber, int pageSize)
{
    public int PageNumber { get; set; } = pageNumber < 1 ? 1 : pageNumber;
    public int PageSize { get; set; } = pageSize < 12 ? 12 : pageSize;
}

