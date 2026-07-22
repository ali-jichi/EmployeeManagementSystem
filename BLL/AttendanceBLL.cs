using System;
using System.Collections.Generic;
using System.Data;
using EmployeeManagementSystem.DAL;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.BLL
{
    /// <summary>
    /// Business Logic Layer for Attendance operations
    /// </summary>
    public class AttendanceBLL
    {
        private readonly AttendanceDAL attendanceDAL;

        public AttendanceBLL()
        {
            attendanceDAL = new AttendanceDAL();
        }

        /// <summary>
        /// Gets attendance for employee in date range
        /// </summary>
        public List<Attendance> GetAttendanceByEmployee(int employeeID, DateTime startDate, DateTime endDate)
        {
            if (employeeID <= 0)
                throw new ArgumentException("Invalid Employee ID");

            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be after end date");

            return attendanceDAL.GetAttendanceByEmployee(employeeID, startDate, endDate);
        }

        /// <summary>
        /// Adds attendance record with validation
        /// </summary>
        public bool AddAttendance(Attendance attendance, string currentUserRole)
        {
            ValidateAttendance(attendance);
            return attendanceDAL.AddAttendance(attendance);
        }

        /// <summary>
        /// Updates attendance record
        /// </summary>
        public bool UpdateAttendance(int attendanceID, string status, TimeSpan? checkIn, TimeSpan? checkOut, string notes, string currentUserRole)
        {
            if (attendanceID <= 0)
                throw new ArgumentException("Invalid Attendance ID");

            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status is required");

            return attendanceDAL.UpdateAttendance(attendanceID, status, checkIn, checkOut, notes);
        }

        /// <summary>
        /// Gets daily attendance for all employees
        /// </summary>
        public DataTable GetDailyAttendance(DateTime date)
        {
            return attendanceDAL.GetDailyAttendance(date);
        }

        /// <summary>
        /// Gets attendance statistics
        /// </summary>
        public DataTable GetAttendanceStatistics(DateTime startDate, DateTime endDate)
        {
            return attendanceDAL.GetAttendanceStatistics(startDate, endDate);
        }

        /// <summary>
        /// Gets monthly attendance summary
        /// </summary>
        public DataTable GetMonthlyAttendanceSummary(int year, int month)
        {
            if (year < 2000 || year > DateTime.Now.Year)
                throw new ArgumentException("Invalid year");

            if (month < 1 || month > 12)
                throw new ArgumentException("Invalid month");

            return attendanceDAL.GetMonthlyAttendanceSummary(year, month);
        }

        /// <summary>
        /// Bulk insert attendance records
        /// </summary>
        public bool BulkInsertAttendance(List<Attendance> attendanceList, string currentUserRole)
        {
            foreach (var attendance in attendanceList)
            {
                ValidateAttendance(attendance);
            }
            return attendanceDAL.BulkInsertAttendance(attendanceList);
        }

        private void ValidateAttendance(Attendance attendance)
        {
            if (attendance.EmployeeID <= 0)
                throw new ArgumentException("Invalid Employee ID");

            if (attendance.AttendanceDate > DateTime.Now)
                throw new ArgumentException("Attendance date cannot be in the future");

            if (string.IsNullOrWhiteSpace(attendance.Status))
                throw new ArgumentException("Status is required");

            string[] validStatuses = { "Present", "Absent", "Leave", "HalfDay", "Holiday", "Weekend" };
            if (Array.IndexOf(validStatuses, attendance.Status) < 0)
                throw new ArgumentException("Invalid status");

            if (attendance.CheckOutTime.HasValue && attendance.CheckInTime.HasValue)
            {
                if (attendance.CheckOutTime.Value < attendance.CheckInTime.Value)
                    throw new ArgumentException("Check-out time cannot be before check-in time");
            }
        }
    }
}