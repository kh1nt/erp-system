using erp_system.model;
using erp_system.repositories;
using FontAwesome.Sharp;
using System.Windows.Input;

namespace erp_system.view_model
{
    public class Main_View_Model : View_Model_Base
    {
        private User_Account_Model _current_user_account;
        private View_Model_Base _current_child_view;
        private string _caption;
        private IconChar _icon;

        private readonly User_Repository _user_repository;

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

        public View_Model_Base Current_Child_View 
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

        //<!-- Commands -->
        public ICommand Show_Dashboard_View_Command { get; }
        public ICommand Show_Employees_View_Command { get; }
        public ICommand Show_Sales_View_Command { get; }
        public ICommand Show_Performance_View_Command { get; }
        public ICommand Show_Insights_Analytics_View_Command { get; }
        public ICommand Show_Payroll_View_Command { get; }
        public ICommand Show_Leave_Management_View_Command { get; }
        public ICommand Show_Form_Documents_View_Command { get; }

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
        }

        private void Execute_Show_Payroll_View_Command(object? obj)
        {
            Current_Child_View = new Payroll_View_Model();
            Caption = "Payroll Management";
            Icon = IconChar.MoneyCheckDollar;
        }

        private void Execute_Show_Insights_Analytics_View_Command(object? obj)
        {
            Current_Child_View = new Insights_Analytics_View_Model();
            Caption = "Insights & Analytics";
            Icon = IconChar.ChartPie;
        }

        private void Execute_Show_Performance_View_Command(object? obj)
        {
            Current_Child_View = new Performance_View_Model();
            Caption = "Performance Tracking";
            Icon = IconChar.HeartBroken;
        }

        private void Execute_Show_Sales_View_Command(object? obj)
        {
            Current_Child_View = new Sales_View_Model();
            Caption = "Sales Tracking";
            Icon = IconChar.HandHoldingHand;
        }

        private void Execute_Show_Employees_View_Command(object? obj)
        {
            Current_Child_View = new Employees_View_Model();
            Caption = "Employee Management";
            Icon = IconChar.BoxesPacking;
        }

        private void Execute_Show_Dashboard_View_Command(object? obj)
        {
            Current_Child_View = new Dashboard_View_Model();
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }

        private void Load_Current_User_Data()
        {
            // Identity.Name may be null → guard it
            var username = Thread.CurrentPrincipal?.Identity?.Name;

            if (!string.IsNullOrEmpty(username))
            {
                var user = _user_repository.GetByUsername(username);
                if (user != null)
                {
                    Current_User_Account.Username = user.Username ?? string.Empty;
                    Current_User_Account.Display_Name = $"{user.First_Name} {user.Last_Name}";
                    //Current_User_Account.Profile_Picture = user.Profile_Picture;  safe assign
                    return;
                }
            }

            // fallback if null or error
            Current_User_Account.Display_Name = "An error has occurred.";
        }
    }
}
