using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.DAL
{
    /// <summary>
    /// Data Access Layer for Leave operations
    /// </summary>
    public class LeaveDAL
    {
        private readonly DatabaseHelper dbHelper;

        public LeaveDAL()
        {
            dbHelper = new DatabaseHelper();
        }

        /// <summary>
        /// Gets all leave records
        /// </summary>
        public List<LeaveRecord> GetAllLeaveRecords()
        {
            List<LeaveRecord> leaves = new List<LeaveRecord>();
            string query = @"SELECT l.*, e.FirstName + ' ' + e.LastName AS EmployeeName
                            FROM LeaveRecords l
                            JOIN Employees e ON l.EmployeeID = e.EmployeeID
                            ORDER BY l.CreatedDate DESC";

            DataTable dt = dbHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                leaves.Add(MapLeaveFromDataRow(row));
            }
            return leaves;
        }

        /// <summary>
        /// Gets leave records by employee
        /// </summary>
        public List<LeaveRecord> GetLeavesByEmployee(int employeeID)
        {
            List<LeaveRecord> leaves = new List<LeaveRecord>();
            string query = @"SELECT l.*, e.FirstName + ' ' + e.LastName AS EmployeeName
                            FROM LeaveRecords l
                            JOIN Employees e ON l.EmployeeID = e.EmployeeID
                            WHERE l.EmployeeID = @EmployeeID
                            ORDER BY l.StartDate DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = employeeID }
            };

            DataTable dt = dbHelper.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
            {
                leaves.Add(MapLeaveFromDataRow(row));
            }
            return leaves;
        }

        /// <summary>
        /// Adds a leave record
        /// </summary>
        public int AddLeave(LeaveRecord leave)
        {
            string query = @"INSERT INTO LeaveRecords (EmployeeID, LeaveType, StartDate, EndDate, Reason, Status)
                            VALUES (@EmployeeID, @LeaveType, @StartDate, @EndDate, @Reason, @Status);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = leave.EmployeeID },
                new SqlParameter("@LeaveType", SqlDbType.NVarChar, 50) { Value = leave.LeaveType },
                new SqlParameter("@StartDate", SqlDbType.Date) { Value = leave.StartDate },
                new SqlParameter("@EndDate", SqlDbType.Date) { Value = leave.EndDate },
                new SqlParameter("@Reason", SqlDbType.NVarChar, 500) { Value = (object)leave.Reason ?? DBNull.Value },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = leave.Status }
            };

            return Convert.ToInt32(dbHelper.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Updates leave status (Approve/Reject)
        /// </summary>
        public bool UpdateLeaveStatus(int leaveID, string status, string approvedBy)
        {
            string query = @"UPDATE LeaveRecords SET 
                            Status = @Status, 
                            ApprovedBy = @ApprovedBy, 
                            ApprovalDate = GETDATE()
                            WHERE LeaveID = @LeaveID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@LeaveID", SqlDbType.Int) { Value = leaveID },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = status },
                new SqlParameter("@ApprovedBy", SqlDbType.NVarChar, 50) { Value = approvedBy }
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Gets pending leave requests
        /// </summary>
        public List<LeaveRecord> GetPendingLeaves()
        {
            List<LeaveRecord> leaves = new List<LeaveRecord>();
            string query = @"SELECT l.*, e.FirstName + ' ' + e.LastName AS EmployeeName
                            FROM LeaveRecords l
                            JOIN Employees e ON l.EmployeeID = e.EmployeeID
                            WHERE l.Status = 'Pending'
                            ORDER BY l.StartDate";

            DataTable dt = dbHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                leaves.Add(MapLeaveFromDataRow(row));
            }
            return leaves;
        }

        private LeaveRecord MapLeaveFromDataRow(DataRow row)
        {
            return new LeaveRecord
            {
                LeaveID = Convert.ToInt32(row["LeaveID"]),
                EmployeeID = Convert.ToInt32(row["EmployeeID"]),
                EmployeeName = row["EmployeeName"].ToString(),
                LeaveType = row["LeaveType"].ToString(),
                StartDate = Convert.ToDateTime(row["StartDate"]),
                EndDate = Convert.ToDateTime(row["EndDate"]),
                TotalDays = Convert.ToInt32(row["TotalDays"]),
                Reason = row["Reason"] != DBNull.Value ? row["Reason"].ToString() : null,
                Status = row["Status"].ToString(),
                ApprovedBy = row["ApprovedBy"] != DBNull.Value ? row["ApprovedBy"].ToString() : null,
                ApprovalDate = row["ApprovalDate"] != DBNull.Value ? Convert.ToDateTime(row["ApprovalDate"]) : (DateTime?)null,
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }
    }
}