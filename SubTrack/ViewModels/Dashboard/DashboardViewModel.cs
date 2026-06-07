namespace SubTrack.ViewModels.Dashboard;

public class DashboardViewModel
{
    public decimal TotalMonthlySpend { get; set; }

    public decimal ProjectedAnnualSpend { get; set; }

    public int ActiveSubscriptionCount { get; set; }

    public int PausedSubscriptionCount { get; set; }

    public int UpcomingBillCount { get; set; }

    public IReadOnlyList<CategorySpendViewModel> CategorySpending { get; set; } = [];

    public IReadOnlyList<UpcomingBillViewModel> UpcomingBills { get; set; } = [];

    public IReadOnlyList<TrialAlertViewModel> TrialAlerts { get; set; } = [];
}
