namespace SubTrack.ViewModels.Dashboard;

public class UpcomingBillViewModel
{
    public int SubscriptionId { get; set; }

    public string SubscriptionName { get; set; } = string.Empty;

    public DateTime NextBillingDate { get; set; }

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public int DaysUntilBilling => Math.Max(0, (NextBillingDate.Date - DateTime.Today).Days);
}
