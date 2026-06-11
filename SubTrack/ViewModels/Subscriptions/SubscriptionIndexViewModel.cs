using SubTrack.Models;

namespace SubTrack.ViewModels.Subscriptions;

public class SubscriptionIndexViewModel
{
    public IReadOnlyList<SubscriptionListItemViewModel> Subscriptions { get; set; } = [];

    public string? SearchTerm { get; set; }

    public SubscriptionCategory? Category { get; set; }

    public SubscriptionStatus? Status { get; set; }

    public bool TrialOnly { get; set; }

    public string CurrencyCode { get; set; } = "USD";

    public int AllCount { get; set; }

    public int ActiveCount { get; set; }

    public int PausedCount { get; set; }

    public int TrialCount { get; set; }

    public int TotalCount => AllCount;

    public int FilteredCount => Subscriptions.Count;

    public bool HasFilters =>
        !string.IsNullOrWhiteSpace(SearchTerm) ||
        Category is not null ||
        Status is not null ||
        TrialOnly;

    public decimal ActiveMonthlySpend => Subscriptions
        .Where(subscription => subscription.Status == SubscriptionStatus.Active)
        .Sum(subscription => subscription.MonthlyCost);
}
