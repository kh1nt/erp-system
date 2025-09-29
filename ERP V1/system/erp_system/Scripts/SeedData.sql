-- Seed data for Avielle ERP system
USE avielle;
GO

-- Insert sample employees
INSERT INTO Employees (FirstName, LastName, Email, Phone, HireDate, Position, Status, DepartmentID) VALUES
('John', 'Doe', 'john.doe@company.com', '123-456-7890', '2020-01-15', 'Software Developer', 'Active', 2),
('Jane', 'Smith', 'jane.smith@company.com', '123-456-7891', '2019-03-20', 'HR Manager', 'Active', 1),
('Mike', 'Johnson', 'mike.johnson@company.com', '123-456-7892', '2021-06-10', 'Sales Manager', 'Active', 4),
('Sarah', 'Wilson', 'sarah.wilson@company.com', '123-456-7893', '2020-09-05', 'Accountant', 'Active', 3),
('David', 'Brown', 'david.brown@company.com', '123-456-7894', '2022-02-14', 'Marketing Specialist', 'Active', 5),
('Lisa', 'Davis', 'lisa.davis@company.com', '123-456-7895', '2021-11-30', 'Senior Developer', 'Active', 2),
('Tom', 'Miller', 'tom.miller@company.com', '123-456-7896', '2020-07-22', 'Finance Director', 'Active', 3),
('Amy', 'Garcia', 'amy.garcia@company.com', '123-456-7897', '2022-04-18', 'Sales Representative', 'Active', 4);

-- Insert sample users (passwords are hashed versions of 'password123')
INSERT INTO Users (UserName, PasswordHash, RoleName, EmployeeID) VALUES
('admin', 'jGl25bVBBBW96Qi9Te4D37+o7rwXpNAruT1Z2vtL1kE=', 'Admin', 1),
('jane.smith', 'jGl25bVBBBW96Qi9Te4D37+o7rwXpNAruT1Z2vtL1kE=', 'HR Manager', 2),
('mike.johnson', 'jGl25bVBBBW96Qi9Te4D37+o7rwXpNAruT1Z2vtL1kE=', 'Sales Manager', 3),
('sarah.wilson', 'jGl25bVBBBW96Qi9Te4D37+o7rwXpNAruT1Z2vtL1kE=', 'Accountant', 4),
('david.brown', 'jGl25bVBBBW96Qi9Te4D37+o7rwXpNAruT1Z2vtL1kE=', 'Marketing Specialist', 5),
('lisa.davis', 'jGl25bVBBBW96Qi9Te4D37+o7rwXpNAruT1Z2vtL1kE=', 'Developer', 6),
('tom.miller', 'jGl25bVBBBW96Qi9Te4D37+o7rwXpNAruT1Z2vtL1kE=', 'Finance Director', 7),
('amy.garcia', 'jGl25bVBBBW96Qi9Te4D37+o7rwXpNAruT1Z2vtL1kE=', 'Sales Representative', 8);

-- Insert sample leave requests
INSERT INTO Leave_Requests (StartDate, EndDate, RequestDate, ApprovedBy, EmployeeID, LeaveID) VALUES
('2024-01-15', '2024-01-17', '2024-01-10', 'jane.smith', 1, 1),
('2024-02-20', '2024-02-22', '2024-02-15', 'jane.smith', 2, 2),
('2024-03-10', '2024-03-12', '2024-03-05', 'jane.smith', 3, 1),
('2024-04-05', '2024-04-07', '2024-04-01', NULL, 4, 3),
('2024-05-15', '2024-05-17', '2024-05-10', NULL, 5, 1);

-- Insert sample performance records
INSERT INTO Performance_Records (ReviewDate, Score, Remarks, EmployeeID) VALUES
('2023-12-31', 85.5, 'Excellent performance throughout the year', 1),
('2023-12-31', 92.0, 'Outstanding leadership and team management', 2),
('2023-12-31', 88.5, 'Great sales results and customer relations', 3),
('2023-12-31', 90.0, 'Accurate financial reporting and analysis', 4),
('2023-12-31', 87.5, 'Creative marketing campaigns and strategies', 5),
('2023-12-31', 91.0, 'Excellent technical skills and code quality', 6),
('2023-12-31', 89.5, 'Strong financial planning and budgeting', 7),
('2023-12-31', 86.0, 'Good sales performance and client relations', 8);

-- Insert sample payroll records
INSERT INTO Payrolls (PeriodStart, PeriodEnd, BasicSalary, Bonuses, NetPay, Generated_Date, EmployeeID) VALUES
('2023-12-01', '2023-12-31', 5000.00, 500.00, 5500.00, '2024-01-05', 1),
('2023-12-01', '2023-12-31', 6000.00, 600.00, 6600.00, '2024-01-05', 2),
('2023-12-01', '2023-12-31', 5500.00, 800.00, 6300.00, '2024-01-05', 3),
('2023-12-01', '2023-12-31', 4500.00, 200.00, 4700.00, '2024-01-05', 4),
('2023-12-01', '2023-12-31', 4800.00, 300.00, 5100.00, '2024-01-05', 5),
('2023-12-01', '2023-12-31', 5200.00, 400.00, 5600.00, '2024-01-05', 6),
('2023-12-01', '2023-12-31', 7000.00, 500.00, 7500.00, '2024-01-05', 7),
('2023-12-01', '2023-12-31', 4000.00, 600.00, 4600.00, '2024-01-05', 8);

-- Insert sample sales records
INSERT INTO Sales (SaleDate, Amount, Description, EmployeeID) VALUES
('2023-12-15', 15000.00, 'Software License Sale', 3),
('2023-12-20', 8500.00, 'Consulting Services', 8),
('2023-12-22', 12000.00, 'Hardware Equipment', 3),
('2023-12-28', 9500.00, 'Training Services', 8),
('2024-01-05', 18000.00, 'Enterprise Software', 3),
('2024-01-10', 7500.00, 'Support Services', 8),
('2024-01-15', 11000.00, 'Cloud Services', 3),
('2024-01-20', 6500.00, 'Maintenance Contract', 8);

PRINT 'Seed data inserted successfully!';
GO
