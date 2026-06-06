using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SubTrack.Models;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 120 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    public ICollection<Subscription> Subscriptions { get; set; } = [];
}
