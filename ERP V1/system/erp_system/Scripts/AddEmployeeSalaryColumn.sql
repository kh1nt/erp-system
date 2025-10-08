-- Add BasicSalary column to Employees table
-- This script adds a salary field to the Employees table for proper payroll calculations

-- Add BasicSalary column to Employees table
ALTER TABLE Employees 
ADD BasicSalary decimal(18,2) NOT NULL DEFAULT 0;

-- Update existing employees with position-based salaries
UPDATE Employees 
SET BasicSalary = CASE 
    WHEN Position LIKE '%Manager%' OR Position LIKE '%Director%' THEN 50000.00
    WHEN Position LIKE '%Supervisor%' OR Position LIKE '%Lead%' THEN 35000.00
    WHEN Position LIKE '%Senior%' THEN 25000.00
    WHEN Position LIKE '%Junior%' OR Position LIKE '%Associate%' THEN 20000.00
    WHEN Position LIKE '%Intern%' OR Position LIKE '%Trainee%' THEN 15000.00
    ELSE 18000.00
END;

-- Update the default value to be more reasonable
ALTER TABLE Employees 
ALTER COLUMN BasicSalary decimal(18,2) NOT NULL;
