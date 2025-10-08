using System;
using System.Collections.ObjectModel;
using System.Windows;
using erp_system.model;
using erp_system.repositories;
using erp_system.Services;

namespace erp_system.view_model
{
    public class EmployeeDialogViewModel : View_Model_Base
    {
        private readonly Employee_Repository _employeeRepository;
        private readonly DataService _dataService;
        private Employee_Model? _originalEmployee;
        private bool _isEditMode;

        public ObservableCollection<Department_Model> Departments { get; } = new ObservableCollection<Department_Model>();
        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string>
        {
            "Active",
            "Inactive", 
            "On Leave",
            "Terminated"
        };

        // Events
        public event EventHandler<bool>? DialogClosed;

        // Properties
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private DateTime _hireDate = DateTime.Today;
        private string _position = string.Empty;
        private decimal _basicSalary = 0;
        private string _status = "Active";
        private Department_Model? _selectedDepartment;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public DateTime HireDate
        {
            get => _hireDate;
            set
            {
                _hireDate = value;
                OnPropertyChanged(nameof(HireDate));
            }
        }

        public string Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        public decimal BasicSalary
        {
            get => _basicSalary;
            set
            {
                _basicSalary = value;
                OnPropertyChanged(nameof(BasicSalary));
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public Department_Model? SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged(nameof(SelectedDepartment));
            }
        }

        public string DialogTitle => _isEditMode ? "Edit Employee" : "Add New Employee";
        public string SaveButtonText => _isEditMode ? "Update" : "Save";

        public EmployeeDialogViewModel(Employee_Model? employee = null)
        {
            _employeeRepository = new Employee_Repository();
            _dataService = new DataService();
            _originalEmployee = employee;
            _isEditMode = employee != null;

            LoadDepartments();
            
            if (_isEditMode)
            {
                LoadEmployeeData();
            }
        }

        private void LoadDepartments()
        {
            Departments.Clear();
            foreach (var department in _dataService.GetDepartments())
            {
                Departments.Add(department);
            }
        }

        private void LoadEmployeeData()
        {
            if (_originalEmployee == null) return;

            FirstName = _originalEmployee.FirstName;
            LastName = _originalEmployee.LastName;
            Email = _originalEmployee.Email;
            Phone = _originalEmployee.Phone;
            HireDate = _originalEmployee.HireDate;
            Position = _originalEmployee.Position;
            BasicSalary = _originalEmployee.BasicSalary;
            Status = _originalEmployee.Status;

            // Find and select the department
            foreach (var department in Departments)
            {
                if (department.DepartmentID == _originalEmployee.DepartmentID)
                {
                    SelectedDepartment = department;
                    break;
                }
            }
        }

        public System.Windows.Input.ICommand Save_Command => new View_Model_Command(_ => SaveEmployee());
        public System.Windows.Input.ICommand Cancel_Command => new View_Model_Command(_ => Cancel());

        private void SaveEmployee()
        {
            if (!ValidateInput())
                return;

            try
            {
                var employee = new Employee_Model
                {
                    FirstName = FirstName.Trim(),
                    LastName = LastName.Trim(),
                    Email = Email.Trim(),
                    Phone = Phone.Trim(),
                    HireDate = HireDate,
                    Position = Position.Trim(),
                    BasicSalary = BasicSalary,
                    Status = Status.Trim(),
                    DepartmentID = SelectedDepartment!.DepartmentID
                };

                if (_isEditMode)
                {
                    employee.EmployeeID = _originalEmployee!.EmployeeID;
                    _employeeRepository.Update(employee);
                    MessageBox.Show("Employee updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _employeeRepository.Add(employee);
                    MessageBox.Show("Employee added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogClosed?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                MessageBox.Show("Please enter a first name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                MessageBox.Show("Please enter a last name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Please enter an email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!IsValidEmail(Email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Position))
            {
                MessageBox.Show("Please enter a position.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedDepartment == null)
            {
                MessageBox.Show("Please select a department.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (BasicSalary < 0)
            {
                MessageBox.Show("Basic salary cannot be negative.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void Cancel()
        {
            DialogClosed?.Invoke(this, false);
        }
    }
}
