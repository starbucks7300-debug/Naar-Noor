using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("revenue")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRevenue(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate   = null,
        CancellationToken cancellationToken = default)
    {
        var from = fromDate ?? DateTime.UtcNow.AddYears(-1);
        var to   = toDate   ?? DateTime.UtcNow;

        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var yearStart  = new DateTime(today.Year, 1, 1);

        var allOrders = await _unitOfWork.Orders.Query()
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
            .Select(o => new { o.TotalAmount, o.CreatedAt })
            .ToListAsync(cancellationToken);

        var todayRev = allOrders.Where(o => o.CreatedAt.Date == today).Sum(o => o.TotalAmount);
        var weekRev  = allOrders.Where(o => o.CreatedAt >= weekStart).Sum(o => o.TotalAmount);
        var monthRev = allOrders.Where(o => o.CreatedAt >= monthStart).Sum(o => o.TotalAmount);
        var yearRev  = allOrders.Where(o => o.CreatedAt >= yearStart).Sum(o => o.TotalAmount);
        var avgOrder = allOrders.Count > 0 ? allOrders.Average(o => (double)o.TotalAmount) : 0;

        return Ok(new
        {
            todayRevenue   = todayRev,
            weekRevenue    = weekRev,
            monthRevenue   = monthRev,
            yearRevenue    = yearRev,
            averagePerOrder = (decimal)avgOrder,
        });
    }

    [HttpGet("orders")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrderStats(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate   = null,
        CancellationToken cancellationToken = default)
    {
        var from = fromDate ?? DateTime.UtcNow.AddYears(-1);
        var to   = toDate   ?? DateTime.UtcNow;

        var orders = await _unitOfWork.Orders.Query()
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
            .Select(o => new { o.Status, o.CreatedAt })
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            totalOrders            = orders.Count,
            completedOrders        = orders.Count(o => o.Status.ToString() == "Completed"),
            pendingOrders          = orders.Count(o => o.Status.ToString() == "Pending"),
            cancelledOrders        = orders.Count(o => o.Status.ToString() == "Cancelled"),
            averagePreparationTime = 25.0,
        });
    }

    [HttpGet("{reportType}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReport(
        string reportType,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate   = null,
        CancellationToken cancellationToken = default)
    {
        var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
        var to   = toDate   ?? DateTime.UtcNow;

        var data = new Dictionary<string, object>
        {
            ["reportType"] = reportType,
            ["period"]     = $"{from:yyyy-MM-dd} to {to:yyyy-MM-dd}",
            ["generated"]  = DateTime.UtcNow,
        };

        switch (reportType.ToLowerInvariant())
        {
            case "revenue":
                var revenueResult = await GetRevenue(fromDate, toDate, cancellationToken);
                data["summary"] = "Revenue report";
                break;
            case "orders":
                var ordersResult = await GetOrderStats(fromDate, toDate, cancellationToken);
                data["summary"] = "Order statistics report";
                break;
            case "reservations":
                var resCount = await _unitOfWork.Reservations.Query()
                    .Where(r => r.CreatedAt >= from && r.CreatedAt <= to)
                    .CountAsync(cancellationToken);
                data["totalReservations"] = resCount;
                break;
            case "menu":
                var menuCount = await _unitOfWork.MenuItems.Query().CountAsync(cancellationToken);
                data["totalMenuItems"] = menuCount;
                break;
        }

        return Ok(new
        {
            reportType = reportType,
            startDate  = from,
            endDate    = to,
            data       = data,
        });
    }
}
