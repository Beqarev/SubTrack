using Microsoft.EntityFrameworkCore;
using SubTrack.Data;
using SubTrack.Models;
using SubTrack.ViewModels.Dashboard;
using SubTrack.ViewModels.Subscriptions;

namespace SubTrack.Services;

public class DashboardService(ApplicationDbContext context) : IDashboardService
{
    private const int UpcomingBillWindowDays = 30;

    public async Task<DashboardViewModel> GetDashboardAsync(string userId)
    {
        var displayCurrencyCode = await GetDisplayCurrencyCodeAsync(userId);
        var subscriptions = await context.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.ApplicationUserId == userId)
            .ToListAsync();

        var activeSubscriptions = subscriptions
            .Where(subscription => subscription.Status == SubscriptionStatus.Active)
            .ToList();

        var today = DateTime.Today;
        var upcomingWindowEnd = today.AddDays(UpcomingBillWindowDays);

        var upcomingBills = activeSubscriptions
            .Where(subscription =>
                subscription.NextBillingDate.Date >= today &&
                subscription.NextBillingDate.Date <= upcomingWindowEnd)
            .OrderBy(subscription => subscription.NextBillingDate)
            .ThenBy(subscription => subscription.Name)
            .Select(subscription => new UpcomingBillViewModel
            {
                SubscriptionId = subscription.Id,
                SubscriptionName = subscription.Name,
                NextBillingDate = subscription.NextBillingDate,
                Amount = Convert(subscription.Price, subscription.CurrencyCode, displayCurrencyCode),
                CurrencyCode = displayCurrencyCode
            })
            .ToList();

        return new DashboardViewModel
        {
            CurrencyCode = displayCurrencyCode,
            TotalMonthlySpend = activeSubscriptions.Sum(subscription => Convert(subscription.MonthlyCost, subscription.CurrencyCode, displayCurrencyCode)),
            ProjectedAnnualSpend = activeSubscriptions.Sum(subscription => Convert(subscription.AnnualCost, subscription.CurrencyCode, displayCurrencyCode)),
            ActiveSubscriptionCount = activeSubscriptions.Count,
            PausedSubscriptionCount = subscriptions.Count(subscription => subscription.Status == SubscriptionStatus.Paused),
            UpcomingBillCount = upcomingBills.Count,
            CategorySpending = activeSubscriptions
                .GroupBy(subscription => subscription.Category)
                .Select(group => new CategorySpendViewModel
                {
                    Category = group.Key,
                    MonthlySpend = group.Sum(subscription => Convert(subscription.MonthlyCost, subscription.CurrencyCode, displayCurrencyCode)),
                    CurrencyCode = displayCurrencyCode,
                    SubscriptionCount = group.Count()
                })
                .OrderByDescending(category => category.MonthlySpend)
                .ToList(),
            UpcomingBills = upcomingBills,
            TrialAlerts = activeSubscriptions
                .Where(subscription =>
                    subscription.IsFreeTrial &&
                    subscription.TrialEndDate is not null &&
                    subscription.TrialEndDate.Value.Date >= today)
                .OrderBy(subscription => subscription.TrialEndDate)
                .Select(subscription => new TrialAlertViewModel
                {
                    SubscriptionId = subscription.Id,
                    SubscriptionName = subscription.Name,
                    TrialEndDate = subscription.TrialEndDate!.Value,
                    DaysRemaining = subscription.TrialDaysRemaining!.Value
                })
                .ToList(),
            ActiveSubscriptions = activeSubscriptions
                .OrderBy(subscription => subscription.NextBillingDate)
                .ThenBy(subscription => subscription.Name)
                .Select(subscription => new SubscriptionListItemViewModel
                {
                    Id = subscription.Id,
                    Name = subscription.Name,
                    Category = subscription.Category,
                    BillingCycle = subscription.BillingCycle,
                    Price = Convert(subscription.Price, subscription.CurrencyCode, displayCurrencyCode),
                    CurrencyCode = displayCurrencyCode,
                    Status = subscription.Status,
                    NextBillingDate = subscription.NextBillingDate,
                    IsFreeTrial = subscription.IsFreeTrial,
                    TrialDaysRemaining = subscription.TrialDaysRemaining,
                    MonthlyCost = Convert(subscription.MonthlyCost, subscription.CurrencyCode, displayCurrencyCode),
                    AnnualCost = Convert(subscription.AnnualCost, subscription.CurrencyCode, displayCurrencyCode),
                    IsTrialExpiringSoon = subscription.IsTrialExpiringSoon,
                    IsBillDueSoon = subscription.IsBillDueSoon
                })
                .Take(8)
                .ToList()
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

    private static decimal Convert(decimal amount, string fromCurrencyCode, string toCurrencyCode) =>
        CurrencyService.Convert(amount, fromCurrencyCode, toCurrencyCode);
}
