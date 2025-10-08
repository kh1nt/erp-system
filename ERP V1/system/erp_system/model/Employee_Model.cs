using System;

namespace erp_system.model
{
    public class Employee_Model
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public string Position { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; } = 0;
        public string Status { get; set; } = string.Empty;
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}


