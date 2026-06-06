using System.ComponentModel.DataAnnotations;

namespace SubTrack.Models;

public enum BillingCycle
{
    [Display(Name = "Weekly")]
    Weekly = 1,

    [Display(Name = "Monthly")]
    Monthly = 2,

    [Display(Name = "Quarterly")]
    Quarterly = 3,

    [Display(Name = "Yearly")]
    Yearly = 4
}
