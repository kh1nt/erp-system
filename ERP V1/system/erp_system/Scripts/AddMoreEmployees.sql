-- Add more employees to test pagination (need more than 14 to see page 2)
INSERT INTO Employees (FirstName, LastName, Email, Phone, HireDate, Position, Status, DepartmentID)
VALUES 
('Alex', 'Rodriguez', 'alex.rodriguez@company.com', '555-0401', '2023-07-10', 'Marketing Manager', 'Active', 1),
('Maria', 'Lopez', 'maria.lopez@company.com', '555-0402', '2023-07-15', 'Marketing Specialist', 'Active', 1),
('Carlos', 'Gonzalez', 'carlos.gonzalez@company.com', '555-0403', '2023-07-20', 'Marketing Specialist', 'Active', 1),
('Ana', 'Hernandez', 'ana.hernandez@company.com', '555-0404', '2023-07-25', 'Marketing Specialist', 'Active', 1),
('Jose', 'Perez', 'jose.perez@company.com', '555-0405', '2023-08-01', 'Marketing Specialist', 'Active', 1),
('Carmen', 'Sanchez', 'carmen.sanchez@company.com', '555-0406', '2023-08-05', 'Marketing Specialist', 'Active', 1),
('Luis', 'Ramirez', 'luis.ramirez@company.com', '555-0407', '2023-08-10', 'Marketing Specialist', 'Active', 1),
('Isabel', 'Torres', 'isabel.torres@company.com', '555-0408', '2023-08-15', 'Marketing Specialist', 'Active', 1);

-- Check total count
SELECT COUNT(*) as TotalEmployees FROM Employees;
