using erp_system.model;
using erp_system.repositories;
using FontAwesome.Sharp;
using System;
using System.Windows;
using System.Windows.Input;

namespace erp_system.view_model
{
    public class Main_View_Model : View_Model_Base
    {
        private User_Account_Model _current_user_account = new();
        private View_Model_Base? _current_child_view;
        private string _caption = string.Empty;
        private IconChar _icon;
        private string _current_view = "Dashboard";

        private readonly User_Repository _user_repository;
        
        // Keep view model instances to preserve data
        private Payroll_View_Model? _payrollViewModel;

        public User_Account_Model Current_User_Account
        {
            get => _current_user_account;
            set
            {
                if (_current_user_account != value)
                {
                    _current_user_account = value ?? throw new ArgumentNullException(nameof(value));
                    OnPropertyChanged(nameof(Current_User_Account));
                }
            }
        }

        public View_Model_Base? Current_Child_View 
        { 
            get => _current_child_view; 
            set
            {
                _current_child_view = value;
                OnPropertyChanged(nameof(Current_Child_View));
            }
        }
        public string Caption 
        {
            get => _caption;
            set 
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }
        public IconChar Icon 
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public string CurrentView 
        {
            get => _current_view;
            set
            {
                _current_view = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        //<!-- Commands -->
        public ICommand Show_Dashboard_View_Command { get; }
        public ICommand Show_Employees_View_Command { get; }
        public ICommand Show_Sales_View_Command { get; }
        public ICommand Show_Performance_View_Command { get; }
        public ICommand Show_Insights_Analytics_View_Command { get; }
        public ICommand Show_Payroll_View_Command { get; }
        public ICommand Show_Leave_Management_View_Command { get; }
        public ICommand Show_Form_Documents_View_Command { get; }
        public ICommand Show_User_Profile_Command { get; }
        public ICommand Show_User_Registration_Command { get; }

        public Main_View_Model()
        {
            _user_repository = new User_Repository();
            _current_user_account = new User_Account_Model();   // always initialized

            //Commands Initialization

            Show_Dashboard_View_Command = new View_Model_Command(Execute_Show_Dashboard_View_Command);
            Show_Employees_View_Command = new View_Model_Command(Execute_Show_Employees_View_Command);
            Show_Sales_View_Command = new View_Model_Command(Execute_Show_Sales_View_Command);
            Show_Performance_View_Command = new View_Model_Command(Execute_Show_Performance_View_Command);
            Show_Insights_Analytics_View_Command = new View_Model_Command(Execute_Show_Insights_Analytics_View_Command);
            Show_Payroll_View_Command = new View_Model_Command(Execute_Show_Payroll_View_Command);
            Show_Leave_Management_View_Command = new View_Model_Command(Execute_Show_Leave_Management_View_Command);
            Show_Form_Documents_View_Command = new View_Model_Command(Execute_Show_Form_Documents_View_Command);
            Show_User_Profile_Command = new View_Model_Command(Execute_Show_User_Profile_Command);
            Show_User_Registration_Command = new View_Model_Command(Execute_Show_User_Registration_Command);

            //Default View
            Execute_Show_Dashboard_View_Command(null);

            Load_Current_User_Data();
        }

        private void Execute_Show_Form_Documents_View_Command(object? obj)
        {
            Current_Child_View = new Form_Documents_View_Model();
            Caption = "Form & Documents";
            Icon = IconChar.FileContract;
        }

        private void Execute_Show_Leave_Management_View_Command(object? obj)
        {
            Current_Child_View = new Leave_Management_View_Model();
            Caption = "Leave Management";
            Icon = IconChar.CalendarDay;
            CurrentView = "LeaveManagement";
        }

        private void Execute_Show_Payroll_View_Command(object? obj)
        {
            try
            {
                // Reuse existing instance to preserve data
                if (_payrollViewModel == null)
                {
                    _payrollViewModel = new Payroll_View_Model();
                }
                
                Current_Child_View = _payrollViewModel;
                Caption = "Payroll Management";
                Icon = IconChar.MoneyCheckDollar;
                CurrentView = "Payroll";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Payroll view:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Execute_Show_Insights_Analytics_View_Command(object? obj)
        {
            Current_Child_View = new Insights_Analytics_View_Model();
            Caption = "Insights & Analytics";
            Icon = IconChar.ChartPie;
            CurrentView = "Insights";
        }

        private void Execute_Show_Performance_View_Command(object? obj)
        {
            Current_Child_View = new Performance_View_Model();
            Caption = "Performance Tracking";
            Icon = IconChar.HeartBroken;
            CurrentView = "Performance";
        }

        private void Execute_Show_Sales_View_Command(object? obj)
        {
            Current_Child_View = new Sales_View_Model();
            Caption = "Sales Tracking";
            Icon = IconChar.HandHoldingHand;
            CurrentView = "Sales";
        }

        private void Execute_Show_Employees_View_Command(object? obj)
        {
            try
            {
                Current_Child_View = new Employees_View_Model();
                Caption = "Employee Management";
                Icon = IconChar.BoxesPacking;
                CurrentView = "Employees";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Employees view error: {ex}");
                MessageBox.Show($"Failed to open Employees view:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Execute_Show_Dashboard_View_Command(object? obj)
        {
            var dashboardViewModel = new Dashboard_View_Model();
            
            // Subscribe to navigation events from dashboard
            dashboardViewModel.NavigationRequested += OnDashboardNavigationRequested;
            
            Current_Child_View = dashboardViewModel;
            Caption = "Dashboard";
            Icon = IconChar.Home;
            CurrentView = "Dashboard";
        }
        
        private void OnDashboardNavigationRequested(object? sender, string viewName)
        {
            try
            {
                switch (viewName)
                {
                    case "Employees":
                        Execute_Show_Employees_View_Command(null);
                        break;
                    case "Sales":
                        Execute_Show_Sales_View_Command(null);
                        break;
                    case "Performance":
                        Execute_Show_Performance_View_Command(null);
                        break;
                    case "Payroll":
                        Execute_Show_Payroll_View_Command(null);
                        break;
                    case "Insights":
                        Execute_Show_Insights_Analytics_View_Command(null);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"Unknown navigation request: {viewName}");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Load_Current_User_Data()
        {
            try
            {
                // Identity.Name may be null → guard it
                var username = Thread.CurrentPrincipal?.Identity?.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    var user = _user_repository.GetByUsername(username);
                    if (user != null)
                    {
                        // Map User_Model properties to User_Account_Model using real database data
                        Current_User_Account.UserID = int.TryParse(user.Id, out int userId) ? userId : 0;
                        Current_User_Account.UserName = user.Username ?? string.Empty;
                        Current_User_Account.RoleName = user.RoleName ?? "Employee"; // Use RoleName from database
                        Current_User_Account.EmployeeID = user.EmployeeID;
                        Current_User_Account.CreatedAt = user.HireDate ?? DateTime.Now; // Use HireDate as Member Since
                        Current_User_Account.EmployeeName = $"{user.First_Name} {user.Last_Name}".Trim();
                        return;
                    }
                }

                // fallback if null or error - minimal default values
                Current_User_Account.UserID = 0;
                Current_User_Account.UserName = "guest";
                Current_User_Account.RoleName = "Guest";
                Current_User_Account.EmployeeID = 0;
                Current_User_Account.CreatedAt = DateTime.Now;
                Current_User_Account.EmployeeName = "Guest User";
            }
            catch (Exception ex)
            {
                // Handle database connection issues gracefully
                Current_User_Account.UserID = 0;
                Current_User_Account.UserName = "error";
                Current_User_Account.RoleName = "Error";
                Current_User_Account.EmployeeID = 0;
                Current_User_Account.CreatedAt = DateTime.Now;
                Current_User_Account.EmployeeName = "Database Connection Error";
                System.Diagnostics.Debug.WriteLine($"Error loading user data: {ex.Message}");
            }
        }

        private void Execute_Show_User_Profile_Command(object? obj)
        {
            try
            {
                var userProfileDialog = new UserProfileDialog();
                var userProfileViewModel = new UserProfile_View_Model(Current_User_Account);
                userProfileDialog.DataContext = userProfileViewModel;
                userProfileDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open User Profile dialog:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Execute_Show_User_Registration_Command(object? obj)
        {
            try
            {
                // Check if current user is Admin
                if (Current_User_Account.RoleName?.ToLower() != "admin")
                {
                    MessageBox.Show("Access denied. Only administrators can create user accounts.", 
                                  "Access Denied", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Warning);
                    return;
                }

                var registrationDialog = new UserRegistrationDialog();
                registrationDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open User Registration dialog:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
