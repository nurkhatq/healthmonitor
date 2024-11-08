using System.ComponentModel.DataAnnotations;
using HealthMonitor.Shared.Models;

namespace HealthMonitor.Shared.Models.DTOs.Health;

public class HealthMetricRequest
{
    [Required]
    public MetricType Type { get; set; }

    [Required]
    [Range(0, 1000)]
    public double Value { get; set; }

    public string? Notes { get; set; }
}