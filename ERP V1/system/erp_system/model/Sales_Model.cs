using System;

namespace erp_system.model
{
    public class Sales_Model
    {
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
