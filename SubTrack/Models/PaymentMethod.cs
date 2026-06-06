using System.ComponentModel.DataAnnotations;

namespace SubTrack.Models;

public enum PaymentMethod
{
    [Display(Name = "Credit Card")]
    CreditCard = 1,

    [Display(Name = "Debit Card")]
    DebitCard = 2,

    [Display(Name = "PayPal")]
    PayPal = 3,

    [Display(Name = "Bank Transfer")]
    BankTransfer = 4,

    [Display(Name = "Apple Pay")]
    ApplePay = 5,

    [Display(Name = "Google Pay")]
    GooglePay = 6,

    [Display(Name = "Other")]
    Other = 99
}
