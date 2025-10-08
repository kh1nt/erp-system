using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using erp_system.Services;
using erp_system.model;
using erp_system.repositories;

namespace erp_system.view_model
{
    public class Payroll_View_Model : View_Model_Base
    {
        public ObservableCollection<Payroll_Model> PayrollRecords { get; } = new ObservableCollection<Payroll_Model>();
        public ObservableCollection<Payroll_Model> AllPayrollRecords { get; } = new ObservableCollection<Payroll_Model>();
        public ObservableCollection<MonthFilterItem> AvailableMonths { get; } = new ObservableCollection<MonthFilterItem>();
        public ObservableCollection<YearFilterItem> AvailableYears { get; } = new ObservableCollection<YearFilterItem>();

        private decimal _totalPayroll = 0;
        private decimal _totalCommissions = 0;
        private MonthFilterItem? _selectedMonth;
        private YearFilterItem? _selectedYear;
        
        private readonly DataService _dataService;


        public decimal TotalPayroll
        {
            get => _totalPayroll;
            set
            {
                _totalPayroll = value;
                OnPropertyChanged(nameof(TotalPayroll));
            }
        }

        public decimal TotalCommissions
        {
            get => _totalCommissions;
            set
            {
                _totalCommissions = value;
                OnPropertyChanged(nameof(TotalCommissions));
            }
        }

        public MonthFilterItem? SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                _selectedMonth = value;
                OnPropertyChanged(nameof(SelectedMonth));
                ApplyFilter();
            }
        }

        public YearFilterItem? SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
                ApplyFilter();
            }
        }



        public Payroll_View_Model()
        {
            _dataService = new DataService();
            try
        {
            LoadData();
            }
            catch (Exception)
            {
                // Error loading payroll data
            }
        }

        public void LoadData()
        {
            try
            {
                PayrollRecords.Clear();
                AllPayrollRecords.Clear();
                AvailableMonths.Clear();

                var data = new DataService();

                // Load payroll records from database
                var payrollData = data.GetPayrollRecords();
                foreach (var record in payrollData)
                {
                    AllPayrollRecords.Add(record);
                }

                // Create filter options
                CreateFilterOptions();
                
                // Apply current filter
                ApplyFilter();
                
                CalculateTotals();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading data: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }


        private void CalculateTotals()
        {
            TotalPayroll = PayrollRecords.Sum(p => p.NetPay);
            TotalCommissions = PayrollRecords.Sum(p => p.Bonuses);
        }

        public System.Windows.Input.ICommand Refresh_Command => new View_Model_Command(_ => LoadData());


        public System.Windows.Input.ICommand GeneratePayroll_Command => new View_Model_Command(_ =>
        {
            try
            {
                GeneratePayrollForPeriod();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error generating payroll: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        });


        private void GeneratePayrollForPeriod()
        {
            var periodStart = DateTime.Today.AddDays(-15);
            var periodEnd = DateTime.Today;
            
            var result = System.Windows.MessageBox.Show(
                $"Generate payroll for all active employees?\n\n" +
                $"Payroll Period:\n" +
                $"Start Date: {periodStart:MMMM dd, yyyy}\n" +
                $"End Date: {periodEnd:MMMM dd, yyyy}\n\n" +
                $"This will create payroll records for the above period and save to database.", 
                "Generate Payroll", 
                System.Windows.MessageBoxButton.YesNo, 
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    // Check if payroll already exists for this period
                    if (_dataService.PayrollExistsForPeriod(periodStart, periodEnd))
                    {
                        var existingPayroll = _dataService.GetExistingPayrollForPeriod(periodStart, periodEnd);
                        var existingPeriod = existingPayroll != null 
                            ? $"{existingPayroll.PeriodStart:MMM dd, yyyy} - {existingPayroll.PeriodEnd:MMM dd, yyyy}"
                            : $"{periodStart:MMM dd, yyyy} - {periodEnd:MMM dd, yyyy}";
                        var generatedDate = existingPayroll?.GeneratedDate.ToString("MMM dd, yyyy 'at' h:mm tt") ?? "Unknown";

                        System.Windows.MessageBox.Show($"Payroll for {periodStart:MMMM yyyy} has already been generated!\n\n" +
                            $"Existing Period: {existingPeriod}\n" +
                            $"Generated: {generatedDate}\n\n" +
                            $"Please select a different month or delete existing payroll records first.", 
                            "Payroll Already Exists", 
                            System.Windows.MessageBoxButton.OK, 
                            System.Windows.MessageBoxImage.Warning);
                        return;
                    }

                    var payrollRepo = new Payroll_Repository();
                    var empRepo = new Employee_Repository();
                    int successCount = 0;
                    int errorCount = 0;

                    // Get all active and on leave employees
                    var employees = empRepo.GetAll().Where(e => e.Status == "Active" || e.Status == "On Leave").ToList();

                    if (employees.Count == 0)
                    {
                        System.Windows.MessageBox.Show("No active or on leave employees found! Please add employees first in Employee Management.", "No Employees", 
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }

                    // Clear existing records first
                    PayrollRecords.Clear();

                    foreach (var employee in employees)
                    {
                        try
                        {
                            // Check if employee has valid ID and salary
                            if (employee.EmployeeID <= 0 || employee.BasicSalary <= 0)
                            {
                                errorCount++;
                                continue;
                            }

                            var basicSalary = CalculateProratedSalary(employee, periodStart, periodEnd);
                            var bonuses = CalculateBonuses(employee);
                            var deductions = CalculateDeductions(basicSalary);
                            var netPay = basicSalary + bonuses - deductions;

                            var payroll = new Payroll_Model
                            {
                                PeriodStart = periodStart,
                                PeriodEnd = periodEnd,
                                BasicSalary = basicSalary, // This will be the prorated salary
                                Bonuses = bonuses,
                                Deductions = deductions,
                                NetPay = netPay,
                                GeneratedDate = DateTime.Now,
                                EmployeeID = employee.EmployeeID,
                                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                                Position = employee.Position
                            };

                            var payrollId = payrollRepo.Add(payroll);
                            payroll.PayrollID = payrollId; // Set the actual ID from database
                            
                            PayrollRecords.Add(payroll);
                            successCount++;
                        }
                        catch (Exception)
                        {
                            errorCount++;
                        }
                    }

                    // Reload data to include new records
                    LoadData();

                    if (successCount > 0)
                    {
                        System.Windows.MessageBox.Show($"Payroll generation completed!\n\n" +
                            $"Successfully processed: {successCount} employees\n" +
                            $"Errors: {errorCount} employees\n\n" +
                            $"Total Payroll: {TotalPayroll:C}\n" +
                            $"Total Bonuses: {TotalCommissions:C}", 
                            "Payroll Generated", 
                            System.Windows.MessageBoxButton.OK, 
                            System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"Failed to generate payroll for any employees. Please check employee data and try again.", "Generation Failed", 
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error during payroll generation: {ex.Message}", "Error", 
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }


        private decimal CalculateProratedSalary(Employee_Model employee, DateTime periodStart, DateTime periodEnd)
        {
            // For active employees, return full salary
            if (employee.Status == "Active")
            {
                return employee.BasicSalary;
            }

            // For employees on leave, calculate prorated salary
            if (employee.Status == "On Leave")
            {
                var dataService = new DataService();
                var totalDaysInPeriod = (periodEnd - periodStart).Days + 1;
                var unpaidLeaveDays = dataService.GetUnpaidLeaveDays(employee.EmployeeID, periodStart, periodEnd);
                var paidLeaveDays = dataService.GetPaidLeaveDays(employee.EmployeeID, periodStart, periodEnd);
                
                // Calculate working days (total days minus unpaid leave days)
                var workingDays = totalDaysInPeriod - unpaidLeaveDays;
                
                // Ensure working days is not negative
                workingDays = Math.Max(0, workingDays);
                
                // Calculate prorated salary based on working days
                var dailyRate = employee.BasicSalary / totalDaysInPeriod;
                var proratedSalary = workingDays * dailyRate;
                
                return Math.Round(proratedSalary, 2);
            }

            // For other statuses, return 0
            return 0;
        }

        private decimal CalculateBonuses(Employee_Model employee)
        {
            // Performance-based bonus calculation
            var random = new Random();
            
            // Base bonus based on position level
            decimal baseBonus = employee.Position.ToLower() switch
            {
                var pos when pos.Contains("manager") || pos.Contains("director") => 5000,
                var pos when pos.Contains("supervisor") || pos.Contains("lead") => 3000,
                var pos when pos.Contains("senior") => 2000,
                var pos when pos.Contains("junior") || pos.Contains("associate") => 1000,
                _ => 500
            };

            // Add performance variation (±30% of base bonus)
            var variation = (decimal)(random.NextDouble() - 0.5) * 0.6m; // -30% to +30%
            var performanceMultiplier = 1 + variation;
            
            return Math.Round(baseBonus * performanceMultiplier, 2);
        }

        private decimal CalculateDeductions(decimal basicSalary)
        {
            // Philippine tax and contribution calculations
            decimal sss = Math.Min(basicSalary * 0.045m, 1125); // SSS max 1,125
            decimal philhealth = Math.Min(basicSalary * 0.0275m, 1750); // PhilHealth max 1,750
            decimal pagibig = Math.Min(basicSalary * 0.02m, 100); // Pag-IBIG max 100
            decimal incomeTax = 0;

            // Income tax calculation (simplified)
            if (basicSalary > 25000)
            {
                incomeTax = (basicSalary - 25000) * 0.15m; // 15% on excess over 25,000
            }

            return sss + philhealth + pagibig + incomeTax;
        }

        private void CreateFilterOptions()
        {
            // Clear existing options
            AvailableMonths.Clear();
            AvailableYears.Clear();

            // Add "All" options
            AvailableMonths.Add(new MonthFilterItem { DisplayText = "All Months", Value = null });
            AvailableYears.Add(new YearFilterItem { DisplayText = "All Years", Value = null });

            // Add all 12 months
            for (int month = 1; month <= 12; month++)
            {
                var monthName = new DateTime(2024, month, 1).ToString("MMMM");
                AvailableMonths.Add(new MonthFilterItem 
                { 
                    DisplayText = monthName, 
                    Value = month 
                });
            }

            // Get unique years from payroll records
            var uniqueYears = AllPayrollRecords
                .Select(p => p.PeriodStart.Year)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();

            foreach (var year in uniqueYears)
            {
                AvailableYears.Add(new YearFilterItem 
                { 
                    DisplayText = year.ToString(), 
                    Value = year 
                });
            }

            // Set default selections
            if (AvailableMonths.Count > 0)
            {
                SelectedMonth = AvailableMonths[0];
            }
            if (AvailableYears.Count > 0)
            {
                SelectedYear = AvailableYears[0];
            }
        }

        private void ApplyFilter()
        {
            PayrollRecords.Clear();

            var filteredRecords = AllPayrollRecords.AsEnumerable();

            // Apply month filter
            if (SelectedMonth?.Value != null)
            {
                var selectedMonth = SelectedMonth.Value.Value;
                
                // Debug: Show what we're filtering for
                System.Diagnostics.Debug.WriteLine($"Filtering for month: {selectedMonth}");
                
                // Show all available months in data
                var availableMonths = AllPayrollRecords.Select(p => p.PeriodStart.Month).Distinct().OrderBy(m => m);
                System.Diagnostics.Debug.WriteLine($"Available months in data: {string.Join(", ", availableMonths)}");
                
                filteredRecords = filteredRecords.Where(p => p.PeriodStart.Month == selectedMonth);
            }

            // Apply year filter
            if (SelectedYear?.Value != null)
            {
                var selectedYear = SelectedYear.Value.Value;
                filteredRecords = filteredRecords.Where(p => p.PeriodStart.Year == selectedYear);
            }

            // Add filtered records to display collection
            foreach (var record in filteredRecords)
            {
                PayrollRecords.Add(record);
            }

            CalculateTotals();
        }
    }

    public class MonthFilterItem
    {
        public string DisplayText { get; set; } = string.Empty;
        public int? Value { get; set; }
    }

    public class YearFilterItem
    {
        public string DisplayText { get; set; } = string.Empty;
        public int? Value { get; set; }
    }
}