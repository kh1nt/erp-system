using System;

namespace erp_system.model
{
    public class Leave_Model
    {
        public int LeaveID { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public int MaxDays { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class LeaveRequest_Model
    {
        public int RequestID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
        public int EmployeeID { get; set; }
        public int LeaveID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
    }
}
