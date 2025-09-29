using System;

namespace erp_system.model
{
    public class Department_Model
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ManagerName { get; set; } = string.Empty;
    }
}
