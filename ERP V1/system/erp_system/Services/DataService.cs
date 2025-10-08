using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using erp_system.Configuration;
using erp_system.model;

namespace erp_system.Services
{
    public class DataService
    {
        private readonly string _cs;
        public DataService()
        {
            var cs = AppConfig.GetConnectionString();
            _cs = string.IsNullOrWhiteSpace(cs)
                ? @"Data Source=DESKTOP-I5DCKI6\SQLEXPRESS;Initial Catalog=avielle;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
                : cs;
        }

        public int GetCount(string table)
        {
            // Allowlist table names to prevent injection via identifier
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Users","Employees","Departments","Leaves","Leave_Requests","Performance_Records","Payrolls","Sales"
            };
            if (!allowed.Contains(table)) return 0;

            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand($"SELECT COUNT(*) FROM [{table}]", con);
            return (int)cmd.ExecuteScalar();
        }

        public bool UpdateEmployeeStatus(int employeeId, string newStatus)
        {
            try
            {
                using var con = new SqlConnection(_cs);
                con.Open();
                using var cmd = new SqlCommand(@"
                    UPDATE Employees 
                    SET Status = @Status 
                    WHERE EmployeeID = @EmployeeID", con);
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating employee status: {ex.Message}");
                return false;
            }
        }

        // EMPLOYEES
        public List<Employee_Model> GetEmployees()
        {
            var list = new List<Employee_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT e.EmployeeID, e.FirstName, e.LastName, e.Email, e.Phone, e.HireDate, e.Position, e.BasicSalary, e.Status, e.DepartmentID, d.DepartmentName
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Employee_Model
                {
                    EmployeeID = rdr.GetInt32(0),
                    FirstName = rdr[1]?.ToString() ?? string.Empty,
                    LastName = rdr[2]?.ToString() ?? string.Empty,
                    Email = rdr[3]?.ToString() ?? string.Empty,
                    Phone = rdr[4]?.ToString() ?? string.Empty,
                    HireDate = rdr.GetDateTime(5),
                    Position = rdr[6]?.ToString() ?? string.Empty,
                    BasicSalary = rdr.GetDecimal(7),
                    Status = rdr[8]?.ToString() ?? string.Empty,
                    DepartmentID = rdr.GetInt32(9),
                    DepartmentName = rdr[10]?.ToString() ?? string.Empty
                });
            }
            return list;
        }

        public List<Employee_Model> GetEmployeesByDepartment(string departmentName)
        {
            var list = new List<Employee_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT e.EmployeeID, e.FirstName, e.LastName, e.Email, e.Phone, e.HireDate, e.Position, e.BasicSalary, e.Status, e.DepartmentID, d.DepartmentName
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                WHERE d.DepartmentName = @DepartmentName", con);
            cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Employee_Model
                {
                    EmployeeID = rdr.GetInt32(0),
                    FirstName = rdr[1]?.ToString() ?? string.Empty,
                    LastName = rdr[2]?.ToString() ?? string.Empty,
                    Email = rdr[3]?.ToString() ?? string.Empty,
                    Phone = rdr[4]?.ToString() ?? string.Empty,
                    HireDate = rdr.GetDateTime(5),
                    Position = rdr[6]?.ToString() ?? string.Empty,
                    BasicSalary = rdr.GetDecimal(7),
                    Status = rdr[8]?.ToString() ?? string.Empty,
                    DepartmentID = rdr.GetInt32(9),
                    DepartmentName = rdr[10]?.ToString() ?? string.Empty
                });
            }
            return list;
        }

        // DEPARTMENTS
        public List<Department_Model> GetDepartments()
        {
            var list = new List<Department_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand("SELECT DepartmentID, DepartmentName, Description FROM Departments", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Department_Model
                {
                    DepartmentID = rdr.GetInt32(0),
                    DepartmentName = rdr[1]?.ToString() ?? string.Empty,
                    Description = rdr[2]?.ToString() ?? string.Empty,
                    CreatedDate = System.DateTime.MinValue
                });
            }
            return list;
        }

        // LEAVE REQUESTS
        public List<LeaveRequest_Model> GetLeaveRequests()
        {
            var list = new List<LeaveRequest_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT lr.LeaveRequestID, lr.StartDate, lr.EndDate, lr.RequestDate, 
                       lr.Status, lr.ApprovedBy, lr.EmployeeID, lr.LeaveID,
                       e.FirstName + ' ' + e.LastName as EmployeeName,
                       l.TypeName as TypeName, lr.Reason
                FROM Leave_Requests lr
                INNER JOIN Employees e ON lr.EmployeeID = e.EmployeeID
                INNER JOIN Leaves l ON lr.LeaveID = l.LeaveID
                ORDER BY lr.RequestDate DESC", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new LeaveRequest_Model
                {
                    RequestID = rdr.GetInt32(0),
                    StartDate = rdr.GetDateTime(1),
                    EndDate = rdr.GetDateTime(2),
                    RequestDate = rdr.GetDateTime(3),
                    Status = rdr[4]?.ToString() ?? string.Empty,
                    ApprovedBy = rdr[5]?.ToString() ?? string.Empty,
                    EmployeeID = rdr.GetInt32(6),
                    LeaveID = rdr.GetInt32(7),
                    EmployeeName = rdr[8]?.ToString() ?? string.Empty,
                    TypeName = rdr[9]?.ToString() ?? string.Empty,
                    Reason = rdr[10]?.ToString() ?? string.Empty
                });
            }
            return list;
        }

        // LEAVE TYPES
        public List<Leave_Model> GetLeaveTypes()
        {
            var list = new List<Leave_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand("SELECT LeaveID, TypeName, MaxDays, Description FROM Leaves ORDER BY TypeName", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Leave_Model
                {
                    LeaveID = rdr.GetInt32(0),
                    TypeName = rdr[1]?.ToString() ?? string.Empty,
                    MaxDays = rdr.GetInt32(2),
                    Description = rdr[3]?.ToString() ?? string.Empty
                });
            }
            return list;
        }

        // LEAVE BALANCE CALCULATION (Calendar Year Reset)
        public int GetEmployeeLeaveBalance(int employeeId, int leaveTypeId)
        {
            using var con = new SqlConnection(_cs);
            con.Open();
            
            // Get max days for this leave type
            using var cmd1 = new SqlCommand("SELECT MaxDays FROM Leaves WHERE LeaveID = @LeaveID", con);
            cmd1.Parameters.AddWithValue("@LeaveID", leaveTypeId);
            var maxDays = (int)cmd1.ExecuteScalar();

            // Get used days for this employee and leave type for CURRENT CALENDAR YEAR ONLY
            using var cmd2 = new SqlCommand(@"
                SELECT ISNULL(SUM(DATEDIFF(day, StartDate, EndDate) + 1), 0) as UsedDays
                FROM Leave_Requests 
                WHERE EmployeeID = @EmployeeID 
                  AND LeaveID = @LeaveID 
                  AND Status = 'Approved'
                  AND YEAR(StartDate) = YEAR(GETDATE())", con);
            cmd2.Parameters.AddWithValue("@EmployeeID", employeeId);
            cmd2.Parameters.AddWithValue("@LeaveID", leaveTypeId);
            var usedDays = (int)cmd2.ExecuteScalar();

            return maxDays - usedDays;
        }

        // LEAVE STATISTICS
        public Dictionary<string, int> GetLeaveStatistics()
        {
            var stats = new Dictionary<string, int>();
            using var con = new SqlConnection(_cs);
            con.Open();
            
            using var cmd = new SqlCommand(@"
                SELECT 
                    COUNT(CASE WHEN Status = 'Pending' THEN 1 END) as PendingCount,
                    COUNT(CASE WHEN Status = 'Approved' THEN 1 END) as ApprovedCount,
                    COUNT(CASE WHEN Status = 'Rejected' THEN 1 END) as RejectedCount,
                    COUNT(CASE WHEN MONTH(StartDate) = MONTH(GETDATE()) AND YEAR(StartDate) = YEAR(GETDATE()) THEN 1 END) as ThisMonthCount
                FROM Leave_Requests", con);
            
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                stats["Pending"] = rdr.GetInt32(0);
                stats["Approved"] = rdr.GetInt32(1);
                stats["Rejected"] = rdr.GetInt32(2);
                stats["ThisMonth"] = rdr.GetInt32(3);
            }
            
            return stats;
        }

        // USER AUTHENTICATION
        public User_Account_Model? GetUserByUsername(string username)
        {
            using var con = new SqlConnection(_cs);
            con.Open();
            
            using var cmd = new SqlCommand(@"
                SELECT u.UserID, u.UserName, u.PasswordHash, u.RoleName, u.CreatedAt, u.EmployeeID,
                       e.FirstName + ' ' + e.LastName as EmployeeName
                FROM Users u
                INNER JOIN Employees e ON u.EmployeeID = e.EmployeeID
                WHERE u.UserName = @Username", con);
            
            cmd.Parameters.AddWithValue("@Username", username);
            
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new User_Account_Model
                {
                    UserID = rdr.GetInt32(0),
                    UserName = rdr[1]?.ToString() ?? string.Empty,
                    PasswordHash = rdr[2]?.ToString() ?? string.Empty,
                    RoleName = rdr[3]?.ToString() ?? string.Empty,
                    CreatedAt = rdr.GetDateTime(4),
                    EmployeeID = rdr.GetInt32(5),
                    EmployeeName = rdr[6]?.ToString() ?? string.Empty
                };
            }
            
            return null;
        }

        public string GetCurrentUser()
        {
            // This should be implemented based on your authentication system
            // For now, returning a default user - you should integrate with your login system
            return "admin"; // This should come from your authentication context
        }

        // LEAVE PAYROLL INTEGRATION METHODS
        public int GetUnpaidLeaveDays(int employeeId, DateTime periodStart, DateTime periodEnd)
        {
            using var con = new SqlConnection(_cs);
            con.Open();
            
            using var cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(DATEDIFF(day, StartDate, EndDate) + 1), 0) as UnpaidDays
                FROM Leave_Requests lr
                JOIN Leaves l ON lr.LeaveID = l.LeaveID
                WHERE lr.EmployeeID = @EmployeeID 
                  AND lr.Status = 'Approved'
                  AND l.TypeName IN ('Personal Leave', 'Emergency Leave')
                  AND lr.StartDate >= @PeriodStart 
                  AND lr.EndDate <= @PeriodEnd", con);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
            cmd.Parameters.AddWithValue("@PeriodStart", periodStart);
            cmd.Parameters.AddWithValue("@PeriodEnd", periodEnd);
            
            return (int)cmd.ExecuteScalar();
        }

        public int GetPaidLeaveDays(int employeeId, DateTime periodStart, DateTime periodEnd)
        {
            using var con = new SqlConnection(_cs);
            con.Open();
            
            using var cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(DATEDIFF(day, StartDate, EndDate) + 1), 0) as PaidDays
                FROM Leave_Requests lr
                JOIN Leaves l ON lr.LeaveID = l.LeaveID
                WHERE lr.EmployeeID = @EmployeeID 
                  AND lr.Status = 'Approved'
                  AND l.TypeName IN ('Annual Leave', 'Sick Leave', 'Maternity Leave', 'Paternity Leave')
                  AND lr.StartDate >= @PeriodStart 
                  AND lr.EndDate <= @PeriodEnd", con);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
            cmd.Parameters.AddWithValue("@PeriodStart", periodStart);
            cmd.Parameters.AddWithValue("@PeriodEnd", periodEnd);
            
            return (int)cmd.ExecuteScalar();
        }

        public decimal GetLeaveBalanceBonus(int employeeId)
        {
            using var con = new SqlConnection(_cs);
            con.Open();
            
            // Calculate bonus based on unused leave days at year-end
            using var cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(l.MaxDays - ISNULL(used.UsedDays, 0)), 0) as UnusedDays
                FROM Leaves l
                LEFT JOIN (
                    SELECT lr.LeaveID, SUM(DATEDIFF(day, lr.StartDate, lr.EndDate) + 1) as UsedDays
                    FROM Leave_Requests lr
                    WHERE lr.EmployeeID = @EmployeeID 
                      AND lr.Status = 'Approved'
                      AND YEAR(lr.StartDate) = YEAR(GETDATE())
                    GROUP BY lr.LeaveID
                ) used ON l.LeaveID = used.LeaveID", con);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
            
            var unusedDays = (int)cmd.ExecuteScalar();
            
            // Bonus calculation: ₱500 per unused day (max ₱5,000)
            return Math.Min(unusedDays * 500, 5000);
        }

        // PERFORMANCE RECORDS
        public List<Performance_Model> GetPerformanceRecords()
        {
            var list = new List<Performance_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT pr.PerformanceRecordID, pr.ReviewDate, pr.Score, pr.Remarks, pr.EmployeeID,
                       e.FirstName + ' ' + e.LastName as EmployeeName, e.Position, d.DepartmentName, d.DepartmentID
                FROM Performance_Records pr
                INNER JOIN Employees e ON pr.EmployeeID = e.EmployeeID
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                ORDER BY pr.ReviewDate DESC, pr.PerformanceRecordID DESC", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Performance_Model
                {
                    RecordID = rdr.GetInt32(0),
                    ReviewDate = rdr.GetDateTime(1),
                    Score = rdr.GetDecimal(2),
                    Remarks = rdr[3]?.ToString() ?? string.Empty,
                    EmployeeID = rdr.GetInt32(4),
                    EmployeeName = rdr[5]?.ToString() ?? string.Empty,
                    Position = rdr[6]?.ToString() ?? string.Empty,
                    Department = rdr[7]?.ToString() ?? string.Empty,
                    DepartmentID = rdr.IsDBNull(8) ? 0 : rdr.GetInt32(8),
                    ReviewType = "Annual", // Default value since column doesn't exist
                    ReviewerName = string.Empty, // Default value since column doesn't exist
                    Goals = string.Empty, // Default value since column doesn't exist
                    Strengths = string.Empty, // Default value since column doesn't exist
                    AreasForImprovement = string.Empty // Default value since column doesn't exist
                });
            }
            return list;
        }

        // GET ALL DEPARTMENTS
        public List<DepartmentInfo> GetAllDepartments()
        {
            var departments = new List<DepartmentInfo>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand("SELECT DepartmentID, DepartmentName FROM Departments ORDER BY DepartmentName", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var deptName = rdr[1]?.ToString();
                if (!string.IsNullOrEmpty(deptName))
                {
                    departments.Add(new DepartmentInfo
                    {
                        DepartmentID = rdr.GetInt32(0),
                        DepartmentName = deptName.Trim()
                    });
                }
            }
            return departments;
        }

        // GET SALES TREND DATA
        public List<SalesTrendData> GetSalesTrendData()
        {
            var list = new List<SalesTrendData>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT 
                    FORMAT(s.SaleDate, 'MMM yyyy') as Month,
                    SUM(s.Amount) as Amount,
                    COUNT(*) as TransactionCount
                FROM Sales s
                WHERE s.SaleDate >= DATEADD(MONTH, -6, GETDATE())
                GROUP BY YEAR(s.SaleDate), MONTH(s.SaleDate), FORMAT(s.SaleDate, 'MMM yyyy')
                ORDER BY YEAR(s.SaleDate), MONTH(s.SaleDate)", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new SalesTrendData
                {
                    Month = rdr[0]?.ToString() ?? string.Empty,
                    Amount = rdr.GetDecimal(1),
                    TransactionCount = rdr.GetInt32(2)
                });
            }
            return list;
        }

        // ADD PERFORMANCE RECORD
        public bool AddPerformanceRecord(Performance_Model performanceRecord)
        {
            try
            {
                using var con = new SqlConnection(_cs);
                con.Open();
                using var cmd = new SqlCommand(@"
                    INSERT INTO Performance_Records (ReviewDate, Score, Remarks, EmployeeID)
                    VALUES (@ReviewDate, @Score, @Remarks, @EmployeeID)", con);
                
                cmd.Parameters.AddWithValue("@ReviewDate", performanceRecord.ReviewDate);
                cmd.Parameters.AddWithValue("@Score", performanceRecord.Score);
                cmd.Parameters.AddWithValue("@Remarks", performanceRecord.Remarks ?? string.Empty);
                cmd.Parameters.AddWithValue("@EmployeeID", performanceRecord.EmployeeID);
                
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the error (you might want to use a proper logging framework)
                System.Diagnostics.Debug.WriteLine($"Error adding performance record: {ex.Message}");
                return false;
            }
        }

        // PERFORMANCE STATISTICS
        public Dictionary<string, object> GetPerformanceStatistics()
        {
            var stats = new Dictionary<string, object>();
            using var con = new SqlConnection(_cs);
            con.Open();
            
            using var cmd = new SqlCommand(@"
                SELECT 
                    COUNT(*) as TotalEvaluations,
                    ISNULL(AVG(pr.Score), 0) as AverageScore,
                    COUNT(CASE WHEN pr.Score >= 4.0 THEN 1 END) as HighPerformers,
                    COUNT(CASE WHEN pr.Score >= 2.5 AND pr.Score < 4.0 THEN 1 END) as AveragePerformers,
                    COUNT(CASE WHEN pr.Score < 2.5 THEN 1 END) as LowPerformers
                FROM Performance_Records pr", con);
            
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                stats["TotalEvaluations"] = rdr.GetInt32(0);
                stats["AverageScore"] = Math.Round(rdr.GetDecimal(1), 2);
                stats["HighPerformers"] = rdr.GetInt32(2);
                stats["AveragePerformers"] = rdr.GetInt32(3);
                stats["LowPerformers"] = rdr.GetInt32(4);
            }
            
            return stats;
        }

        // DEPARTMENT PERFORMANCE STATISTICS
        public Dictionary<string, decimal> GetDepartmentPerformanceStats()
        {
            var stats = new Dictionary<string, decimal>();
            using var con = new SqlConnection(_cs);
            con.Open();
            
            using var cmd = new SqlCommand(@"
                SELECT d.DepartmentName, ISNULL(AVG(pr.Score), 0) as AvgScore
                FROM Departments d
                LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID
                LEFT JOIN Performance_Records pr ON e.EmployeeID = pr.EmployeeID
                GROUP BY d.DepartmentName
                ORDER BY AvgScore DESC", con);
            
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                stats[rdr[0]?.ToString() ?? "Unknown"] = rdr.GetDecimal(1);
            }
            
            return stats;
        }

        // MONTHLY PERFORMANCE TRENDS
        public Dictionary<string, decimal> GetMonthlyPerformanceTrends()
        {
            var trends = new Dictionary<string, decimal>();
            using var con = new SqlConnection(_cs);
            con.Open();
            
            using var cmd = new SqlCommand(@"
                SELECT 
                    FORMAT(ReviewDate, 'yyyy-MM') as Period,
                    AVG(Score) as AvgScore
                FROM Performance_Records
                WHERE ReviewDate >= DATEADD(MONTH, -12, GETDATE())
                GROUP BY FORMAT(ReviewDate, 'yyyy-MM')
                ORDER BY Period", con);
            
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                trends[rdr[0]?.ToString() ?? ""] = rdr.GetDecimal(1);
            }
            
            return trends;
        }

        // PAYROLL RECORDS
        public bool PayrollExistsForPeriod(DateTime periodStart, DateTime periodEnd)
        {
            using var con = new SqlConnection(_cs);
            con.Open();
            
            // Check for exact period match first
            using var cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM Payrolls 
                WHERE PeriodStart = @PeriodStart AND PeriodEnd = @PeriodEnd", con);
            cmd.Parameters.AddWithValue("@PeriodStart", periodStart);
            cmd.Parameters.AddWithValue("@PeriodEnd", periodEnd);
            
            var exactCount = (int)cmd.ExecuteScalar();
            if (exactCount > 0) return true;
            
            // Also check for overlapping periods in the same month/year
            using var cmd2 = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM Payrolls 
                WHERE YEAR(PeriodStart) = @Year AND MONTH(PeriodStart) = @Month", con);
            cmd2.Parameters.AddWithValue("@Year", periodStart.Year);
            cmd2.Parameters.AddWithValue("@Month", periodStart.Month);
            
            var monthCount = (int)cmd2.ExecuteScalar();
            return monthCount > 0;
        }

        public Payroll_Model? GetExistingPayrollForPeriod(DateTime periodStart, DateTime periodEnd)
        {
            using var con = new SqlConnection(_cs);
            con.Open();
            
            using var cmd = new SqlCommand(@"
                SELECT TOP 1 p.PayrollID, p.PeriodStart, p.PeriodEnd, e.BasicSalary, p.Bonuses, 
                       p.Deductions, p.NetPay, p.Generated_Date, p.EmployeeID,
                       e.FirstName + ' ' + e.LastName as EmployeeName, e.Position
                FROM Payrolls p
                INNER JOIN Employees e ON p.EmployeeID = e.EmployeeID
                WHERE YEAR(p.PeriodStart) = @Year AND MONTH(p.PeriodStart) = @Month
                ORDER BY p.Generated_Date DESC", con);
            cmd.Parameters.AddWithValue("@Year", periodStart.Year);
            cmd.Parameters.AddWithValue("@Month", periodStart.Month);
            
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new Payroll_Model
                {
                    PayrollID = rdr.GetInt32(0),
                    PeriodStart = rdr.GetDateTime(1),
                    PeriodEnd = rdr.GetDateTime(2),
                    BasicSalary = rdr.GetDecimal(3),
                    Bonuses = rdr.GetDecimal(4),
                    Deductions = rdr.GetDecimal(5),
                    NetPay = rdr.GetDecimal(6),
                    GeneratedDate = rdr.GetDateTime(7),
                    EmployeeID = rdr.GetInt32(8),
                    EmployeeName = rdr[9]?.ToString() ?? string.Empty,
                    Position = rdr[10]?.ToString() ?? string.Empty
                };
            }
            return null;
        }

        public List<Payroll_Model> GetPayrollRecords()
        {
            var list = new List<Payroll_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT p.PayrollID, p.PeriodStart, p.PeriodEnd, e.BasicSalary, p.Bonuses, 
                       p.Deductions, p.NetPay, p.Generated_Date, p.EmployeeID,
                       e.FirstName + ' ' + e.LastName as EmployeeName, e.Position
                FROM Payrolls p
                INNER JOIN Employees e ON p.EmployeeID = e.EmployeeID
                ORDER BY p.Generated_Date DESC, p.PayrollID DESC", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Payroll_Model
                {
                    PayrollID = rdr.GetInt32(0),
                    PeriodStart = rdr.GetDateTime(1),
                    PeriodEnd = rdr.GetDateTime(2),
                    BasicSalary = rdr.GetDecimal(3),
                    Bonuses = rdr.GetDecimal(4),
                    Deductions = rdr.GetDecimal(5),
                    NetPay = rdr.GetDecimal(6),
                    GeneratedDate = rdr.GetDateTime(7),
                    EmployeeID = rdr.GetInt32(8),
                    EmployeeName = rdr[9]?.ToString() ?? string.Empty,
                    Position = rdr[10]?.ToString() ?? string.Empty
                });
            }
            return list;
        }

        // SALES RECORDS
        public List<Sales_Model> GetSalesRecords()
        {
            var list = new List<Sales_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT s.SalesID as SaleID, s.SaleDate, s.Amount, s.Description, s.EmployeeID,
                       e.FirstName + ' ' + e.LastName as EmployeeName, d.DepartmentName
                FROM Sales s
                INNER JOIN Employees e ON s.EmployeeID = e.EmployeeID
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Sales_Model
                {
                    SaleID = rdr.GetInt32(0),
                    SaleDate = rdr.GetDateTime(1),
                    Amount = rdr.GetDecimal(2),
                    Description = rdr[3]?.ToString() ?? string.Empty,
                    EmployeeID = rdr.GetInt32(4),
                    EmployeeName = rdr[5]?.ToString() ?? string.Empty,
                    Department = rdr[6]?.ToString() ?? string.Empty
                });
            }
            return list;
        }

        // SUMMARY STATS
        public Dictionary<string, decimal> GetSalesSummary()
        {
            var stats = new Dictionary<string, decimal>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT 
                    SUM(Amount) as TotalSales,
                    AVG(Amount) as AverageSale,
                    COUNT(*) as TotalTransactions,
                    -- This month sales
                    ISNULL(SUM(CASE WHEN YEAR(SaleDate) = YEAR(GETDATE()) AND MONTH(SaleDate) = MONTH(GETDATE()) THEN Amount END), 0) as ThisMonthSales,
                    ISNULL(COUNT(CASE WHEN YEAR(SaleDate) = YEAR(GETDATE()) AND MONTH(SaleDate) = MONTH(GETDATE()) THEN 1 END), 0) as ThisMonthTransactions
                FROM Sales", con);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                stats["TotalSales"] = rdr.GetDecimal(0);
                stats["AverageSale"] = rdr.GetDecimal(1);
                stats["TotalTransactions"] = rdr.GetInt32(2);
                stats["ThisMonthSales"] = rdr.GetDecimal(3);
                stats["ThisMonthTransactions"] = rdr.GetInt32(4);
            }
            return stats;
        }

        // TOP PERFORMER
        public (string Name, decimal Amount) GetTopPerformer()
        {
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT TOP 1
                    e.FirstName + ' ' + e.LastName as EmployeeName,
                    SUM(s.Amount) as TotalSales
                FROM Sales s
                INNER JOIN Employees e ON s.EmployeeID = e.EmployeeID
                GROUP BY e.EmployeeID, e.FirstName, e.LastName
                ORDER BY TotalSales DESC", con);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return (rdr[0]?.ToString() ?? "N/A", rdr.GetDecimal(1));
            }
            return ("N/A", 0);
        }

        // TOP PERFORMERS LIST
        public List<TopPerformer_Model> GetTopPerformers(int count = 5)
        {
            var list = new List<TopPerformer_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand($@"
                SELECT TOP {count}
                    ROW_NUMBER() OVER (ORDER BY SUM(s.Amount) DESC) as Rank,
                    e.FirstName + ' ' + e.LastName as EmployeeName,
                    SUM(s.Amount) as TotalSales,
                    COUNT(*) as TransactionCount
                FROM Sales s
                INNER JOIN Employees e ON s.EmployeeID = e.EmployeeID
                GROUP BY e.EmployeeID, e.FirstName, e.LastName
                ORDER BY TotalSales DESC", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new TopPerformer_Model
                {
                    Rank = (int)rdr.GetInt64(0),
                    EmployeeName = rdr[1]?.ToString() ?? string.Empty,
                    TotalSales = rdr.GetDecimal(2),
                    TransactionCount = rdr.GetInt32(3)
                });
            }
            return list;
        }

        // DEPARTMENT PERFORMANCE
        public List<DepartmentPerformance_Model> GetDepartmentPerformance()
        {
            var list = new List<DepartmentPerformance_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT 
                    'Sales' as DepartmentName,
                    SUM(s.Amount) as TotalSales,
                    COUNT(*) as TransactionCount,
                    AVG(s.Amount) as AverageSale
                FROM Sales s
                UNION ALL
                SELECT 
                    d.DepartmentName,
                    0 as TotalSales,
                    0 as TransactionCount,
                    0 as AverageSale
                FROM Departments d
                WHERE d.DepartmentName != 'Sales'
                ORDER BY TotalSales DESC", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new DepartmentPerformance_Model
                {
                    DepartmentName = rdr[0]?.ToString() ?? string.Empty,
                    TotalSales = rdr.GetDecimal(1),
                    TransactionCount = rdr.GetInt32(2),
                    AverageSale = rdr.GetDecimal(3)
                });
            }
            return list;
        }

        // MONTHLY TRENDS (Last 6 months)
        public List<MonthlyTrend_Model> GetMonthlyTrends()
        {
            var list = new List<MonthlyTrend_Model>();
            using var con = new SqlConnection(_cs);
            con.Open();
            using var cmd = new SqlCommand(@"
                SELECT 
                    FORMAT(s.SaleDate, 'MMM yyyy') as Month,
                    SUM(s.Amount) as Amount,
                    COUNT(*) as TransactionCount
                FROM Sales s
                WHERE s.SaleDate >= DATEADD(MONTH, -6, GETDATE())
                GROUP BY YEAR(s.SaleDate), MONTH(s.SaleDate), FORMAT(s.SaleDate, 'MMM yyyy')
                ORDER BY YEAR(s.SaleDate), MONTH(s.SaleDate)", con);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new MonthlyTrend_Model
                {
                    Month = rdr[0]?.ToString() ?? string.Empty,
                    Amount = rdr.GetDecimal(1),
                    TransactionCount = rdr.GetInt32(2)
                });
            }
            return list;
        }
    }
}


