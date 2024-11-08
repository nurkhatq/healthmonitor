using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HealthMonitor.Server.Data;
using HealthMonitor.Shared.Models;
using HealthMonitor.Shared.Models.DTOs.Health;

namespace HealthMonitor.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HealthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<HealthSummary>> GetSummary()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var lastMonth = DateTime.UtcNow.AddMonths(-1);

        var metrics = await _context.HealthMetrics
            .Where(m => m.UserId == userId && m.Date >= lastMonth)
            .OrderByDescending(m => m.Date)
            .ToListAsync();

        var summary = new HealthSummary
        {
            RecentMetrics = metrics,
            Stats = metrics.GroupBy(m => m.Type)
                .ToDictionary(
                    g => g.Key,
                    g => new HealthMetricStats
                    {
                        Latest = g.FirstOrDefault()?.Value,
                        Average = g.Average(m => m.Value),
                        Min = g.Min(m => m.Value),
                        Max = g.Max(m => m.Value),
                        Unit = g.FirstOrDefault()?.Unit,
                        Trend = DetermineTrend(g.OrderBy(m => m.Date).Select(m => m.Value).ToList())
                    }
                )
        };

        return summary;
    }

    [HttpPost]
    public async Task<ActionResult<HealthMetrics>> AddMetric(HealthMetricRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var metric = new HealthMetrics
        {
            UserId = userId,
            Type = request.Type,
            Value = request.Value,
            Date = DateTime.UtcNow,
            Notes = request.Notes,
            Unit = GetUnitForMetricType(request.Type)
        };

        _context.HealthMetrics.Add(metric);
        await _context.SaveChangesAsync();

        return metric;
    }

    private string GetUnitForMetricType(MetricType type) => type switch
    {
        MetricType.Height => "см",
        MetricType.Weight => "кг",
        MetricType.BloodPressure => "мм.рт.ст",
        MetricType.HeartRate => "уд/мин",
        MetricType.Temperature => "°C",
        MetricType.BloodSugar => "ммоль/л",
        MetricType.Oxygen => "%",
        _ => ""
    };

    private string DetermineTrend(List<double> values)
    {
        if (values.Count < 2) return "stable";
        
        var firstHalf = values.Take(values.Count / 2).Average();
        var secondHalf = values.Skip(values.Count / 2).Average();
        
        var difference = secondHalf - firstHalf;
        var threshold = values.Average() * 0.05; // 5% change threshold

        return difference switch
        {
            > 0 when Math.Abs(difference) > threshold => "up",
            < 0 when Math.Abs(difference) > threshold => "down",
            _ => "stable"
        };
    }
}