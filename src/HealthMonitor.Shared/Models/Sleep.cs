using System.ComponentModel.DataAnnotations;

namespace HealthMonitor.Shared.Models;

public class Sleep
{
    public int Id { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [Range(1, 5)]
    public int Quality { get; set; }

    // Foreign key
    public int UserId { get; set; }
    public virtual User? User { get; set; }
}
