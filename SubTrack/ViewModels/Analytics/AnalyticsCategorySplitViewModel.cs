using SubTrack.Models;

namespace SubTrack.ViewModels.Analytics;

public class AnalyticsCategorySplitViewModel
{
    public SubscriptionCategory Category { get; set; }

    public decimal MonthlySpend { get; set; }

    public decimal Percentage { get; set; }
}
