using System;
using System.Collections.ObjectModel;
using System.Windows;
using erp_system.model;
using erp_system.repositories;

namespace erp_system.view_model
{
    public class LeaveType_Management_View_Model : View_Model_Base
    {
        private readonly Leave_Repository _leaveRepository;

        public ObservableCollection<Leave_Model> LeaveTypes { get; } = new ObservableCollection<Leave_Model>();

        // Properties for new/edit leave type
        private string _leaveTypeName = string.Empty;
        public string LeaveTypeName
        {
            get => _leaveTypeName;
            set => SetProperty(ref _leaveTypeName, value);
        }

        private int _maxDays;
        public int MaxDays
        {
            get => _maxDays;
            set => SetProperty(ref _maxDays, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        private Leave_Model? _selectedLeaveType;
        public Leave_Model? SelectedLeaveType
        {
            get => _selectedLeaveType;
            set => SetProperty(ref _selectedLeaveType, value);
        }

        public LeaveType_Management_View_Model()
        {
            _leaveRepository = new Leave_Repository();
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                LeaveTypes.Clear();
                var types = _leaveRepository.GetAll();
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

        public void AddLeaveType()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LeaveTypeName))
                {
                    MessageBox.Show("Please enter a leave type name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MaxDays <= 0)
                {
                    MessageBox.Show("Max days must be greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Show password verification dialog
                var dataService = new Services.DataService();
                var currentUser = dataService.GetCurrentUser();
                var dialog = new view.PasswordVerificationDialog(
                    $"Add new leave type: {LeaveTypeName.Trim()}", 
                    currentUser);
                
                // Don't set Owner to avoid the "Window that has not been shown previously" error
                var result = dialog.ShowDialog();
                
                if (result == true && dialog.IsVerified)
                {
                    var leaveType = new Leave_Model
                    {
                        TypeName = LeaveTypeName.Trim(),
                        MaxDays = MaxDays,
                        Description = Description.Trim()
                    };

                    _leaveRepository.Add(leaveType);
                    LoadData();
                    ClearForm();
                    MessageBox.Show("Leave type added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding leave type: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void EditLeaveType(Leave_Model leaveType)
        {
            try
            {
                SelectedLeaveType = leaveType;
                LeaveTypeName = leaveType.TypeName;
                MaxDays = leaveType.MaxDays;
                Description = leaveType.Description;
                IsEditing = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing leave type: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateLeaveType()
        {
            try
            {
                if (SelectedLeaveType == null)
                {
                    MessageBox.Show("No leave type selected for editing.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(LeaveTypeName))
                {
                    MessageBox.Show("Please enter a leave type name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MaxDays <= 0)
                {
                    MessageBox.Show("Max days must be greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Show password verification dialog
                var dataService = new Services.DataService();
                var currentUser = dataService.GetCurrentUser();
                var dialog = new view.PasswordVerificationDialog(
                    $"Update leave type: {LeaveTypeName.Trim()}", 
                    currentUser);
                
                // Don't set Owner to avoid the "Window that has not been shown previously" error
                var result = dialog.ShowDialog();
                
                if (result == true && dialog.IsVerified)
                {
                    SelectedLeaveType.TypeName = LeaveTypeName.Trim();
                    SelectedLeaveType.MaxDays = MaxDays;
                    SelectedLeaveType.Description = Description.Trim();

                    _leaveRepository.Update(SelectedLeaveType);
                    LoadData();
                    ClearForm();
                    MessageBox.Show("Leave type updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating leave type: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteLeaveType(Leave_Model leaveType)
        {
            try
            {
                // Show password verification dialog
                var dataService = new Services.DataService();
                var currentUser = dataService.GetCurrentUser();
                var dialog = new view.PasswordVerificationDialog(
                    $"Delete leave type: {leaveType.TypeName}", 
                    currentUser);
                
                // Don't set Owner to avoid the "Window that has not been shown previously" error
                var result = dialog.ShowDialog();
                
                if (result == true && dialog.IsVerified)
                {
                    _leaveRepository.Delete(leaveType.LeaveID);
                    LoadData();
                    MessageBox.Show("Leave type deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting leave type: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CancelEdit()
        {
            ClearForm();
        }

        private void ClearForm()
        {
            LeaveTypeName = string.Empty;
            MaxDays = 0;
            Description = string.Empty;
            IsEditing = false;
            SelectedLeaveType = null;
        }

        // Commands
        public System.Windows.Input.ICommand AddLeaveType_Command => new View_Model_Command(_ => AddLeaveType());
        public System.Windows.Input.ICommand EditLeaveType_Command => new View_Model_Command(parameter => 
        {
            if (parameter is Leave_Model leaveType)
                EditLeaveType(leaveType);
        });
        public System.Windows.Input.ICommand UpdateLeaveType_Command => new View_Model_Command(_ => UpdateLeaveType());
        public System.Windows.Input.ICommand DeleteLeaveType_Command => new View_Model_Command(parameter => 
        {
            if (parameter is Leave_Model leaveType)
                DeleteLeaveType(leaveType);
        });
        public System.Windows.Input.ICommand CancelEdit_Command => new View_Model_Command(_ => CancelEdit());
    }
}
