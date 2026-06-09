using Microsoft.EntityFrameworkCore;
using SubTrack.Data;
using SubTrack.Models;
using SubTrack.ViewModels.Analytics;

namespace SubTrack.Services;

public class AnalyticsService(ApplicationDbContext context) : IAnalyticsService
{
    public async Task<AnalyticsViewModel> GetAnalyticsAsync(string userId, int months = 7)
    {
        months = Math.Clamp(months, 3, 12);
        var today = DateTime.Today;
        var firstMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-(months - 1));

        var subscriptions = await context.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.ApplicationUserId == userId)
            .ToListAsync();

        var activeSubscriptions = subscriptions
            .Where(subscription => subscription.Status == SubscriptionStatus.Active)
            .ToList();

        var monthlyTrend = Enumerable.Range(0, months)
            .Select(offset =>
            {
                var monthStart = firstMonth.AddMonths(offset);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                var spend = activeSubscriptions
                    .Where(subscription => subscription.StartDate.Date <= monthEnd)
                    .Sum(subscription => subscription.MonthlyCost);

                return new AnalyticsMonthPointViewModel
                {
                    Label = monthStart.ToString("MMM"),
                    MonthlySpend = spend
                };
            })
            .ToList();

        var currentMonthlySpend = activeSubscriptions.Sum(subscription => subscription.MonthlyCost);
        var highestMonth = monthlyTrend
            .OrderByDescending(point => point.MonthlySpend)
            .FirstOrDefault();

        var topServices = activeSubscriptions
            .OrderByDescending(subscription => subscription.MonthlyCost)
            .ThenBy(subscription => subscription.Name)
            .Take(5)
            .ToList();
        var highestServiceSpend = topServices.Count == 0 ? 0 : topServices.Max(subscription => subscription.MonthlyCost);

        return new AnalyticsViewModel
        {
            CurrentMonthlySpend = currentMonthlySpend,
            AverageMonthlySpend = monthlyTrend.Count == 0 ? 0 : monthlyTrend.Average(point => point.MonthlySpend),
            HighestMonthlySpend = highestMonth?.MonthlySpend ?? 0,
            HighestMonthLabel = highestMonth?.Label ?? string.Empty,
            AnnualizedSpend = currentMonthlySpend * 12m,
            MonthlyTrend = monthlyTrend,
            CategorySplit = activeSubscriptions
                .GroupBy(subscription => subscription.Category)
                .Select(group =>
                {
                    var monthlySpend = group.Sum(subscription => subscription.MonthlyCost);
                    return new AnalyticsCategorySplitViewModel
                    {
                        Category = group.Key,
                        MonthlySpend = monthlySpend,
                        Percentage = currentMonthlySpend == 0 ? 0 : decimal.Round(monthlySpend / currentMonthlySpend * 100m, 1)
                    };
                })
                .OrderByDescending(category => category.MonthlySpend)
                .ToList(),
            TopServices = topServices
                .Select(subscription => new AnalyticsTopServiceViewModel
                {
                    SubscriptionId = subscription.Id,
                    Name = subscription.Name,
                    Category = subscription.Category,
                    BillingCycle = subscription.BillingCycle,
                    MonthlyCost = subscription.MonthlyCost,
                    ShareOfHighest = highestServiceSpend == 0 ? 0 : decimal.Round(subscription.MonthlyCost / highestServiceSpend * 100m, 1)
                })
                .ToList()
        };
    }
}
