using SubTrack.Models;
using SubTrack.ViewModels.Subscriptions;

namespace SubTrack.Services;

public interface ISubscriptionService
{
    Task<SubscriptionIndexViewModel> GetIndexAsync(
        string userId,
        string? searchTerm = null,
        SubscriptionCategory? category = null,
        SubscriptionStatus? status = null);

    Task<SubscriptionFormViewModel?> GetFormModelAsync(string userId, int subscriptionId);

    Task<int> CreateAsync(string userId, SubscriptionFormViewModel model);

    Task<bool> UpdateAsync(string userId, SubscriptionFormViewModel model);

    Task<bool> DeleteAsync(string userId, int subscriptionId);

    Task<bool> ToggleActivePausedAsync(string userId, int subscriptionId);
}
