using System;
using System.Collections.Generic;
using System.Linq;
using erp_system.model;
using erp_system.Services;

namespace erp_system.Services
{
    public class PerformanceAnalyticsService
    {
        private readonly DataService _dataService;

        public PerformanceAnalyticsService()
        {
            _dataService = new DataService();
        }

        public PerformanceAnalytics GetPerformanceAnalytics()
        {
            var performanceRecords = _dataService.GetPerformanceRecords();
            var analytics = new PerformanceAnalytics();

            if (!performanceRecords.Any())
            {
                return analytics;
            }

            // Calculate basic statistics
            analytics.TotalEvaluations = performanceRecords.Count;
            analytics.AverageScore = performanceRecords.Average(p => p.Score);

            // Categorize performers
            analytics.HighPerformers = performanceRecords.Count(p => p.Score >= 4.0m);
            analytics.AveragePerformers = performanceRecords.Count(p => p.Score >= 2.5m && p.Score < 4.0m);
            analytics.LowPerformers = performanceRecords.Count(p => p.Score < 2.5m);

            // Department averages
            var departmentGroups = performanceRecords
                .Where(p => !string.IsNullOrEmpty(p.Department))
                .GroupBy(p => p.Department)
                .ToDictionary(g => g.Key, g => g.Average(p => p.Score));

            analytics.DepartmentAverages = departmentGroups;

            // Monthly trends (last 12 months)
            var monthlyTrends = performanceRecords
                .Where(p => p.ReviewDate >= DateTime.Now.AddMonths(-12))
                .GroupBy(p => new { p.ReviewDate.Year, p.ReviewDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .ToDictionary(
                    g => $"{g.Key.Year}-{g.Key.Month:D2}",
                    g => g.Count()
                );

            analytics.MonthlyTrends = monthlyTrends;

            // Performance trends for charts
            analytics.PerformanceTrends = performanceRecords
                .Where(p => p.ReviewDate >= DateTime.Now.AddMonths(-12))
                .GroupBy(p => new { p.ReviewDate.Year, p.ReviewDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new PerformanceTrend
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                    AverageScore = g.Average(p => p.Score),
                    EvaluationCount = g.Count()
                })
                .ToList();

            return analytics;
        }

        public List<ChartDataPoint> GetScoreDistributionData()
        {
            var performanceRecords = _dataService.GetPerformanceRecords();
            var scoreRanges = new[]
            {
                new { Range = "1.0-1.9", Min = 1.0m, Max = 1.9m },
                new { Range = "2.0-2.9", Min = 2.0m, Max = 2.9m },
                new { Range = "3.0-3.9", Min = 3.0m, Max = 3.9m },
                new { Range = "4.0-4.9", Min = 4.0m, Max = 4.9m },
                new { Range = "5.0", Min = 5.0m, Max = 5.0m }
            };

            return scoreRanges.Select(range => new ChartDataPoint
            {
                Label = range.Range,
                Value = performanceRecords.Count(p => p.Score >= range.Min && p.Score <= range.Max)
            }).ToList();
        }

        public List<ChartDataPoint> GetDepartmentPerformanceData()
        {
            var performanceRecords = _dataService.GetPerformanceRecords();
            return performanceRecords
                .Where(p => !string.IsNullOrEmpty(p.Department))
                .GroupBy(p => p.Department)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key,
                    Value = (double)g.Average(p => p.Score)
                })
                .OrderByDescending(d => d.Value)
                .ToList();
        }

        public List<ChartDataPoint> GetMonthlyTrendData()
        {
            var analytics = GetPerformanceAnalytics();
            return analytics.PerformanceTrends.Select(trend => new ChartDataPoint
            {
                Label = trend.Period,
                Value = (double)trend.AverageScore
            }).ToList();
        }

        public List<ChartDataPoint> GetReviewTypeData()
        {
            var performanceRecords = _dataService.GetPerformanceRecords();
            return performanceRecords
                .Where(p => !string.IsNullOrEmpty(p.ReviewType))
                .GroupBy(p => p.ReviewType)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key,
                    Value = (double)g.Average(p => p.Score)
                })
                .OrderByDescending(d => d.Value)
                .ToList();
        }

        public Dictionary<string, object> GetPerformanceSummary()
        {
            var analytics = GetPerformanceAnalytics();
            return new Dictionary<string, object>
            {
                ["TotalEvaluations"] = analytics.TotalEvaluations,
                ["AverageScore"] = Math.Round(analytics.AverageScore, 2),
                ["HighPerformers"] = analytics.HighPerformers,
                ["AveragePerformers"] = analytics.AveragePerformers,
                ["LowPerformers"] = analytics.LowPerformers,
                ["TopDepartment"] = analytics.DepartmentAverages.Any() 
                    ? analytics.DepartmentAverages.OrderByDescending(d => d.Value).First().Key 
                    : "N/A"
            };
        }

        // Filter Methods
        public List<Performance_Model> GetFilteredPerformanceRecords(DateTime startDate, DateTime endDate, 
            string department, string employee, decimal minScore, decimal maxScore, string reviewType)
        {
            var allRecords = _dataService.GetPerformanceRecords();
            
            return allRecords.Where(record =>
            {
                // Date filter - be more inclusive with time
                if (record.ReviewDate.Date < startDate.Date || record.ReviewDate.Date > endDate.Date)
                    return false;

                // Department filter
                if (department != "All" && record.Department != department)
                    return false;

                // Employee filter
                if (employee != "All" && record.EmployeeName != employee)
                    return false;

                // Score range filter (use default values for removed filters)
                if (minScore != 0 || maxScore != 5)
                {
                    if (record.Score < minScore || record.Score > maxScore)
                        return false;
                }

                // Review type filter
                if (reviewType != "All" && record.ReviewType != reviewType)
                    return false;

                return true;
            }).ToList();
        }

        public List<ChartDataPoint> GetFilteredScoreDistributionData(DateTime startDate, DateTime endDate, 
            string department, string employee, decimal minScore, decimal maxScore, string reviewType)
        {
            var filteredRecords = GetFilteredPerformanceRecords(startDate, endDate, department, employee, minScore, maxScore, reviewType);
            
            var scoreRanges = new[]
            {
                new { Range = "1.0-1.9", Min = 1.0m, Max = 1.9m },
                new { Range = "2.0-2.9", Min = 2.0m, Max = 2.9m },
                new { Range = "3.0-3.9", Min = 3.0m, Max = 3.9m },
                new { Range = "4.0-4.9", Min = 4.0m, Max = 4.9m },
                new { Range = "5.0", Min = 5.0m, Max = 5.0m }
            };

            return scoreRanges.Select(range => new ChartDataPoint
            {
                Label = range.Range,
                Value = filteredRecords.Count(p => p.Score >= range.Min && p.Score <= range.Max)
            }).ToList();
        }

        public List<ChartDataPoint> GetFilteredDepartmentPerformanceData(DateTime startDate, DateTime endDate, 
            string department, string employee, decimal minScore, decimal maxScore, string reviewType)
        {
            var filteredRecords = GetFilteredPerformanceRecords(startDate, endDate, department, employee, minScore, maxScore, reviewType);
            
            return filteredRecords
                .Where(p => !string.IsNullOrEmpty(p.Department))
                .GroupBy(p => p.Department)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key,
                    Value = (double)g.Average(p => p.Score)
                })
                .OrderByDescending(d => d.Value)
                .ToList();
        }
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}
