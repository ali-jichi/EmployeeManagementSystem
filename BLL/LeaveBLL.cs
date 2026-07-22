using System;
using System.Collections.Generic;
using EmployeeManagementSystem.DAL;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.BLL
{
    /// <summary>
    /// Business Logic Layer for Leave operations
    /// </summary>
    public class LeaveBLL
    {
        private readonly LeaveDAL leaveDAL;

        public LeaveBLL()
        {
            leaveDAL = new LeaveDAL();
        }

        /// <summary>
        /// Gets all leave records
        /// </summary>
        public List<LeaveRecord> GetAllLeaveRecords()
        {
            return leaveDAL.GetAllLeaveRecords();
        }

        /// <summary>
        /// Gets leaves by employee
        /// </summary>
        public List<LeaveRecord> GetLeavesByEmployee(int employeeID)
        {
            return leaveDAL.GetLeavesByEmployee(employeeID);
        }

        /// <summary>
        /// Adds leave request with validation
        /// </summary>
        public int AddLeave(LeaveRecord leave, string currentUserRole)
        {
            ValidateLeave(leave);
            leave.Status = "Pending"; // Default status
            return leaveDAL.AddLeave(leave);
        }

        /// <summary>
        /// Approves or rejects leave (Admin/HR)
        /// </summary>
        public bool ProcessLeave(int leaveID, string status, string approvedBy, string currentUserRole)
        {
            if (currentUserRole != "Admin" && currentUserRole != "HR")
                throw new UnauthorizedAccessException("Only Admin or HR can process leaves");

            if (leaveID <= 0)
                throw new ArgumentException("Invalid Leave ID");

            if (status != "Approved" && status != "Rejected")
                throw new ArgumentException("Status must be Approved or Rejected");

            return leaveDAL.UpdateLeaveStatus(leaveID, status, approvedBy);
        }

        /// <summary>
        /// Gets pending leave requests
        /// </summary>
        public List<LeaveRecord> GetPendingLeaves(string currentUserRole)
        {
            if (currentUserRole != "Admin" && currentUserRole != "HR")
                throw new UnauthorizedAccessException("Only Admin or HR can view pending leaves");

            return leaveDAL.GetPendingLeaves();
        }

        private void ValidateLeave(LeaveRecord leave)
        {
            if (leave.EmployeeID <= 0)
                throw new ArgumentException("Invalid Employee ID");

            if (leave.StartDate > leave.EndDate)
                throw new ArgumentException("Start date cannot be after end date");

            if (leave.StartDate < DateTime.Now.Date)
                throw new ArgumentException("Cannot request leave for past dates");

            if (string.IsNullOrWhiteSpace(leave.LeaveType))
                throw new ArgumentException("Leave type is required");

            string[] validTypes = { "Annual", "Sick", "Maternity", "Paternity", "Unpaid", "Emergency" };
            if (Array.IndexOf(validTypes, leave.LeaveType) < 0)
                throw new ArgumentException("Invalid leave type");
        }
    }
}