using SubTrack.Models;

namespace SubTrack.ViewModels.Analytics;

public class AnalyticsViewModel
{
    public decimal AverageMonthlySpend { get; set; }

    public decimal HighestMonthlySpend { get; set; }

    public string HighestMonthLabel { get; set; } = string.Empty;

    public decimal AnnualizedSpend { get; set; }

    public decimal CurrentMonthlySpend { get; set; }

    public IReadOnlyList<AnalyticsMonthPointViewModel> MonthlyTrend { get; set; } = [];

    public IReadOnlyList<AnalyticsCategorySplitViewModel> CategorySplit { get; set; } = [];

    public IReadOnlyList<AnalyticsTopServiceViewModel> TopServices { get; set; } = [];

    public bool HasData => CurrentMonthlySpend > 0 || MonthlyTrend.Any(point => point.MonthlySpend > 0);
}
