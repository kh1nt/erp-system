using System;

namespace erp_system.model
{
    public class SalesTrendData
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }
}
