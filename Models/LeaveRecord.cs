using System;

namespace EmployeeManagementSystem.Models
{
    /// <summary>
    /// Represents a leave record
    /// </summary>
    public class LeaveRecord
    {
        public int LeaveID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; } // Annual, Sick, Maternity, etc.
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected, Cancelled
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}