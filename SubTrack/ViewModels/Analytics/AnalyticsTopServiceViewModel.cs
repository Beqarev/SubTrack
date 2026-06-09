using SubTrack.Models;

namespace SubTrack.ViewModels.Analytics;

public class AnalyticsTopServiceViewModel
{
    public int SubscriptionId { get; set; }

    public string Name { get; set; } = string.Empty;

    public SubscriptionCategory Category { get; set; }

    public BillingCycle BillingCycle { get; set; }

    public decimal MonthlyCost { get; set; }

    public decimal ShareOfHighest { get; set; }
}
