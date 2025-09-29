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
            }
        }

        public Employees_View_Model()
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data in constructor: {ex.Message}");
                // Continue with empty collections to prevent crash
            }

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
                    || emp.DepartmentID == SelectedDepartment.DepartmentID;

                return matchText && matchDept;
            };
        }

        public void LoadData()
        {
            try
            {
                Employees.Clear();
                Departments.Clear();
                var data = new DataService();
                foreach (var d in data.GetDepartments())
                    Departments.Add(d);
                foreach (var e in data.GetEmployees())
                    Employees.Add(e);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadData: {ex.Message}");
                // Keep collections empty but don't crash
            }
        }

        public System.Windows.Input.ICommand Refresh_Command => new View_Model_Command(_ =>
        {
            LoadData();
            _filteredEmployees?.Refresh();
        });
    }
}