# Step-by-Step Setup Instructions

## Step 1: Database Installation

### Option A: Using SQL Server Management Studio (SSMS)
1. Install SQL Server and SSMS if not already installed
2. Open SSMS and connect to your server (e.g., localhost or .\SQLEXPRESS)
3. Open the file: Database/01_CreateDatabase.sql
4. Press F5 or click Execute to run the script
5. Verify the database EmployeeManagementDB appears in Object Explorer

### Option B: Using Command Line (sqlcmd)
```cmd
sqlcmd -S localhost -i "Database\01_CreateDatabase.sql"
```

## Step 2: Visual Studio Project Setup

1. Open Visual Studio 2022
   - File -> New -> Project
   - Select "Windows Forms App (.NET Framework)"
   - Name: EmployeeManagementSystem
   - Framework: .NET Framework 4.8
   - Click Create

2. Install Required NuGet Package
   - Tools -> NuGet Package Manager -> Package Manager Console
   - Run: Install-Package System.Data.SqlClient

3. Add Project Folders
   - In Solution Explorer, create folders:
     - DAL
     - BLL
     - Models
     - UI/Forms
     - Utilities

4. Add Source Files
   - Copy all .cs files to their respective folders
   - Ensure Program.cs is in the root
   - Add App.config to the root

5. Configure Database Connection
   - Open App.config
   - Update connection string:
   ```xml
   <connectionStrings>
     <add name="EmployeeDB" 
          connectionString="Server=YOUR_SERVER;Database=EmployeeManagementDB;Integrated Security=True;TrustServerCertificate=True;" 
          providerName="System.Data.SqlClient"/>
   </connectionStrings>
   ```
   - Replace YOUR_SERVER with your SQL Server instance name

6. Build the Project
   - Build -> Build Solution (Ctrl+Shift+B)
   - Fix any missing references

7. Run the Application
   - Press F5 to start debugging
   - Login with default credentials

## Step 3: Troubleshooting

### Common Issues:

1. Connection Error
   - Verify SQL Server is running
   - Check connection string in App.config
   - Ensure Windows Authentication is enabled
   - For SQL Authentication, add User ID and Password to connection string

2. Missing References
   - Right-click References -> Add Reference
   - Add System.Configuration

3. Build Errors
   - Clean solution: Build -> Clean Solution
   - Rebuild: Build -> Rebuild Solution
   - Ensure all files are included in the project

4. Database Not Found
   - Verify script executed successfully
   - Check database exists in SSMS
   - Ensure user has permissions to access database

## Step 4: Post-Installation

1. Change Default Passwords
   - Login as admin
   - Navigate to User Management
   - Update default passwords immediately

2. Add Your Company Departments
   - Go to Department Management
   - Add relevant departments

3. Add Employees
   - Use Employee Management form
   - Fill in all required fields

4. Backup Schedule
   - Set up regular database backups
   - Use SQL Server Agent for automated backups

## Support
For issues or questions, refer to the project documentation or contact your system administrator.
