using SubTrack.ViewModels.Dashboard;

namespace SubTrack.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(string userId);
}
