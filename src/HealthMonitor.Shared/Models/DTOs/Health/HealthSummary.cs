using HealthMonitor.Shared.Models;

namespace HealthMonitor.Shared.Models.DTOs.Health;

public class HealthSummary
{
    public List<HealthMetrics> RecentMetrics { get; set; } = new();
    public Dictionary<MetricType, HealthMetricStats> Stats { get; set; } = new();
}

public class HealthMetricStats
{
    public double? Latest { get; set; }
    public double? Average { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public string? Unit { get; set; }
    public string? Trend { get; set; } // "up", "down", "stable"
}