namespace SubTrack.ViewModels.Settings;

public class SettingsViewModel
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string CurrencyCode { get; set; } = "USD";

    public bool TrialConversionAlerts { get; set; } = true;

    public bool UpcomingRenewalAlerts { get; set; } = true;

    public bool SpendAnomalyDetection { get; set; } = true;

    public bool TwoFactorEnabled { get; set; }
}
