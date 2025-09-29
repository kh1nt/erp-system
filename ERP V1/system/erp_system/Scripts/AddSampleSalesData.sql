-- Add Sample Sales Data for Performance Tracking
-- This script adds sample sales data to test sales achievement calculations

-- First, let's add some sample sales data for employees
-- We'll add sales for different months to show trends

-- Sales for January 2024
INSERT INTO Sales (EmployeeID, SaleDate, Amount, Description)
VALUES 
    (1, '2024-01-15', 5000.00, 'Product A Sale'),
    (1, '2024-01-20', 3000.00, 'Product B Sale'),
    (2, '2024-01-10', 4000.00, 'Service Contract'),
    (2, '2024-01-25', 6000.00, 'Product C Sale'),
    (3, '2024-01-12', 3500.00, 'Product D Sale'),
    (3, '2024-01-28', 4500.00, 'Product E Sale');

-- Sales for February 2024
INSERT INTO Sales (EmployeeID, SaleDate, Amount, Description)
VALUES 
    (1, '2024-02-05', 7000.00, 'Product A Sale'),
    (1, '2024-02-18', 4000.00, 'Product B Sale'),
    (2, '2024-02-12', 5500.00, 'Service Contract'),
    (2, '2024-02-22', 8000.00, 'Product C Sale'),
    (3, '2024-02-08', 6000.00, 'Product D Sale'),
    (3, '2024-02-25', 5000.00, 'Product E Sale');

-- Sales for March 2024
INSERT INTO Sales (EmployeeID, SaleDate, Amount, Description)
VALUES 
    (1, '2024-03-10', 9000.00, 'Product A Sale'),
    (1, '2024-03-20', 5000.00, 'Product B Sale'),
    (2, '2024-03-15', 7000.00, 'Service Contract'),
    (2, '2024-03-28', 10000.00, 'Product C Sale'),
    (3, '2024-03-12', 8000.00, 'Product D Sale'),
    (3, '2024-03-25', 6000.00, 'Product E Sale');

-- Sales for April 2024
INSERT INTO Sales (EmployeeID, SaleDate, Amount, Description)
VALUES 
    (1, '2024-04-08', 12000.00, 'Product A Sale'),
    (1, '2024-04-18', 6000.00, 'Product B Sale'),
    (2, '2024-04-12', 9000.00, 'Service Contract'),
    (2, '2024-04-25', 12000.00, 'Product C Sale'),
    (3, '2024-04-10', 10000.00, 'Product D Sale'),
    (3, '2024-04-22', 8000.00, 'Product E Sale');

-- Sales for May 2024
INSERT INTO Sales (EmployeeID, SaleDate, Amount, Description)
VALUES 
    (1, '2024-05-05', 15000.00, 'Product A Sale'),
    (1, '2024-05-15', 8000.00, 'Product B Sale'),
    (2, '2024-05-10', 11000.00, 'Service Contract'),
    (2, '2024-05-20', 14000.00, 'Product C Sale'),
    (3, '2024-05-08', 12000.00, 'Product D Sale'),
    (3, '2024-05-25', 10000.00, 'Product E Sale');

-- Sales for June 2024
INSERT INTO Sales (EmployeeID, SaleDate, Amount, Description)
VALUES 
    (1, '2024-06-12', 18000.00, 'Product A Sale'),
    (1, '2024-06-22', 10000.00, 'Product B Sale'),
    (2, '2024-06-15', 13000.00, 'Service Contract'),
    (2, '2024-06-28', 16000.00, 'Product C Sale'),
    (3, '2024-06-18', 14000.00, 'Product D Sale'),
    (3, '2024-06-30', 12000.00, 'Product E Sale');

-- Add some sales for different employees to show department performance
-- Sales for HR employees (they shouldn't have sales targets)
INSERT INTO Sales (EmployeeID, SaleDate, Amount, Description)
VALUES 
    (4, '2024-01-15', 1000.00, 'HR Service'),
    (4, '2024-02-20', 1500.00, 'HR Service'),
    (5, '2024-01-25', 2000.00, 'HR Service'),
    (5, '2024-02-28', 1800.00, 'HR Service');

-- Sales for IT employees (they shouldn't have sales targets)
INSERT INTO Sales (EmployeeID, SaleDate, Amount, Description)
VALUES 
    (6, '2024-01-10', 3000.00, 'IT Service'),
    (6, '2024-02-15', 3500.00, 'IT Service'),
    (7, '2024-01-20', 4000.00, 'IT Service'),
    (7, '2024-02-25', 4500.00, 'IT Service');

PRINT 'Sample sales data added successfully!';
PRINT 'Total sales records added: ' + CAST(@@ROWCOUNT as VARCHAR(10));
