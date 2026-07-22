-- =====================================================
-- Employee Management System - Database Script
-- SQL Server 2016+ Compatible
-- =====================================================

-- Create Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'EmployeeManagementDB')
BEGIN
    CREATE DATABASE EmployeeManagementDB;
END
GO

USE EmployeeManagementDB;
GO

-- =====================================================
-- 1. USERS TABLE (Authentication)
-- =====================================================
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'HR')),
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    LastLoginDate DATETIME NULL
);

-- =====================================================
-- 2. DEPARTMENTS TABLE
-- =====================================================
IF OBJECT_ID('dbo.Departments', 'U') IS NOT NULL DROP TABLE dbo.Departments;

CREATE TABLE Departments (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    ManagerID INT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

-- =====================================================
-- 3. EMPLOYEES TABLE
-- =====================================================
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL DROP TABLE dbo.Employees;

CREATE TABLE Employees (
    EmployeeID INT IDENTITY(1000,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    FullName AS (FirstName + ' ' + LastName) PERSISTED,
    DateOfBirth DATE NULL,
    Gender NVARCHAR(10) NULL CHECK (Gender IN ('Male', 'Female', 'Other')),
    Address NVARCHAR(255),
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    DepartmentID INT NULL,
    JobTitle NVARCHAR(100),
    DateOfJoining DATE NOT NULL,
    Salary DECIMAL(18,2) NOT NULL DEFAULT 0,
    EmploymentStatus NVARCHAR(20) DEFAULT 'Active' CHECK (EmploymentStatus IN ('Active', 'Inactive', 'Terminated', 'OnLeave')),
    EmergencyContact NVARCHAR(100),
    EmergencyPhone NVARCHAR(20),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE(),
    CreatedBy NVARCHAR(50) DEFAULT SYSTEM_USER,

    CONSTRAINT FK_Employees_Departments 
        FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID) ON DELETE SET NULL,
    CONSTRAINT CHK_Salary CHECK (Salary >= 0),
    CONSTRAINT CHK_DateOfJoining CHECK (DateOfJoining <= GETDATE())
);

-- Add foreign key for department manager
ALTER TABLE Departments ADD CONSTRAINT FK_Departments_Manager 
    FOREIGN KEY (ManagerID) REFERENCES Employees(EmployeeID);

-- =====================================================
-- 4. ATTENDANCE TABLE
-- =====================================================
IF OBJECT_ID('dbo.Attendance', 'U') IS NOT NULL DROP TABLE dbo.Attendance;

CREATE TABLE Attendance (
    AttendanceID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT NOT NULL,
    AttendanceDate DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Present', 'Absent', 'Leave', 'HalfDay', 'Holiday', 'Weekend')),
    CheckInTime TIME NULL,
    CheckOutTime TIME NULL,
    WorkHours AS (DATEDIFF(MINUTE, CheckInTime, CheckOutTime) / 60.0),
    Notes NVARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy NVARCHAR(50) DEFAULT SYSTEM_USER,

    CONSTRAINT FK_Attendance_Employees 
        FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID) ON DELETE CASCADE,
    CONSTRAINT UQ_Attendance_Employee_Date UNIQUE (EmployeeID, AttendanceDate)
);

-- =====================================================
-- 5. LEAVE RECORDS TABLE
-- =====================================================
IF OBJECT_ID('dbo.LeaveRecords', 'U') IS NOT NULL DROP TABLE dbo.LeaveRecords;

CREATE TABLE LeaveRecords (
    LeaveID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT NOT NULL,
    LeaveType NVARCHAR(50) NOT NULL CHECK (LeaveType IN ('Annual', 'Sick', 'Maternity', 'Paternity', 'Unpaid', 'Emergency')),
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    TotalDays AS (DATEDIFF(DAY, StartDate, EndDate) + 1),
    Reason NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Approved', 'Rejected', 'Cancelled')),
    ApprovedBy NVARCHAR(50),
    ApprovalDate DATETIME NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Leave_Employees 
        FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID) ON DELETE CASCADE,
    CONSTRAINT CHK_LeaveDates CHECK (EndDate >= StartDate)
);

-- =====================================================
-- 6. AUDIT LOG TABLE (Security)
-- =====================================================
IF OBJECT_ID('dbo.AuditLog', 'U') IS NOT NULL DROP TABLE dbo.AuditLog;

CREATE TABLE AuditLog (
    AuditID INT IDENTITY(1,1) PRIMARY KEY,
    TableName NVARCHAR(50) NOT NULL,
    RecordID NVARCHAR(50) NOT NULL,
    Action NVARCHAR(20) NOT NULL CHECK (Action IN ('INSERT', 'UPDATE', 'DELETE')),
    OldValues NVARCHAR(MAX),
    NewValues NVARCHAR(MAX),
    UserName NVARCHAR(50) DEFAULT SYSTEM_USER,
    ActionDate DATETIME DEFAULT GETDATE(),
    IPAddress NVARCHAR(50)
);

GO

-- =====================================================
-- VIEWS
-- =====================================================

-- View: Employee Details with Department
IF OBJECT_ID('dbo.vw_EmployeeDetails', 'V') IS NOT NULL DROP VIEW dbo.vw_EmployeeDetails;
GO

CREATE VIEW vw_EmployeeDetails AS
SELECT 
    e.EmployeeID,
    e.FirstName,
    e.LastName,
    e.FullName,
    e.DateOfBirth,
    e.Gender,
    e.Address,
    e.Phone,
    e.Email,
    d.DepartmentID,
    d.DepartmentName,
    e.JobTitle,
    e.DateOfJoining,
    e.Salary,
    e.EmploymentStatus,
    e.EmergencyContact,
    e.EmergencyPhone,
    e.CreatedDate,
    e.ModifiedDate,
    DATEDIFF(YEAR, e.DateOfJoining, GETDATE()) AS YearsOfService
FROM Employees e
LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
WHERE e.EmploymentStatus != 'Terminated';
GO

-- View: Department Summary
IF OBJECT_ID('dbo.vw_DepartmentSummary', 'V') IS NOT NULL DROP VIEW dbo.vw_DepartmentSummary;
GO

CREATE VIEW vw_DepartmentSummary AS
SELECT 
    d.DepartmentID,
    d.DepartmentName,
    d.Description,
    COUNT(e.EmployeeID) AS TotalEmployees,
    AVG(e.Salary) AS AverageSalary,
    MAX(e.Salary) AS MaxSalary,
    MIN(e.Salary) AS MinSalary,
    SUM(e.Salary) AS TotalPayroll
FROM Departments d
LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID AND e.EmploymentStatus = 'Active'
WHERE d.IsActive = 1
GROUP BY d.DepartmentID, d.DepartmentName, d.Description;
GO

-- View: Monthly Attendance Summary
IF OBJECT_ID('dbo.vw_MonthlyAttendance', 'V') IS NOT NULL DROP VIEW dbo.vw_MonthlyAttendance;
GO

CREATE VIEW vw_MonthlyAttendance AS
SELECT 
    e.EmployeeID,
    e.FullName,
    d.DepartmentName,
    YEAR(a.AttendanceDate) AS Year,
    MONTH(a.AttendanceDate) AS Month,
    COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS PresentDays,
    COUNT(CASE WHEN a.Status = 'Absent' THEN 1 END) AS AbsentDays,
    COUNT(CASE WHEN a.Status = 'Leave' THEN 1 END) AS LeaveDays,
    COUNT(CASE WHEN a.Status = 'HalfDay' THEN 1 END) AS HalfDays,
    COUNT(*) AS TotalRecords,
    AVG(a.WorkHours) AS AverageWorkHours
FROM Employees e
LEFT JOIN Attendance a ON e.EmployeeID = a.EmployeeID
LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
GROUP BY e.EmployeeID, e.FullName, d.DepartmentName, YEAR(a.AttendanceDate), MONTH(a.AttendanceDate);
GO

-- =====================================================
-- STORED PROCEDURES
-- =====================================================

-- SP: Authenticate User
IF OBJECT_ID('dbo.sp_AuthenticateUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AuthenticateUser;
GO

CREATE PROCEDURE sp_AuthenticateUser
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserID, Username, Role, CreatedDate
    FROM Users
    WHERE Username = @Username 
    AND PasswordHash = @PasswordHash
    AND IsActive = 1;

    -- Update last login
    UPDATE Users 
    SET LastLoginDate = GETDATE()
    WHERE Username = @Username AND PasswordHash = @PasswordHash;
END;
GO

-- SP: Get Employee Attendance
IF OBJECT_ID('dbo.sp_GetEmployeeAttendance', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetEmployeeAttendance;
GO

CREATE PROCEDURE sp_GetEmployeeAttendance
    @EmployeeID INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        a.AttendanceDate,
        DATENAME(WEEKDAY, a.AttendanceDate) AS DayOfWeek,
        a.Status,
        a.CheckInTime,
        a.CheckOutTime,
        a.WorkHours,
        a.Notes
    FROM Attendance a
    WHERE a.EmployeeID = @EmployeeID
    AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
    ORDER BY a.AttendanceDate DESC;
END;
GO

-- SP: Search Employees
IF OBJECT_ID('dbo.sp_SearchEmployees', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_SearchEmployees;
GO

CREATE PROCEDURE sp_SearchEmployees
    @SearchTerm NVARCHAR(100) = NULL,
    @DepartmentID INT = NULL,
    @JobTitle NVARCHAR(100) = NULL,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM vw_EmployeeDetails
    WHERE (@SearchTerm IS NULL OR 
           FullName LIKE '%' + @SearchTerm + '%' OR 
           CAST(EmployeeID AS NVARCHAR) LIKE '%' + @SearchTerm + '%' OR
           Email LIKE '%' + @SearchTerm + '%')
    AND (@DepartmentID IS NULL OR DepartmentID = @DepartmentID)
    AND (@JobTitle IS NULL OR JobTitle LIKE '%' + @JobTitle + '%')
    AND (@Status IS NULL OR EmploymentStatus = @Status)
    ORDER BY FullName;
END;
GO

-- SP: Generate Salary Report
IF OBJECT_ID('dbo.sp_SalaryReport', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_SalaryReport;
GO

CREATE PROCEDURE sp_SalaryReport
    @DepartmentID INT = NULL,
    @MinSalary DECIMAL(18,2) = NULL,
    @MaxSalary DECIMAL(18,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.EmployeeID,
        e.FullName,
        d.DepartmentName,
        e.JobTitle,
        e.DateOfJoining,
        e.Salary,
        e.EmploymentStatus,
        CASE 
            WHEN e.Salary < 50000 THEN 'Low'
            WHEN e.Salary BETWEEN 50000 AND 100000 THEN 'Medium'
            ELSE 'High'
        END AS SalaryBand
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
    WHERE e.EmploymentStatus = 'Active'
    AND (@DepartmentID IS NULL OR e.DepartmentID = @DepartmentID)
    AND (@MinSalary IS NULL OR e.Salary >= @MinSalary)
    AND (@MaxSalary IS NULL OR e.Salary <= @MaxSalary)
    ORDER BY e.Salary DESC;
END;
GO

-- SP: Backup Database
IF OBJECT_ID('dbo.sp_BackupDatabase', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_BackupDatabase;
GO

CREATE PROCEDURE sp_BackupDatabase
    @BackupPath NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FileName NVARCHAR(500);
    SET @FileName = @BackupPath + 'EmployeeManagementDB_' + REPLACE(CONVERT(NVARCHAR, GETDATE(), 120), ':', '-') + '.bak';

    BACKUP DATABASE EmployeeManagementDB 
    TO DISK = @FileName
    WITH FORMAT, COMPRESSION;

    SELECT @FileName AS BackupFile, GETDATE() AS BackupDate;
END;
GO

-- =====================================================
-- TRIGGERS (Audit Logging)
-- =====================================================

-- Trigger: Audit Employee Changes
IF OBJECT_ID('dbo.trg_Employees_Audit', 'TR') IS NOT NULL DROP TRIGGER dbo.trg_Employees_Audit;
GO

CREATE TRIGGER trg_Employees_Audit
ON Employees
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Action NVARCHAR(20);

    IF EXISTS(SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM deleted)
        SET @Action = 'UPDATE';
    ELSE IF EXISTS(SELECT 1 FROM inserted)
        SET @Action = 'INSERT';
    ELSE
        SET @Action = 'DELETE';

    INSERT INTO AuditLog (TableName, RecordID, Action, OldValues, NewValues)
    SELECT 
        'Employees',
        COALESCE(i.EmployeeID, d.EmployeeID),
        @Action,
        (SELECT * FROM deleted FOR JSON AUTO),
        (SELECT * FROM inserted FOR JSON AUTO)
    FROM inserted i
    FULL JOIN deleted d ON i.EmployeeID = d.EmployeeID;
END;
GO

-- =====================================================
-- INDEXES (Performance)
-- =====================================================

CREATE NONCLUSTERED INDEX IX_Employees_Name ON Employees(LastName, FirstName);
CREATE NONCLUSTERED INDEX IX_Employees_Department ON Employees(DepartmentID) INCLUDE (FullName, JobTitle, Salary);
CREATE NONCLUSTERED INDEX IX_Employees_Status ON Employees(EmploymentStatus);
CREATE NONCLUSTERED INDEX IX_Attendance_Date ON Attendance(AttendanceDate) INCLUDE (EmployeeID, Status);
CREATE NONCLUSTERED INDEX IX_Attendance_Employee ON Attendance(EmployeeID, AttendanceDate);
CREATE NONCLUSTERED INDEX IX_Leave_Employee ON LeaveRecords(EmployeeID, StartDate, EndDate);
CREATE NONCLUSTERED INDEX IX_AuditLog_Date ON AuditLog(ActionDate) INCLUDE (TableName, Action);

GO

-- =====================================================
-- SAMPLE DATA
-- =====================================================

-- Insert Default Users (Passwords: admin123, hr123)
INSERT INTO Users (Username, PasswordHash, Role) VALUES
('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Admin'),
('hruser', '070a3b5e8d4bd5c46acccb91c9c54614c0cd649e78c4c4719e3a64270bae5ddf', 'HR');

-- Insert Departments


-- Insert Sample Employees


-- Update Department Managers
UPDATE Departments SET ManagerID = 1000 WHERE DepartmentID = 1;
UPDATE Departments SET ManagerID = 1001 WHERE DepartmentID = 2;
UPDATE Departments SET ManagerID = 1002 WHERE DepartmentID = 3;
UPDATE Departments SET ManagerID = 1003 WHERE DepartmentID = 4;
UPDATE Departments SET ManagerID = 1005 WHERE DepartmentID = 5;
UPDATE Departments SET ManagerID = 1006 WHERE DepartmentID = 6;

-- Insert Sample Attendance (Last 30 days)
DECLARE @DateCounter DATE = DATEADD(DAY, -30, GETDATE());
WHILE @DateCounter <= GETDATE()
BEGIN
    INSERT INTO Attendance (EmployeeID, AttendanceDate, Status, CheckInTime, CheckOutTime, Notes)
    SELECT 
        EmployeeID,
        @DateCounter,
        CASE 
            WHEN DATENAME(WEEKDAY, @DateCounter) IN ('Saturday', 'Sunday') THEN 'Weekend'
            WHEN ABS(CHECKSUM(NEWID())) % 20 = 0 THEN 'Leave'
            WHEN ABS(CHECKSUM(NEWID())) % 15 = 0 THEN 'Absent'
            WHEN ABS(CHECKSUM(NEWID())) % 10 = 0 THEN 'HalfDay'
            ELSE 'Present'
        END,
        CASE 
            WHEN DATENAME(WEEKDAY, @DateCounter) NOT IN ('Saturday', 'Sunday') 
                 AND ABS(CHECKSUM(NEWID())) % 20 != 0
            THEN DATEADD(MINUTE, ABS(CHECKSUM(NEWID())) % 60, CAST('08:00' AS TIME))
            ELSE NULL
        END,
        CASE 
            WHEN DATENAME(WEEKDAY, @DateCounter) NOT IN ('Saturday', 'Sunday') 
                 AND ABS(CHECKSUM(NEWID())) % 20 != 0
            THEN DATEADD(MINUTE, ABS(CHECKSUM(NEWID())) % 120, CAST('16:00' AS TIME))
            ELSE NULL
        END,
        CASE 
            WHEN ABS(CHECKSUM(NEWID())) % 20 = 0 THEN 'Sick leave'
            WHEN ABS(CHECKSUM(NEWID())) % 15 = 0 THEN 'Personal emergency'
            ELSE NULL
        END
    FROM Employees
    WHERE EmploymentStatus = 'Active';

    SET @DateCounter = DATEADD(DAY, 1, @DateCounter);
END;

-- Insert Sample Leave Records

GO

-- =====================================================
-- VERIFICATION
-- =====================================================

SELECT 'Database Setup Complete' AS Status, GETDATE() AS SetupDate;
SELECT COUNT(*) AS TotalUsers FROM Users;
SELECT COUNT(*) AS TotalDepartments FROM Departments;
SELECT COUNT(*) AS TotalEmployees FROM Employees;
SELECT COUNT(*) AS TotalAttendance FROM Attendance;
SELECT COUNT(*) AS TotalLeaves FROM LeaveRecords;
GO


