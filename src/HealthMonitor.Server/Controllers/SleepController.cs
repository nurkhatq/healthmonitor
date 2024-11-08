using HealthMonitor.Server.Data;
using HealthMonitor.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthMonitor.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SleepController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SleepController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Sleep>>> GetSleepRecords()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var sleepRecords = await _context.SleepRecords
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
        
        return sleepRecords;
    }

    [HttpPost]
    public async Task<ActionResult<Sleep>> CreateSleepRecord(Sleep sleep)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        sleep.UserId = userId;
        
        _context.SleepRecords.Add(sleep);
        await _context.SaveChangesAsync();
        
        return sleep;
    }
}