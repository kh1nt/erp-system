-- Add Reason column to Leave_Requests table
USE avielle;
GO

-- Check if the Reason column already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Leave_Requests' AND COLUMN_NAME = 'Reason')
BEGIN
    -- Add the Reason column
    ALTER TABLE Leave_Requests 
    ADD Reason NVARCHAR(500) NULL;
    
    PRINT 'Reason column added successfully to Leave_Requests table.';
END
ELSE
BEGIN
    PRINT 'Reason column already exists in Leave_Requests table.';
END
GO
