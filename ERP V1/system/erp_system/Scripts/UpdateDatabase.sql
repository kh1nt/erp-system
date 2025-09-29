-- Update existing database to match new schema
USE avielle;
GO

-- Add missing columns to existing tables
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Leave_Requests') AND name = 'Status')
BEGIN
    ALTER TABLE Leave_Requests ADD Status nvarchar(20) NOT NULL DEFAULT 'Pending';
    PRINT 'Added Status column to Leave_Requests table';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Leaves') AND name = 'Description')
BEGIN
    ALTER TABLE Leaves ADD Description nvarchar(500) NULL;
    PRINT 'Added Description column to Leaves table';
END
GO

-- Rename MaxDayPerYear to MaxDays in Leaves table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Leaves') AND name = 'MaxDayPerYear')
BEGIN
    EXEC sp_rename 'Leaves.MaxDayPerYear', 'MaxDays', 'COLUMN';
    PRINT 'Renamed MaxDayPerYear to MaxDays in Leaves table';
END
GO

-- Update existing leave types with descriptions
UPDATE Leaves SET Description = 'Annual vacation leave' WHERE TypeName = 'Annual Leave';
UPDATE Leaves SET Description = 'Medical and health-related leave' WHERE TypeName = 'Sick Leave';
UPDATE Leaves SET Description = 'Personal and emergency leave' WHERE TypeName = 'Personal Leave';
UPDATE Leaves SET Description = 'Maternity leave for new mothers' WHERE TypeName = 'Maternity Leave';
UPDATE Leaves SET Description = 'Paternity leave for new fathers' WHERE TypeName = 'Paternity Leave';

PRINT 'Database update completed successfully!';
GO
