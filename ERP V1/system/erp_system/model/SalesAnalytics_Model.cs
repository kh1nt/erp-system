using System;

namespace erp_system.model
{
    public class TopPerformer_Model
    {
        public int Rank { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public int TransactionCount { get; set; }
    }

    public class DepartmentPerformance_Model
    {
        public string DepartmentName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageSale { get; set; }
    }

    public class MonthlyTrend_Model
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }
}
