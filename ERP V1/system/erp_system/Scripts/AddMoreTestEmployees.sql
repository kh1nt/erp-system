-- Add more test employees to demonstrate pagination
-- This script adds 20 more employees to test the 14-per-page pagination

INSERT INTO Employees (FirstName, LastName, Email, Phone, HireDate, Position, Status, DepartmentID)
VALUES 
-- Sales Department (DepartmentID = 1)
('John', 'Smith', 'john.smith@company.com', '555-0101', '2023-01-15', 'Sales Manager', 'Active', 1),
('Sarah', 'Johnson', 'sarah.johnson@company.com', '555-0102', '2023-02-20', 'Sales Representative', 'Active', 1),
('Mike', 'Brown', 'mike.brown@company.com', '555-0103', '2023-03-10', 'Sales Representative', 'Active', 1),
('Lisa', 'Davis', 'lisa.davis@company.com', '555-0104', '2023-04-05', 'Sales Representative', 'Active', 1),
('David', 'Wilson', 'david.wilson@company.com', '555-0105', '2023-05-12', 'Sales Representative', 'Active', 1),

-- Human Resources Department (DepartmentID = 2)
('Jennifer', 'Miller', 'jennifer.miller@company.com', '555-0201', '2023-01-20', 'HR Manager', 'Active', 2),
('Robert', 'Garcia', 'robert.garcia@company.com', '555-0202', '2023-02-15', 'HR Specialist', 'Active', 2),
('Emily', 'Martinez', 'emily.martinez@company.com', '555-0203', '2023-03-25', 'HR Specialist', 'Active', 2),
('Christopher', 'Anderson', 'christopher.anderson@company.com', '555-0204', '2023-04-10', 'HR Specialist', 'Active', 2),
('Jessica', 'Taylor', 'jessica.taylor@company.com', '555-0205', '2023-05-18', 'HR Specialist', 'Active', 2),

-- Information Technology Department (DepartmentID = 3)
('Daniel', 'Thomas', 'daniel.thomas@company.com', '555-0301', '2023-01-10', 'Senior Developer', 'Active', 3),
('Ashley', 'Jackson', 'ashley.jackson@company.com', '555-0302', '2023-02-08', 'Developer', 'Active', 3),
('Matthew', 'White', 'matthew.white@company.com', '555-0303', '2023-03-15', 'Developer', 'Active', 3),
('Amanda', 'Harris', 'amanda.harris@company.com', '555-0304', '2023-04-22', 'Developer', 'Active', 3),
('James', 'Martin', 'james.martin@company.com', '555-0305', '2023-05-30', 'Developer', 'Active', 3),
('Michelle', 'Thompson', 'michelle.thompson@company.com', '555-0306', '2023-06-05', 'QA Engineer', 'Active', 3),
('Kevin', 'Garcia', 'kevin.garcia@company.com', '555-0307', '2023-06-12', 'QA Engineer', 'Active', 3),
('Nicole', 'Martinez', 'nicole.martinez@company.com', '555-0308', '2023-06-18', 'DevOps Engineer', 'Active', 3),
('Ryan', 'Robinson', 'ryan.robinson@company.com', '555-0309', '2023-06-25', 'DevOps Engineer', 'Active', 3),
('Stephanie', 'Clark', 'stephanie.clark@company.com', '555-0310', '2023-07-01', 'System Administrator', 'Active', 3);

-- Verify the count
SELECT COUNT(*) as TotalEmployees FROM Employees;
