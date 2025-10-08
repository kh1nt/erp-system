using erp_system.model;
using erp_system.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;

namespace erp_system.view_model
{
    public class Dashboard_View_Model : View_Model_Base
    {
        public ObservableCollection<Status_Cards_Model> QuickStats { get; set; } = new();
        public ObservableCollection<RecentActivity_Model> RecentActivities { get; set; } = new();
        public ObservableCollection<MonthlyTrend_Model> MonthlyTrends { get; set; } = new();
        public ObservableCollection<TopPerformer_Model> TopPerformers { get; set; } = new();
        
        // Sales Analytics Collections
        public ObservableCollection<TopPerformer_Model> SalesTopPerformers { get; set; } = new();
        public ObservableCollection<DepartmentPerformance_Model> DepartmentPerformance { get; set; } = new();
        public Dictionary<string, decimal> SalesSummary { get; set; } = new Dictionary<string, decimal>();
        
        // Properties for dashboard data
        public decimal MonthlySales { get; set; }
        public decimal AveragePerformanceScore { get; set; }
        public string TopPerformerName { get; set; } = string.Empty;
        public decimal TopPerformerSales { get; set; }
        
        // Performance Statistics Properties
        public int TotalEvaluations { get; set; }
        public int HighPerformers { get; set; }
        public int AveragePerformers { get; set; }
        public int LowPerformers { get; set; }
        
        // Performance Summary Metrics
        public decimal AverageScore { get; set; }
        public decimal HighPerformanceRate { get; set; }
        
        // Department Performance Collection
        public ObservableCollection<DepartmentPerformance_Model> TopDepartments { get; set; } = new();

        // Chart Series for Monthly Sales Performance
        public SeriesCollection MonthlySalesSeries { get; set; } = new SeriesCollection();
        public string[] MonthlySalesLabels { get; set; } = new string[0];

        private DataService _dataService;
        
        // Navigation event
        public event EventHandler<string>? NavigationRequested;

        public Dashboard_View_Model()
        {
            _dataService = new DataService();
            LoadDashboardData();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            NavigateToEmployeesCommand = new View_Model_Command(NavigateToEmployees);
            NavigateToSalesCommand = new View_Model_Command(NavigateToSales);
            NavigateToPerformanceCommand = new View_Model_Command(NavigateToPerformance);
            NavigateToPayrollCommand = new View_Model_Command(NavigateToPayroll);
            NavigateToInsightsCommand = new View_Model_Command(NavigateToInsights);
            RefreshCommand = new View_Model_Command(RefreshData);
        }

        public ICommand NavigateToEmployeesCommand { get; private set; } = null!;
        public ICommand NavigateToSalesCommand { get; private set; } = null!;
        public ICommand NavigateToPerformanceCommand { get; private set; } = null!;
        public ICommand NavigateToPayrollCommand { get; private set; } = null!;
        public ICommand NavigateToInsightsCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;

        private void NavigateToEmployees(object? parameter)
        {
            try
            {
                NavigationRequested?.Invoke(this, "Employees");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Navigation error: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void NavigateToSales(object? parameter)
        {
            try
            {
                NavigationRequested?.Invoke(this, "Sales");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Navigation error: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void NavigateToPerformance(object? parameter)
        {
            try
            {
                NavigationRequested?.Invoke(this, "Performance");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Navigation error: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void NavigateToPayroll(object? parameter)
        {
            try
            {
                NavigationRequested?.Invoke(this, "Payroll");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Navigation error: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void NavigateToInsights(object? parameter)
        {
            try
            {
                NavigationRequested?.Invoke(this, "Insights");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Navigation error: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void LoadDashboardData()
        {
            // Load quick stats
            LoadQuickStats();
            
            // Load recent activities
            LoadRecentActivities();
            
            // Load monthly trends
            LoadMonthlyTrends();
            
            // Load top performers
            LoadTopPerformers();
            
            // Load sales analytics
            LoadSalesAnalytics();
            
            // Load performance statistics
            LoadPerformanceStatistics();
            
            // Load monthly sales chart
            LoadMonthlySalesChart();
            
            // Load additional metrics
            LoadAdditionalMetrics();
        }

        private void LoadQuickStats()
        {
            var salesSummary = _dataService.GetSalesSummary();
            
            QuickStats = new ObservableCollection<Status_Cards_Model>
            {
                new Status_Cards_Model 
                { 
                    Title = "Monthly Sales", 
                    Icon = "💲", 
                    Value = (int)salesSummary.GetValueOrDefault("ThisMonthSales", 0), 
                    NavigationTarget = "Sales_View",
                    Description = "Current month revenue"
                }
            };
        }

        private void LoadRecentActivities()
        {
            RecentActivities = new ObservableCollection<RecentActivity_Model>();
            
            // Get recent sales operations - sort by SaleID descending to get the most recently added records
            var recentSales = _dataService.GetSalesRecords()
                .OrderByDescending(s => s.SaleID)
                .Take(3);
            
            foreach (var sale in recentSales)
            {
                RecentActivities.Add(new RecentActivity_Model
                {
                    Type = "Sales",
                    Description = "💰 New sale recorded",
                    Details = $"{sale.Description} – ₱{sale.Amount:N0} by {sale.EmployeeName}",
                    EmployeeName = sale.EmployeeName,
                    Date = sale.SaleDate,
                    Color = "#10B981"
                });
            }
            
            // Get recent employee operations
            var recentEmployees = _dataService.GetEmployees()
                .OrderByDescending(e => e.HireDate)
                .Take(1);
            
            foreach (var emp in recentEmployees)
            {
                RecentActivities.Add(new RecentActivity_Model
                {
                    Type = "Employees",
                    Description = "👤 New employee added",
                    Details = $"{emp.FirstName} {emp.LastName} – {emp.Position} in {emp.DepartmentName}",
                    EmployeeName = $"{emp.FirstName} {emp.LastName}",
                    Date = emp.HireDate,
                    Color = "#3B82F6"
                });
            }
            
            // Get recent leave requests
            var recentLeaves = _dataService.GetLeaveRequests()
                .OrderByDescending(l => l.RequestDate)
                .Take(1);
            
            foreach (var leave in recentLeaves)
            {
                RecentActivities.Add(new RecentActivity_Model
                {
                    Type = "Leave",
                    Description = "🏖️ Leave request submitted",
                    Details = $"{leave.EmployeeName} requested {leave.TypeName} leave",
                    EmployeeName = leave.EmployeeName,
                    Date = leave.RequestDate,
                    Color = "#F59E0B"
                });
            }
            
            // Get recent performance reviews
            var recentPerformance = _dataService.GetPerformanceRecords()
                .OrderByDescending(p => p.ReviewDate)
                .Take(1);
            
            foreach (var perf in recentPerformance)
            {
                RecentActivities.Add(new RecentActivity_Model
                {
                    Type = "Performance",
                    Description = "📊 Performance review completed",
                    Details = $"{perf.EmployeeName} scored {perf.Score}/5.0 in {perf.Department}",
                    EmployeeName = perf.EmployeeName,
                    Date = perf.ReviewDate,
                    Color = "#8B5CF6"
                });
            }
            
            // Get recent payroll operations (if available)
            try
            {
                var recentPayroll = _dataService.GetPayrollRecords()
                    .OrderByDescending(p => p.PeriodEnd)
                    .Take(1);
                
                foreach (var payroll in recentPayroll)
                {
                    RecentActivities.Add(new RecentActivity_Model
                    {
                        Type = "Payroll",
                        Description = "💳 Payroll processed",
                        Details = $"Payroll for {payroll.EmployeeName} – ₱{payroll.NetPay:N0}",
                        EmployeeName = payroll.EmployeeName,
                        Date = payroll.PeriodEnd,
                        Color = "#EF4444"
                    });
                }
            }
            catch
            {
                // Payroll data might not be available
            }
            
            // Sort all activities by date (latest first) and take the most recent 6
            var sortedActivities = RecentActivities
                .OrderByDescending(a => a.Date)
                .Take(6)
                .ToList();
            
            RecentActivities.Clear();
            foreach (var activity in sortedActivities)
            {
                RecentActivities.Add(activity);
            }
        }

        private void LoadMonthlyTrends()
        {
            try
            {
                var trends = _dataService.GetMonthlyTrends().Take(6).ToList();
                MonthlyTrends = new ObservableCollection<MonthlyTrend_Model>(trends);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading monthly trends: {ex.Message}");
                MonthlyTrends = new ObservableCollection<MonthlyTrend_Model>();
            }
        }

        private void LoadTopPerformers()
        {
            TopPerformers = new ObservableCollection<TopPerformer_Model>(
                _dataService.GetTopPerformers(5));
        }

        private void LoadSalesAnalytics()
        {
            // Load sales summary
            SalesSummary = _dataService.GetSalesSummary();
            
            // Load sales top performers
            SalesTopPerformers.Clear();
            foreach (var performer in _dataService.GetTopPerformers())
                SalesTopPerformers.Add(performer);

            // Load department performance
            DepartmentPerformance.Clear();
            foreach (var dept in _dataService.GetDepartmentPerformance())
                DepartmentPerformance.Add(dept);
        }

        private void LoadPerformanceStatistics()
        {
            try
            {
                // Get all performance records
                var performanceRecords = _dataService.GetPerformanceRecords();
                
                // Calculate statistics
                TotalEvaluations = performanceRecords.Count();
                AverageScore = performanceRecords.Any() ? 
                    Math.Round(performanceRecords.Average(p => p.Score), 1) : 0;
                
                HighPerformers = performanceRecords.Count(p => p.Score >= 4.0m);
                AveragePerformers = performanceRecords.Count(p => p.Score >= 2.5m && p.Score < 4.0m);
                LowPerformers = performanceRecords.Count(p => p.Score < 2.5m);
                
                // Calculate high performance rate (percentage of high performers)
                HighPerformanceRate = TotalEvaluations > 0 ? 
                    Math.Round((decimal)HighPerformers / TotalEvaluations * 100, 0) : 0;
                
                // Load top departments by performance
                LoadTopDepartments(performanceRecords);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading performance statistics: {ex.Message}");
                TotalEvaluations = 0;
                AverageScore = 0;
                HighPerformanceRate = 0;
                HighPerformers = 0;
                AveragePerformers = 0;
                LowPerformers = 0;
                TopDepartments.Clear();
            }
        }

        private void LoadTopDepartments(IEnumerable<Performance_Model> performanceRecords)
        {
            TopDepartments.Clear();
            
            try
            {
                // Get all departments
                var allDepartments = _dataService.GetAllDepartments();
                var departmentStats = new List<DepartmentPerformance_Model>();
                
                foreach (var dept in allDepartments)
                {
                    var deptRecords = performanceRecords
                        .Where(p => p.DepartmentID == dept.DepartmentID)
                        .ToList();
                    
                    if (deptRecords.Any())
                    {
                        var averageScore = deptRecords.Average(p => p.Score);
                        departmentStats.Add(new DepartmentPerformance_Model
                        {
                            DepartmentID = dept.DepartmentID,
                            DepartmentName = dept.DepartmentName,
                            AverageScore = Math.Round(averageScore, 1),
                            TotalEmployees = deptRecords.Count
                        });
                    }
                }
                
                // Sort by average score and take top 8 (to fit in card without scrolling)
                var topDepts = departmentStats
                    .OrderByDescending(d => d.AverageScore)
                    .Take(8)
                    .ToList();
                
                // Add rank numbers
                for (int i = 0; i < topDepts.Count; i++)
                {
                    topDepts[i].Rank = (i + 1).ToString();
                    TopDepartments.Add(topDepts[i]);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading top departments: {ex.Message}");
            }
        }

        private void LoadMonthlySalesChart()
        {
            try
            {
                // Clear existing series
                MonthlySalesSeries.Clear();

                // Get monthly trends data
                var monthlyTrends = _dataService.GetMonthlyTrends();
                
                if (monthlyTrends != null && monthlyTrends.Any())
                {
                    // Create line series
                    var lineSeries = new LineSeries
                    {
                        Title = "Monthly Sales",
                        Values = new ChartValues<decimal>(monthlyTrends.Select(t => t.Amount)),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8,
                        StrokeThickness = 3,
                        Fill = System.Windows.Media.Brushes.Transparent,
                        Stroke = System.Windows.Media.Brushes.DodgerBlue
                    };

                    MonthlySalesSeries.Add(lineSeries);

                    // Set labels
                    MonthlySalesLabels = monthlyTrends.Select(t => t.Month).ToArray();
                }

                OnPropertyChanged(nameof(MonthlySalesSeries));
                OnPropertyChanged(nameof(MonthlySalesLabels));
            }
            catch (Exception ex)
            {
                // Handle error gracefully
                System.Diagnostics.Debug.WriteLine($"Error loading monthly sales chart: {ex.Message}");
            }
        }

        private void LoadAdditionalMetrics()
        {
            var salesSummary = _dataService.GetSalesSummary();
            var performanceStats = _dataService.GetPerformanceStatistics();
            var topPerformer = _dataService.GetTopPerformer();
            
            MonthlySales = salesSummary.GetValueOrDefault("ThisMonthSales", 0);
            AveragePerformanceScore = (decimal)performanceStats.GetValueOrDefault("AverageScore", 0);
            TopPerformerName = topPerformer.Name;
            TopPerformerSales = topPerformer.Amount;
        }

        public void RefreshData(object? parameter = null)
        {
            LoadDashboardData();
            OnPropertyChanged(nameof(QuickStats));
            OnPropertyChanged(nameof(RecentActivities));
            OnPropertyChanged(nameof(MonthlyTrends));
            OnPropertyChanged(nameof(TopPerformers));
            OnPropertyChanged(nameof(SalesTopPerformers));
            OnPropertyChanged(nameof(DepartmentPerformance));
            OnPropertyChanged(nameof(SalesSummary));
            OnPropertyChanged(nameof(MonthlySales));
            OnPropertyChanged(nameof(AveragePerformanceScore));
            OnPropertyChanged(nameof(AverageScore));
            OnPropertyChanged(nameof(HighPerformanceRate));
            OnPropertyChanged(nameof(TopPerformerName));
            OnPropertyChanged(nameof(TopPerformerSales));
            OnPropertyChanged(nameof(TotalEvaluations));
            OnPropertyChanged(nameof(HighPerformers));
            OnPropertyChanged(nameof(AveragePerformers));
            OnPropertyChanged(nameof(LowPerformers));
            OnPropertyChanged(nameof(TopDepartments));
            OnPropertyChanged(nameof(MonthlySalesSeries));
            OnPropertyChanged(nameof(MonthlySalesLabels));
        }
    }

    public class RecentActivity_Model
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}
