using System.ComponentModel.DataAnnotations;

namespace HealthMonitor.Shared.Models;

public class Activity
{
    public int Id { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    [Range(1, 1440)] // Max 24 hours in minutes
    public int Duration { get; set; }

    [Required]
    [Range(0, 10000)]
    public int CaloriesBurned { get; set; }

    [Required]
    public DateTime Date { get; set; }

    // Foreign key
    public int UserId { get; set; }
    public virtual User? User { get; set; }
}