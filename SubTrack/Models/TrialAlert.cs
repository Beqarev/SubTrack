using System.ComponentModel.DataAnnotations;

namespace SubTrack.Models;

public class TrialAlert
{
    public int SubscriptionId { get; set; }

    [Display(Name = "Subscription")]
    public string SubscriptionName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Trial End Date")]
    public DateTime TrialEndDate { get; set; }

    [Display(Name = "Days Remaining")]
    public int DaysRemaining { get; set; }

    [Display(Name = "Converts On")]
    public DateTime ConvertsOn => TrialEndDate;

    [Display(Name = "Urgent")]
    public bool IsUrgent => DaysRemaining <= 3;
}
