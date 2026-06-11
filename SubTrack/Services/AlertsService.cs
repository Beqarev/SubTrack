using Microsoft.EntityFrameworkCore;
using SubTrack.Data;
using SubTrack.Models;
using SubTrack.ViewModels.Alerts;

namespace SubTrack.Services;

public class AlertsService(ApplicationDbContext context) : IAlertsService
{
    private const int RenewalWindowDays = 30;

    public async Task<AlertsViewModel> GetAlertsAsync(string userId)
    {
        var displayCurrencyCode = await GetDisplayCurrencyCodeAsync(userId);
        var today = DateTime.Today;
        var renewalWindowEnd = today.AddDays(RenewalWindowDays);

        var subscriptions = await context.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.ApplicationUserId == userId)
            .ToListAsync();

        var activeSubscriptions = subscriptions
            .Where(subscription => subscription.Status == SubscriptionStatus.Active)
            .ToList();

        var trialCountdowns = activeSubscriptions
            .Where(subscription =>
                subscription.IsFreeTrial &&
                subscription.TrialEndDate is not null &&
                subscription.TrialEndDate.Value.Date >= today)
            .OrderBy(subscription => subscription.TrialEndDate)
            .Select(subscription => new TrialCountdownAlertViewModel
            {
                SubscriptionId = subscription.Id,
                SubscriptionName = subscription.Name,
                ConversionPrice = Convert(subscription.Price, subscription.CurrencyCode, displayCurrencyCode),
                CurrencyCode = displayCurrencyCode,
                TrialEndDate = subscription.TrialEndDate!.Value,
                DaysRemaining = subscription.TrialDaysRemaining ?? 0
            })
            .ToList();

        var upcomingRenewals = activeSubscriptions
            .Where(subscription =>
                subscription.NextBillingDate.Date >= today &&
                subscription.NextBillingDate.Date <= renewalWindowEnd)
            .OrderBy(subscription => subscription.NextBillingDate)
            .ThenBy(subscription => subscription.Name)
            .Select(subscription => new RenewalAlertViewModel
            {
                SubscriptionId = subscription.Id,
                SubscriptionName = subscription.Name,
                Amount = Convert(subscription.Price, subscription.CurrencyCode, displayCurrencyCode),
                CurrencyCode = displayCurrencyCode,
                BillingCycle = subscription.BillingCycle,
                NextBillingDate = subscription.NextBillingDate
            })
            .ToList();

        var anomaly = BuildSpendAnomaly(activeSubscriptions, displayCurrencyCode);

        return new AlertsViewModel
        {
            CurrencyCode = displayCurrencyCode,
            TrialsEndingCount = trialCountdowns.Count,
            UpcomingRenewalsCount = upcomingRenewals.Count,
            SpendAnomalyCount = anomaly is null ? 0 : 1,
            ResolvedThisMonthCount = 0,
            TrialCountdowns = trialCountdowns,
            UpcomingRenewals = upcomingRenewals.Take(5).ToList(),
            SpendAnomalies = anomaly is null ? [] : [anomaly],
            RecentlyResolved = []
        };
    }

    private async Task<string> GetDisplayCurrencyCodeAsync(string userId)
    {
        var currencyCode = await context.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.CurrencyCode)
            .FirstOrDefaultAsync();

        return CurrencyService.Normalize(currencyCode);
    }

    private static SpendAnomalyAlertViewModel? BuildSpendAnomaly(IReadOnlyList<Subscription> activeSubscriptions, string displayCurrencyCode)
    {
        var categoryGroups = activeSubscriptions
            .GroupBy(subscription => subscription.Category)
            .Select(group => new
            {
                Category = group.Key,
                MonthlySpend = group.Sum(subscription => Convert(subscription.MonthlyCost, subscription.CurrencyCode, displayCurrencyCode)),
                Count = group.Count()
            })
            .OrderByDescending(group => group.MonthlySpend)
            .ToList();

        if (categoryGroups.Count < 2)
        {
            return null;
        }

        var topCategory = categoryGroups[0];
        var averageOtherSpend = categoryGroups.Skip(1).Average(group => group.MonthlySpend);

        if (averageOtherSpend <= 0 || topCategory.MonthlySpend < averageOtherSpend * 1.75m)
        {
            return null;
        }

        return new SpendAnomalyAlertViewModel
        {
            Title = $"{topCategory.Category} spend is unusually high",
            Detail = $"{topCategory.Count} active subscriptions account for {CurrencyService.Format(topCategory.MonthlySpend, displayCurrencyCode)} per month."
        };
    }

    private static decimal Convert(decimal amount, string fromCurrencyCode, string toCurrencyCode) =>
        CurrencyService.Convert(amount, fromCurrencyCode, toCurrencyCode);
}
