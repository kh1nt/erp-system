using erp_system.model;
using erp_system.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace erp_system.view_model
{
    public class Dashboard_View_Model : View_Model_Base
    {
        public ObservableCollection<Status_Cards_Model> QuickStats { get; set; } = new();
        public ObservableCollection<RecentActivity_Model> RecentActivities { get; set; } = new();
        public ObservableCollection<MonthlyTrend_Model> MonthlyTrends { get; set; } = new();
        public ObservableCollection<TopPerformer_Model> TopPerformers { get; set; } = new();
        
        // Properties for dashboard data
        public decimal MonthlySales { get; set; }
        public int PendingLeaves { get; set; }
        public int TotalEmployees { get; set; }
        public int ActiveConsultants { get; set; }
        public decimal AveragePerformanceScore { get; set; }
        public string TopPerformerName { get; set; } = string.Empty;
        public decimal TopPerformerSales { get; set; }

        private DataService _dataService;

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
            NavigateToPayrollCommand = new View_Model_Command(NavigateToPayroll);
            NavigateToInsightsCommand = new View_Model_Command(NavigateToInsights);
            RefreshCommand = new View_Model_Command(RefreshData);
        }

        public ICommand NavigateToEmployeesCommand { get; private set; } = null!;
        public ICommand NavigateToSalesCommand { get; private set; } = null!;
        public ICommand NavigateToPayrollCommand { get; private set; } = null!;
        public ICommand NavigateToInsightsCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;

        private void NavigateToEmployees(object? parameter)
        {
            // This should trigger navigation to Employees view
            // You'll need to implement this based on your navigation system
            System.Windows.MessageBox.Show("Navigate to Employees View", "Navigation", System.Windows.MessageBoxButton.OK);
        }

        private void NavigateToSales(object? parameter)
        {
            // This should trigger navigation to Sales view
            System.Windows.MessageBox.Show("Navigate to Sales View", "Navigation", System.Windows.MessageBoxButton.OK);
        }

        private void NavigateToPayroll(object? parameter)
        {
            // This should trigger navigation to Payroll view
            System.Windows.MessageBox.Show("Navigate to Payroll View", "Navigation", System.Windows.MessageBoxButton.OK);
        }

        private void NavigateToInsights(object? parameter)
        {
            // This should trigger navigation to Insights/Analytics view
            System.Windows.MessageBox.Show("Navigate to Insights View", "Navigation", System.Windows.MessageBoxButton.OK);
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
            
            // Load additional metrics
            LoadAdditionalMetrics();
        }

        private void LoadQuickStats()
        {
            var leaveStats = _dataService.GetLeaveStatistics();
            var salesSummary = _dataService.GetSalesSummary();
            var performanceStats = _dataService.GetPerformanceStatistics();
            
            QuickStats = new ObservableCollection<Status_Cards_Model>
            {
                new Status_Cards_Model 
                { 
                    Title = "Total Employees", 
                    Icon = "👥", 
                    Value = _dataService.GetCount("Employees"), 
                    NavigationTarget = "Employees_View",
                    Description = "Active workforce"
                },
                new Status_Cards_Model 
                { 
                    Title = "Monthly Sales", 
                    Icon = "💲", 
                    Value = (int)salesSummary.GetValueOrDefault("ThisMonthSales", 0), 
                    NavigationTarget = "Sales_View",
                    Description = "Current month revenue"
                },
                new Status_Cards_Model 
                { 
                    Title = "Pending Leaves", 
                    Icon = "📅", 
                    Value = leaveStats.GetValueOrDefault("Pending", 0), 
                    NavigationTarget = "Leave_Management_View",
                    Description = "Awaiting approval"
                },
                new Status_Cards_Model 
                { 
                    Title = "Active Consultants", 
                    Icon = "👤", 
                    Value = _dataService.GetEmployeesByDepartment("Sales").Count, 
                    NavigationTarget = "Employees_View",
                    Description = "Sales team members"
                }
            };
        }

        private void LoadRecentActivities()
        {
            RecentActivities = new ObservableCollection<RecentActivity_Model>();
            
            // Get recent sales
            var recentSales = _dataService.GetSalesRecords()
                .OrderByDescending(s => s.SaleDate)
                .Take(3);
            
            foreach (var sale in recentSales)
            {
                RecentActivities.Add(new RecentActivity_Model
                {
                    Type = "Sale",
                    Description = $"New sale recorded",
                    Details = $"{sale.Description} – ₱{sale.Amount:N0}",
                    EmployeeName = sale.EmployeeName,
                    Date = sale.SaleDate,
                    Color = "Green"
                });
            }
            
            // Get recent leave requests
            var recentLeaves = _dataService.GetLeaveRequests()
                .OrderByDescending(l => l.RequestDate)
                .Take(2);
            
            foreach (var leave in recentLeaves)
            {
                RecentActivities.Add(new RecentActivity_Model
                {
                    Type = "Leave",
                    Description = "Leave application submitted",
                    Details = $"{leave.EmployeeName} – {leave.TypeName}",
                    EmployeeName = leave.EmployeeName,
                    Date = leave.RequestDate,
                    Color = "#3B82F6"
                });
            }
            
            // Get recent performance reviews
            var recentPerformance = _dataService.GetPerformanceRecords()
                .OrderByDescending(p => p.ReviewDate)
                .Take(2);
            
            foreach (var perf in recentPerformance)
            {
                RecentActivities.Add(new RecentActivity_Model
                {
                    Type = "Performance",
                    Description = "Performance review completed",
                    Details = $"{perf.EmployeeName} – Score: {perf.Score}/5",
                    EmployeeName = perf.EmployeeName,
                    Date = perf.ReviewDate,
                    Color = "#A855F7"
                });
            }
            
            // Sort by date and take top 5
            RecentActivities = new ObservableCollection<RecentActivity_Model>(
                RecentActivities.OrderByDescending(a => a.Date).Take(5));
        }

        private void LoadMonthlyTrends()
        {
            MonthlyTrends = new ObservableCollection<MonthlyTrend_Model>(
                _dataService.GetMonthlyTrends().Take(6));
        }

        private void LoadTopPerformers()
        {
            TopPerformers = new ObservableCollection<TopPerformer_Model>(
                _dataService.GetTopPerformers(5));
        }

        private void LoadAdditionalMetrics()
        {
            var salesSummary = _dataService.GetSalesSummary();
            var leaveStats = _dataService.GetLeaveStatistics();
            var performanceStats = _dataService.GetPerformanceStatistics();
            var topPerformer = _dataService.GetTopPerformer();
            
            MonthlySales = salesSummary.GetValueOrDefault("ThisMonthSales", 0);
            PendingLeaves = leaveStats.GetValueOrDefault("Pending", 0);
            TotalEmployees = _dataService.GetCount("Employees");
            ActiveConsultants = _dataService.GetEmployeesByDepartment("Sales").Count;
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
            OnPropertyChanged(nameof(MonthlySales));
            OnPropertyChanged(nameof(PendingLeaves));
            OnPropertyChanged(nameof(TotalEmployees));
            OnPropertyChanged(nameof(ActiveConsultants));
            OnPropertyChanged(nameof(AveragePerformanceScore));
            OnPropertyChanged(nameof(TopPerformerName));
            OnPropertyChanged(nameof(TopPerformerSales));
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
