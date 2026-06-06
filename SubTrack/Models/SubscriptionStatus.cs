using System.ComponentModel.DataAnnotations;

namespace SubTrack.Models;

public enum SubscriptionStatus
{
    [Display(Name = "Active")]
    Active = 1,

    [Display(Name = "Paused")]
    Paused = 2,

    [Display(Name = "Canceled")]
    Canceled = 3
}
