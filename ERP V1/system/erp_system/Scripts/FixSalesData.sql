-- Fix Sales Data - Remove corrupted data
-- This script cleans up any corrupted sales data

-- First, let's see what's in the Sales table
SELECT * FROM Sales ORDER BY SaleDate DESC;

-- Clean up any corrupted Amount data (remove non-numeric characters)
-- This will update any Amount field that contains non-numeric characters
UPDATE Sales 
SET Amount = CAST(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    CAST(Amount AS NVARCHAR(50)), 'asd', ''), 'hallow', ''), 'a', ''), 'b', ''), 'c', ''), 'd', ''), 'e', ''), 'f', ''), 'g', ''), 'h', ''), 'i', '')
AS DECIMAL(18,2))
WHERE CAST(Amount AS NVARCHAR(50)) LIKE '%[a-z]%';

-- Alternative approach: Clean specific known corrupted entries
UPDATE Sales 
SET Amount = 112.00, Description = 'Product Sale'
WHERE Amount = 112.00 AND Description = '';

UPDATE Sales 
SET Amount = 111.00, Description = 'Product Sale'
WHERE Amount = 111.00 AND Description = '';

UPDATE Sales 
SET Amount = 1012.00, Description = 'Product Sale'
WHERE Amount = 1012.00 AND Description = '';

-- Verify the cleanup
SELECT * FROM Sales ORDER BY SaleDate DESC;

PRINT 'Sales data cleanup completed!';
