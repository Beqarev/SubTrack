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
                ConversionPrice = subscription.Price,
                CurrencyCode = subscription.CurrencyCode,
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
                Amount = subscription.Price,
                CurrencyCode = subscription.CurrencyCode,
                BillingCycle = subscription.BillingCycle,
                NextBillingDate = subscription.NextBillingDate
            })
            .ToList();

        var anomaly = BuildSpendAnomaly(activeSubscriptions);

        return new AlertsViewModel
        {
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

    private static SpendAnomalyAlertViewModel? BuildSpendAnomaly(IReadOnlyList<Subscription> activeSubscriptions)
    {
        var categoryGroups = activeSubscriptions
            .GroupBy(subscription => subscription.Category)
            .Select(group => new
            {
                Category = group.Key,
                MonthlySpend = group.Sum(subscription => subscription.MonthlyCost),
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
            Detail = $"{topCategory.Count} active subscriptions account for {topCategory.MonthlySpend:C} per month."
        };
    }
}
