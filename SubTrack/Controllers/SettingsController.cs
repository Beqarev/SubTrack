using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubTrack.Models;
using SubTrack.ViewModels.Settings;

namespace SubTrack.Controllers;

[Authorize]
public class SettingsController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await GetCurrentUserAsync();
        return View(CreateModel(user));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SettingsViewModel model)
    {
        var user = await GetCurrentUserAsync();

        model.CurrencyCode = "USD";
        model.TwoFactorEnabled = user.TwoFactorEnabled;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var fullName = model.FullName.Trim();
        var email = model.Email.Trim();
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is not null && existingUser.Id != user.Id)
        {
            ModelState.AddModelError(nameof(model.Email), "That email is already used by another account.");
            return View(model);
        }

        user.FullName = fullName;
        user.Email = email;
        user.UserName = email;
        user.NormalizedEmail = userManager.NormalizeEmail(email);
        user.NormalizedUserName = userManager.NormalizeName(email);

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        await signInManager.RefreshSignInAsync(user);
        TempData["SettingsStatus"] = "Profile updated.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<ApplicationUser> GetCurrentUserAsync()
    {
        return await userManager.GetUserAsync(User)
            ?? throw new InvalidOperationException("Authenticated user was not found.");
    }

    private static SettingsViewModel CreateModel(ApplicationUser user)
    {
        return new SettingsViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            TwoFactorEnabled = user.TwoFactorEnabled
        };
    }
}
