# Database Setup Instructions

## Quick Setup

To fix the Employee Management error, you need to set up the database. Follow these steps:

### Option 1: Using SQL Server Management Studio (SSMS)

1. Open SQL Server Management Studio
2. Connect to your SQL Server instance (usually `DESKTOP-I5DCKI6\SQLEXPRESS`)
3. Open the `CreateDatabase.sql` file from the `Scripts` folder
4. Execute the entire script to create the database and tables
5. Open the `SeedData.sql` file from the `Scripts` folder
6. Execute the entire script to add sample data

### Option 2: Using Command Line (sqlcmd)

1. Open Command Prompt as Administrator
2. Navigate to the Scripts folder:
   ```cmd
   cd "C:\Users\khint\source\repos\erp-system\ERP V1\system\erp_system\Scripts"
   ```
3. Run the database creation script:
   ```cmd
   sqlcmd -S DESKTOP-I5DCKI6\SQLEXPRESS -E -i CreateDatabase.sql
   ```
4. Run the seed data script:
   ```cmd
   sqlcmd -S DESKTOP-I5DCKI6\SQLEXPRESS -E -i SeedData.sql
   ```

### Option 3: Using Visual Studio

1. Open Visual Studio
2. Go to View → SQL Server Object Explorer
3. Connect to your SQL Server instance
4. Right-click on your server → New Query
5. Copy and paste the contents of `CreateDatabase.sql` and execute
6. Copy and paste the contents of `SeedData.sql` and execute

## Verification

After running the scripts, you should have:
- A database named `avielle`
- Tables: Departments, Employees, Users, Leaves, Leave_Requests, Performance_Records, Payrolls, Sales
- Sample data in all tables

## Troubleshooting

If you still get errors:

1. **Check SQL Server is running**: Open Services and ensure SQL Server (SQLEXPRESS) is running
2. **Check connection string**: The connection string in `appsettings.json` should match your SQL Server instance
3. **Check permissions**: Ensure your Windows account has access to create databases
4. **Check firewall**: Ensure SQL Server port (usually 1433) is not blocked

## Alternative: Use Sample Data

If you can't set up the database, the application will now automatically use sample data when the database connection fails, so Employee Management should work with demo data.

## Connection String

The current connection string is:
```
Data Source=DESKTOP-I5DCKI6\SQLEXPRESS;Initial Catalog=avielle;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False
```

If your SQL Server instance has a different name, update the `appsettings.json` file accordingly.
