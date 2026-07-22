using System;
using System.Data;
using System.Data.SqlClient;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.DAL
{
    /// <summary>
    /// Data Access Layer for Report generation
    /// </summary>
    public class ReportDAL
    {
        private readonly DatabaseHelper dbHelper;

        public ReportDAL()
        {
            dbHelper = new DatabaseHelper();
        }

        /// <summary>
        /// Gets employee list report
        /// </summary>
        public DataTable GetEmployeeListReport(int? departmentID = null, string status = null)
        {
            string query = @"SELECT 
                            e.EmployeeID,
                            e.FullName,
                            e.Gender,
                            e.Phone,
                            e.Email,
                            d.DepartmentName,
                            e.JobTitle,
                            e.DateOfJoining,
                            e.Salary,
                            e.EmploymentStatus,
                            DATEDIFF(YEAR, e.DateOfJoining, GETDATE()) AS YearsOfService
                            FROM vw_EmployeeDetails e
                            LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                            WHERE 1=1";

            var parameters = new System.Collections.Generic.List<SqlParameter>();

            if (departmentID.HasValue)
            {
                query += " AND e.DepartmentID = @DepartmentID";
                parameters.Add(new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = departmentID.Value });
            }

            if (!string.IsNullOrEmpty(status))
            {
                query += " AND e.EmploymentStatus = @Status";
                parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = status });
            }

            query += " ORDER BY e.FullName";

            return dbHelper.ExecuteQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets department employee report
        /// </summary>
        public DataTable GetDepartmentReport(int? departmentID = null)
        {
            if (departmentID.HasValue)
            {
                return dbHelper.ExecuteQuery(
                    "SELECT * FROM vw_EmployeeDetails WHERE DepartmentID = @DepartmentID ORDER BY FullName",
                    new SqlParameter[] { new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = departmentID.Value } }
                );
            }
            return dbHelper.ExecuteQuery("SELECT * FROM vw_DepartmentSummary ORDER BY DepartmentName");
        }

        /// <summary>
        /// Gets attendance report
        /// </summary>
        public DataTable GetAttendanceReport(DateTime startDate, DateTime endDate, int? employeeID = null)
        {
            string query = @"SELECT 
                            e.EmployeeID,
                            e.FirstName + ' ' + e.LastName AS EmployeeName,
                            d.DepartmentName,
                            COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS PresentDays,
                            COUNT(CASE WHEN a.Status = 'Absent' THEN 1 END) AS AbsentDays,
                            COUNT(CASE WHEN a.Status = 'Leave' THEN 1 END) AS LeaveDays,
                            COUNT(CASE WHEN a.Status = 'HalfDay' THEN 1 END) AS HalfDays,
                            COUNT(*) AS TotalDays,
                            AVG(a.WorkHours) AS AvgWorkHours
                            FROM Employees e
                            LEFT JOIN Attendance a ON e.EmployeeID = a.EmployeeID 
                                AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
                            LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                            WHERE e.EmploymentStatus = 'Active'";

            var parameters = new System.Collections.Generic.List<SqlParameter>
            {
                new SqlParameter("@StartDate", SqlDbType.Date) { Value = startDate },
                new SqlParameter("@EndDate", SqlDbType.Date) { Value = endDate }
            };

            if (employeeID.HasValue)
            {
                query += " AND e.EmployeeID = @EmployeeID";
                parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = employeeID.Value });
            }

            query += @" GROUP BY e.EmployeeID, e.FirstName, e.LastName, d.DepartmentName
                       ORDER BY d.DepartmentName, e.LastName";

            return dbHelper.ExecuteQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets salary report
        /// </summary>
        public DataTable GetSalaryReport(int? departmentID = null, decimal? minSalary = null, decimal? maxSalary = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = (object)departmentID ?? DBNull.Value },
                new SqlParameter("@MinSalary", SqlDbType.Decimal) { Value = (object)minSalary ?? DBNull.Value },
                new SqlParameter("@MaxSalary", SqlDbType.Decimal) { Value = (object)maxSalary ?? DBNull.Value }
            };

            return dbHelper.ExecuteStoredProcedure("sp_SalaryReport", parameters);
        }

        /// <summary>
        /// Gets dashboard statistics
        /// </summary>
        public DataTable GetDashboardStatistics()
        {
            string query = @"SELECT 
                            (SELECT COUNT(*) FROM Employees WHERE EmploymentStatus = 'Active') AS TotalEmployees,
                            (SELECT COUNT(*) FROM Departments WHERE IsActive = 1) AS TotalDepartments,
                            (SELECT COUNT(*) FROM Attendance WHERE AttendanceDate = CAST(GETDATE() AS DATE) AND Status = 'Present') AS PresentToday,
                            (SELECT COUNT(*) FROM LeaveRecords WHERE Status = 'Pending') AS PendingLeaves,
                            (SELECT AVG(Salary) FROM Employees WHERE EmploymentStatus = 'Active') AS AverageSalary,
                            (SELECT SUM(Salary) FROM Employees WHERE EmploymentStatus = 'Active') AS TotalPayroll";

            return dbHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// Exports data to CSV format
        /// </summary>
        public string ExportToCSV(DataTable data, string[] columnNames = null)
        {
            System.Text.StringBuilder csv = new System.Text.StringBuilder();

            // Headers
            string[] headers = columnNames ?? new string[data.Columns.Count];
            if (columnNames == null)
            {
                for (int i = 0; i < data.Columns.Count; i++)
                    headers[i] = data.Columns[i].ColumnName;
            }
            csv.AppendLine(string.Join(",", headers));

            // Data
            foreach (DataRow row in data.Rows)
            {
                string[] fields = new string[data.Columns.Count];
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    string value = row[i]?.ToString() ?? "";
                    // Escape commas and quotes
                    if (value.Contains(",") || value.Contains("\""))
                        value = "\"" + value.Replace("\"", "\"\"") + "\"";

                    fields[i] = value;
                }
                csv.AppendLine(string.Join(",", fields));
            }

            return csv.ToString();
        }
    }
}