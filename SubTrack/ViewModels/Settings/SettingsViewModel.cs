using System.ComponentModel.DataAnnotations;

namespace SubTrack.ViewModels.Settings;

public class SettingsViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 120 characters.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    public string CurrencyCode { get; set; } = "USD";

    public bool TrialConversionAlerts { get; set; } = true;

    public bool UpcomingRenewalAlerts { get; set; } = true;

    public bool SpendAnomalyDetection { get; set; } = true;

    public bool TwoFactorEnabled { get; set; }
}
