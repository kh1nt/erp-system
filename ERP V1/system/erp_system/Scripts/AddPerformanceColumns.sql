-- SQL Script to add SalesTarget and SalesAchieved columns to Performance_Records table
-- Run this script if you want to enable sales tracking functionality

-- Add SalesTarget column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Performance_Records]') AND name = 'SalesTarget')
BEGIN
    ALTER TABLE [dbo].[Performance_Records] 
    ADD [SalesTarget] DECIMAL(18,2) NULL DEFAULT 0
    PRINT 'SalesTarget column added successfully'
END
ELSE
BEGIN
    PRINT 'SalesTarget column already exists'
END

-- Add SalesAchieved column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Performance_Records]') AND name = 'SalesAchieved')
BEGIN
    ALTER TABLE [dbo].[Performance_Records] 
    ADD [SalesAchieved] DECIMAL(18,2) NULL DEFAULT 0
    PRINT 'SalesAchieved column added successfully'
END
ELSE
BEGIN
    PRINT 'SalesAchieved column already exists'
END

-- Add ReviewType column for additional functionality
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Performance_Records]') AND name = 'ReviewType')
BEGIN
    ALTER TABLE [dbo].[Performance_Records] 
    ADD [ReviewType] NVARCHAR(50) NULL DEFAULT 'Annual'
    PRINT 'ReviewType column added successfully'
END
ELSE
BEGIN
    PRINT 'ReviewType column already exists'
END

-- Add ReviewerName column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Performance_Records]') AND name = 'ReviewerName')
BEGIN
    ALTER TABLE [dbo].[Performance_Records] 
    ADD [ReviewerName] NVARCHAR(100) NULL
    PRINT 'ReviewerName column added successfully'
END
ELSE
BEGIN
    PRINT 'ReviewerName column already exists'
END

-- Add Goals column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Performance_Records]') AND name = 'Goals')
BEGIN
    ALTER TABLE [dbo].[Performance_Records] 
    ADD [Goals] NVARCHAR(MAX) NULL
    PRINT 'Goals column added successfully'
END
ELSE
BEGIN
    PRINT 'Goals column already exists'
END

-- Add Strengths column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Performance_Records]') AND name = 'Strengths')
BEGIN
    ALTER TABLE [dbo].[Performance_Records] 
    ADD [Strengths] NVARCHAR(MAX) NULL
    PRINT 'Strengths column added successfully'
END
ELSE
BEGIN
    PRINT 'Strengths column already exists'
END

-- Add AreasForImprovement column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Performance_Records]') AND name = 'AreasForImprovement')
BEGIN
    ALTER TABLE [dbo].[Performance_Records] 
    ADD [AreasForImprovement] NVARCHAR(MAX) NULL
    PRINT 'AreasForImprovement column added successfully'
END
ELSE
BEGIN
    PRINT 'AreasForImprovement column already exists'
END

PRINT 'Database schema update completed!'
