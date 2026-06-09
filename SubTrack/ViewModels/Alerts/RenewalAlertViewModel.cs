using SubTrack.Models;

namespace SubTrack.ViewModels.Alerts;

public class RenewalAlertViewModel
{
    public int SubscriptionId { get; set; }

    public string SubscriptionName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public BillingCycle BillingCycle { get; set; }

    public DateTime NextBillingDate { get; set; }
}
