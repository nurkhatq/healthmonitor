using System;

namespace HealthMonitor.Shared.Models;

public class HealthMetrics
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public MetricType Type { get; set; }
    public double Value { get; set; }
    public string? Unit { get; set; }
    public string? Notes { get; set; }
}

public enum MetricType
{
    Height,             // см
    Weight,            // кг
    BloodPressure,     // мм.рт.ст
    HeartRate,         // уд/мин
    Temperature,       // °C
    BloodSugar,       // ммоль/л
    Oxygen            // %
}