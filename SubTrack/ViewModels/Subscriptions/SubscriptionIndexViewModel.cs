using SubTrack.Models;

namespace SubTrack.ViewModels.Subscriptions;

public class SubscriptionIndexViewModel
{
    public IReadOnlyList<SubscriptionListItemViewModel> Subscriptions { get; set; } = [];

    public string? SearchTerm { get; set; }

    public SubscriptionCategory? Category { get; set; }

    public SubscriptionStatus? Status { get; set; }

    public int TotalCount => Subscriptions.Count;

    public decimal ActiveMonthlySpend => Subscriptions
        .Where(subscription => subscription.Status == SubscriptionStatus.Active)
        .Sum(subscription => subscription.MonthlyCost);
}
