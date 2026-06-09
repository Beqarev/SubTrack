using SubTrack.ViewModels.Analytics;

namespace SubTrack.Services;

public interface IAnalyticsService
{
    Task<AnalyticsViewModel> GetAnalyticsAsync(string userId, int months = 7);
}
