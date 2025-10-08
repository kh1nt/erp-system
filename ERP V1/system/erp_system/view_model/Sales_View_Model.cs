using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using erp_system.Services;
using erp_system.model;
using erp_system.repositories;

namespace erp_system.view_model
{
    public class Sales_View_Model : View_Model_Base
    {
        private readonly DataService _dataService;
        private readonly Sales_Repository _salesRepository;

        public ObservableCollection<Sales_Model> Sales { get; } = new ObservableCollection<Sales_Model>();
        public ObservableCollection<Sales_Model> FilteredSales { get; } = new ObservableCollection<Sales_Model>();
        public ObservableCollection<Sales_Model> PagedSales { get; } = new ObservableCollection<Sales_Model>();
        public ObservableCollection<Employee_Model> Employees { get; } = new ObservableCollection<Employee_Model>();
        public ObservableCollection<Department_Model> Departments { get; } = new ObservableCollection<Department_Model>();
        
        // Pagination properties
        private int _currentPage = 1;
        private const int PageSize = 7;
        
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                UpdatePagedSales();
            }
        }
        
        public int TotalPages => (int)Math.Ceiling((double)FilteredSales.Count / PageSize);
        public string PageInfo => $"Page {CurrentPage} of {TotalPages}";
        
        
        public Dictionary<string, decimal> SalesSummary { get; set; } = new Dictionary<string, decimal>();
        
        // Top Performer Properties
        private string _topPerformer = "N/A";
        private decimal _topPerformerSales = 0;

        public string TopPerformer
        {
            get => _topPerformer;
            set
            {
                _topPerformer = value;
                OnPropertyChanged(nameof(TopPerformer));
            }
        }

        public decimal TopPerformerSales
        {
            get => _topPerformerSales;
            set
            {
                _topPerformerSales = value;
                OnPropertyChanged(nameof(TopPerformerSales));
            }
        }

        // Filter Properties
        private DateTime? _filterStartDate;
        private DateTime? _filterEndDate;
        private Employee_Model _selectedEmployee;
        private Department_Model _selectedDepartment;
        private string _searchText = string.Empty;
        private string _minAmount = string.Empty;
        private string _maxAmount = string.Empty;

        public DateTime? FilterStartDate
        {
            get => _filterStartDate;
            set
            {
                _filterStartDate = value;
                OnPropertyChanged(nameof(FilterStartDate));
                ApplyFilters();
            }
        }

        public DateTime? FilterEndDate
        {
            get => _filterEndDate;
            set
            {
                _filterEndDate = value;
                OnPropertyChanged(nameof(FilterEndDate));
                ApplyFilters();
            }
        }

        public Employee_Model? SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
                ApplyFilters();
            }
        }

        public Department_Model? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged(nameof(SelectedDepartment));
                ApplyFilters();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilters();
            }
        }

        public string MinAmount
        {
            get => _minAmount;
            set
            {
                _minAmount = value;
                OnPropertyChanged(nameof(MinAmount));
                ApplyFilters();
            }
        }

        public string MaxAmount
        {
            get => _maxAmount;
            set
            {
                _maxAmount = value;
                OnPropertyChanged(nameof(MaxAmount));
                ApplyFilters();
            }
        }

        // Summary Properties
        public int FilteredSalesCount => FilteredSales.Count;
        public decimal FilteredTotalAmount => FilteredSales.Sum(s => s.Amount);
        public decimal FilteredAverageAmount => FilteredSales.Count > 0 ? FilteredSales.Average(s => s.Amount) : 0;
        public string FilteredDateRange
        {
            get
            {
                if (FilterStartDate.HasValue && FilterEndDate.HasValue)
                    return $"{FilterStartDate.Value:MMM dd} - {FilterEndDate.Value:MMM dd, yyyy}";
                else if (FilterStartDate.HasValue)
                    return $"From {FilterStartDate.Value:MMM dd, yyyy}";
                else if (FilterEndDate.HasValue)
                    return $"Until {FilterEndDate.Value:MMM dd, yyyy}";
                return "All Time";
            }
        }

        public Sales_View_Model()
        {
            _dataService = new DataService();
            _salesRepository = new Sales_Repository();
            LoadData();
            LoadEmployeesAndDepartments();
        }

        public void LoadData()
        {
            Sales.Clear();
            foreach (var sale in _dataService.GetSalesRecords())
                Sales.Add(sale);
            
            SalesSummary = _dataService.GetSalesSummary();
            OnPropertyChanged(nameof(SalesSummary));
            
            // Load top performer
            var (name, amount) = _dataService.GetTopPerformer();
            TopPerformer = name;
            TopPerformerSales = amount;
            
            ApplyFilters();
        }


        private void LoadEmployeesAndDepartments()
        {
            Employees.Clear();
            foreach (var employee in _dataService.GetEmployees())
            {
                Employees.Add(employee);
            }

            Departments.Clear();
            foreach (var department in _dataService.GetDepartments())
                Departments.Add(department);
        }

        private void ApplyFilters()
        {
            FilteredSales.Clear();
            
            var filtered = Sales.AsEnumerable();

            // Date filters
            if (FilterStartDate.HasValue)
                filtered = filtered.Where(s => s.SaleDate.Date >= FilterStartDate.Value.Date);
            
            if (FilterEndDate.HasValue)
                filtered = filtered.Where(s => s.SaleDate.Date <= FilterEndDate.Value.Date);

            // Employee filter
            if (SelectedEmployee != null)
                filtered = filtered.Where(s => s.EmployeeID == SelectedEmployee.EmployeeID);

            // Department filter
            if (SelectedDepartment != null)
                filtered = filtered.Where(s => s.Department == SelectedDepartment.DepartmentName);

            // Amount filters
            if (decimal.TryParse(MinAmount, out decimal minAmount))
                filtered = filtered.Where(s => s.Amount >= minAmount);

            if (decimal.TryParse(MaxAmount, out decimal maxAmount))
                filtered = filtered.Where(s => s.Amount <= maxAmount);

            // Search text filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(s => 
                    s.Description.ToLower().Contains(searchLower) ||
                    s.EmployeeName.ToLower().Contains(searchLower) ||
                    s.Department.ToLower().Contains(searchLower));
            }

            foreach (var sale in filtered.OrderByDescending(s => s.SaleID))
                FilteredSales.Add(sale);

            // Update summary properties
            OnPropertyChanged(nameof(FilteredSalesCount));
            OnPropertyChanged(nameof(FilteredTotalAmount));
            OnPropertyChanged(nameof(FilteredAverageAmount));
            OnPropertyChanged(nameof(FilteredDateRange));
            
            // Reset to first page and update paged sales
            CurrentPage = 1;
            UpdatePagedSales();
        }
        
        private void UpdatePagedSales()
        {
            var filteredList = FilteredSales.ToList();
            
            // Ensure current page is valid
            if (CurrentPage > TotalPages && TotalPages > 0)
                CurrentPage = TotalPages;
            if (CurrentPage < 1)
                CurrentPage = 1;

            var startIndex = (CurrentPage - 1) * PageSize;
            var endIndex = Math.Min(startIndex + PageSize, filteredList.Count);

            PagedSales.Clear();
            for (int i = startIndex; i < endIndex; i++)
            {
                PagedSales.Add(filteredList[i]);
            }

            OnPropertyChanged(nameof(PageInfo));
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CurrentPage));
        }

        // Commands
        public System.Windows.Input.ICommand Refresh_Command => new View_Model_Command(_ => LoadData());
        
        public System.Windows.Input.ICommand Search_Command => new View_Model_Command(_ => ApplyFilters());
        
        public System.Windows.Input.ICommand ClearFilters_Command => new View_Model_Command(_ =>
        {
            FilterStartDate = null;
            FilterEndDate = null;
            SelectedEmployee = null;
            SelectedDepartment = null;
            SearchText = string.Empty;
            MinAmount = string.Empty;
            MaxAmount = string.Empty;
        });

        public System.Windows.Input.ICommand FilterToday_Command => new View_Model_Command(_ =>
        {
            var today = DateTime.Today;
            FilterStartDate = today;
            FilterEndDate = today;
        });

        public System.Windows.Input.ICommand FilterThisWeek_Command => new View_Model_Command(_ =>
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            FilterStartDate = startOfWeek;
            FilterEndDate = today;
        });

        public System.Windows.Input.ICommand FilterThisMonth_Command => new View_Model_Command(_ =>
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            FilterStartDate = startOfMonth;
            FilterEndDate = today;
        });

        public System.Windows.Input.ICommand FilterThisYear_Command => new View_Model_Command(_ =>
        {
            var today = DateTime.Today;
            var startOfYear = new DateTime(today.Year, 1, 1);
            FilterStartDate = startOfYear;
            FilterEndDate = today;
        });

        public System.Windows.Input.ICommand AddSale_Command => new View_Model_Command(_ => ShowAddSaleDialog());
        
        public System.Windows.Input.ICommand EditSale_Command => new View_Model_Command(sale => ShowEditSaleDialog(sale as Sales_Model));
        
        public System.Windows.Input.ICommand DeleteSale_Command => new View_Model_Command(sale => DeleteSale(sale as Sales_Model));
        
        public System.Windows.Input.ICommand ShowAnalytics_Command => new View_Model_Command(_ => ShowAnalytics());
        
        public System.Windows.Input.ICommand ExportReport_Command => new View_Model_Command(_ => ExportReport());
        
        public System.Windows.Input.ICommand SetTargets_Command => new View_Model_Command(_ => SetTargets());
        
        // Pagination commands
        public System.Windows.Input.ICommand FirstPage_Command => new View_Model_Command(_ =>
        {
            CurrentPage = 1;
        });

        public System.Windows.Input.ICommand PreviousPage_Command => new View_Model_Command(_ =>
        {
            if (CurrentPage > 1)
                CurrentPage--;
        });

        public System.Windows.Input.ICommand NextPage_Command => new View_Model_Command(_ =>
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        });

        public System.Windows.Input.ICommand LastPage_Command => new View_Model_Command(_ =>
        {
            CurrentPage = TotalPages;
        });

        private void ShowAddSaleDialog()
        {
            try
            {
                var dialog = new view.SalesDialog();
                var viewModel = new SalesDialogViewModel();
                viewModel.DialogClosed += (sender, result) =>
                {
                    dialog.DialogResult = result;
                    dialog.Close();
                };
                dialog.DataContext = viewModel;
                
                if (dialog.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening add sale dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowEditSaleDialog(Sales_Model? sale)
        {
            if (sale == null) return;
            
            try
            {
                var dialog = new view.SalesDialog();
                var viewModel = new SalesDialogViewModel(sale);
                viewModel.DialogClosed += (sender, result) =>
                {
                    dialog.DialogResult = result;
                    dialog.Close();
                };
                dialog.DataContext = viewModel;
                
                if (dialog.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening edit sale dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSale(Sales_Model sale)
        {
            if (sale == null) return;
            
            var result = MessageBox.Show($"Are you sure you want to delete this sale?\n\nDate: {sale.SaleDate:yyyy-MM-dd}\nAmount: ₱{sale.Amount:N2}\nDescription: {sale.Description}", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _salesRepository.Delete(sale.SaleID);
                    LoadData();
                    MessageBox.Show("Sale deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting sale: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ShowAnalytics()
        {
            // TODO: Implement analytics view
            MessageBox.Show("Sales Analytics will be implemented in the next phase.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportReport()
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|Excel files (*.xlsx)|*.xlsx",
                    DefaultExt = "csv",
                    FileName = $"Sales_Report_{DateTime.Now:yyyyMMdd}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    // TODO: Implement actual export functionality
                    MessageBox.Show($"Export functionality will save to: {saveDialog.FileName}\n\nThis will be implemented in the next phase.", "Export Preview", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error preparing export: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetTargets()
        {
            // TODO: Implement sales targets dialog
            MessageBox.Show("Sales Targets management will be implemented in the next phase.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
