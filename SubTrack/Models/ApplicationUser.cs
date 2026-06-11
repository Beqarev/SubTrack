using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SubTrack.Models;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 120 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string CurrencyCode { get; set; } = "USD";

    public ICollection<Subscription> Subscriptions { get; set; } = [];
}
