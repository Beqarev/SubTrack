using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubTrack.Models;
using SubTrack.ViewModels.Settings;

namespace SubTrack.Controllers;

[Authorize]
public class SettingsController(UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User)
            ?? throw new InvalidOperationException("Authenticated user was not found.");

        var model = new SettingsViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            TwoFactorEnabled = user.TwoFactorEnabled
        };

        return View(model);
    }
}
