using erp_system.model;
using erp_system.repositories;
using erp_system.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace erp_system.view_model
{
    public class UserRegistrationViewModel : View_Model_Base
    {
        private readonly User_Repository _userRepository;
        private readonly Employee_Repository _employeeRepository;
        private readonly DataService _dataService;

        // Collections
        private ObservableCollection<Employee_Model> _hrEmployees;
        private ObservableCollection<string> _availableRoles;

        // Selected items
        private Employee_Model? _selectedEmployee;
        private string _selectedRole = string.Empty;

        // Form fields
        private string _username = string.Empty;
        private string _password = string.Empty;

        // Error states
        private string _employeeSelectionError = string.Empty;
        private string _usernameError = string.Empty;
        private string _roleError = string.Empty;
        private string _passwordError = string.Empty;

        // Commands
        public ICommand CreateUserCommand { get; }

        public UserRegistrationViewModel()
        {
            _userRepository = new User_Repository();
            _employeeRepository = new Employee_Repository();
            _dataService = new DataService();

            _hrEmployees = new ObservableCollection<Employee_Model>();
            _availableRoles = new ObservableCollection<string>();

            CreateUserCommand = new View_Model_Command(ExecuteCreateUserCommand);

            LoadHREmployees();
            LoadAvailableRoles();
        }

        #region Properties

        public ObservableCollection<Employee_Model> HREmployees
        {
            get => _hrEmployees;
            set
            {
                _hrEmployees = value;
                OnPropertyChanged(nameof(HREmployees));
            }
        }

        public ObservableCollection<string> AvailableRoles
        {
            get => _availableRoles;
            set
            {
                _availableRoles = value;
                OnPropertyChanged(nameof(AvailableRoles));
            }
        }

        public Employee_Model? SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
                ClearEmployeeSelectionError();
                
                // Auto-generate username based on employee
                if (value != null)
                {
                    Username = GenerateUsernameFromEmployee(value);
                }
            }
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged(nameof(SelectedRole));
                ClearRoleError();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                ClearUsernameError();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                ClearPasswordError();
            }
        }

        // Error Properties
        public string EmployeeSelectionError
        {
            get => _employeeSelectionError;
            set
            {
                _employeeSelectionError = value;
                OnPropertyChanged(nameof(EmployeeSelectionError));
                OnPropertyChanged(nameof(HasEmployeeSelectionError));
            }
        }

        public bool HasEmployeeSelectionError => !string.IsNullOrEmpty(_employeeSelectionError);

        public string UsernameError
        {
            get => _usernameError;
            set
            {
                _usernameError = value;
                OnPropertyChanged(nameof(UsernameError));
                OnPropertyChanged(nameof(HasUsernameError));
            }
        }

        public bool HasUsernameError => !string.IsNullOrEmpty(_usernameError);

        public string RoleError
        {
            get => _roleError;
            set
            {
                _roleError = value;
                OnPropertyChanged(nameof(RoleError));
                OnPropertyChanged(nameof(HasRoleError));
            }
        }

        public bool HasRoleError => !string.IsNullOrEmpty(_roleError);

        public string PasswordError
        {
            get => _passwordError;
            set
            {
                _passwordError = value;
                OnPropertyChanged(nameof(PasswordError));
                OnPropertyChanged(nameof(HasPasswordError));
            }
        }

        public bool HasPasswordError => !string.IsNullOrEmpty(_passwordError);

        #endregion

        #region Methods

        private void LoadHREmployees()
        {
            try
            {
                // Get all employees from HR department (DepartmentID = 1)
                var allEmployees = _dataService.GetEmployeesByDepartment("Human Resources");
                
                // Filter out employees who already have user accounts
                var employeesWithoutAccounts = allEmployees.Where(emp => !EmployeeHasUserAccount(emp.EmployeeID)).ToList();

                HREmployees.Clear();
                foreach (var employee in employeesWithoutAccounts)
                {
                    HREmployees.Add(employee);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading HR employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAvailableRoles()
        {
            AvailableRoles.Clear();
            AvailableRoles.Add("HR Manager");
            AvailableRoles.Add("HR Specialist");
            AvailableRoles.Add("HR Assistant");
            AvailableRoles.Add("HR Coordinator");
            AvailableRoles.Add("HR Generalist");
        }

        private bool EmployeeHasUserAccount(int employeeId)
        {
            try
            {
                // Check if employee already has a user account
                var existingUser = _userRepository.GetByEmployeeId(employeeId);
                return existingUser != null;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateUsernameFromEmployee(Employee_Model employee)
        {
            // Generate username from employee's first and last name
            var firstName = employee.FirstName.ToLower().Replace(" ", "");
            var lastName = employee.LastName.ToLower().Replace(" ", "");
            
            // Check if username already exists and add number if needed
            var baseUsername = $"{firstName}.{lastName}";
            var username = baseUsername;
            var counter = 1;

            while (UsernameExists(username))
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            return username;
        }

        private bool UsernameExists(string username)
        {
            try
            {
                var existingUser = _userRepository.GetByUsername(username);
                return existingUser != null;
            }
            catch
            {
                return false;
            }
        }



        private bool ValidateForm()
        {
            bool isValid = true;

            // Validate employee selection
            if (SelectedEmployee == null)
            {
                EmployeeSelectionError = "Please select an HR employee.";
                isValid = false;
            }

            // Validate username
            if (string.IsNullOrWhiteSpace(Username))
            {
                UsernameError = "Username is required.";
                isValid = false;
            }
            else if (UsernameExists(Username))
            {
                UsernameError = "Username already exists.";
                isValid = false;
            }
            else if (Username.Length < 3)
            {
                UsernameError = "Username must be at least 3 characters long.";
                isValid = false;
            }

            // Validate role
            if (string.IsNullOrWhiteSpace(SelectedRole))
            {
                RoleError = "Please select a role.";
                isValid = false;
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = "Please enter a password.";
                isValid = false;
            }
            else if (!IsPasswordValid(Password))
            {
                PasswordError = "Password does not meet requirements.";
                isValid = false;
            }

            return isValid;
        }

        private bool IsPasswordValid(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            return true;
        }

        private void ClearAllErrors()
        {
            EmployeeSelectionError = string.Empty;
            UsernameError = string.Empty;
            RoleError = string.Empty;
            PasswordError = string.Empty;
        }

        private void ClearEmployeeSelectionError()
        {
            EmployeeSelectionError = string.Empty;
        }

        private void ClearUsernameError()
        {
            UsernameError = string.Empty;
        }

        private void ClearRoleError()
        {
            RoleError = string.Empty;
        }

        private void ClearPasswordError()
        {
            PasswordError = string.Empty;
        }

        #endregion

        #region Commands

        private void ExecuteCreateUserCommand(object? parameter)
        {
            try
            {
                ClearAllErrors();

                if (!ValidateForm())
                {
                    return;
                }

                // Create new user account (password will be hashed by repository)
                var newUser = new User_Model
                {
                    Username = Username,
                    Password = Password, // Plain password - repository will hash it
                    EmployeeID = SelectedEmployee!.EmployeeID,
                    First_Name = SelectedEmployee.FirstName,
                    Last_Name = SelectedEmployee.LastName,
                    Email = SelectedEmployee.Email,
                    Position = SelectedRole,
                    HireDate = SelectedEmployee.HireDate
                };

                // Add user to database
                _userRepository.Add(newUser);

                MessageBox.Show(
                    $"User account created successfully!\n\n" +
                    $"Employee: {SelectedEmployee.FullName}\n" +
                    $"Username: {Username}\n" +
                    $"Role: {SelectedRole}\n\n" +
                    $"Please provide the login credentials to the employee securely.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Reset form
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating user account: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetForm()
        {
            SelectedEmployee = null;
            SelectedRole = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            ClearAllErrors();
            LoadHREmployees(); // Refresh the list
        }

        #endregion
    }
}
