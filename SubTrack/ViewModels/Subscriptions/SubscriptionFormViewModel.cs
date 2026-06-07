using System.ComponentModel.DataAnnotations;
using SubTrack.Models;

namespace SubTrack.ViewModels.Subscriptions;

public class SubscriptionFormViewModel : IValidatableObject
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Subscription name is required.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 80 characters.")]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [Display(Name = "Category")]
    public SubscriptionCategory Category { get; set; } = SubscriptionCategory.Other;

    [Required(ErrorMessage = "Billing cycle is required.")]
    [Display(Name = "Billing Cycle")]
    public BillingCycle BillingCycle { get; set; } = BillingCycle.Monthly;

    [Required(ErrorMessage = "Price is required.")]
    [Range(typeof(decimal), "0.01", "999999.99", ErrorMessage = "Price must be between 0.01 and 999,999.99.")]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO code, such as USD, EUR, or GEL.")]
    [StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string CurrencyCode { get; set; } = "USD";

    [Required(ErrorMessage = "Status is required.")]
    [Display(Name = "Status")]
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    [Required(ErrorMessage = "Payment method is required.")]
    [Display(Name = "Payment Method")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Next billing date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Next Billing Date")]
    public DateTime NextBillingDate { get; set; } = DateTime.Today;

    [Display(Name = "Free Trial")]
    public bool IsFreeTrial { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Trial Start Date")]
    public DateTime? TrialStartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Trial End Date")]
    public DateTime? TrialEndDate { get; set; }

    [Range(0, 365, ErrorMessage = "Reminder days must be between 0 and 365.")]
    [Display(Name = "Reminder Days Before Renewal")]
    public int ReminderDaysBeforeRenewal { get; set; } = 3;

    [StringLength(120, ErrorMessage = "Website URL cannot exceed 120 characters.")]
    [Url(ErrorMessage = "Enter a valid URL.")]
    [Display(Name = "Website")]
    public string? WebsiteUrl { get; set; }

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1,000 characters.")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (NextBillingDate.Date < StartDate.Date)
        {
            yield return new ValidationResult(
                "Next billing date cannot be before the start date.",
                [nameof(NextBillingDate)]);
        }

        if (IsFreeTrial)
        {
            if (TrialStartDate is null)
            {
                yield return new ValidationResult(
                    "Trial start date is required for free trial subscriptions.",
                    [nameof(TrialStartDate)]);
            }

            if (TrialEndDate is null)
            {
                yield return new ValidationResult(
                    "Trial end date is required for free trial subscriptions.",
                    [nameof(TrialEndDate)]);
            }

            if (TrialStartDate is not null && TrialEndDate is not null && TrialEndDate.Value.Date < TrialStartDate.Value.Date)
            {
                yield return new ValidationResult(
                    "Trial end date cannot be before the trial start date.",
                    [nameof(TrialEndDate)]);
            }
        }

        if (!IsFreeTrial && (TrialStartDate is not null || TrialEndDate is not null))
        {
            yield return new ValidationResult(
                "Trial dates should only be set when free trial is enabled.",
                [nameof(IsFreeTrial)]);
        }
    }
}
