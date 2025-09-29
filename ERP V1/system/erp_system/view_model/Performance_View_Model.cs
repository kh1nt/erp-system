using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using erp_system.Services;
using erp_system.model;

namespace erp_system.view_model
{
    public class Performance_View_Model : View_Model_Base
    {
        private readonly PerformanceAnalyticsService _analyticsService;
        private readonly DataService _dataService;

        public ObservableCollection<Performance_Model> PerformanceRecords { get; } = new ObservableCollection<Performance_Model>();
        
        // Chart Series Collections
        public SeriesCollection ScoreDistributionSeries { get; set; } = new SeriesCollection();
        public SeriesCollection DepartmentPerformanceSeries { get; set; } = new SeriesCollection();
        public SeriesCollection MonthlyTrendSeries { get; set; } = new SeriesCollection();
        public SeriesCollection SalesAchievementSeries { get; set; } = new SeriesCollection();
        
        // Chart Labels
        public string[] ScoreDistributionLabels { get; set; } = Array.Empty<string>();
        public string[] DepartmentLabels { get; set; } = Array.Empty<string>();
        public string[] MonthlyTrendLabels { get; set; } = Array.Empty<string>();
        
        // Performance Statistics
        public int TotalEvaluations { get; set; }
        public decimal AverageScore { get; set; }
        public decimal SalesAchievementRate { get; set; }
        public int HighPerformers { get; set; }
        public int AveragePerformers { get; set; }
        public int LowPerformers { get; set; }
        public string TopDepartment { get; set; } = "N/A";

        // Simplified Filter Properties - Only Date Range and Department
        public DateTime FilterStartDate { get; set; } = new DateTime(2020, 1, 1);
        public DateTime FilterEndDate { get; set; } = new DateTime(2030, 12, 31);
        public string SelectedDepartment { get; set; } = "All";

        // Filter Options Collections
        public ObservableCollection<string> Departments { get; } = new ObservableCollection<string>();

        // Chart Formatters
        public Func<double, string> ScoreFormatter => value => $"{value:F1}";
        public Func<double, string> PercentageFormatter => value => $"{value:F1}%";
        public Func<double, string> CountFormatter => value => $"{value:F0}";

        public Performance_View_Model()
        {
            _analyticsService = new PerformanceAnalyticsService();
            _dataService = new DataService();
            
            InitializeCharts();
            LoadData();
        }

        private void InitializeCharts()
        {
            // Initialize empty series collections
            ScoreDistributionSeries = new SeriesCollection();
            DepartmentPerformanceSeries = new SeriesCollection();
            MonthlyTrendSeries = new SeriesCollection();
            SalesAchievementSeries = new SeriesCollection();
        }

        public void LoadData()
        {
            PerformanceRecords.Clear();
            var data = _dataService.GetPerformanceRecords();

            foreach (var record in data)
                PerformanceRecords.Add(record);

            LoadFilterOptions();
            LoadAnalytics();
            LoadCharts();
        }

        private void LoadAnalytics()
        {
            // Calculate statistics from current filtered data
            if (PerformanceRecords.Any())
            {
                TotalEvaluations = PerformanceRecords.Count;
                AverageScore = Math.Round(PerformanceRecords.Average(p => p.Score), 2);
                
                var salesRecords = PerformanceRecords.Where(p => p.SalesTarget > 0).ToList();
                SalesAchievementRate = salesRecords.Any() 
                    ? Math.Round(salesRecords.Average(p => p.SalesAchievementPercentage), 1)
                    : 0;

                HighPerformers = PerformanceRecords.Count(p => p.Score >= 4.0m);
                AveragePerformers = PerformanceRecords.Count(p => p.Score >= 2.5m && p.Score < 4.0m);
                LowPerformers = PerformanceRecords.Count(p => p.Score < 2.5m);

                TopDepartment = PerformanceRecords
                    .Where(p => !string.IsNullOrEmpty(p.Department))
                    .GroupBy(p => p.Department)
                    .OrderByDescending(g => g.Average(p => p.Score))
                    .FirstOrDefault()?.Key ?? "N/A";
            }
            else
            {
                TotalEvaluations = 0;
                AverageScore = 0;
                SalesAchievementRate = 0;
                HighPerformers = 0;
                AveragePerformers = 0;
                LowPerformers = 0;
                TopDepartment = "N/A";
            }

            OnPropertyChanged(nameof(TotalEvaluations));
            OnPropertyChanged(nameof(AverageScore));
            OnPropertyChanged(nameof(SalesAchievementRate));
            OnPropertyChanged(nameof(HighPerformers));
            OnPropertyChanged(nameof(AveragePerformers));
            OnPropertyChanged(nameof(LowPerformers));
            OnPropertyChanged(nameof(TopDepartment));
        }

        private void LoadCharts()
        {
            LoadScoreDistributionChart();
            LoadDepartmentPerformanceChart();
            LoadMonthlyTrendChart();
            LoadSalesAchievementChart();
        }

        private void LoadScoreDistributionChart()
        {
            // Use the current PerformanceRecords for chart data
            var scoreRanges = new[]
            {
                new { Range = "1.0-1.9", Min = 1.0m, Max = 1.9m },
                new { Range = "2.0-2.9", Min = 2.0m, Max = 2.9m },
                new { Range = "3.0-3.9", Min = 3.0m, Max = 3.9m },
                new { Range = "4.0-4.9", Min = 4.0m, Max = 4.9m },
                new { Range = "5.0", Min = 5.0m, Max = 5.0m }
            };

            var data = scoreRanges.Select(range => new ChartDataPoint
            {
                Label = range.Range,
                Value = PerformanceRecords.Count(p => p.Score >= range.Min && p.Score <= range.Max)
            }).ToList();
            
            ScoreDistributionSeries.Clear();
            ScoreDistributionSeries.Add(new ColumnSeries
            {
                Title = "Score Distribution",
                Values = new ChartValues<double>(data.Select(d => d.Value)),
                Fill = new SolidColorBrush(Color.FromRgb(99, 102, 241)), // Indigo
                Stroke = new SolidColorBrush(Color.FromRgb(79, 70, 229)),
                StrokeThickness = 2
            });

            ScoreDistributionLabels = data.Select(d => d.Label).ToArray();
            OnPropertyChanged(nameof(ScoreDistributionSeries));
            OnPropertyChanged(nameof(ScoreDistributionLabels));
        }

        private void LoadDepartmentPerformanceChart()
        {
            // Use the current PerformanceRecords for chart data
            var data = PerformanceRecords
                .Where(p => !string.IsNullOrEmpty(p.Department))
                .GroupBy(p => p.Department)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key,
                    Value = (double)g.Average(p => p.Score)
                })
                .OrderByDescending(d => d.Value)
                .ToList();

            // If no data, show a default message
            if (!data.Any())
            {
                data.Add(new ChartDataPoint { Label = "No Data", Value = 0 });
            }
            
            DepartmentPerformanceSeries.Clear();
            DepartmentPerformanceSeries.Add(new ColumnSeries
            {
                Title = "Average Score",
                Values = new ChartValues<double>(data.Select(d => d.Value)),
                Fill = new SolidColorBrush(Color.FromRgb(16, 185, 129)), // Emerald
                Stroke = new SolidColorBrush(Color.FromRgb(5, 150, 105)),
                StrokeThickness = 2
            });

            DepartmentLabels = data.Select(d => d.Label).ToArray();
            OnPropertyChanged(nameof(DepartmentPerformanceSeries));
            OnPropertyChanged(nameof(DepartmentLabels));
        }

        private void LoadMonthlyTrendChart()
        {
            // Create monthly trend data from current PerformanceRecords
            var monthlyData = PerformanceRecords
                .GroupBy(p => new { p.ReviewDate.Year, p.ReviewDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new ChartDataPoint
                {
                    Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Value = (double)g.Average(p => p.Score)
                })
                .ToList();

            // If no data, show a default message
            if (!monthlyData.Any())
            {
                monthlyData.Add(new ChartDataPoint { Label = "No Data", Value = 0 });
            }

            MonthlyTrendSeries.Clear();
            MonthlyTrendSeries.Add(new LineSeries
            {
                Title = "Average Score Trend",
                Values = new ChartValues<double>(monthlyData.Select(d => d.Value)),
                Fill = Brushes.Transparent,
                Stroke = new SolidColorBrush(Color.FromRgb(239, 68, 68)), // Red
                StrokeThickness = 3,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 8
            });

            MonthlyTrendLabels = monthlyData.Select(d => d.Label).ToArray();
            OnPropertyChanged(nameof(MonthlyTrendSeries));
            OnPropertyChanged(nameof(MonthlyTrendLabels));
        }

        private void LoadSalesAchievementChart()
        {
            // Create sales achievement data from current PerformanceRecords
            var salesData = PerformanceRecords
                .Where(p => p.SalesTarget > 0) // Only employees with sales targets
                .GroupBy(p => new { p.ReviewDate.Year, p.ReviewDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new ChartDataPoint
                {
                    Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Value = (double)g.Average(p => p.SalesAchievementPercentage)
                })
                .ToList();

            // If no sales data, show a default message
            if (!salesData.Any())
            {
                salesData.Add(new ChartDataPoint { Label = "No Sales Data", Value = 0 });
            }

            SalesAchievementSeries.Clear();
            SalesAchievementSeries.Add(new LineSeries
            {
                Title = "Sales Achievement %",
                Values = new ChartValues<double>(salesData.Select(d => d.Value)),
                Fill = Brushes.Transparent,
                Stroke = new SolidColorBrush(Color.FromRgb(245, 158, 11)), // Amber
                StrokeThickness = 3,
                PointGeometry = DefaultGeometries.Square,
                PointGeometrySize = 8
            });

            OnPropertyChanged(nameof(SalesAchievementSeries));
        }

        private void LoadFilterOptions()
        {
            // Load departments from all data, not just filtered data
            Departments.Clear();
            Departments.Add("All");
            var allRecords = _dataService.GetPerformanceRecords();
            var departments = allRecords
                .Where(p => !string.IsNullOrEmpty(p.Department))
                .Select(p => p.Department)
                .Distinct()
                .OrderBy(d => d);
            foreach (var dept in departments)
                Departments.Add(dept);
        }

        private void ApplyFilters()
        {
            // Get all data from database
            var allRecords = _dataService.GetPerformanceRecords();
            
            // Apply filters to the data
            var filteredRecords = allRecords.Where(record =>
            {
                // Date range filter
                if (record.ReviewDate.Date < FilterStartDate.Date || record.ReviewDate.Date > FilterEndDate.Date)
                    return false;
                
                // Department filter
                if (SelectedDepartment != "All" && record.Department != SelectedDepartment)
                    return false;
                
                return true;
            }).ToList();

            // Update the PerformanceRecords collection
            PerformanceRecords.Clear();
            foreach (var record in filteredRecords)
                PerformanceRecords.Add(record);

            // Update analytics and charts with filtered data
            LoadAnalytics();
            LoadCharts();
        }

        private void ClearFilters()
        {
            FilterStartDate = new DateTime(2020, 1, 1);
            FilterEndDate = new DateTime(2030, 12, 31);
            SelectedDepartment = "All";

            OnPropertyChanged(nameof(FilterStartDate));
            OnPropertyChanged(nameof(FilterEndDate));
            OnPropertyChanged(nameof(SelectedDepartment));

            // Load all data without any filters
            LoadData();
        }

        private void ExportData()
        {
            // TODO: Implement export functionality
            System.Windows.MessageBox.Show("Export functionality will be implemented soon!", "Export Data", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public System.Windows.Input.ICommand Refresh_Command => new View_Model_Command(_ => LoadData());
        public System.Windows.Input.ICommand ApplyFilters_Command => new View_Model_Command(_ => ApplyFilters());
        public System.Windows.Input.ICommand ClearFilters_Command => new View_Model_Command(_ => ClearFilters());
        public System.Windows.Input.ICommand ExportData_Command => new View_Model_Command(_ => ExportData());
    }
}
