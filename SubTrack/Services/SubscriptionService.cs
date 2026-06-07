using Microsoft.EntityFrameworkCore;
using SubTrack.Data;
using SubTrack.Models;
using SubTrack.ViewModels.Subscriptions;

namespace SubTrack.Services;

public class SubscriptionService(ApplicationDbContext context) : ISubscriptionService
{
    public async Task<SubscriptionIndexViewModel> GetIndexAsync(
        string userId,
        string? searchTerm = null,
        SubscriptionCategory? category = null,
        SubscriptionStatus? status = null)
    {
        var query = context.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.ApplicationUserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(subscription =>
                subscription.Name.ToLower().Contains(normalizedSearchTerm) ||
                (subscription.Description != null && subscription.Description.ToLower().Contains(normalizedSearchTerm)));
        }

        if (category is not null)
        {
            query = query.Where(subscription => subscription.Category == category);
        }

        if (status is not null)
        {
            query = query.Where(subscription => subscription.Status == status);
        }

        var subscriptions = await query
            .OrderBy(subscription => subscription.NextBillingDate)
            .ThenBy(subscription => subscription.Name)
            .ToListAsync();

        return new SubscriptionIndexViewModel
        {
            SearchTerm = searchTerm,
            Category = category,
            Status = status,
            Subscriptions = subscriptions.Select(MapToListItem).ToList()
        };
    }

    public async Task<SubscriptionFormViewModel?> GetFormModelAsync(string userId, int subscriptionId)
    {
        var subscription = await context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(subscription =>
                subscription.Id == subscriptionId &&
                subscription.ApplicationUserId == userId);

        return subscription is null ? null : MapToFormModel(subscription);
    }

    public async Task<int> CreateAsync(string userId, SubscriptionFormViewModel model)
    {
        var subscription = new Subscription
        {
            ApplicationUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        ApplyFormModel(subscription, model);

        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync();

        return subscription.Id;
    }

    public async Task<bool> UpdateAsync(string userId, SubscriptionFormViewModel model)
    {
        if (model.Id is null)
        {
            return false;
        }

        var subscription = await context.Subscriptions
            .FirstOrDefaultAsync(subscription =>
                subscription.Id == model.Id.Value &&
                subscription.ApplicationUserId == userId);

        if (subscription is null)
        {
            return false;
        }

        ApplyFormModel(subscription, model);
        subscription.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string userId, int subscriptionId)
    {
        var subscription = await context.Subscriptions
            .FirstOrDefaultAsync(subscription =>
                subscription.Id == subscriptionId &&
                subscription.ApplicationUserId == userId);

        if (subscription is null)
        {
            return false;
        }

        context.Subscriptions.Remove(subscription);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActivePausedAsync(string userId, int subscriptionId)
    {
        var subscription = await context.Subscriptions
            .FirstOrDefaultAsync(subscription =>
                subscription.Id == subscriptionId &&
                subscription.ApplicationUserId == userId);

        if (subscription is null || subscription.Status == SubscriptionStatus.Canceled)
        {
            return false;
        }

        subscription.Status = subscription.Status == SubscriptionStatus.Active
            ? SubscriptionStatus.Paused
            : SubscriptionStatus.Active;
        subscription.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }

    private static SubscriptionListItemViewModel MapToListItem(Subscription subscription)
    {
        return new SubscriptionListItemViewModel
        {
            Id = subscription.Id,
            Name = subscription.Name,
            Category = subscription.Category,
            BillingCycle = subscription.BillingCycle,
            Price = subscription.Price,
            CurrencyCode = subscription.CurrencyCode,
            Status = subscription.Status,
            NextBillingDate = subscription.NextBillingDate,
            IsFreeTrial = subscription.IsFreeTrial,
            TrialDaysRemaining = subscription.TrialDaysRemaining,
            MonthlyCost = subscription.MonthlyCost,
            AnnualCost = subscription.AnnualCost,
            IsTrialExpiringSoon = subscription.IsTrialExpiringSoon,
            IsBillDueSoon = subscription.IsBillDueSoon
        };
    }

    private static SubscriptionFormViewModel MapToFormModel(Subscription subscription)
    {
        return new SubscriptionFormViewModel
        {
            Id = subscription.Id,
            Name = subscription.Name,
            Description = subscription.Description,
            Category = subscription.Category,
            BillingCycle = subscription.BillingCycle,
            Price = subscription.Price,
            CurrencyCode = subscription.CurrencyCode,
            Status = subscription.Status,
            PaymentMethod = subscription.PaymentMethod,
            StartDate = subscription.StartDate,
            NextBillingDate = subscription.NextBillingDate,
            IsFreeTrial = subscription.IsFreeTrial,
            TrialStartDate = subscription.TrialStartDate,
            TrialEndDate = subscription.TrialEndDate,
            ReminderDaysBeforeRenewal = subscription.ReminderDaysBeforeRenewal,
            WebsiteUrl = subscription.WebsiteUrl,
            Notes = subscription.Notes
        };
    }

    private static void ApplyFormModel(Subscription subscription, SubscriptionFormViewModel model)
    {
        subscription.Name = model.Name.Trim();
        subscription.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        subscription.Category = model.Category;
        subscription.BillingCycle = model.BillingCycle;
        subscription.Price = model.Price;
        subscription.CurrencyCode = model.CurrencyCode.Trim().ToUpperInvariant();
        subscription.Status = model.Status;
        subscription.PaymentMethod = model.PaymentMethod;
        subscription.StartDate = model.StartDate.Date;
        subscription.NextBillingDate = model.NextBillingDate.Date;
        subscription.IsFreeTrial = model.IsFreeTrial;
        subscription.TrialStartDate = model.IsFreeTrial ? model.TrialStartDate?.Date : null;
        subscription.TrialEndDate = model.IsFreeTrial ? model.TrialEndDate?.Date : null;
        subscription.ReminderDaysBeforeRenewal = model.ReminderDaysBeforeRenewal;
        subscription.WebsiteUrl = string.IsNullOrWhiteSpace(model.WebsiteUrl) ? null : model.WebsiteUrl.Trim();
        subscription.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
    }
}
