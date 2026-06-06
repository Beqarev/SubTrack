using System.ComponentModel.DataAnnotations;

namespace SubTrack.Models;

public enum SubscriptionCategory
{
    [Display(Name = "Streaming")]
    Streaming = 1,

    [Display(Name = "Music")]
    Music = 2,

    [Display(Name = "Software")]
    Software = 3,

    [Display(Name = "Cloud Storage")]
    CloudStorage = 4,

    [Display(Name = "Productivity")]
    Productivity = 5,

    [Display(Name = "Fitness")]
    Fitness = 6,

    [Display(Name = "News")]
    News = 7,

    [Display(Name = "Education")]
    Education = 8,

    [Display(Name = "Finance")]
    Finance = 9,

    [Display(Name = "Gaming")]
    Gaming = 10,

    [Display(Name = "Other")]
    Other = 99
}
