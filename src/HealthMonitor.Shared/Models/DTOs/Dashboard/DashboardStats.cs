namespace HealthMonitor.Shared.Models.DTOs.Dashboard;

public class DashboardStats
{
    public int TotalCaloriesThisWeek { get; set; }
    public double AverageSleepHours { get; set; }
    public List<Activity> RecentActivities { get; set; } = new List<Activity>();
    public List<Sleep> RecentSleepRecords { get; set; } = new List<Sleep>();
}