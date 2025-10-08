-- =============================================
-- Script: Remove Sales Columns from Performance Table
-- Description: Removes SalesTarget and SalesAchieved columns from the Performance table
-- Date: $(date)
-- =============================================

USE [avielle]
GO

-- Drop SalesTarget column and its default constraint
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Performance_Records' AND COLUMN_NAME = 'SalesTarget')
BEGIN
    PRINT 'Dropping SalesTarget column from Performance_Records table...'
    
    DECLARE @sql1 NVARCHAR(MAX)
    
    -- Find and drop the default constraint for SalesTarget
    SELECT @sql1 = 'ALTER TABLE [dbo].[Performance_Records] DROP CONSTRAINT ' + name
    FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('dbo.Performance_Records')
    AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('dbo.Performance_Records'), 'SalesTarget', 'ColumnId')
    
    IF @sql1 IS NOT NULL
    BEGIN
        EXEC sp_executesql @sql1
        PRINT 'Default constraint for SalesTarget dropped.'
    END
    
    -- Now drop the column
    ALTER TABLE [dbo].[Performance_Records] DROP COLUMN [SalesTarget]
    PRINT 'SalesTarget column dropped successfully.'
END
ELSE
BEGIN
    PRINT 'SalesTarget column does not exist in Performance_Records table.'
END
GO

-- Drop SalesAchieved column and its default constraint
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Performance_Records' AND COLUMN_NAME = 'SalesAchieved')
BEGIN
    PRINT 'Dropping SalesAchieved column from Performance_Records table...'
    
    DECLARE @sql2 NVARCHAR(MAX)
    
    -- Find and drop the default constraint for SalesAchieved
    SELECT @sql2 = 'ALTER TABLE [dbo].[Performance_Records] DROP CONSTRAINT ' + name
    FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('dbo.Performance_Records')
    AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('dbo.Performance_Records'), 'SalesAchieved', 'ColumnId')
    
    IF @sql2 IS NOT NULL
    BEGIN
        EXEC sp_executesql @sql2
        PRINT 'Default constraint for SalesAchieved dropped.'
    END
    
    -- Now drop the column
    ALTER TABLE [dbo].[Performance_Records] DROP COLUMN [SalesAchieved]
    PRINT 'SalesAchieved column dropped successfully.'
END
ELSE
BEGIN
    PRINT 'SalesAchieved column does not exist in Performance_Records table.'
END
GO

-- Verify the columns have been removed
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Performance_Records'
ORDER BY ORDINAL_POSITION

PRINT 'Sales columns removal completed successfully!'
PRINT 'Performance_Records table now contains only performance-related fields without sales data.'
