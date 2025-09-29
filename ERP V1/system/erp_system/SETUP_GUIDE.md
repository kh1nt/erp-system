# 🚀 ERP System Setup Guide

## 📋 **Complete Setup Instructions**

### **Step 1: Database Setup in SSMS**

1. **Open SQL Server Management Studio**
   - Connect to: `LAPTOP-E70PTJD4\SQLEXPRESS`

2. **Create Database**
   - Right-click server → "New Query"
   - Copy entire content from `Scripts/CreateDatabase.sql`
   - Execute (F5)

3. **Add Sample Data**
   - Clear query window
   - Copy entire content from `Scripts/SeedData.sql`
   - Execute (F5)

4. **Verify Setup**
   ```sql
   USE avielle;
   SELECT 'Departments' as Table, COUNT(*) as Records FROM Departments
   UNION ALL SELECT 'Employees', COUNT(*) FROM Employees
   UNION ALL SELECT 'Users', COUNT(*) FROM Users;
   ```

### **Step 2: Backend API Setup**

1. **Open Terminal in Project Directory**
   ```bash
   cd "ERP V1/system/erp_system"
   ```

2. **Restore and Build**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run Backend**
   ```bash
   dotnet run
   ```

4. **Test API**
   - Open browser: `https://localhost:5001/swagger`
   - Test authentication: `POST /api/users/authenticate`
   - Username: `admin`, Password: `password123`

### **Step 3: WPF Frontend Integration**

Your WPF application is now configured to use the backend API with fallback to direct database access.

**Key Features Added:**
- ✅ API client service for all backend operations
- ✅ Fallback to direct database queries if API unavailable
- ✅ Updated User_Repository to use both API and database
- ✅ Seamless integration with existing WPF views

### **Step 4: Testing the Integration**

1. **Start Backend API** (if not already running)
2. **Run WPF Application**
3. **Login with sample credentials:**
   - Username: `admin`
   - Password: `password123`

### **🔧 Configuration Options**

**API Endpoint Configuration:**
- Update `ApiClientService.cs` line 12:
  ```csharp
  _baseUrl = "https://localhost:5001/api"; // Change if needed
  ```

**Database Connection:**
- Update `appsettings.json` if using different SQL Server instance
- Update `Repository_Base.cs` connection string if needed

### **📊 Sample Data Available**

After setup, you'll have:
- **5 Departments**: HR, IT, Finance, Sales, Marketing
- **8 Employees**: With different roles and departments
- **8 Users**: With authentication (password: `password123`)
- **Sample Records**: Leave requests, payroll, sales, performance data

### **🔍 Troubleshooting**

**Backend Issues:**
- Check if port 5001 is available
- Verify SQL Server is running
- Check connection string in `appsettings.json`

**Database Issues:**
- Ensure SQL Server instance is running
- Check if `LAPTOP-E70PTJD4\SQLEXPRESS` is correct
- Verify database was created successfully

**WPF Integration Issues:**
- Check if backend API is running
- Verify API endpoint URL in `ApiClientService.cs`
- Check network connectivity between WPF and API

### **🎯 Next Steps**

1. **Test all functionality** in WPF application
2. **Customize API endpoints** as needed
3. **Add more business logic** to services
4. **Implement additional features** using the API

Your ERP system is now fully integrated with a modern backend API! 🎉
