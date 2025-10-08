using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using erp_system.Services;
using erp_system.model;

namespace erp_system.view_model
{
    public class AddPerformanceReviewViewModel : View_Model_Base
    {
        private readonly DataService _dataService;

        public string DialogTitle => "Add Performance Review";

        public DateTime ReviewDate { get; set; } = DateTime.Today;
        
        private decimal _score = 3.0m;
        public decimal Score 
        { 
            get => _score; 
            set 
            { 
                _score = value; 
                OnPropertyChanged(nameof(Score)); 
            } 
        }
        
        public string Remarks { get; set; } = string.Empty;
        public ObservableCollection<EmployeeForReview> Employees { get; } = new ObservableCollection<EmployeeForReview>();
        public EmployeeForReview? SelectedEmployee { get; set; }

        public AddPerformanceReviewViewModel()
        {
            _dataService = new DataService();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _dataService.GetEmployees();
                Employees.Clear();
                
                foreach (var emp in employees)
                {
                    Employees.Add(new EmployeeForReview
                    {
                        EmployeeID = emp.EmployeeID,
                        DisplayName = $"{emp.FirstName} {emp.LastName} - {emp.Position}",
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        Position = emp.Position
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool SavePerformanceReview()
        {
            try
            {
                // Validation
                if (SelectedEmployee == null)
                {
                    MessageBox.Show("Please select an employee.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (Score < 1.0m || Score > 5.0m)
                {
                    MessageBox.Show("Performance score must be between 1.0 and 5.0.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Create performance record
                var performanceRecord = new Performance_Model
                {
                    EmployeeID = SelectedEmployee.EmployeeID,
                    ReviewDate = ReviewDate,
                    Score = Score,
                    Remarks = Remarks ?? string.Empty,
                    ReviewType = "Annual", // Default to Annual
                    EmployeeName = $"{SelectedEmployee.FirstName} {SelectedEmployee.LastName}",
                    Position = SelectedEmployee.Position
                };

                // Save to database
                bool success = _dataService.AddPerformanceRecord(performanceRecord);
                
                if (success)
                {
                    MessageBox.Show("Performance review added successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("Failed to save performance review. Please try again.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving performance review: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }

    public class EmployeeForReview
    {
        public int EmployeeID { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}
