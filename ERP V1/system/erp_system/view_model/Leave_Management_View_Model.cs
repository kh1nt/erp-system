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
    public class Leave_Management_View_Model : View_Model_Base
    {
        private readonly LeaveRequest_Repository _leaveRequestRepository;
        private readonly Leave_Repository _leaveRepository;
        private readonly Employee_Repository _employeeRepository;
        private readonly DataService _dataService;

        public ObservableCollection<LeaveRequest_Model> LeaveRequests { get; } = new ObservableCollection<LeaveRequest_Model>();
        public ObservableCollection<Leave_Model> LeaveTypes { get; } = new ObservableCollection<Leave_Model>();
        public ObservableCollection<Employee_Model> Employees { get; } = new ObservableCollection<Employee_Model>();

        // Properties for summary cards
        private int _pendingApprovals;
        public int PendingApprovals
        {
            get => _pendingApprovals;
            set => SetProperty(ref _pendingApprovals, value);
        }

        private int _thisMonthLeaves;
        public int ThisMonthLeaves
        {
            get => _thisMonthLeaves;
            set => SetProperty(ref _thisMonthLeaves, value);
        }

        private int _availableDays;
        public int AvailableDays
        {
            get => _availableDays;
            set => SetProperty(ref _availableDays, value);
        }

        // Properties for filtering
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterLeaveRequests();
            }
        }

        private string _selectedStatus = "All";
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                SetProperty(ref _selectedStatus, value);
                FilterLeaveRequests();
            }
        }

        // Filtered collection
        private ObservableCollection<LeaveRequest_Model> _filteredLeaveRequests = new ObservableCollection<LeaveRequest_Model>();
        public ObservableCollection<LeaveRequest_Model> FilteredLeaveRequests
        {
            get => _filteredLeaveRequests;
            set => SetProperty(ref _filteredLeaveRequests, value);
        }

        // Properties for new leave request
        private Employee_Model? _selectedEmployee;
        public Employee_Model? SelectedEmployee
        {
            get => _selectedEmployee;
            set 
            { 
                SetProperty(ref _selectedEmployee, value);
                UpdateAvailableDays();
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
            }
        }

        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private string _reason = string.Empty;
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        public Leave_Management_View_Model()
        {
            _leaveRequestRepository = new LeaveRequest_Repository();
            _leaveRepository = new Leave_Repository();
            _employeeRepository = new Employee_Repository();
            _dataService = new DataService();
            
            LoadData();
            LoadLeaveTypes();
            LoadEmployees();
        }

        public void LoadData()
        {
            try
            {
                LeaveRequests.Clear();
                var requests = _dataService.GetLeaveRequests();
                foreach (var request in requests)
                {
                    LeaveRequests.Add(request);
                }
                
                FilteredLeaveRequests.Clear();
                foreach (var request in LeaveRequests)
                {
                    FilteredLeaveRequests.Add(request);
                }

                UpdateSummaryCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading leave requests: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSummaryCards()
        {
            try
            {
                var stats = _dataService.GetLeaveStatistics();
                PendingApprovals = stats.ContainsKey("Pending") ? stats["Pending"] : 0;
                ThisMonthLeaves = stats.ContainsKey("ThisMonth") ? stats["ThisMonth"] : 0;
                
                UpdateAvailableDays();
            }
            catch (Exception ex)
            {
                // Fallback to simple calculation if statistics fail
                PendingApprovals = LeaveRequests.Count(r => r.Status == "Pending");
                ThisMonthLeaves = LeaveRequests.Count(r => r.StartDate.Month == DateTime.Now.Month && r.StartDate.Year == DateTime.Now.Year);
                AvailableDays = 15;
            }
        }

        private void UpdateAvailableDays()
        {
            try
            {
                if (SelectedEmployee != null && SelectedLeaveType != null)
                {
                    // Calculate available days for current calendar year only
                    AvailableDays = _dataService.GetEmployeeLeaveBalance(SelectedEmployee.EmployeeID, SelectedLeaveType.LeaveID);
                }
                else
                {
                    AvailableDays = 15; // Default value
                }
            }
            catch (Exception ex)
            {
                AvailableDays = 15; // Default value on error
            }
        }

        private void FilterLeaveRequests()
        {
            FilteredLeaveRequests.Clear();
            
            var filtered = LeaveRequests.AsEnumerable();

            // Filter by status
            if (SelectedStatus != "All")
            {
                filtered = filtered.Where(r => r.Status == SelectedStatus);
            }

            // Filter by search text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(r => 
                    r.EmployeeName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    r.TypeName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    r.Status.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var request in filtered)
            {
                FilteredLeaveRequests.Add(request);
            }
        }

        public void ApproveLeaveRequest(LeaveRequest_Model request)
        {
            try
            {
                // Show password verification dialog
                var currentUser = _dataService.GetCurrentUser();
                var dialog = new view.PasswordVerificationDialog(
                    $"Approve leave request for {request.EmployeeName} ({request.TypeName})", 
                    currentUser);
                
                // Don't set Owner to avoid the "Window that has not been shown previously" error
                var result = dialog.ShowDialog();
                
                if (result == true && dialog.IsVerified)
                {
                    request.Status = "Approved";
                    request.ApprovedBy = currentUser;
                    _leaveRequestRepository.Update(request);
                    LoadData();
                    MessageBox.Show("Leave request approved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error approving leave request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RejectLeaveRequest(LeaveRequest_Model request)
        {
            try
            {
                // Show password verification dialog
                var currentUser = _dataService.GetCurrentUser();
                var dialog = new view.PasswordVerificationDialog(
                    $"Reject leave request for {request.EmployeeName} ({request.TypeName})", 
                    currentUser);
                
                // Don't set Owner to avoid the "Window that has not been shown previously" error
                var result = dialog.ShowDialog();
                
                if (result == true && dialog.IsVerified)
                {
                    request.Status = "Rejected";
                    request.ApprovedBy = currentUser;
                    _leaveRequestRepository.Update(request);
                    LoadData();
                    MessageBox.Show("Leave request rejected.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error rejecting leave request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CreateLeaveRequest()
        {
            try
            {
                if (SelectedEmployee == null)
                {
                    MessageBox.Show("Please select an employee.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SelectedLeaveType == null)
                {
                    MessageBox.Show("Please select a leave type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (StartDate > EndDate)
                {
                    MessageBox.Show("Start date cannot be after end date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (StartDate < DateTime.Today)
                {
                    MessageBox.Show("Start date cannot be in the past.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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

                // Check for overlapping leave requests
                var overlappingRequests = LeaveRequests.Any(r => 
                    r.EmployeeID == SelectedEmployee.EmployeeID && 
                    r.Status == "Approved" &&
                    ((StartDate >= r.StartDate && StartDate <= r.EndDate) ||
                     (EndDate >= r.StartDate && EndDate <= r.EndDate) ||
                     (StartDate <= r.StartDate && EndDate >= r.EndDate)));

                if (overlappingRequests)
                {
                    MessageBox.Show("This employee already has approved leave for the selected period.", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var leaveRequest = new LeaveRequest_Model
                {
                    EmployeeID = SelectedEmployee.EmployeeID,
                    LeaveID = SelectedLeaveType.LeaveID,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    RequestDate = DateTime.Now,
                    Status = "Pending",
                    ApprovedBy = string.Empty,
                    EmployeeName = $"{SelectedEmployee.FirstName} {SelectedEmployee.LastName}",
                    TypeName = SelectedLeaveType.TypeName
                };

                _leaveRequestRepository.Add(leaveRequest);
                LoadData();
                
                // Reset form
                SelectedEmployee = null;
                SelectedLeaveType = null;
                StartDate = DateTime.Today;
                EndDate = DateTime.Today;
                Reason = string.Empty;

                MessageBox.Show("Leave request created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating leave request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteLeaveRequest(LeaveRequest_Model request)
        {
            try
            {
                // Show password verification dialog
                var currentUser = _dataService.GetCurrentUser();
                var dialog = new view.PasswordVerificationDialog(
                    $"Delete leave request for {request.EmployeeName} ({request.TypeName})", 
                    currentUser);
                
                // Don't set Owner to avoid the "Window that has not been shown previously" error
                var result = dialog.ShowDialog();
                
                if (result == true && dialog.IsVerified)
                {
                    _leaveRequestRepository.Delete(request.RequestID);
                    LoadData();
                    MessageBox.Show("Leave request deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting leave request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Commands
        public System.Windows.Input.ICommand Refresh_Command => new View_Model_Command(_ => LoadData());
        public System.Windows.Input.ICommand CreateLeaveRequest_Command => new View_Model_Command(_ => CreateLeaveRequest());
        public System.Windows.Input.ICommand ApproveLeaveRequest_Command => new View_Model_Command(parameter => 
        {
            if (parameter is LeaveRequest_Model request)
                ApproveLeaveRequest(request);
        });
        public System.Windows.Input.ICommand RejectLeaveRequest_Command => new View_Model_Command(parameter => 
        {
            if (parameter is LeaveRequest_Model request)
                RejectLeaveRequest(request);
        });
        public System.Windows.Input.ICommand DeleteLeaveRequest_Command => new View_Model_Command(parameter => 
        {
            if (parameter is LeaveRequest_Model request)
                DeleteLeaveRequest(request);
        });
    }
}
