using System;
using System.Collections.Generic;

namespace erp_system.model
{
    public class ReportData
    {
        public string ReportTitle { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
        public string CompanyName { get; set; } = "Avielle ERP System";
        public Dictionary<string, object> SummaryData { get; set; } = new Dictionary<string, object>();
        public List<object> DetailData { get; set; } = new List<object>();
    }

    public class EmployeeReportData
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Salary { get; set; }
    }

    public class SalesReportData
    {
        public int SaleId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime SaleDate { get; set; }
        public string SalesPerson { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class PayrollReportData
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public decimal Commission { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPay { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class PerformanceReportData
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int OverallRating { get; set; }
        public string Goals { get; set; } = string.Empty;
        public string Achievements { get; set; } = string.Empty;
        public string AreasForImprovement { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
    }

    public class LeaveReportData
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRequested { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
    }

    public enum ReportType
    {
        EmployeeDirectory,
        SalesSummary,
        PayrollReport,
        PerformanceAnalysis,
        AttendanceReport,
        CommissionReport
    }
}
