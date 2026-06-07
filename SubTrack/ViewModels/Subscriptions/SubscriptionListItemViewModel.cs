using SubTrack.Models;

namespace SubTrack.ViewModels.Subscriptions;

public class SubscriptionListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public SubscriptionCategory Category { get; set; }

    public BillingCycle BillingCycle { get; set; }

    public decimal Price { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public SubscriptionStatus Status { get; set; }

    public DateTime NextBillingDate { get; set; }

    public bool IsFreeTrial { get; set; }

    public int? TrialDaysRemaining { get; set; }

    public decimal MonthlyCost { get; set; }

    public decimal AnnualCost { get; set; }

    public int DaysUntilBilling => Math.Max(0, (NextBillingDate.Date - DateTime.Today).Days);

    public bool IsTrialExpiringSoon { get; set; }

    public bool IsBillDueSoon { get; set; }

    public string StatusBadgeCssClass => Status switch
    {
        SubscriptionStatus.Active => "bg-success",
        SubscriptionStatus.Paused => "bg-warning text-dark",
        SubscriptionStatus.Canceled => "bg-secondary",
        _ => "bg-secondary"
    };
}
