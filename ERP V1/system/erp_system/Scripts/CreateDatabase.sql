-- Create Avielle ERP Database
USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'avielle')
BEGIN
    ALTER DATABASE avielle SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE avielle;
END
GO

-- Create database
CREATE DATABASE avielle;
GO

USE avielle;
GO

-- Create Departments table
CREATE TABLE Departments (
    DepartmentID int IDENTITY(1,1) PRIMARY KEY,
    DepartmentName nvarchar(100) NOT NULL,
    Description nvarchar(500) NULL
);

-- Create Employees table
CREATE TABLE Employees (
    EmployeeID int IDENTITY(1,1) PRIMARY KEY,
    FirstName nvarchar(50) NOT NULL,
    LastName nvarchar(50) NOT NULL,
    Email nvarchar(100) NOT NULL UNIQUE,
    Phone nvarchar(20) NULL,
    HireDate datetime2 NOT NULL,
    Position nvarchar(100) NOT NULL,
    Status nvarchar(20) NOT NULL DEFAULT 'Active',
    DepartmentID int NOT NULL,
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);

-- Create Users table
CREATE TABLE Users (
    UserID int IDENTITY(1,1) PRIMARY KEY,
    UserName nvarchar(50) NOT NULL UNIQUE,
    PasswordHash nvarchar(255) NOT NULL,
    RoleName nvarchar(50) NOT NULL,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    EmployeeID int NOT NULL,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID) ON DELETE CASCADE
);

-- Create Leave types table
CREATE TABLE Leaves (
    LeaveID int IDENTITY(1,1) PRIMARY KEY,
    TypeName nvarchar(50) NOT NULL,
    MaxDays int NOT NULL,
    Description nvarchar(500) NULL
);

-- Create Leave Requests table
CREATE TABLE Leave_Requests (
    LeaveRequestID int IDENTITY(1,1) PRIMARY KEY,
    StartDate datetime2 NOT NULL,
    EndDate datetime2 NOT NULL,
    RequestDate datetime2 NOT NULL DEFAULT GETUTCDATE(),
    Status nvarchar(20) NOT NULL DEFAULT 'Pending',
    ApprovedBy nvarchar(50) NULL,
    EmployeeID int NOT NULL,
    LeaveID int NOT NULL,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID) ON DELETE CASCADE,
    FOREIGN KEY (LeaveID) REFERENCES Leaves(LeaveID)
);

-- Create Performance Records table
CREATE TABLE Performance_Records (
    PerformanceRecordID int IDENTITY(1,1) PRIMARY KEY,
    ReviewDate datetime2 NOT NULL,
    Score decimal(5,2) NOT NULL CHECK (Score >= 0 AND Score <= 100),
    Remarks nvarchar(1000) NULL,
    EmployeeID int NOT NULL,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID) ON DELETE CASCADE
);

-- Create Payrolls table
CREATE TABLE Payrolls (
    PayrollID int IDENTITY(1,1) PRIMARY KEY,
    PeriodStart datetime2 NOT NULL,
    PeriodEnd datetime2 NOT NULL,
    BasicSalary decimal(18,2) NOT NULL,
    Bonuses decimal(18,2) NOT NULL DEFAULT 0,
    NetPay decimal(18,2) NOT NULL,
    Generated_Date datetime2 NOT NULL DEFAULT GETUTCDATE(),
    EmployeeID int NOT NULL,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID) ON DELETE CASCADE
);

-- Create Sales table
CREATE TABLE Sales (
    SalesID int IDENTITY(1,1) PRIMARY KEY,
    SaleDate datetime2 NOT NULL,
    Amount decimal(18,2) NOT NULL,
    Description nvarchar(500) NULL,
    EmployeeID int NOT NULL,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID) ON DELETE CASCADE
);

-- Insert seed data for Departments
INSERT INTO Departments (DepartmentName, Description) VALUES
('Human Resources', 'HR Department'),
('Information Technology', 'IT Department'),
('Finance', 'Finance Department'),
('Sales', 'Sales Department'),
('Marketing', 'Marketing Department');

-- Insert seed data for Leave types
INSERT INTO Leaves (TypeName, MaxDays, Description) VALUES
('Annual Leave', 21, 'Annual vacation leave'),
('Sick Leave', 10, 'Medical and health-related leave'),
('Personal Leave', 5, 'Personal and emergency leave'),
('Maternity Leave', 90, 'Maternity leave for new mothers'),
('Paternity Leave', 14, 'Paternity leave for new fathers');

-- Create indexes for better performance
CREATE INDEX IX_Employees_DepartmentID ON Employees(DepartmentID);
CREATE INDEX IX_Employees_Email ON Employees(Email);
CREATE INDEX IX_Users_UserName ON Users(UserName);
CREATE INDEX IX_Users_EmployeeID ON Users(EmployeeID);
CREATE INDEX IX_Leave_Requests_EmployeeID ON Leave_Requests(EmployeeID);
CREATE INDEX IX_Leave_Requests_LeaveID ON Leave_Requests(LeaveID);
CREATE INDEX IX_Performance_Records_EmployeeID ON Performance_Records(EmployeeID);
CREATE INDEX IX_Payrolls_EmployeeID ON Payrolls(EmployeeID);
CREATE INDEX IX_Sales_EmployeeID ON Sales(EmployeeID);
CREATE INDEX IX_Sales_SaleDate ON Sales(SaleDate);

PRINT 'Database avielle created successfully with all tables and seed data!';
GO
