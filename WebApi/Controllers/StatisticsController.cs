using Domain.DTOs.Statistics;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpPost("sales")]
    public async Task<IActionResult> GetSales([FromBody] SalesStatisticsFilter filter)
    {
        var result = await _statisticsService.GetSalesStatisticsAsync(filter);
        return StatusCode((int)result.StatusCode, result);
    }

    // [HttpGet("sales")]
    // public async Task<IActionResult> GetSales([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    // {
    //     var filter = new SalesStatisticsFilter
    //     {
    //         StartDate = startDate,
    //         EndDate = endDate
    //     };

    //     var result = await _statisticsService.GetSalesStatisticsAsync(filter);
    //     return StatusCode((int)result.StatusCode, result);
    // }

    [HttpGet("top-products")]
    public async Task<IActionResult> GetTopProducts([FromQuery] int top = 5)
    {
        var result = await _statisticsService.GetTopSellingProductsAsync(top);
        return Ok(result);
    }

}
