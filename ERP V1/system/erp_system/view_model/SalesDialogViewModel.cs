using System;
using System.Collections.ObjectModel;
using System.Windows;
using erp_system.model;
using erp_system.repositories;
using erp_system.Services;

namespace erp_system.view_model
{
    public class SalesDialogViewModel : View_Model_Base
    {
        private readonly Sales_Repository _salesRepository;
        private readonly DataService _dataService;
        private Sales_Model _originalSale;
        private bool _isEditMode;

        public ObservableCollection<Employee_Model> Employees { get; } = new ObservableCollection<Employee_Model>();

        // Events
        public event EventHandler<bool>? DialogClosed;

        // Properties
        private DateTime _saleDate = DateTime.Today;
        private string _amount = string.Empty;
        private Employee_Model? _selectedEmployee;
        private string _description = string.Empty;

        public DateTime SaleDate
        {
            get => _saleDate;
            set
            {
                _saleDate = value;
                OnPropertyChanged(nameof(SaleDate));
            }
        }

        public string Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }

        public Employee_Model? SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string DialogTitle => _isEditMode ? "Edit Sale" : "Add New Sale";
        public string SaveButtonText => _isEditMode ? "Update" : "Save";

        public SalesDialogViewModel(Sales_Model? sale = null)
        {
            _salesRepository = new Sales_Repository();
            _dataService = new DataService();
            _originalSale = sale;
            _isEditMode = sale != null;

            LoadEmployees();
            
            if (_isEditMode)
            {
                LoadSaleData();
            }
        }

        private void LoadEmployees()
        {
            Employees.Clear();
            foreach (var employee in _dataService.GetEmployeesByDepartment("Sales"))
            {
                Employees.Add(employee);
            }
        }

        private void LoadSaleData()
        {
            if (_originalSale == null) return;

            SaleDate = _originalSale.SaleDate;
            Amount = _originalSale.Amount.ToString("F2");
            Description = _originalSale.Description;

            // Find and select the employee
            foreach (var employee in Employees)
            {
                if (employee.EmployeeID == _originalSale.EmployeeID)
                {
                    SelectedEmployee = employee;
                    break;
                }
            }
        }

        public System.Windows.Input.ICommand Save_Command => new View_Model_Command(_ => SaveSale());
        public System.Windows.Input.ICommand Cancel_Command => new View_Model_Command(_ => Cancel());

        private void SaveSale()
        {
            if (!ValidateInput())
                return;

            try
            {
                var sale = new Sales_Model
                {
                    SaleDate = SaleDate,
                    Amount = decimal.Parse(Amount),
                    Description = Description,
                    EmployeeID = SelectedEmployee!.EmployeeID
                };

                if (_isEditMode)
                {
                    sale.SaleID = _originalSale!.SaleID;
                    _salesRepository.Update(sale);
                    MessageBox.Show("Sale updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _salesRepository.Add(sale);
                    MessageBox.Show("Sale added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogClosed?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving sale: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (SelectedEmployee == null)
            {
                MessageBox.Show("Please select an employee.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Amount))
            {
                MessageBox.Show("Please enter an amount.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(Amount, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid amount greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Cancel()
        {
            DialogClosed?.Invoke(this, false);
        }
    }
}
