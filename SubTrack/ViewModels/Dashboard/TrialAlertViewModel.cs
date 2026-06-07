namespace SubTrack.ViewModels.Dashboard;

public class TrialAlertViewModel
{
    public int SubscriptionId { get; set; }

    public string SubscriptionName { get; set; } = string.Empty;

    public DateTime TrialEndDate { get; set; }

    public int DaysRemaining { get; set; }

    public bool IsUrgent => DaysRemaining <= 3;
}
