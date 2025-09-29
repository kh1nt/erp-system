-- Quick Performance Data Setup Script
-- This script adds minimal performance data to get your charts working

USE avielle;
GO

-- Add missing columns if they don't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Performance_Records]') AND name = 'SalesTarget')
BEGIN
    ALTER TABLE [dbo].[Performance_Records] 
    ADD [SalesTarget] DECIMAL(18,2) NULL DEFAULT 0
    PRINT 'SalesTarget column added'
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Performance_Records]') AND name = 'SalesAchieved')
BEGIN
    ALTER TABLE [dbo].[Performance_Records] 
    ADD [SalesAchieved] DECIMAL(18,2) NULL DEFAULT 0
    PRINT 'SalesAchieved column added'
END
GO

-- Clear existing performance records
DELETE FROM Performance_Records;

-- Insert sample performance data for the last 6 months
-- Note: SalesTarget and SalesAchieved will be 0 since columns were just added
-- This creates a realistic distribution and trend for your charts

-- July 2024
INSERT INTO Performance_Records (ReviewDate, Score, Remarks, EmployeeID) VALUES
('2024-07-31', 4.2, 'Good summer performance', 1),
('2024-07-31', 4.5, 'Excellent team management', 2),
('2024-07-31', 4.8, 'Outstanding sales results', 3),
('2024-07-31', 4.1, 'Accurate financial reporting', 4),
('2024-07-31', 3.9, 'Good marketing campaigns', 5),
('2024-07-31', 4.6, 'Excellent technical work', 6),
('2024-07-31', 4.3, 'Strong financial planning', 7),
('2024-07-31', 4.0, 'Met sales targets', 8);

-- August 2024
INSERT INTO Performance_Records (ReviewDate, Score, Remarks, EmployeeID) VALUES
('2024-08-31', 4.3, 'Continued strong performance', 1),
('2024-08-31', 4.4, 'Effective HR programs', 2),
('2024-08-31', 4.7, 'Great sales growth', 3),
('2024-08-31', 4.2, 'Timely month-end closing', 4),
('2024-08-31', 4.0, 'Improved marketing metrics', 5),
('2024-08-31', 4.7, 'Advanced technical solutions', 6),
('2024-08-31', 4.4, 'Excellent budget forecasting', 7),
('2024-08-31', 3.8, 'Below target sales', 8);

-- September 2024
INSERT INTO Performance_Records (ReviewDate, Score, Remarks, EmployeeID) VALUES
('2024-09-30', 4.1, 'Good Q3 performance', 1),
('2024-09-30', 4.6, 'Successful recruitment', 2),
('2024-09-30', 4.9, 'Outstanding Q3 results', 3),
('2024-09-30', 4.3, 'Comprehensive analysis', 4),
('2024-09-30', 4.2, 'Creative strategies', 5),
('2024-09-30', 4.5, 'System improvements', 6),
('2024-09-30', 4.5, 'Strategic planning', 7),
('2024-09-30', 4.1, 'Sales recovery', 8);

-- October 2024
INSERT INTO Performance_Records (ReviewDate, Score, Remarks, EmployeeID) VALUES
('2024-10-31', 4.4, 'Excellent Q4 start', 1),
('2024-10-31', 4.3, 'Effective management', 2),
('2024-10-31', 4.7, 'Strong client acquisitions', 3),
('2024-10-31', 4.1, 'Tax preparation', 4),
('2024-10-31', 4.3, 'Brand awareness campaigns', 5),
('2024-10-31', 4.6, 'Technical automation', 6),
('2024-10-31', 4.2, 'Cost reduction', 7),
('2024-10-31', 3.9, 'Market challenges', 8);

-- November 2024
INSERT INTO Performance_Records (ReviewDate, Score, Remarks, EmployeeID) VALUES
('2024-11-30', 4.2, 'Consistent quality', 1),
('2024-11-30', 4.5, 'Employee development', 2),
('2024-11-30', 4.8, 'Record sales month', 3),
('2024-11-30', 4.4, 'Financial reporting', 4),
('2024-11-30', 4.1, 'Digital marketing', 5),
('2024-11-30', 4.7, 'Security implementations', 6),
('2024-11-30', 4.6, 'Financial forecasting', 7),
('2024-11-30', 4.2, 'Improved techniques', 8);

-- December 2024 (Most Recent)
INSERT INTO Performance_Records (ReviewDate, Score, Remarks, EmployeeID) VALUES
('2024-12-31', 4.5, 'Outstanding year-end', 1),
('2024-12-31', 4.7, 'Exceptional leadership', 2),
('2024-12-31', 4.9, 'Record-breaking year', 3),
('2024-12-31', 4.6, 'Year-end analysis', 4),
('2024-12-31', 4.7, 'Successful campaigns', 5),
('2024-12-31', 4.8, 'Technical innovations', 6),
('2024-12-31', 4.8, 'Growth initiatives', 7),
('2024-12-31', 4.6, 'Client satisfaction', 8);

-- Update sales data for sales employees (EmployeeID 3 and 8)
UPDATE Performance_Records 
SET SalesTarget = CASE 
    WHEN EmployeeID = 3 AND ReviewDate = '2024-07-31' THEN 50000
    WHEN EmployeeID = 3 AND ReviewDate = '2024-08-31' THEN 50000
    WHEN EmployeeID = 3 AND ReviewDate = '2024-09-30' THEN 50000
    WHEN EmployeeID = 3 AND ReviewDate = '2024-10-31' THEN 55000
    WHEN EmployeeID = 3 AND ReviewDate = '2024-11-30' THEN 55000
    WHEN EmployeeID = 3 AND ReviewDate = '2024-12-31' THEN 55000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-07-31' THEN 30000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-08-31' THEN 30000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-09-30' THEN 30000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-10-31' THEN 30000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-11-30' THEN 30000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-12-31' THEN 30000
    ELSE 0
END,
SalesAchieved = CASE 
    WHEN EmployeeID = 3 AND ReviewDate = '2024-07-31' THEN 60000
    WHEN EmployeeID = 3 AND ReviewDate = '2024-08-31' THEN 55000
    WHEN EmployeeID = 3 AND ReviewDate = '2024-09-30' THEN 62000
    WHEN EmployeeID = 3 AND ReviewDate = '2024-10-31' THEN 58000
    WHEN EmployeeID = 3 AND ReviewDate = '2024-11-30' THEN 66000
    WHEN EmployeeID = 3 AND ReviewDate = '2024-12-31' THEN 78000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-07-31' THEN 30000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-08-31' THEN 24000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-09-30' THEN 31500
    WHEN EmployeeID = 8 AND ReviewDate = '2024-10-31' THEN 27000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-11-30' THEN 33000
    WHEN EmployeeID = 8 AND ReviewDate = '2024-12-31' THEN 37500
    ELSE 0
END
WHERE EmployeeID IN (3, 8);

PRINT 'Quick performance data setup completed!';
GO
