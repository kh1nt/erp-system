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
using erp_system.view;

namespace erp_system.view_model
{
    public class Performance_View_Model : View_Model_Base
    {
        private readonly PerformanceAnalyticsService _analyticsService;
        private readonly DataService _dataService;

        public ObservableCollection<Performance_Model> PerformanceRecords { get; } = new ObservableCollection<Performance_Model>();
        public ObservableCollection<Performance_Model> PagedPerformanceRecords { get; } = new ObservableCollection<Performance_Model>();
        
        // Chart Series Collections
        public SeriesCollection ScoreDistributionSeries { get; set; } = new SeriesCollection();
        public SeriesCollection DepartmentPerformanceSeries { get; set; } = new SeriesCollection();
        public SeriesCollection MonthlyTrendSeries { get; set; } = new SeriesCollection();
        public SeriesCollection SalesAmountSeries { get; set; } = new SeriesCollection();
        public SeriesCollection SalesCountSeries { get; set; } = new SeriesCollection();
        
        // Chart Labels
        public string[] ScoreDistributionLabels { get; set; } = Array.Empty<string>();
        public string[] DepartmentLabels { get; set; } = Array.Empty<string>();
        public string[] MonthlyTrendLabels { get; set; } = Array.Empty<string>();
        public string[] SalesTrendLabels { get; set; } = Array.Empty<string>();
        
        // Performance Statistics
        public int TotalEvaluations { get; set; }
        public decimal AverageScore { get; set; }
        public int HighPerformers { get; set; }
        public int AveragePerformers { get; set; }
        public int LowPerformers { get; set; }
        public string TopDepartment { get; set; } = "N/A";

        // Simplified Filter Properties - Only Date Range and Department
        private DateTime _filterStartDate = new DateTime(2020, 1, 1);
        private DateTime _filterEndDate = new DateTime(2030, 12, 31);
        private string _selectedDepartment = "All";

        public DateTime FilterStartDate 
        { 
            get => _filterStartDate; 
            set 
            { 
                if (_filterStartDate != value)
                {
                    _filterStartDate = value; 
                    OnPropertyChanged(nameof(FilterStartDate));
                    
                    // Only apply filters if we're not in the middle of initialization
                    if (_dataService != null && PerformanceRecords != null)
                    {
                        try
                        {
                            ApplyFilters(); // Auto-apply when changed
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex.Message}");
                        }
                    }
                }
            } 
        }
        
        public DateTime FilterEndDate 
        { 
            get => _filterEndDate; 
            set 
            { 
                if (_filterEndDate != value)
                {
                    _filterEndDate = value; 
                    OnPropertyChanged(nameof(FilterEndDate));
                    
                    // Only apply filters if we're not in the middle of initialization
                    if (_dataService != null && PerformanceRecords != null)
                    {
                        try
                        {
                            ApplyFilters(); // Auto-apply when changed
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex.Message}");
                        }
                    }
                }
            } 
        }
        
        public string SelectedDepartment 
        { 
            get => _selectedDepartment; 
            set 
            { 
                if (_selectedDepartment != value)
                {
                    _selectedDepartment = value; 
                    OnPropertyChanged(nameof(SelectedDepartment));
                    
                    // Only apply filters if we're not in the middle of initialization
                    if (_dataService != null && PerformanceRecords != null)
                    {
                        try
                        {
                            ApplyFilters(); // Auto-apply when changed
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex.Message}");
                        }
                    }
                }
            } 
        }

        // Filter Options Collections
        public ObservableCollection<string> Departments { get; } = new ObservableCollection<string>();

        // Pagination Properties
        private const int PageSize = 9;
        private int _currentPage = 1;
        private int _totalPages = 1;
        
        // Sorting Properties
        private string _sortColumn = "ReviewDate";
        private bool _sortAscending = false;

        public int CurrentPage 
        { 
            get => _currentPage; 
            set 
            { 
                _currentPage = value; 
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(CurrentPageNumber));
                OnPropertyChanged(nameof(PaginationInfo));
                OnPropertyChanged(nameof(CanGoToFirstPage));
                OnPropertyChanged(nameof(CanGoToPreviousPage));
                OnPropertyChanged(nameof(CanGoToNextPage));
                OnPropertyChanged(nameof(CanGoToLastPage));
            } 
        }

        public int CurrentPageNumber => _currentPage;
        public string PaginationInfo => $"Page {_currentPage} of {_totalPages}";
        public bool CanGoToFirstPage => _currentPage > 1;
        public bool CanGoToPreviousPage => _currentPage > 1;
        public bool CanGoToNextPage => _currentPage < _totalPages;
        public bool CanGoToLastPage => _currentPage < _totalPages;

        // Chart Formatters
        public Func<double, string> ScoreFormatter => value => $"{value:F1}";
        public Func<double, string> CountFormatter => value => $"{value:F0}";
        public Func<double, string> CurrencyFormatter => value => $"₱{value:N0}";

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
            SalesAmountSeries = new SeriesCollection();
            SalesCountSeries = new SeriesCollection();
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
            UpdatePagination();
        }

        private void LoadAnalytics()
        {
            // Calculate statistics from current filtered data
            if (PerformanceRecords.Any())
            {
                TotalEvaluations = PerformanceRecords.Count;
                AverageScore = Math.Round(PerformanceRecords.Average(p => p.Score), 2);

                HighPerformers = PerformanceRecords.Count(p => p.Score >= 4.0m);
                AveragePerformers = PerformanceRecords.Count(p => p.Score >= 2.5m && p.Score < 4.0m);
                LowPerformers = PerformanceRecords.Count(p => p.Score < 2.5m);

                // Find the department with the highest average performance
                var allDepartments = _dataService.GetAllDepartments();
                var topDept = "N/A";
                var highestScore = 0.0m;

                foreach (var dept in allDepartments)
                {
                    var deptRecords = PerformanceRecords
                        .Where(p => p.DepartmentID == dept.DepartmentID)
                        .ToList();

                    if (deptRecords.Any())
                    {
                        var avgScore = deptRecords.Average(p => p.Score);
                        if (avgScore > highestScore)
                        {
                            highestScore = avgScore;
                            topDept = dept.DepartmentName;
                        }
                    }
                }

                TopDepartment = topDept;
            }
            else
            {
                TotalEvaluations = 0;
                AverageScore = 0;
                HighPerformers = 0;
                AveragePerformers = 0;
                LowPerformers = 0;
                TopDepartment = "N/A";
            }

            OnPropertyChanged(nameof(TotalEvaluations));
            OnPropertyChanged(nameof(AverageScore));
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
            LoadSalesPerformanceCharts();
        }

        private void LoadScoreDistributionChart()
        {
            try
            {
                // Ensure ScoreDistributionSeries is initialized
                if (ScoreDistributionSeries == null)
                {
                    ScoreDistributionSeries = new SeriesCollection();
                }

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading score distribution chart: {ex.Message}");
                if (ScoreDistributionSeries == null)
                {
                    ScoreDistributionSeries = new SeriesCollection();
                }
                ScoreDistributionSeries.Clear();
                ScoreDistributionLabels = new[] { "Error Loading Data" };
                OnPropertyChanged(nameof(ScoreDistributionSeries));
                OnPropertyChanged(nameof(ScoreDistributionLabels));
            }
        }

        private void LoadDepartmentPerformanceChart()
        {
            try
            {
                // Ensure DepartmentPerformanceSeries is initialized
                if (DepartmentPerformanceSeries == null)
                {
                    DepartmentPerformanceSeries = new SeriesCollection();
                }

                // Get all departments from the database
                var allDepartments = _dataService.GetAllDepartments();
                var data = new List<ChartDataPoint>();

                // For each department, calculate average performance or show 0 if no records
                foreach (var dept in allDepartments)
                {
                    var deptRecords = PerformanceRecords
                        .Where(p => p.DepartmentID == dept.DepartmentID)
                        .ToList();

                    var averageScore = deptRecords.Any() ? deptRecords.Average(p => p.Score) : 0;
                    
                    data.Add(new ChartDataPoint
                    {
                        Label = dept.DepartmentName,
                        Value = (double)averageScore
                    });
                }

                // Sort by average score (descending)
                data = data.OrderByDescending(d => d.Value).ToList();

                // If no data at all, show a default message
                if (!data.Any())
                {
                    data.Add(new ChartDataPoint { Label = "No Department Data", Value = 0 });
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading department performance chart: {ex.Message}");
                if (DepartmentPerformanceSeries == null)
                {
                    DepartmentPerformanceSeries = new SeriesCollection();
                }
                DepartmentPerformanceSeries.Clear();
                DepartmentLabels = new[] { "Error Loading Data" };
                OnPropertyChanged(nameof(DepartmentPerformanceSeries));
                OnPropertyChanged(nameof(DepartmentLabels));
            }
        }

        private void LoadMonthlyTrendChart()
        {
            try
            {
                // Ensure MonthlyTrendSeries is initialized
                if (MonthlyTrendSeries == null)
                {
                    MonthlyTrendSeries = new SeriesCollection();
                }

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

                // Clear and rebuild the series safely
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading monthly trend chart: {ex.Message}");
                // Initialize with empty data if there's an error
                if (MonthlyTrendSeries == null)
                {
                    MonthlyTrendSeries = new SeriesCollection();
                }
                MonthlyTrendSeries.Clear();
                MonthlyTrendLabels = new[] { "Error Loading Data" };
                OnPropertyChanged(nameof(MonthlyTrendSeries));
                OnPropertyChanged(nameof(MonthlyTrendLabels));
            }
        }

        private void LoadSalesPerformanceCharts()
        {
            try
            {
                // Ensure series are initialized
                if (SalesAmountSeries == null)
                {
                    SalesAmountSeries = new SeriesCollection();
                }
                if (SalesCountSeries == null)
                {
                    SalesCountSeries = new SeriesCollection();
                }

                // Get sales trend data from DataService
                var salesTrendData = _dataService.GetSalesTrendData();
                
                if (salesTrendData.Any())
                {
                    // Sales Amount Chart
                    SalesAmountSeries.Clear();
                    SalesAmountSeries.Add(new LineSeries
                    {
                        Title = "Sales Amount",
                        Values = new ChartValues<double>(salesTrendData.Select(d => (double)d.Amount)),
                        Fill = Brushes.Transparent,
                        Stroke = new SolidColorBrush(Color.FromRgb(16, 185, 129)), // Green
                        StrokeThickness = 3,
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8
                    });

                    // Sales Count Chart
                    SalesCountSeries.Clear();
                    SalesCountSeries.Add(new ColumnSeries
                    {
                        Title = "Transaction Count",
                        Values = new ChartValues<double>(salesTrendData.Select(d => (double)d.TransactionCount)),
                        Fill = new SolidColorBrush(Color.FromRgb(59, 130, 246)), // Blue
                        Stroke = new SolidColorBrush(Color.FromRgb(37, 99, 235)),
                        StrokeThickness = 2
                    });

                    SalesTrendLabels = salesTrendData.Select(d => d.Month).ToArray();
                }
                else
                {
                    // No sales data
                    SalesAmountSeries.Clear();
                    SalesCountSeries.Clear();
                    SalesTrendLabels = new[] { "No Sales Data" };
                }

                OnPropertyChanged(nameof(SalesAmountSeries));
                OnPropertyChanged(nameof(SalesCountSeries));
                OnPropertyChanged(nameof(SalesTrendLabels));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading sales performance charts: {ex.Message}");
                if (SalesAmountSeries == null)
                {
                    SalesAmountSeries = new SeriesCollection();
                }
                if (SalesCountSeries == null)
                {
                    SalesCountSeries = new SeriesCollection();
                }
                SalesAmountSeries.Clear();
                SalesCountSeries.Clear();
                SalesTrendLabels = new[] { "Error Loading Data" };
                OnPropertyChanged(nameof(SalesAmountSeries));
                OnPropertyChanged(nameof(SalesCountSeries));
                OnPropertyChanged(nameof(SalesTrendLabels));
            }
        }

        private void LoadFilterOptions()
        {
            // Load departments from all data, not just filtered data
            Departments.Clear();
            Departments.Add("All");
            
            // Get all departments from the database, not just from performance records
            var allDepartments = _dataService.GetAllDepartments();
            foreach (var dept in allDepartments)
            {
                if (!string.IsNullOrEmpty(dept.DepartmentName) && dept.DepartmentName.Trim() != "")
                {
                    Departments.Add(dept.DepartmentName.Trim());
                }
            }
            
            // Also add any departments that might exist in performance records but not in the main departments table
            var allRecords = _dataService.GetPerformanceRecords();
            var performanceDepartments = allRecords
                .Where(p => !string.IsNullOrEmpty(p.Department) && p.Department.Trim() != "")
                .Select(p => p.Department.Trim())
                .Distinct();
            
            foreach (var dept in performanceDepartments)
            {
                if (!Departments.Contains(dept))
                {
                    Departments.Add(dept);
                }
            }
            
            // Ensure we have at least "All" option
            if (Departments.Count == 1)
            {
                Departments.Add("No Departments Found");
            }
        }

        private void ApplyFilters()
        {
            try
            {
                // Get all data from database
                var allRecords = _dataService.GetPerformanceRecords();
                
                // Get department ID for filtering
                int? selectedDeptId = null;
                if (SelectedDepartment != "All" && !string.IsNullOrEmpty(SelectedDepartment))
                {
                    var allDepartments = _dataService.GetAllDepartments();
                    var selectedDept = allDepartments.FirstOrDefault(d => d.DepartmentName.Equals(SelectedDepartment.Trim(), StringComparison.OrdinalIgnoreCase));
                    selectedDeptId = selectedDept?.DepartmentID;
                }
                
                // Apply filters to the data
                var filteredRecords = allRecords.Where(record =>
                {
                    // Date range filter
                    if (record.ReviewDate.Date < FilterStartDate.Date || record.ReviewDate.Date > FilterEndDate.Date)
                        return false;
                    
                    // Department filter using Department ID
                    if (selectedDeptId.HasValue && record.DepartmentID != selectedDeptId.Value)
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
                UpdatePagination();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex.Message}");
            }
        }

        private void ClearFilters()
        {
            _filterStartDate = new DateTime(2020, 1, 1);
            _filterEndDate = new DateTime(2030, 12, 31);
            _selectedDepartment = "All";

            OnPropertyChanged(nameof(FilterStartDate));
            OnPropertyChanged(nameof(FilterEndDate));
            OnPropertyChanged(nameof(SelectedDepartment));

            // Load all data without any filters
            LoadData();
        }

        private void RefreshAndClear()
        {
            // Store current selected department to preserve it after refresh
            var currentDepartment = SelectedDepartment;
            
            // Clear filters first
            ClearFilters();
            
            // Restore the selected department if it was "All" or a valid department
            if (currentDepartment == "All" || Departments.Contains(currentDepartment))
            {
                SelectedDepartment = currentDepartment;
            }
            else
            {
                SelectedDepartment = "All";
            }
            
            // Then refresh the data
            LoadData();
        }

        private void ExportData()
        {
            // TODO: Implement export functionality
            System.Windows.MessageBox.Show("Export functionality will be implemented soon!", "Export Data", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void AddPerformanceReview()
        {
            try
            {
                var dialog = new AddPerformanceReviewDialog(this);
                
                // Set owner only if MainWindow is available and has been shown
                if (System.Windows.Application.Current.MainWindow != null && 
                    System.Windows.Application.Current.MainWindow.IsVisible)
                {
                    dialog.Owner = System.Windows.Application.Current.MainWindow;
                }
                
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error opening add review dialog: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void UpdatePagination()
        {
            var sortedData = GetSortedData();
            _totalPages = (int)Math.Ceiling((double)sortedData.Count / PageSize);
            if (_totalPages == 0) _totalPages = 1;
            
            if (_currentPage > _totalPages)
                _currentPage = _totalPages;
            
            LoadCurrentPage();
        }

        private void LoadCurrentPage()
        {
            PagedPerformanceRecords.Clear();
            
            // Get sorted data
            var sortedData = GetSortedData();
            
            var startIndex = (_currentPage - 1) * PageSize;
            var endIndex = Math.Min(startIndex + PageSize, sortedData.Count);
            
            for (int i = startIndex; i < endIndex; i++)
            {
                PagedPerformanceRecords.Add(sortedData[i]);
            }
            
            OnPropertyChanged(nameof(PagedPerformanceRecords));
            OnPropertyChanged(nameof(PaginationInfo));
            OnPropertyChanged(nameof(CanGoToFirstPage));
            OnPropertyChanged(nameof(CanGoToPreviousPage));
            OnPropertyChanged(nameof(CanGoToNextPage));
            OnPropertyChanged(nameof(CanGoToLastPage));
        }

        private void GoToFirstPage()
        {
            CurrentPage = 1;
            LoadCurrentPage();
        }

        private void GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadCurrentPage();
            }
        }

        private void GoToNextPage()
        {
            if (CurrentPage < _totalPages)
            {
                CurrentPage++;
                LoadCurrentPage();
            }
        }

        private void GoToLastPage()
        {
            CurrentPage = _totalPages;
            LoadCurrentPage();
        }

        private List<Performance_Model> GetSortedData()
        {
            var data = PerformanceRecords.ToList();
            
            return _sortColumn switch
            {
                "RecordID" => _sortAscending ? data.OrderBy(x => x.RecordID).ToList() : data.OrderByDescending(x => x.RecordID).ToList(),
                "EmployeeName" => _sortAscending ? data.OrderBy(x => x.EmployeeName).ToList() : data.OrderByDescending(x => x.EmployeeName).ToList(),
                "Position" => _sortAscending ? data.OrderBy(x => x.Position).ToList() : data.OrderByDescending(x => x.Position).ToList(),
                "Department" => _sortAscending ? data.OrderBy(x => x.Department).ToList() : data.OrderByDescending(x => x.Department).ToList(),
                "ReviewDate" => _sortAscending ? data.OrderBy(x => x.ReviewDate).ToList() : data.OrderByDescending(x => x.ReviewDate).ToList(),
                "Score" => _sortAscending ? data.OrderBy(x => x.Score).ToList() : data.OrderByDescending(x => x.Score).ToList(),
                "Remarks" => _sortAscending ? data.OrderBy(x => x.Remarks).ToList() : data.OrderByDescending(x => x.Remarks).ToList(),
                _ => _sortAscending ? data.OrderBy(x => x.ReviewDate).ToList() : data.OrderByDescending(x => x.ReviewDate).ToList()
            };
        }

        public void SortData(string columnName)
        {
            if (_sortColumn == columnName)
            {
                // Toggle sort direction if same column
                _sortAscending = !_sortAscending;
            }
            else
            {
                // New column, default to descending for most columns, ascending for dates
                _sortColumn = columnName;
                _sortAscending = columnName == "ReviewDate" || columnName == "RecordID";
            }
            
            // Reset to first page when sorting
            _currentPage = 1;
            UpdatePagination();
        }

        public System.Windows.Input.ICommand Refresh_Command => new View_Model_Command(_ => RefreshAndClear());
        public System.Windows.Input.ICommand ExportData_Command => new View_Model_Command(_ => ExportData());
        public System.Windows.Input.ICommand AddPerformanceReview_Command => new View_Model_Command(_ => AddPerformanceReview());
        public System.Windows.Input.ICommand FirstPage_Command => new View_Model_Command(_ => GoToFirstPage());
        public System.Windows.Input.ICommand PreviousPage_Command => new View_Model_Command(_ => GoToPreviousPage());
        public System.Windows.Input.ICommand NextPage_Command => new View_Model_Command(_ => GoToNextPage());
        public System.Windows.Input.ICommand LastPage_Command => new View_Model_Command(_ => GoToLastPage());
        public System.Windows.Input.ICommand CurrentPage_Command => new View_Model_Command(_ => LoadCurrentPage());
    }
}