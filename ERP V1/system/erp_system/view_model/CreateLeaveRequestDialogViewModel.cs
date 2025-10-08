using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using erp_system.model;
using erp_system.repositories;
using erp_system.Services;

namespace erp_system.view_model
{
    public class CreateLeaveRequestDialogViewModel : View_Model_Base
    {
        private readonly LeaveRequest_Repository _leaveRequestRepository;
        private readonly Leave_Repository _leaveRepository;
        private readonly Employee_Repository _employeeRepository;
        private readonly DataService _dataService;

        public ObservableCollection<Leave_Model> LeaveTypes { get; } = new ObservableCollection<Leave_Model>();
        public ObservableCollection<Employee_Model> Employees { get; } = new ObservableCollection<Employee_Model>();

        // Properties for form
        private Employee_Model? _selectedEmployee;
        public Employee_Model? SelectedEmployee
        {
            get => _selectedEmployee;
            set 
            { 
                SetProperty(ref _selectedEmployee, value);
                UpdateAvailableDays();
                ValidateForm();
            }
        }

        private Leave_Model? _selectedLeaveType;
        public Leave_Model? SelectedLeaveType
        {
            get => _selectedLeaveType;
            set 
            { 
                SetProperty(ref _selectedLeaveType, value);
                UpdateAvailableDays();
                SetDatesBasedOnLeaveType();
                ValidateForm();
            }
        }

        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => _startDate;
            set 
            { 
                SetProperty(ref _startDate, value);
                ValidateForm();
            }
        }

        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => _endDate;
            set 
            { 
                SetProperty(ref _endDate, value);
                ValidateForm();
            }
        }

        private string _reason = string.Empty;
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        private int _availableDays;
        public int AvailableDays
        {
            get => _availableDays;
            set => SetProperty(ref _availableDays, value);
        }

        // Validation properties
        private bool _hasValidationErrors;
        public bool HasValidationErrors
        {
            get => _hasValidationErrors;
            set => SetProperty(ref _hasValidationErrors, value);
        }

        private string _validationMessage = string.Empty;
        public string ValidationMessage
        {
            get => _validationMessage;
            set => SetProperty(ref _validationMessage, value);
        }

        // Events
        public event EventHandler? RequestCreated;
        public event EventHandler? DialogClosed;

        public CreateLeaveRequestDialogViewModel()
        {
            _leaveRequestRepository = new LeaveRequest_Repository();
            _leaveRepository = new Leave_Repository();
            _employeeRepository = new Employee_Repository();
            _dataService = new DataService();
            
            LoadLeaveTypes();
            LoadEmployees();
        }

        private void LoadLeaveTypes()
        {
            try
            {
                LeaveTypes.Clear();
                var types = _dataService.GetLeaveTypes();
                foreach (var type in types)
                {
                    LeaveTypes.Add(type);
                }
            }
            catch (Exception ex)
            {
                // Error loading leave types
                MessageBox.Show($"Error loading leave types: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadEmployees()
        {
            try
            {
                Employees.Clear();
                var employees = _dataService.GetEmployees();
                foreach (var employee in employees)
                {
                    Employees.Add(employee);
                }
            }
            catch (Exception ex)
            {
                // Error loading employees
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateAvailableDays()
        {
            try
            {
                if (SelectedEmployee != null && SelectedLeaveType != null)
                {
                    var balance = _dataService.GetEmployeeLeaveBalance(SelectedEmployee.EmployeeID, SelectedLeaveType.LeaveID);
                    AvailableDays = balance;
                }
                else
                {
                    AvailableDays = 0;
                }
            }
            catch (Exception)
            {
                // Error updating available days
                AvailableDays = 0;
            }
        }

        private void SetDatesBasedOnLeaveType()
        {
            if (SelectedLeaveType == null)
            {
                // Reset to today if no leave type selected
                StartDate = DateTime.Today;
                EndDate = DateTime.Today;
                return;
            }

            var leaveTypeName = SelectedLeaveType.TypeName.ToLower();
            var today = DateTime.Today;

            switch (leaveTypeName)
            {
                case var name when name.Contains("sick"):
                    // Sick leave: starts today, typically 1-3 days
                    StartDate = today;
                    EndDate = today.AddDays(2); // 3 days total
                    break;

                case var name when name.Contains("annual") || name.Contains("vacation"):
                    // Annual leave: starts next Monday, typically 1 week
                    var nextMonday = today.AddDays((8 - (int)today.DayOfWeek) % 7);
                    if (nextMonday == today) nextMonday = today.AddDays(7);
                    StartDate = nextMonday;
                    EndDate = nextMonday.AddDays(6); // 1 week
                    break;

                case var name when name.Contains("maternity"):
                    // Maternity leave: starts in 2 weeks, typically 3 months
                    StartDate = today.AddDays(14);
                    EndDate = today.AddDays(14 + 90); // 3 months
                    break;

                case var name when name.Contains("personal"):
                    // Personal leave: starts tomorrow, typically 1-2 days
                    StartDate = today.AddDays(1);
                    EndDate = today.AddDays(2);
                    break;

                case var name when name.Contains("emergency"):
                    // Emergency leave: starts today, typically 1 day
                    StartDate = today;
                    EndDate = today;
                    break;

                default:
                    // Default: starts tomorrow, 1 day
                    StartDate = today.AddDays(1);
                    EndDate = today.AddDays(1);
                    break;
            }

        }

        private void ValidateForm()
        {
            var errors = new List<string>();

            if (SelectedEmployee == null)
                errors.Add("Please select an employee.");

            if (SelectedLeaveType == null)
                errors.Add("Please select a leave type.");

            if (StartDate > EndDate)
                errors.Add("Start date cannot be after end date.");

            if (StartDate < DateTime.Today)
                errors.Add("Start date cannot be in the past.");

            if (SelectedEmployee != null && SelectedLeaveType != null)
            {
                var requestedDays = (EndDate - StartDate).Days + 1;
                
                // Check if duration exceeds MaxDays for this leave type
                if (requestedDays > SelectedLeaveType.MaxDays)
                    errors.Add($"Duration ({requestedDays} days) exceeds maximum allowed ({SelectedLeaveType.MaxDays} days) for {SelectedLeaveType.TypeName}.");
                
                // Check leave balance
                if (requestedDays > AvailableDays)
                    errors.Add($"Insufficient leave balance. Available: {AvailableDays} days, Requested: {requestedDays} days.");
                
                // Maternity Leave specific validation (minimum 60 days)
                if (SelectedLeaveType.TypeName.ToLower().Contains("maternity") && requestedDays < 60)
                    errors.Add("Maternity leave must be at least 60 days.");
                
                // Annual Leave specific validation (must be filed at least 3 days before start date)
                if (SelectedLeaveType.TypeName.ToLower().Contains("annual") || SelectedLeaveType.TypeName.ToLower().Contains("vacation"))
                {
                    var daysInAdvance = (StartDate - DateTime.Today).Days;
                    if (daysInAdvance < 3)
                        errors.Add("Annual leave must be filed at least 3 days before the start date.");
                }
            }

            HasValidationErrors = errors.Count > 0;
            ValidationMessage = string.Join(" ", errors);
        }

        public void CreateRequest()
        {
            try
            {
                ValidateForm();
                if (HasValidationErrors)
                    return;

                if (SelectedEmployee == null || SelectedLeaveType == null)
                    return;

                // Calculate requested days
                var requestedDays = (EndDate - StartDate).Days + 1;
                
                // Check leave balance
                var availableDays = _dataService.GetEmployeeLeaveBalance(SelectedEmployee.EmployeeID, SelectedLeaveType.LeaveID);
                if (requestedDays > availableDays)
                {
                    MessageBox.Show($"Insufficient leave balance. Available: {availableDays} days, Requested: {requestedDays} days.", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check for duplicate entries (same employee, leave type, dates, and request date)
                var existingRequests = _dataService.GetLeaveRequests();
                var duplicateRequest = existingRequests.Any(r => 
                    r.EmployeeID == SelectedEmployee.EmployeeID && 
                    r.LeaveID == SelectedLeaveType.LeaveID &&
                    r.StartDate.Date == StartDate.Date &&
                    r.EndDate.Date == EndDate.Date &&
                    r.RequestDate.Date == DateTime.Today);

                if (duplicateRequest)
                {
                    MessageBox.Show("A duplicate leave request already exists for this employee with the same leave type and dates.", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check for overlapping leave requests (same employee, any status)
                var overlappingRequests = existingRequests.Any(r => 
                    r.EmployeeID == SelectedEmployee.EmployeeID && 
                    r.Status != "Rejected" && // Allow overlapping with rejected requests
                    ((StartDate >= r.StartDate && StartDate <= r.EndDate) ||
                     (EndDate >= r.StartDate && EndDate <= r.EndDate) ||
                     (StartDate <= r.StartDate && EndDate >= r.EndDate)));

                if (overlappingRequests)
                {
                    MessageBox.Show("This employee already has a leave request (pending or approved) for the selected period.", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check for conflicting leave types on the same day
                var conflictingRequests = existingRequests.Any(r => 
                    r.EmployeeID == SelectedEmployee.EmployeeID && 
                    r.Status != "Rejected" &&
                    r.LeaveID != SelectedLeaveType.LeaveID && // Different leave type
                    ((StartDate >= r.StartDate && StartDate <= r.EndDate) ||
                     (EndDate >= r.StartDate && EndDate <= r.EndDate) ||
                     (StartDate <= r.StartDate && EndDate >= r.EndDate)));

                if (conflictingRequests)
                {
                    MessageBox.Show("This employee has a conflicting leave type request for the selected period.", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Ensure request date is not after start date (business rule: request_date ≤ start_date)
                var requestDate = DateTime.Now.Date;
                if (requestDate > StartDate.Date)
                {
                    requestDate = StartDate.Date;
                }

                var leaveRequest = new LeaveRequest_Model
                {
                    EmployeeID = SelectedEmployee.EmployeeID,
                    LeaveID = SelectedLeaveType.LeaveID,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    RequestDate = requestDate,
                    Status = "Pending",
                    ApprovedBy = string.Empty,
                    EmployeeName = $"{SelectedEmployee.FirstName} {SelectedEmployee.LastName}",
                    TypeName = SelectedLeaveType.TypeName,
                    Reason = Reason
                };

                _leaveRequestRepository.Add(leaveRequest);
                
                MessageBox.Show("Leave request created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                RequestCreated?.Invoke(this, EventArgs.Empty);
                DialogClosed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating leave request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Cancel()
        {
            DialogClosed?.Invoke(this, EventArgs.Empty);
        }

        // Commands
        public System.Windows.Input.ICommand CreateRequest_Command => new View_Model_Command(_ => CreateRequest());
        public System.Windows.Input.ICommand Cancel_Command => new View_Model_Command(_ => Cancel());
    }
}
