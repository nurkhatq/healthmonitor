using HealthMonitor.Server.Data;
using HealthMonitor.Shared.Models.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthMonitor.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardStats>> GetDashboardStats()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var weekAgo = DateTime.UtcNow.AddDays(-7);

        var recentActivities = await _context.Activities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Date)
            .Take(5)
            .ToListAsync();

        var recentSleep = await _context.SleepRecords
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartTime)
            .Take(5)
            .ToListAsync();

        var weeklyCalories = await _context.Activities
            .Where(a => a.UserId == userId && a.Date >= weekAgo)
            .SumAsync(a => a.CaloriesBurned);

        var averageSleepHours = recentSleep.Any()
            ? recentSleep.Average(s => (s.EndTime - s.StartTime).TotalHours)
            : 0;

        return new DashboardStats
        {
            TotalCaloriesThisWeek = weeklyCalories,
            AverageSleepHours = Math.Round(averageSleepHours, 1),
            RecentActivities = recentActivities,
            RecentSleepRecords = recentSleep
        };
    }
}