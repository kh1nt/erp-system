using System;

namespace erp_system.model
{
    public class Performance_Model
    {
        public int RecordID { get; set; }
        public DateTime ReviewDate { get; set; }
        public decimal Score { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int DepartmentID { get; set; }
        public string ReviewType { get; set; } = "Annual"; // Annual, Quarterly, Monthly
        public string ReviewerName { get; set; } = string.Empty;
        public string Goals { get; set; } = string.Empty;
        public string Strengths { get; set; } = string.Empty;
        public string AreasForImprovement { get; set; } = string.Empty;
    }

    public class PerformanceAnalytics
    {
        public decimal AverageScore { get; set; }
        public int TotalEvaluations { get; set; }
        public int HighPerformers { get; set; } // Score >= 4.0
        public int AveragePerformers { get; set; } // Score 2.5-3.9
        public int LowPerformers { get; set; } // Score < 2.5
        public Dictionary<string, decimal> DepartmentAverages { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> MonthlyTrends { get; set; } = new Dictionary<string, int>();
        public List<PerformanceTrend> PerformanceTrends { get; set; } = new List<PerformanceTrend>();
    }

    public class PerformanceTrend
    {
        public string Period { get; set; } = string.Empty;
        public decimal AverageScore { get; set; }
        public int EvaluationCount { get; set; }
    }
}
