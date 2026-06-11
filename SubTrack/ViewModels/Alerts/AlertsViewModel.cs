namespace SubTrack.ViewModels.Alerts;

public class AlertsViewModel
{
    public string CurrencyCode { get; set; } = "USD";

    public int TrialsEndingCount { get; set; }

    public int UpcomingRenewalsCount { get; set; }

    public int SpendAnomalyCount { get; set; }

    public int ResolvedThisMonthCount { get; set; }

    public IReadOnlyList<TrialCountdownAlertViewModel> TrialCountdowns { get; set; } = [];

    public IReadOnlyList<RenewalAlertViewModel> UpcomingRenewals { get; set; } = [];

    public IReadOnlyList<SpendAnomalyAlertViewModel> SpendAnomalies { get; set; } = [];

    public IReadOnlyList<ResolvedAlertViewModel> RecentlyResolved { get; set; } = [];
}
