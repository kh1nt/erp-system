using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using erp_system.Services;
using erp_system.model;

namespace erp_system.view_model
{
    public class Employees_View_Model : View_Model_Base
    {
        public ObservableCollection<Employee_Model> Employees { get; } = new ObservableCollection<Employee_Model>();
        public ObservableCollection<Department_Model> Departments { get; } = new ObservableCollection<Department_Model>();

        private ICollectionView? _filteredEmployees;
        public ICollectionView? FilteredEmployees
        {
            get => _filteredEmployees;
            private set
            {
                _filteredEmployees = value;
                OnPropertyChanged(nameof(FilteredEmployees));
            }
        }

        // Pagination properties
        private const int PageSize = 10;
        private int _currentPage = 1;
        private int _totalPages = 1;

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value) return;
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                UpdatePagedEmployees();
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            private set
            {
                if (_totalPages == value) return;
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        public ObservableCollection<Employee_Model> PagedEmployees { get; } = new ObservableCollection<Employee_Model>();

        public string PageInfo => $"Page {CurrentPage} of {TotalPages}";

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                _filteredEmployees?.Refresh();
                CurrentPage = 1; // Reset to first page when filtering
                UpdatePagedEmployees();
            }
        }

        private Department_Model? _selectedDepartment;
        public Department_Model? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                if (_selectedDepartment == value) return;
                _selectedDepartment = value;
                OnPropertyChanged(nameof(SelectedDepartment));
                _filteredEmployees?.Refresh();
                CurrentPage = 1; // Reset to first page when filtering
                UpdatePagedEmployees();
            }
        }

        public Employees_View_Model()
        {
            // Set up FilteredEmployees first
            FilteredEmployees = CollectionViewSource.GetDefaultView(Employees);
            FilteredEmployees.Filter = obj =>
            {
                if (obj is not Employee_Model emp) return false;

                var matchText = string.IsNullOrWhiteSpace(SearchText)
                    || emp.FirstName.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)
                    || emp.LastName.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)
                    || emp.Email.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)
                    || emp.Position.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase);

                var matchDept = SelectedDepartment == null
                    || SelectedDepartment.DepartmentID == -1  // "All departments" option
                    || emp.DepartmentID == SelectedDepartment.DepartmentID;

                return matchText && matchDept;
            };

            // Then load data
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data in constructor: {ex.Message}");
                // Continue with empty collections to prevent crash
            }
        }

        public void LoadData()
        {
            try
            {
                Employees.Clear();
                Departments.Clear();
                
                // Add "All departments" option first
                Departments.Add(new Department_Model { DepartmentID = -1, DepartmentName = "All departments" });
                
                var data = new DataService();
                var departments = data.GetDepartments();
                var employees = data.GetEmployees();
                
                foreach (var d in departments)
                    Departments.Add(d);
                foreach (var e in employees)
                    Employees.Add(e);
                    
                // Set "All departments" as default selection
                SelectedDepartment = Departments.FirstOrDefault();
                
                // Refresh filtered view and update pagination after loading data
                _filteredEmployees?.Refresh();
                CurrentPage = 1;
                UpdatePagedEmployees();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadData: {ex.Message}");
                // Keep collections empty but don't crash
            }
        }

        private void UpdatePagedEmployees()
        {
            if (_filteredEmployees == null) return;

            var filteredList = _filteredEmployees.Cast<Employee_Model>().ToList();
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)filteredList.Count / PageSize));
            
            // Ensure current page is valid
            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;
            if (CurrentPage < 1)
                CurrentPage = 1;

            var startIndex = (CurrentPage - 1) * PageSize;
            var endIndex = Math.Min(startIndex + PageSize, filteredList.Count);

            PagedEmployees.Clear();
            for (int i = startIndex; i < endIndex; i++)
            {
                PagedEmployees.Add(filteredList[i]);
            }

            OnPropertyChanged(nameof(PageInfo));
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CurrentPage));
        }

        public System.Windows.Input.ICommand Refresh_Command => new View_Model_Command(_ =>
        {
            LoadData();
        });


        public System.Windows.Input.ICommand AddEmployee_Command => new View_Model_Command(_ => ShowAddEmployeeDialog());
        public System.Windows.Input.ICommand UpdateStatus_Command => new View_Model_Command(parameter => UpdateEmployeeStatus(parameter));

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

        private void ShowAddEmployeeDialog()
        {
            try
            {
                var dialog = new view.EmployeeDialog();
                var viewModel = new EmployeeDialogViewModel();
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
                System.Windows.MessageBox.Show($"Error opening add employee dialog: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void UpdateEmployeeStatus(object parameter)
        {
            try
            {
                if (parameter is Employee_Model employee)
                {
                    // Open status update dialog
                    var dialog = new view.StatusUpdateDialog();
                    var viewModel = new StatusUpdateDialogViewModel
                    {
                        Employee = employee
                    };
                    dialog.DataContext = viewModel;

                    if (dialog.ShowDialog() == true)
                    {
                        var newStatus = viewModel.SelectedStatus;
                        
                        // Check if status actually changed
                        if (newStatus != employee.Status)
                        {
                            // Update in database
                            var dataService = new DataService();
                            if (dataService.UpdateEmployeeStatus(employee.EmployeeID, newStatus))
                            {
                                // Update local collection
                                employee.Status = newStatus;
                                _filteredEmployees?.Refresh();
                                UpdatePagedEmployees();
                                
                                System.Windows.MessageBox.Show($"Employee status updated to: {newStatus}", "Status Updated", 
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Failed to update employee status", "Error", 
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating employee status: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public void UpdateEmployeeStatusFromLeave(int employeeId, string status)
        {
            try
            {
                var dataService = new DataService();
                if (dataService.UpdateEmployeeStatus(employeeId, status))
                {
                    // Update local collection
                    var employee = Employees.FirstOrDefault(e => e.EmployeeID == employeeId);
                    if (employee != null)
                    {
                        employee.Status = status;
                        _filteredEmployees?.Refresh();
                        UpdatePagedEmployees();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating employee status from leave: {ex.Message}");
            }
        }
    }
}