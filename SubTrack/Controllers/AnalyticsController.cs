using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubTrack.Models;
using SubTrack.Services;

namespace SubTrack.Controllers;

[Authorize]
public class AnalyticsController(
    IAnalyticsService analyticsService,
    UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await analyticsService.GetAnalyticsAsync(GetCurrentUserId());
        return View(model);
    }

    private string GetCurrentUserId()
    {
        return userManager.GetUserId(User)
            ?? throw new InvalidOperationException("Authenticated user id was not found.");
    }
}
