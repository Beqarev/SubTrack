namespace SubTrack.ViewModels.Alerts;

public class TrialCountdownAlertViewModel
{
    public int SubscriptionId { get; set; }

    public string SubscriptionName { get; set; } = string.Empty;

    public decimal ConversionPrice { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public DateTime TrialEndDate { get; set; }

    public int DaysRemaining { get; set; }
}
