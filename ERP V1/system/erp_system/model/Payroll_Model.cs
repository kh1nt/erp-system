using System;

namespace erp_system.model
{
    public class Payroll_Model
    {
        public int PayrollID { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Bonuses { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPay { get; set; }
        public DateTime GeneratedDate { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        
        // Leave-related payroll fields
        public decimal LeaveDeductions { get; set; } = 0;
        public decimal LeaveBonuses { get; set; } = 0;
        public int UnpaidLeaveDays { get; set; } = 0;
        public int PaidLeaveDays { get; set; } = 0;
        public decimal LeaveBalanceBonus { get; set; } = 0;
    }
}
