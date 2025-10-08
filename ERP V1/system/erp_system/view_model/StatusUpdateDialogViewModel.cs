using System;
using System.Windows.Input;
using erp_system.model;

namespace erp_system.view_model
{
    public class StatusUpdateDialogViewModel : View_Model_Base
    {
        private Employee_Model _employee;
        private string _currentStatus;
        private bool _isActiveSelected;
        private bool _isInactiveSelected;
        private bool _isTerminatedSelected;

        public Employee_Model Employee
        {
            get => _employee;
            set
            {
                _employee = value;
                OnPropertyChanged(nameof(Employee));
                OnPropertyChanged(nameof(EmployeeName));
                OnPropertyChanged(nameof(CurrentStatus));
                SetCurrentStatusSelection();
            }
        }

        public string EmployeeName => _employee?.FullName ?? string.Empty;
        public string CurrentStatus => _employee?.Status ?? string.Empty;

        public bool IsActiveSelected
        {
            get => _isActiveSelected;
            set => SetProperty(ref _isActiveSelected, value);
        }

        public bool IsInactiveSelected
        {
            get => _isInactiveSelected;
            set => SetProperty(ref _isInactiveSelected, value);
        }


        public bool IsTerminatedSelected
        {
            get => _isTerminatedSelected;
            set => SetProperty(ref _isTerminatedSelected, value);
        }

        public string SelectedStatus
        {
            get
            {
                if (IsActiveSelected) return "Active";
                if (IsInactiveSelected) return "Inactive";
                if (IsTerminatedSelected) return "Terminated";
                return string.Empty;
            }
        }

        public ICommand UpdateStatusCommand { get; }
        public ICommand CancelCommand { get; }

        public StatusUpdateDialogViewModel()
        {
            UpdateStatusCommand = new View_Model_Command(ExecuteUpdateStatus);
            CancelCommand = new View_Model_Command(ExecuteCancel);
        }

        private void ExecuteUpdateStatus(object parameter)
        {
            // Close dialog with true result
            if (parameter is System.Windows.Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void ExecuteCancel(object parameter)
        {
            // Close dialog with false result
            if (parameter is System.Windows.Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        private void SetCurrentStatusSelection()
        {
            // Reset all selections
            IsActiveSelected = false;
            IsInactiveSelected = false;
            IsTerminatedSelected = false;

            // Set the current status as selected
            switch (_employee?.Status)
            {
                case "Active":
                    IsActiveSelected = true;
                    break;
                case "Inactive":
                    IsInactiveSelected = true;
                    break;
                case "Terminated":
                    IsTerminatedSelected = true;
                    break;
                case "On Leave":
                    // If employee is on leave, show as Active (since On Leave is managed by leave system)
                    IsActiveSelected = true;
                    break;
            }
        }
    }
}
