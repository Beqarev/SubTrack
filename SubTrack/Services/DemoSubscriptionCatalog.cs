using SubTrack.Models;
using SubTrack.ViewModels.Subscriptions;

namespace SubTrack.Services;

public static class DemoSubscriptionCatalog
{
    public static IReadOnlyList<SubscriptionFormViewModel> Create(DateTime today)
    {
        return
        [
            new SubscriptionFormViewModel
            {
                Name = "Netflix",
                Description = "Family streaming plan",
                Category = SubscriptionCategory.Streaming,
                BillingCycle = BillingCycle.Monthly,
                Price = 15.99m,
                CurrencyCode = "USD",
                Status = SubscriptionStatus.Active,
                PaymentMethod = PaymentMethod.CreditCard,
                StartDate = today.AddMonths(-10),
                NextBillingDate = today.AddDays(6),
                WebsiteUrl = "https://netflix.com",
                ReminderDaysBeforeRenewal = 5
            },
            new SubscriptionFormViewModel
            {
                Name = "Spotify Premium",
                Description = "Music subscription",
                Category = SubscriptionCategory.Music,
                BillingCycle = BillingCycle.Monthly,
                Price = 10.99m,
                CurrencyCode = "USD",
                Status = SubscriptionStatus.Active,
                PaymentMethod = PaymentMethod.DebitCard,
                StartDate = today.AddMonths(-16),
                NextBillingDate = today.AddDays(18),
                WebsiteUrl = "https://spotify.com",
                ReminderDaysBeforeRenewal = 4
            },
            new SubscriptionFormViewModel
            {
                Name = "Adobe Creative Cloud",
                Description = "Design and media tools",
                Category = SubscriptionCategory.Software,
                BillingCycle = BillingCycle.Monthly,
                Price = 59.99m,
                CurrencyCode = "USD",
                Status = SubscriptionStatus.Active,
                PaymentMethod = PaymentMethod.CreditCard,
                StartDate = today.AddMonths(-5),
                NextBillingDate = today.AddDays(2),
                WebsiteUrl = "https://adobe.com",
                ReminderDaysBeforeRenewal = 7
            },
            new SubscriptionFormViewModel
            {
                Name = "Dropbox",
                Description = "Cloud storage backup",
                Category = SubscriptionCategory.CloudStorage,
                BillingCycle = BillingCycle.Yearly,
                Price = 119.88m,
                CurrencyCode = "USD",
                Status = SubscriptionStatus.Active,
                PaymentMethod = PaymentMethod.PayPal,
                StartDate = today.AddYears(-1),
                NextBillingDate = today.AddDays(28),
                WebsiteUrl = "https://dropbox.com",
                ReminderDaysBeforeRenewal = 14
            },
            new SubscriptionFormViewModel
            {
                Name = "Notion Plus",
                Description = "Paused workspace subscription",
                Category = SubscriptionCategory.Productivity,
                BillingCycle = BillingCycle.Monthly,
                Price = 8.00m,
                CurrencyCode = "USD",
                Status = SubscriptionStatus.Paused,
                PaymentMethod = PaymentMethod.CreditCard,
                StartDate = today.AddMonths(-7),
                NextBillingDate = today.AddDays(20),
                WebsiteUrl = "https://notion.so",
                ReminderDaysBeforeRenewal = 3
            },
            new SubscriptionFormViewModel
            {
                Name = "Coursera Plus Trial",
                Description = "Trial converts soon",
                Category = SubscriptionCategory.Education,
                BillingCycle = BillingCycle.Monthly,
                Price = 59.00m,
                CurrencyCode = "USD",
                Status = SubscriptionStatus.Active,
                PaymentMethod = PaymentMethod.CreditCard,
                StartDate = today.AddDays(-5),
                NextBillingDate = today.AddDays(2),
                IsFreeTrial = true,
                TrialStartDate = today.AddDays(-5),
                TrialEndDate = today.AddDays(2),
                WebsiteUrl = "https://coursera.org",
                ReminderDaysBeforeRenewal = 5
            }
        ];
    }
}
