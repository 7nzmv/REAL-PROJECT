using Domain.DTOs.Statistics;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IStatisticsService
{
    Task<Response<SalesStatisticsDto>> GetSalesStatisticsAsync(SalesStatisticsFilter filter);
    Task<List<TopProductDto>> GetTopSellingProductsAsync(int top = 5);

}
