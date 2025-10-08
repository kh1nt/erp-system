-- Complete Payroll System Setup Script
-- This script sets up the payroll system with proper employee salaries

PRINT 'Setting up Payroll System...'

-- Step 1: Add BasicSalary column to Employees table (if not exists)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'BasicSalary')
BEGIN
    PRINT 'Adding BasicSalary column to Employees table...'
    ALTER TABLE Employees ADD BasicSalary decimal(18,2) NOT NULL DEFAULT 0;
    PRINT 'BasicSalary column added successfully.'
END
ELSE
BEGIN
    PRINT 'BasicSalary column already exists.'
END

-- Step 2: Update employee salaries based on their positions
PRINT 'Updating employee salaries based on positions...'

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

PRINT 'Employee salaries updated successfully.'

-- Step 3: Show summary of updated employees
PRINT 'Employee Salary Summary:'
SELECT 
    COUNT(*) as TotalEmployees,
    COUNT(CASE WHEN Status = 'Active' THEN 1 END) as ActiveEmployees,
    AVG(BasicSalary) as AverageSalary,
    MIN(BasicSalary) as MinSalary,
    MAX(BasicSalary) as MaxSalary
FROM Employees;

-- Step 4: Show detailed employee list with salaries
PRINT 'Employee Details with Salaries:'
SELECT 
    EmployeeID, 
    FirstName + ' ' + LastName as FullName,
    Position, 
    BasicSalary,
    Status,
    DepartmentID
FROM Employees 
ORDER BY BasicSalary DESC;

PRINT 'Payroll System setup completed successfully!'
PRINT 'You can now use the Generate Payroll feature in the application.'
