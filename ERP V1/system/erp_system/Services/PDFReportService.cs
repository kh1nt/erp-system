using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using erp_system.model;
using erp_system.repositories;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace erp_system.Services
{
    public class PDFReportService
    {
        private readonly DataService _dataService;
        private readonly Employee_Repository _employeeRepo;
        private readonly Sales_Repository _salesRepo;
        private readonly Payroll_Repository _payrollRepo;
        private readonly Performance_Repository _performanceRepo;
        private readonly LeaveRequest_Repository _leaveRepo;

        public PDFReportService()
        {
            _dataService = new DataService();
            _employeeRepo = new Employee_Repository();
            _salesRepo = new Sales_Repository();
            _payrollRepo = new Payroll_Repository();
            _performanceRepo = new Performance_Repository();
            _leaveRepo = new LeaveRequest_Repository();
        }

        public string GenerateReport(ReportType reportType, DateTime? startDate = null, DateTime? endDate = null, string? customFilePath = null)
        {
            try
            {
                var reportData = GetReportData(reportType, startDate, endDate);
                var filePath = customFilePath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), GenerateFileName(reportType, startDate, endDate));

                using (var writer = new PdfWriter(filePath))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);
                        
                        // Add header
                        AddHeader(document, reportData);
                        
                        // Add summary section
                        if (reportData.SummaryData.Any())
                        {
                            AddSummarySection(document, reportData);
                        }
                        
                        // Add detailed data
                        AddDetailSection(document, reportData, reportType);
                        
                        // Add footer
                        AddFooter(document);
                    }
                }

                return filePath;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error generating PDF report: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nInner Exception: {ex.InnerException.Message}";
                }
                errorMessage += $"\n\nStack Trace: {ex.StackTrace}";
                
                MessageBox.Show(errorMessage, "PDF Generation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        private ReportData GetReportData(ReportType reportType, DateTime? startDate, DateTime? endDate)
        {
            var reportData = new ReportData
            {
                ReportTitle = GetReportTitle(reportType),
                GeneratedDate = DateTime.Now
            };

            switch (reportType)
            {
                case ReportType.EmployeeDirectory:
                    reportData.DetailData = _employeeRepo.GetAll().Cast<object>().ToList();
                    reportData.SummaryData = new Dictionary<string, object>
                    {
                        { "Total Employees", _employeeRepo.GetAll().Count() },
                        { "Active Employees", _employeeRepo.GetAll().Count(e => e.Status == "Active") },
                        { "Departments", _employeeRepo.GetAll().Select(e => e.DepartmentName).Distinct().Count() }
                    };
                    break;

                case ReportType.SalesSummary:
                    var sales = _salesRepo.GetAll();
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        sales = sales.Where(s => s.SaleDate >= startDate.Value && s.SaleDate <= endDate.Value);
                    }
                    
                    // Populate EmployeeName from Employee data
                    var employees = _employeeRepo.GetAll();
                    foreach (var sale in sales)
                    {
                        var employee = employees.FirstOrDefault(e => e.EmployeeID == sale.EmployeeID);
                        if (employee != null)
                        {
                            sale.EmployeeName = employee.FullName;
                        }
                    }
                    
                    reportData.DetailData = sales.Cast<object>().ToList();
                    reportData.SummaryData = new Dictionary<string, object>
                    {
                        { "Total Sales", sales.Sum(s => s.Amount) },
                        { "Total Transactions", sales.Count() },
                        { "Average Sale", sales.Any() ? sales.Average(s => s.Amount) : 0 },
                        { "Top Salesperson", sales.Where(s => !string.IsNullOrEmpty(s.EmployeeName)).GroupBy(s => s.EmployeeName).OrderByDescending(g => g.Sum(s => s.Amount)).FirstOrDefault()?.Key ?? "N/A" }
                    };
                    break;

                case ReportType.PayrollReport:
                    var payrolls = _payrollRepo.GetAll();
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        payrolls = payrolls.Where(p => p.PeriodStart >= startDate.Value && p.PeriodEnd <= endDate.Value);
                    }
                    
                    // Populate EmployeeName and BasicSalary from Employee data
                    var employeesForPayroll = _employeeRepo.GetAll();
                    foreach (var payroll in payrolls)
                    {
                        var employee = employeesForPayroll.FirstOrDefault(e => e.EmployeeID == payroll.EmployeeID);
                        if (employee != null)
                        {
                            payroll.EmployeeName = employee.FullName;
                            payroll.BasicSalary = employee.BasicSalary;
                        }
                    }
                    
                    reportData.DetailData = payrolls.Cast<object>().ToList();
                    reportData.SummaryData = new Dictionary<string, object>
                    {
                        { "Total Payroll", payrolls.Sum(p => p.NetPay) },
                        { "Total Employees", payrolls.Count() },
                        { "Average Salary", payrolls.Any() ? payrolls.Average(p => p.BasicSalary) : 0 },
                        { "Total Deductions", payrolls.Sum(p => p.Deductions) }
                    };
                    break;

                case ReportType.PerformanceAnalysis:
                    var performances = _performanceRepo.GetAll();
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        performances = performances.Where(p => p.ReviewDate >= startDate.Value && p.ReviewDate <= endDate.Value);
                    }
                    
                    // Populate EmployeeName and Department from Employee data
                    var employeesForPerformance = _employeeRepo.GetAll();
                    foreach (var performance in performances)
                    {
                        var employee = employeesForPerformance.FirstOrDefault(e => e.EmployeeID == performance.EmployeeID);
                        if (employee != null)
                        {
                            performance.EmployeeName = employee.FullName;
                            performance.Department = employee.DepartmentName;
                        }
                    }
                    
                    reportData.DetailData = performances.Cast<object>().ToList();
                    reportData.SummaryData = new Dictionary<string, object>
                    {
                        { "Total Reviews", performances.Count() },
                        { "Average Rating", performances.Any() ? performances.Average(p => p.Score) : 0 },
                        { "High Performers", performances.Count(p => p.Score >= 4) },
                        { "Needs Improvement", performances.Count(p => p.Score <= 2) }
                    };
                    break;

                case ReportType.AttendanceReport:
                    var leaves = _leaveRepo.GetAll();
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        leaves = leaves.Where(l => l.StartDate >= startDate.Value && l.EndDate <= endDate.Value);
                    }
                    
                    // Populate EmployeeName from Employee data
                    var employeesForLeave = _employeeRepo.GetAll();
                    foreach (var leave in leaves)
                    {
                        var employee = employeesForLeave.FirstOrDefault(e => e.EmployeeID == leave.EmployeeID);
                        if (employee != null)
                        {
                            leave.EmployeeName = employee.FullName;
                        }
                    }
                    
                    reportData.DetailData = leaves.Cast<object>().ToList();
                    reportData.SummaryData = new Dictionary<string, object>
                    {
                        { "Total Leave Requests", leaves.Count() },
                        { "Approved Requests", leaves.Count(l => l.Status == "Approved") },
                        { "Pending Requests", leaves.Count(l => l.Status == "Pending") },
                        { "Total Days Requested", leaves.Sum(l => (l.EndDate - l.StartDate).Days + 1) }
                    };
                    break;

                case ReportType.CommissionReport:
                    var salesForCommission = _salesRepo.GetAll();
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        salesForCommission = salesForCommission.Where(s => s.SaleDate >= startDate.Value && s.SaleDate <= endDate.Value);
                    }
                    
                    // Populate EmployeeName from Employee data
                    var employeesForCommission = _employeeRepo.GetAll();
                    foreach (var sale in salesForCommission)
                    {
                        var employee = employeesForCommission.FirstOrDefault(e => e.EmployeeID == sale.EmployeeID);
                        if (employee != null)
                        {
                            sale.EmployeeName = employee.FullName;
                        }
                    }
                    
                    // Calculate commission data (5% commission rate)
                    var commissionData = salesForCommission
                        .Where(s => !string.IsNullOrEmpty(s.EmployeeName))
                        .GroupBy(s => s.EmployeeName)
                        .Select(g => new { 
                            SalesPerson = g.Key, 
                            TotalSales = g.Sum(s => s.Amount), 
                            Commission = g.Sum(s => s.Amount) * 0.05m,
                            TransactionCount = g.Count()
                        })
                        .OrderByDescending(c => c.Commission)
                        .ToList();
                    
                    reportData.DetailData = commissionData.Cast<object>().ToList();
                    reportData.SummaryData = new Dictionary<string, object>
                    {
                        { "Total Commission Paid", commissionData.Sum(c => c.Commission) },
                        { "Sales Consultants", commissionData.Count() },
                        { "Average Commission", commissionData.Any() ? commissionData.Average(c => c.Commission) : 0 },
                        { "Top Performer", commissionData.FirstOrDefault()?.SalesPerson ?? "N/A" }
                    };
                    break;
            }

            return reportData;
        }

        private string GetReportTitle(ReportType reportType)
        {
            return reportType switch
            {
                ReportType.EmployeeDirectory => "Employee Directory Report",
                ReportType.SalesSummary => "Sales Summary Report",
                ReportType.PayrollReport => "Payroll Report",
                ReportType.PerformanceAnalysis => "Performance Analysis Report",
                ReportType.AttendanceReport => "Leave Report",
                ReportType.CommissionReport => "Sales Commission Report",
                _ => "Custom Report"
            };
        }

        private string GenerateFileName(ReportType reportType, DateTime? startDate, DateTime? endDate)
        {
            var baseName = reportType.ToString().Replace("Report", "").Replace("Analysis", "").Replace("Summary", "");
            var dateRange = startDate.HasValue && endDate.HasValue 
                ? $"_{startDate.Value:yyyyMMdd}_{endDate.Value:yyyyMMdd}" 
                : $"_{DateTime.Now:yyyyMMdd}";
            return $"{baseName}_Report{dateRange}.pdf";
        }

        private void AddHeader(Document document, ReportData reportData)
        {
            // Company logo/header
            var headerTable = new Table(1).UseAllAvailableWidth();
            headerTable.AddCell(new Cell().Add(new Paragraph(reportData.CompanyName)
                .SetFontSize(20)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10)));
            
            headerTable.AddCell(new Cell().Add(new Paragraph(reportData.ReportTitle)
                .SetFontSize(16)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(5)));
            
            headerTable.AddCell(new Cell().Add(new Paragraph($"Generated on: {reportData.GeneratedDate:MMMM dd, yyyy 'at' h:mm tt}")
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(15)));

            document.Add(headerTable);
        }

        private void AddSummarySection(Document document, ReportData reportData)
        {
            document.Add(new Paragraph("SUMMARY")
                .SetFontSize(14)
                .SetBold()
                .SetMarginTop(20)
                .SetMarginBottom(10));

            var summaryTable = new Table(2).UseAllAvailableWidth();
            summaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Metric").SetBold()));
            summaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Value").SetBold()));

            foreach (var item in reportData.SummaryData)
            {
                summaryTable.AddCell(new Cell().Add(new Paragraph(item.Key)));
                summaryTable.AddCell(new Cell().Add(new Paragraph(item.Value?.ToString() ?? "N/A")));
            }

            document.Add(summaryTable);
        }

        private void AddDetailSection(Document document, ReportData reportData, ReportType reportType)
        {
            if (!reportData.DetailData.Any()) return;

            document.Add(new Paragraph("DETAILED DATA")
                .SetFontSize(14)
                .SetBold()
                .SetMarginTop(20)
                .SetMarginBottom(10));

            switch (reportType)
            {
                case ReportType.EmployeeDirectory:
                    AddEmployeeTable(document, reportData.DetailData.Cast<Employee_Model>().ToList());
                    break;
                case ReportType.SalesSummary:
                    AddSalesTable(document, reportData.DetailData.Cast<Sales_Model>().ToList());
                    break;
                case ReportType.PayrollReport:
                    AddPayrollTable(document, reportData.DetailData.Cast<Payroll_Model>().ToList());
                    break;
                case ReportType.PerformanceAnalysis:
                    AddPerformanceTable(document, reportData.DetailData.Cast<Performance_Model>().ToList());
                    break;
                case ReportType.AttendanceReport:
                    AddLeaveTable(document, reportData.DetailData.Cast<LeaveRequest_Model>().ToList());
                    break;
                case ReportType.CommissionReport:
                    AddCommissionTable(document, reportData.DetailData);
                    break;
            }
        }

        private void AddEmployeeTable(Document document, List<Employee_Model> employees)
        {
            var table = new Table(6).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("ID").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Name").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Department").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Position").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Hire Date").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Status").SetBold()));

            foreach (var emp in employees)
            {
                table.AddCell(new Cell().Add(new Paragraph(emp.EmployeeID.ToString())));
                table.AddCell(new Cell().Add(new Paragraph($"{emp.FirstName} {emp.LastName}")));
                table.AddCell(new Cell().Add(new Paragraph(emp.DepartmentName)));
                table.AddCell(new Cell().Add(new Paragraph(emp.Position)));
                table.AddCell(new Cell().Add(new Paragraph(emp.HireDate.ToString("MMM dd, yyyy"))));
                table.AddCell(new Cell().Add(new Paragraph(emp.Status)));
            }

            document.Add(table);
        }

        private void AddSalesTable(Document document, List<Sales_Model> sales)
        {
            var table = new Table(6).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Sale ID").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Customer").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Product").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Amount").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Sales Person").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Date").SetBold()));

            foreach (var sale in sales)
            {
                table.AddCell(new Cell().Add(new Paragraph(sale.SaleID.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(sale.Description)));
                table.AddCell(new Cell().Add(new Paragraph(sale.Description)));
                table.AddCell(new Cell().Add(new Paragraph(sale.Amount.ToString("C"))));
                table.AddCell(new Cell().Add(new Paragraph(sale.EmployeeName)));
                table.AddCell(new Cell().Add(new Paragraph(sale.SaleDate.ToString("MMM dd, yyyy"))));
            }

            document.Add(table);
        }

        private void AddPayrollTable(Document document, List<Payroll_Model> payrolls)
        {
            var table = new Table(6).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Employee ID").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Employee Name").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Base Salary").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Commission").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Deductions").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Net Pay").SetBold()));

            foreach (var payroll in payrolls)
            {
                table.AddCell(new Cell().Add(new Paragraph(payroll.EmployeeID.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(payroll.EmployeeName)));
                table.AddCell(new Cell().Add(new Paragraph(payroll.BasicSalary.ToString("C"))));
                table.AddCell(new Cell().Add(new Paragraph(payroll.Bonuses.ToString("C"))));
                table.AddCell(new Cell().Add(new Paragraph(payroll.Deductions.ToString("C"))));
                table.AddCell(new Cell().Add(new Paragraph(payroll.NetPay.ToString("C"))));
            }

            document.Add(table);
        }

        private void AddPerformanceTable(Document document, List<Performance_Model> performances)
        {
            var table = new Table(5).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Employee ID").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Employee Name").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Department").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Rating").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Review Date").SetBold()));

            foreach (var perf in performances)
            {
                table.AddCell(new Cell().Add(new Paragraph(perf.EmployeeID.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(perf.EmployeeName)));
                table.AddCell(new Cell().Add(new Paragraph(perf.Department)));
                table.AddCell(new Cell().Add(new Paragraph(perf.Score.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(perf.ReviewDate.ToString("MMM dd, yyyy"))));
            }

            document.Add(table);
        }

        private void AddLeaveTable(Document document, List<LeaveRequest_Model> leaves)
        {
            var table = new Table(6).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Employee ID").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Employee Name").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Leave Type").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Start Date").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("End Date").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Status").SetBold()));

            foreach (var leave in leaves)
            {
                table.AddCell(new Cell().Add(new Paragraph(leave.EmployeeID.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(leave.EmployeeName)));
                table.AddCell(new Cell().Add(new Paragraph(leave.TypeName)));
                table.AddCell(new Cell().Add(new Paragraph(leave.StartDate.ToString("MMM dd, yyyy"))));
                table.AddCell(new Cell().Add(new Paragraph(leave.EndDate.ToString("MMM dd, yyyy"))));
                table.AddCell(new Cell().Add(new Paragraph(leave.Status)));
            }

            document.Add(table);
        }

        private void AddCommissionTable(Document document, List<object> commissionData)
        {
            var table = new Table(4).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Sales Person").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Total Sales").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Transactions").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Commission (5%)").SetBold()));

            foreach (dynamic item in commissionData)
            {
                table.AddCell(new Cell().Add(new Paragraph(item.SalesPerson)));
                table.AddCell(new Cell().Add(new Paragraph(item.TotalSales.ToString("C"))));
                table.AddCell(new Cell().Add(new Paragraph(item.TransactionCount.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(item.Commission.ToString("C"))));
            }

            document.Add(table);
        }

        private void AddFooter(Document document)
        {
            document.Add(new Paragraph("--- End of Report ---")
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(30));
        }
    }
}
