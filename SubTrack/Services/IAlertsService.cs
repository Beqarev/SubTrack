using SubTrack.ViewModels.Alerts;

namespace SubTrack.Services;

public interface IAlertsService
{
    Task<AlertsViewModel> GetAlertsAsync(string userId);
}
