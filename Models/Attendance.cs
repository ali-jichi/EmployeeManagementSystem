using System;

namespace EmployeeManagementSystem.Models
{
    /// <summary>
    /// Represents an attendance record
    /// </summary>
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; } // Present, Absent, Leave, HalfDay, Holiday, Weekend
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public decimal? WorkHours { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool IsPresent => Status == "Present";
        public bool IsAbsent => Status == "Absent";
    }
}