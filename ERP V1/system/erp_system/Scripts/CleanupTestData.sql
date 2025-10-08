-- Clean up test data from Employees table
-- This script removes test employees with invalid data

PRINT 'Cleaning up test data...'

-- Show what will be deleted before actually deleting
PRINT 'Test employees that will be removed:'
SELECT EmployeeID, FirstName, LastName, Position, Status 
FROM Employees 
WHERE Position IN ('213123', '123123', 'Mid lane') 
   OR FirstName LIKE 'test%' 
   OR FirstName LIKE 'asd%'
   OR Email LIKE '%@aaa.%'
   OR Email LIKE '%@adazz'
   OR Phone = '123123'
   OR Phone = '12321312'
   OR Phone = '12221123';

-- Uncomment the following lines to actually delete the test data
-- DELETE FROM Employees 
-- WHERE Position IN ('213123', '123123', 'Mid lane') 
--    OR FirstName LIKE 'test%' 
--    OR FirstName LIKE 'asd%'
--    OR Email LIKE '%@aaa.%'
--    OR Email LIKE '%@adazz'
--    OR Phone = '123123'
--    OR Phone = '12321312'
--    OR Phone = '12221123';

-- PRINT 'Test data cleanup completed.'

-- Show remaining employees
PRINT 'Remaining employees after cleanup:'
SELECT COUNT(*) as RemainingEmployees FROM Employees;
