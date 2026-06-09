using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubTrack.Models;
using SubTrack.Services;
using SubTrack.ViewModels.Subscriptions;

namespace SubTrack.Controllers;

[Authorize]
public class SubscriptionsController(
    ISubscriptionService subscriptionService,
    UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchTerm,
        SubscriptionCategory? category,
        SubscriptionStatus? status,
        bool trialOnly = false)
    {
        var model = await subscriptionService.GetIndexAsync(
            GetCurrentUserId(),
            searchTerm,
            category,
            status,
            trialOnly);

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new SubscriptionFormViewModel
        {
            StartDate = DateTime.Today,
            NextBillingDate = DateTime.Today.AddMonths(1)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubscriptionFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await subscriptionService.CreateAsync(GetCurrentUserId(), model);
        TempData["SuccessMessage"] = "Subscription created.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await subscriptionService.GetFormModelAsync(GetCurrentUserId(), id);

        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubscriptionFormViewModel model)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await subscriptionService.UpdateAsync(GetCurrentUserId(), model);
        if (!updated)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Subscription updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await subscriptionService.GetFormModelAsync(GetCurrentUserId(), id);

        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await subscriptionService.DeleteAsync(GetCurrentUserId(), id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Subscription deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var toggled = await subscriptionService.ToggleActivePausedAsync(GetCurrentUserId(), id);
        if (!toggled)
        {
            TempData["ErrorMessage"] = "Canceled subscriptions cannot be toggled.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeedDemoData()
    {
        var createdCount = await subscriptionService.SeedDemoDataAsync(GetCurrentUserId());
        TempData[createdCount == 0 ? "ErrorMessage" : "SuccessMessage"] = createdCount == 0
            ? "Demo data is only added when your subscription list is empty."
            : $"Added {createdCount} demo subscriptions.";

        return RedirectToAction(nameof(Index));
    }

    private string GetCurrentUserId()
    {
        return userManager.GetUserId(User)
            ?? throw new InvalidOperationException("Authenticated user id was not found.");
    }
}
