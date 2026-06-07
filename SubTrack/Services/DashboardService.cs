using Microsoft.EntityFrameworkCore;
using SubTrack.Data;
using SubTrack.Models;
using SubTrack.ViewModels.Dashboard;

namespace SubTrack.Services;

public class DashboardService(ApplicationDbContext context) : IDashboardService
{
    private const int UpcomingBillWindowDays = 30;

    public async Task<DashboardViewModel> GetDashboardAsync(string userId)
    {
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
                Amount = subscription.Price,
                CurrencyCode = subscription.CurrencyCode
            })
            .ToList();

        return new DashboardViewModel
        {
            TotalMonthlySpend = activeSubscriptions.Sum(subscription => subscription.MonthlyCost),
            ProjectedAnnualSpend = activeSubscriptions.Sum(subscription => subscription.AnnualCost),
            ActiveSubscriptionCount = activeSubscriptions.Count,
            PausedSubscriptionCount = subscriptions.Count(subscription => subscription.Status == SubscriptionStatus.Paused),
            UpcomingBillCount = upcomingBills.Count,
            CategorySpending = activeSubscriptions
                .GroupBy(subscription => subscription.Category)
                .Select(group => new CategorySpendViewModel
                {
                    Category = group.Key,
                    MonthlySpend = group.Sum(subscription => subscription.MonthlyCost),
                    SubscriptionCount = group.Count()
                })
                .OrderByDescending(category => category.MonthlySpend)
                .ToList(),
            UpcomingBills = upcomingBills,
            TrialAlerts = activeSubscriptions
                .Where(subscription => subscription.IsFreeTrial && subscription.TrialDaysRemaining is not null)
                .OrderBy(subscription => subscription.TrialEndDate)
                .Select(subscription => new TrialAlertViewModel
                {
                    SubscriptionId = subscription.Id,
                    SubscriptionName = subscription.Name,
                    TrialEndDate = subscription.TrialEndDate!.Value,
                    DaysRemaining = subscription.TrialDaysRemaining!.Value
                })
                .ToList()
        };
    }
}
