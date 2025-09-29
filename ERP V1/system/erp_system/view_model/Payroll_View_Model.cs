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
        public ObservableCollection<Employee_Model> Employees { get; } = new ObservableCollection<Employee_Model>();
        public ObservableCollection<Payroll_Model> SelectedEmployeePayrolls { get; } = new ObservableCollection<Payroll_Model>();

        private Employee_Model? _selectedEmployee;
        private decimal _totalPayroll = 0;
        private decimal _totalCommissions = 0;
        private DateTime _payrollPeriodStart = DateTime.Today.AddDays(-15);
        private DateTime _payrollPeriodEnd = DateTime.Today;
        
        private readonly DataService _dataService;

        public Employee_Model? SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
                LoadEmployeePayrolls();
            }
        }

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


        public DateTime PayrollPeriodStart
        {
            get => _payrollPeriodStart;
            set
            {
                _payrollPeriodStart = value;
                OnPropertyChanged(nameof(PayrollPeriodStart));
            }
        }

        public DateTime PayrollPeriodEnd
        {
            get => _payrollPeriodEnd;
            set
            {
                _payrollPeriodEnd = value;
                OnPropertyChanged(nameof(PayrollPeriodEnd));
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
                Employees.Clear();

            var data = new DataService();
                var empRepo = new Employee_Repository();

                // Load payroll records
                var payrollData = data.GetPayrollRecords();
                foreach (var record in payrollData)
                {
                PayrollRecords.Add(record);
                }

                // Load employees
                var empData = empRepo.GetAll();
                foreach (var emp in empData)
                {
                    Employees.Add(emp);
                }

                CalculateTotals();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading data: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void LoadEmployeePayrolls()
        {
            SelectedEmployeePayrolls.Clear();
            if (SelectedEmployee != null)
            {
                var employeePayrolls = PayrollRecords.Where(p => p.EmployeeID == SelectedEmployee.EmployeeID).ToList();
                foreach (var payroll in employeePayrolls)
                    SelectedEmployeePayrolls.Add(payroll);
            }
        }

        private void CalculateTotals()
        {
            TotalPayroll = PayrollRecords.Sum(p => p.NetPay);
            TotalCommissions = PayrollRecords.Sum(p => p.Bonuses);
        }

        public System.Windows.Input.ICommand Refresh_Command => new View_Model_Command(_ => LoadData());

        public System.Windows.Input.ICommand ProcessPayroll_Command => new View_Model_Command(_ =>
        {
            try
            {
                ProcessPayroll();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error processing payroll: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        });

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

        private void ProcessPayroll()
        {
            if (SelectedEmployee == null)
            {
                System.Windows.MessageBox.Show("Please select an employee to process payroll.", "No Selection", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            var result = System.Windows.MessageBox.Show(
                $"Generate payroll for {SelectedEmployee.FirstName} {SelectedEmployee.LastName} and save to database?", 
                "Confirm Payroll Processing", 
                System.Windows.MessageBoxButton.YesNo, 
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    var payrollRepo = new Payroll_Repository();
                    var basicSalary = CalculateBasicSalary(SelectedEmployee);
                    var bonuses = CalculateBonuses(SelectedEmployee);
                    var deductions = CalculateDeductions(basicSalary);
                    
                    // Calculate leave-related payroll impacts
                    var unpaidLeaveDays = _dataService.GetUnpaidLeaveDays(SelectedEmployee.EmployeeID, PayrollPeriodStart, PayrollPeriodEnd);
                    var paidLeaveDays = _dataService.GetPaidLeaveDays(SelectedEmployee.EmployeeID, PayrollPeriodStart, PayrollPeriodEnd);
                    var leaveBalanceBonus = _dataService.GetLeaveBalanceBonus(SelectedEmployee.EmployeeID);
                    
                    var dailyRate = basicSalary / 22; // Assuming 22 working days per month
                    var leaveDeductions = unpaidLeaveDays * dailyRate;
                    var leaveBonuses = leaveBalanceBonus;
                    
                    var netPay = basicSalary + bonuses + leaveBonuses - deductions - leaveDeductions;

                    var payroll = new Payroll_Model
                    {
                        PeriodStart = PayrollPeriodStart,
                        PeriodEnd = PayrollPeriodEnd,
                        BasicSalary = basicSalary,
                        Bonuses = bonuses,
                        Deductions = deductions,
                        NetPay = netPay,
                        GeneratedDate = DateTime.Now,
                        EmployeeID = SelectedEmployee.EmployeeID,
                        EmployeeName = $"{SelectedEmployee.FirstName} {SelectedEmployee.LastName}",
                        Position = SelectedEmployee.Position,
                        // Leave-related fields
                        LeaveDeductions = leaveDeductions,
                        LeaveBonuses = leaveBonuses,
                        UnpaidLeaveDays = unpaidLeaveDays,
                        PaidLeaveDays = paidLeaveDays,
                        LeaveBalanceBonus = leaveBalanceBonus
                    };

                    var payrollId = payrollRepo.Add(payroll);
                    payroll.PayrollID = payrollId; // Set the actual ID from database
                    
                    PayrollRecords.Add(payroll);
                    CalculateTotals();

                    System.Windows.MessageBox.Show($"Payroll saved for {SelectedEmployee.FirstName} {SelectedEmployee.LastName}!\n\n" +
                        $"Payroll ID: {payrollId}\n" +
                        $"Basic Salary: {basicSalary:C}\n" +
                        $"Bonuses: {bonuses:C}\n" +
                        $"Net Pay: {netPay:C}", 
                        "Payroll Saved", 
                        System.Windows.MessageBoxButton.OK, 
                        System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error processing payroll: {ex.Message}", "Error", 
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void GeneratePayrollForPeriod()
        {
            var result = System.Windows.MessageBox.Show(
                $"Generate payroll for all employees from {PayrollPeriodStart:yyyy-MM-dd} to {PayrollPeriodEnd:yyyy-MM-dd} and save to database?", 
                "Generate Payroll", 
                System.Windows.MessageBoxButton.YesNo, 
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    var payrollRepo = new Payroll_Repository();
                    int successCount = 0;
                    int errorCount = 0;


                    if (Employees.Count == 0)
                    {
                        System.Windows.MessageBox.Show("No employees found! Please add employees first in Employee Management.", "No Employees", 
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }

                    // Clear existing records first
                    PayrollRecords.Clear();

                    foreach (var employee in Employees)
                    {
                        try
                        {
                            // Check if employee has valid ID
                            if (employee.EmployeeID <= 0)
                            {
                                errorCount++;
                                continue;
                            }

                            var basicSalary = CalculateBasicSalary(employee);
                            var bonuses = CalculateBonuses(employee);
                            var deductions = CalculateDeductions(basicSalary);
                            
                            // Calculate leave-related payroll impacts
                            var unpaidLeaveDays = _dataService.GetUnpaidLeaveDays(employee.EmployeeID, PayrollPeriodStart, PayrollPeriodEnd);
                            var paidLeaveDays = _dataService.GetPaidLeaveDays(employee.EmployeeID, PayrollPeriodStart, PayrollPeriodEnd);
                            var leaveBalanceBonus = _dataService.GetLeaveBalanceBonus(employee.EmployeeID);
                            
                            var dailyRate = basicSalary / 22; // Assuming 22 working days per month
                            var leaveDeductions = unpaidLeaveDays * dailyRate;
                            var leaveBonuses = leaveBalanceBonus;
                            
                            var netPay = basicSalary + bonuses + leaveBonuses - deductions - leaveDeductions;

                            var payroll = new Payroll_Model
                            {
                                PeriodStart = PayrollPeriodStart,
                                PeriodEnd = PayrollPeriodEnd,
                                BasicSalary = basicSalary,
                                Bonuses = bonuses,
                                Deductions = deductions,
                                NetPay = netPay,
                                GeneratedDate = DateTime.Now,
                                EmployeeID = employee.EmployeeID,
                                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                                Position = employee.Position,
                                // Leave-related fields
                                LeaveDeductions = leaveDeductions,
                                LeaveBonuses = leaveBonuses,
                                UnpaidLeaveDays = unpaidLeaveDays,
                                PaidLeaveDays = paidLeaveDays,
                                LeaveBalanceBonus = leaveBalanceBonus
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

                    CalculateTotals();

                    // If all failed, create sample data for demonstration
                    if (successCount == 0 && errorCount > 0)
                    {
                        CreateSamplePayrollData();
                        CalculateTotals();
                        
                        System.Windows.MessageBox.Show($"Database save failed, but created {PayrollRecords.Count} sample records for demonstration.\n\n" +
                            $"Total Payroll: {TotalPayroll:C}\n" +
                            $"Total Bonuses: {TotalCommissions:C}\n\n" +
                            $"Check Debug Output for specific error details.", 
                            "Sample Data Created", 
                            System.Windows.MessageBoxButton.OK, 
                            System.Windows.MessageBoxImage.Warning);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"Payroll generation completed!\n\n" +
                            $"Successfully saved: {successCount} records\n" +
                            $"Errors: {errorCount} records\n\n" +
                            $"Total Payroll: {TotalPayroll:C}\n" +
                            $"Total Bonuses: {TotalCommissions:C}", 
                            "Payroll Generated", 
                            System.Windows.MessageBoxButton.OK, 
                            System.Windows.MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error during payroll generation: {ex.Message}", "Error", 
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void CreateSamplePayrollData()
        {
            var random = new Random();
            PayrollRecords.Clear();

            foreach (var employee in Employees)
            {
                var basicSalary = CalculateBasicSalary(employee);
                var bonuses = CalculateBonuses(employee);
                var deductions = CalculateDeductions(basicSalary);
                var netPay = basicSalary + bonuses - deductions;

                var payroll = new Payroll_Model
                {
                    PayrollID = random.Next(1000, 9999), // Sample ID
                    PeriodStart = PayrollPeriodStart,
                    PeriodEnd = PayrollPeriodEnd,
                    BasicSalary = basicSalary,
                    Bonuses = bonuses,
                    Deductions = deductions,
                    NetPay = netPay,
                    GeneratedDate = DateTime.Now,
                    EmployeeID = employee.EmployeeID,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    Position = employee.Position
                };

                PayrollRecords.Add(payroll);
            }
        }

        private decimal CalculateBasicSalary(Employee_Model employee)
        {
            // Basic salary calculation based on position
            return employee.Position.ToLower() switch
            {
                "manager" => 50000,
                "supervisor" => 35000,
                "senior" => 25000,
                "junior" => 20000,
                _ => 15000
            };
        }

        private decimal CalculateBonuses(Employee_Model employee)
        {
            // Simple bonus calculation - could be enhanced with performance metrics
            var random = new Random();
            return random.Next(0, 5000); // Random bonus between 0-5000
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
    }
}