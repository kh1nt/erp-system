-- Update Employee Salaries based on their positions
-- This script sets proper salary values for existing employees

-- First, ensure the BasicSalary column exists (run AddEmployeeSalaryColumn.sql first if not done)
-- ALTER TABLE Employees ADD BasicSalary decimal(18,2) NOT NULL DEFAULT 0;

-- Update salaries based on actual positions in your database
UPDATE Employees 
SET BasicSalary = CASE 
    -- Directors and Senior Management
    WHEN Position LIKE '%Director%' OR Position LIKE '%Finance Director%' THEN 60000.00
    
    -- Managers
    WHEN Position LIKE '%Manager%' OR Position LIKE '%HR Manager%' THEN 50000.00
    
    -- Senior Developers and Specialists
    WHEN Position LIKE '%Senior%' OR Position LIKE '%Lead%' THEN 45000.00
    
    -- Mid-level positions
    WHEN Position LIKE '%Mid Level%' OR Position LIKE '%Mid Level Developer%' THEN 35000.00
    
    -- Software Developers
    WHEN Position LIKE '%Software Developer%' OR Position LIKE '%Developer%' THEN 30000.00
    
    -- Sales Representatives
    WHEN Position LIKE '%Sales Representative%' OR Position LIKE '%Sales%' THEN 28000.00
    
    -- Accountants and Finance
    WHEN Position LIKE '%Accountant%' OR Position LIKE '%Finance%' THEN 32000.00
    
    -- Marketing Specialists
    WHEN Position LIKE '%Marketing%' OR Position LIKE '%Marketing Specialist%' THEN 25000.00
    
    -- Junior positions
    WHEN Position LIKE '%Junior%' OR Position LIKE '%Associate%' THEN 20000.00
    
    -- Interns and Trainees
    WHEN Position LIKE '%Intern%' OR Position LIKE '%Trainee%' THEN 15000.00
    
    -- Default for any other positions
    ELSE 25000.00
END
WHERE BasicSalary = 0 OR BasicSalary IS NULL;

-- Clean up test data with invalid positions (optional - uncomment if you want to remove test data)
-- DELETE FROM Employees WHERE Position IN ('213123', '123123', 'Mid lane') OR FirstName LIKE 'test%';

-- Show updated employee data with salaries
SELECT EmployeeID, FirstName, LastName, Position, BasicSalary, Status 
FROM Employees 
ORDER BY BasicSalary DESC;
