using SubTrack.Models;

namespace SubTrack.ViewModels.Dashboard;

public class CategorySpendViewModel
{
    public SubscriptionCategory Category { get; set; }

    public decimal MonthlySpend { get; set; }

    public int SubscriptionCount { get; set; }
}
