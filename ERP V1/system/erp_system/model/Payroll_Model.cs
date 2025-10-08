using System;

namespace erp_system.model
{
    public class Payroll_Model
    {
        public int PayrollID { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal Bonuses { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPay { get; set; }
        public DateTime GeneratedDate { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        
        // Navigation property to get employee's basic salary
        public decimal BasicSalary { get; set; } // This will be populated from Employee data
    }
}
