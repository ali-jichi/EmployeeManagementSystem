using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.DAL
{
    /// <summary>
    /// Data Access Layer for Attendance operations
    /// </summary>
    public class AttendanceDAL
    {
        private readonly DatabaseHelper dbHelper;

        public AttendanceDAL()
        {
            dbHelper = new DatabaseHelper();
        }

        /// <summary>
        /// Gets attendance records for an employee within date range
        /// </summary>
        public List<Attendance> GetAttendanceByEmployee(int employeeID, DateTime startDate, DateTime endDate)
        {
            List<Attendance> attendanceList = new List<Attendance>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = employeeID },
                new SqlParameter("@StartDate", SqlDbType.Date) { Value = startDate },
                new SqlParameter("@EndDate", SqlDbType.Date) { Value = endDate }
            };

            DataTable dt = dbHelper.ExecuteStoredProcedure("sp_GetEmployeeAttendance", parameters);

            foreach (DataRow row in dt.Rows)
            {
                attendanceList.Add(MapAttendanceFromDataRow(row));
            }
            return attendanceList;
        }

        /// <summary>
        /// Adds attendance record
        /// </summary>
        public bool AddAttendance(Attendance attendance)
        {
            string query = @"INSERT INTO Attendance (EmployeeID, AttendanceDate, Status, CheckInTime, CheckOutTime, Notes)
                            VALUES (@EmployeeID, @AttendanceDate, @Status, @CheckInTime, @CheckOutTime, @Notes)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = attendance.EmployeeID },
                new SqlParameter("@AttendanceDate", SqlDbType.Date) { Value = attendance.AttendanceDate },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = attendance.Status },
                new SqlParameter("@CheckInTime", SqlDbType.Time) { Value = (object)attendance.CheckInTime ?? DBNull.Value },
                new SqlParameter("@CheckOutTime", SqlDbType.Time) { Value = (object)attendance.CheckOutTime ?? DBNull.Value },
                new SqlParameter("@Notes", SqlDbType.NVarChar, 500) { Value = (object)attendance.Notes ?? DBNull.Value }
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Updates attendance record
        /// </summary>
        public bool UpdateAttendance(int attendanceID, string status, TimeSpan? checkIn, TimeSpan? checkOut, string notes)
        {
            string query = @"UPDATE Attendance SET 
                            Status = @Status, CheckInTime = @CheckInTime, 
                            CheckOutTime = @CheckOutTime, Notes = @Notes
                            WHERE AttendanceID = @AttendanceID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@AttendanceID", SqlDbType.Int) { Value = attendanceID },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = status },
                new SqlParameter("@CheckInTime", SqlDbType.Time) { Value = (object)checkIn ?? DBNull.Value },
                new SqlParameter("@CheckOutTime", SqlDbType.Time) { Value = (object)checkOut ?? DBNull.Value },
                new SqlParameter("@Notes", SqlDbType.NVarChar, 500) { Value = (object)notes ?? DBNull.Value }
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Gets daily attendance for all employees
        /// </summary>
        public DataTable GetDailyAttendance(DateTime date)
        {
            string query = @"SELECT 
                            e.EmployeeID, 
                            e.FirstName + ' ' + e.LastName AS EmployeeName,
                            d.DepartmentName, 
                            a.Status, 
                            a.CheckInTime, 
                            a.CheckOutTime,
                            a.WorkHours,
                            a.Notes
                            FROM Employees e
                            LEFT JOIN Attendance a ON e.EmployeeID = a.EmployeeID AND a.AttendanceDate = @Date
                            LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                            WHERE e.EmploymentStatus = 'Active'
                            ORDER BY d.DepartmentName, e.LastName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Date", SqlDbType.Date) { Value = date }
            };

            return dbHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Gets attendance statistics
        /// </summary>
        public DataTable GetAttendanceStatistics(DateTime startDate, DateTime endDate)
        {
            string query = @"SELECT 
                            Status,
                            COUNT(*) AS Count,
                            CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) AS Percentage
                            FROM Attendance
                            WHERE AttendanceDate BETWEEN @StartDate AND @EndDate
                            GROUP BY Status";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@StartDate", SqlDbType.Date) { Value = startDate },
                new SqlParameter("@EndDate", SqlDbType.Date) { Value = endDate }
            };

            return dbHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Gets monthly attendance summary
        /// </summary>
        public DataTable GetMonthlyAttendanceSummary(int year, int month)
        {
            return dbHelper.ExecuteQuery(
                "SELECT * FROM vw_MonthlyAttendance WHERE Year = @Year AND Month = @Month",
                new SqlParameter[]
                {
                    new SqlParameter("@Year", SqlDbType.Int) { Value = year },
                    new SqlParameter("@Month", SqlDbType.Int) { Value = month }
                }
            );
        }

        /// <summary>
        /// Bulk insert attendance records
        /// </summary>
        public bool BulkInsertAttendance(List<Attendance> attendanceList)
        {
            try
            {
                foreach (var attendance in attendanceList)
                {
                    AddAttendance(attendance);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Attendance MapAttendanceFromDataRow(DataRow row)
        {
            return new Attendance
            {
                AttendanceID = row.Table.Columns.Contains("AttendanceID") ? Convert.ToInt32(row["AttendanceID"]) : 0,
                EmployeeID = row.Table.Columns.Contains("EmployeeID") ? Convert.ToInt32(row["EmployeeID"]) : 0,
                EmployeeName = row.Table.Columns.Contains("EmployeeName") ? row["EmployeeName"].ToString() : null,
                AttendanceDate = Convert.ToDateTime(row["AttendanceDate"]),
                Status = row["Status"].ToString(),
                CheckInTime = row["CheckInTime"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["CheckInTime"].ToString()) : null,
                CheckOutTime = row["CheckOutTime"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["CheckOutTime"].ToString()) : null,
                WorkHours = row.Table.Columns.Contains("WorkHours") && row["WorkHours"] != DBNull.Value ? Convert.ToDecimal(row["WorkHours"]) : (decimal?)null,
                Notes = row["Notes"] != DBNull.Value ? row["Notes"].ToString() : null
            };
        }
    }
}