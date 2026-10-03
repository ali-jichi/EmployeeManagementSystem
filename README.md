# Employee Management System

## Complete Windows Forms Application (C# .NET Framework 4.8)

### System Overview
A comprehensive Employee Database Management System with role-based access control, full CRUD operations, attendance tracking, leave management, and report generation.

### Architecture
```
EmployeeManagementSystem/
├── DAL/              # Data Access Layer (ADO.NET)
├── BLL/              # Business Logic Layer
├── Models/           # Data Transfer Objects
├── UI/Forms/         # Windows Forms
├── Utilities/        # Helper Classes
└── Database/         # SQL Scripts
```

### Prerequisites
- Windows 10/11
- Visual Studio 2022 (Community Edition or higher)
- SQL Server 2016+ or SQL Server Express
- .NET Framework 4.8

### Default Login Credentials
- **Admin**: Username: admin, Password: admin123
- **HR**: Username: hruser, Password: hr123

### Database Setup
1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Open Database/01_CreateDatabase.sql
4. Execute the script (F5)
5. Verify database EmployeeManagementDB is created

### Project Setup in Visual Studio
1. Open Visual Studio 2022
2. Create New Project -> Windows Forms App (.NET Framework)
3. Name: EmployeeManagementSystem
4. Target Framework: .NET Framework 4.8
5. Add all source files from the provided structure
6. Update App.config connection string with your SQL Server details
7. Build and Run (F5)

### Features
- Authentication: Secure login with SHA256 password hashing
- Role-Based Access: Admin (full) vs HR (limited) permissions
- Employee CRUD: Add, edit, delete, view employees
- Department Management: Create departments, assign managers
- Attendance Tracking: Daily attendance with check-in/out times
- Leave Management: Apply, approve, reject leave requests
- Reports: Employee list, department, attendance, salary reports
- Search: Advanced search by name, ID, department, status
- Export: CSV export for all reports
- Security: Audit logging, parameterized queries, input validation

### License
This project is provided as-is for educational and development purposes.

### Author
Ali Jichi
