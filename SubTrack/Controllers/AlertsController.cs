using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubTrack.Models;
using SubTrack.Services;

namespace SubTrack.Controllers;

[Authorize]
public class AlertsController(
    IAlertsService alertsService,
    UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await alertsService.GetAlertsAsync(GetCurrentUserId());
        return View(model);
    }

    private string GetCurrentUserId()
    {
        return userManager.GetUserId(User)
            ?? throw new InvalidOperationException("Authenticated user id was not found.");
    }
}
