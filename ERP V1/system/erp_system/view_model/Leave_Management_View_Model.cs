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
        public ObservableCollection<LeaveRequest_Model> PagedLeaveRequests { get; } = new ObservableCollection<LeaveRequest_Model>();
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

        private int _totalRequestsThisYear;
        public int TotalRequestsThisYear
        {
            get => _totalRequestsThisYear;
            set => SetProperty(ref _totalRequestsThisYear, value);
        }

        // Pagination properties
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value) return;
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(PageInfo));
                FilterLeaveRequests();
            }
        }

        private int _itemsPerPage = 7;
        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set => SetProperty(ref _itemsPerPage, value);
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            set => SetProperty(ref _totalItems, value);
        }

        // Page info string
        public string PageInfo => $"Page {CurrentPage} of {TotalPages}";

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

        private string _selectedStatus = "All Status";
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                SetProperty(ref _selectedStatus, value);
                FilterLeaveRequests();
            }
        }

        // Status options for the filter
        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string>
        {
            "All Status",
            "Pending",
            "Approved",
            "Rejected"
        };

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
            }
        }

        private Leave_Model? _selectedLeaveType;
        public Leave_Model? SelectedLeaveType
        {
            get => _selectedLeaveType;
            set 
            { 
                SetProperty(ref _selectedLeaveType, value);
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
                
                // Call FilterLeaveRequests to populate both FilteredLeaveRequests and PagedLeaveRequests
                FilterLeaveRequests();
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
                
                UpdateTotalRequestsThisYear();
            }
            catch (Exception)
            {
                // Fallback to simple calculation if statistics fail
                // Error loading statistics
                PendingApprovals = LeaveRequests.Count(r => r.Status == "Pending");
                ThisMonthLeaves = LeaveRequests.Count(r => r.StartDate.Month == DateTime.Now.Month && r.StartDate.Year == DateTime.Now.Year);
                UpdateTotalRequestsThisYear(); // Calculate total requests this year
            }
        }

        private void UpdateTotalRequestsThisYear()
        {
            try
            {
                // Count all leave requests for the current calendar year
                var currentYear = DateTime.Now.Year;
                TotalRequestsThisYear = LeaveRequests.Count(r => r.StartDate.Year == currentYear);
            }
            catch (Exception)
            {
                // Error updating total requests this year
                TotalRequestsThisYear = 0;
            }
        }

        private void FilterLeaveRequests()
        {
            FilteredLeaveRequests.Clear();
            PagedLeaveRequests.Clear();
            
            
            var filtered = LeaveRequests.AsEnumerable();

            // Filter by status
            if (SelectedStatus != "All Status")
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

            // Add all filtered items to FilteredLeaveRequests (for summary calculations)
            foreach (var request in filtered)
            {
                FilteredLeaveRequests.Add(request);
            }

            // Calculate pagination
            TotalItems = FilteredLeaveRequests.Count;
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
            
            
            // Ensure current page is valid
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            // Get items for current page
            var pagedItems = FilteredLeaveRequests
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage);

            foreach (var request in pagedItems)
            {
                PagedLeaveRequests.Add(request);
            }
            

            // Notify property changes
            OnPropertyChanged(nameof(PageInfo));
        }

        public void ApproveLeaveRequest(LeaveRequest_Model request)
        {
            try
            {
                // Show password verification dialog
                var currentUser = _dataService.GetCurrentUser();
                var reasonText = string.IsNullOrWhiteSpace(request.Reason) ? "No reason provided" : request.Reason;
                var dialog = new view.PasswordVerificationDialog(
                    $"Approve leave request for {request.EmployeeName} ({request.TypeName})\nReason: {reasonText}", 
                    currentUser);
                
                // Don't set Owner to avoid the "Window that has not been shown previously" error
                var result = dialog.ShowDialog();
                
                if (result == true && dialog.IsVerified)
                {
                    request.Status = "Approved";
                    request.ApprovedBy = currentUser;
                    _leaveRequestRepository.Update(request);
                    
                    // Update employee status to "On Leave" when leave is approved
                    if (_dataService.UpdateEmployeeStatus(request.EmployeeID, "On Leave"))
                    {
                    }
                    
                    LoadData();
                    MessageBox.Show("Leave request approved successfully. Employee status updated to 'On Leave'.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
                var reasonText = string.IsNullOrWhiteSpace(request.Reason) ? "No reason provided" : request.Reason;
                var dialog = new view.PasswordVerificationDialog(
                    $"Reject leave request for {request.EmployeeName} ({request.TypeName})\nReason: {reasonText}", 
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
                
                // Check if duration exceeds MaxDays for this leave type
                if (requestedDays > SelectedLeaveType.MaxDays)
                {
                    MessageBox.Show($"Duration ({requestedDays} days) exceeds maximum allowed ({SelectedLeaveType.MaxDays} days) for {SelectedLeaveType.TypeName}.", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Check leave balance
                var availableDays = _dataService.GetEmployeeLeaveBalance(SelectedEmployee.EmployeeID, SelectedLeaveType.LeaveID);
                if (requestedDays > availableDays)
                {
                    MessageBox.Show($"Insufficient leave balance. Available: {availableDays} days, Requested: {requestedDays} days.", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Maternity Leave specific validation (minimum 60 days)
                if (SelectedLeaveType.TypeName.ToLower().Contains("maternity") && requestedDays < 60)
                {
                    MessageBox.Show("Maternity leave must be at least 60 days.", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Annual Leave specific validation (must be filed at least 3 days before start date)
                if (SelectedLeaveType.TypeName.ToLower().Contains("annual") || SelectedLeaveType.TypeName.ToLower().Contains("vacation"))
                {
                    var daysInAdvance = (StartDate - DateTime.Today).Days;
                    if (daysInAdvance < 3)
                    {
                        MessageBox.Show("Annual leave must be filed at least 3 days before the start date.", 
                            "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Check for duplicate entries
                var duplicateRequest = LeaveRequests.Any(r => 
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
                var overlappingRequests = LeaveRequests.Any(r => 
                    r.EmployeeID == SelectedEmployee.EmployeeID && 
                    r.Status != "Rejected" &&
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
                var conflictingRequests = LeaveRequests.Any(r => 
                    r.EmployeeID == SelectedEmployee.EmployeeID && 
                    r.Status != "Rejected" &&
                    r.LeaveID != SelectedLeaveType.LeaveID &&
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
                var reasonText = string.IsNullOrWhiteSpace(request.Reason) ? "No reason provided" : request.Reason;
                var dialog = new view.PasswordVerificationDialog(
                    $"Delete leave request for {request.EmployeeName} ({request.TypeName})\nReason: {reasonText}", 
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
        public System.Windows.Input.ICommand OpenCreateRequestDialog_Command => new View_Model_Command(_ => OpenCreateRequestDialog());
        
        public void OpenCreateRequestDialog()
        {
            try
            {
                var dialog = new view.CreateLeaveRequestDialog();
                var viewModel = new CreateLeaveRequestDialogViewModel();
                dialog.DataContext = viewModel;
                
                viewModel.RequestCreated += (sender, e) =>
                {
                    LoadData(); // Refresh the data when a request is created
                };
                
                viewModel.DialogClosed += (sender, e) =>
                {
                    dialog.Close();
                };
                
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                // Error opening create request dialog
                MessageBox.Show($"Error opening create request dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
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

        public System.Windows.Input.ICommand ReturnFromLeave_Command => new View_Model_Command(parameter => 
        {
            if (parameter is LeaveRequest_Model request)
                ReturnEmployeeFromLeave(request);
        });

        public void ReturnEmployeeFromLeave(LeaveRequest_Model request)
        {
            try
            {
                // Show password verification dialog
                var currentUser = _dataService.GetCurrentUser();
                var reasonText = string.IsNullOrWhiteSpace(request.Reason) ? "No reason provided" : request.Reason;
                var dialog = new view.PasswordVerificationDialog(
                    $"Mark {request.EmployeeName} as returned from leave\nOriginal Reason: {reasonText}", 
                    currentUser);
                
                var result = dialog.ShowDialog();
                
                if (result == true && dialog.IsVerified)
                {
                    // Update employee status back to "Active"
                    if (_dataService.UpdateEmployeeStatus(request.EmployeeID, "Active"))
                    {
                        LoadData();
                        MessageBox.Show($"{request.EmployeeName} has been marked as returned from leave. Status updated to 'Active'.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to update employee status.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating employee status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Pagination Commands
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

        public System.Windows.Input.ICommand DeleteLeaveRequest_Command => new View_Model_Command(parameter => 
        {
            if (parameter is LeaveRequest_Model request)
                DeleteLeaveRequest(request);
        });
    }
}
