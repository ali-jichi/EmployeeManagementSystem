using System;
using System.Data;
using EmployeeManagementSystem.DAL;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.BLL
{
    /// <summary>
    /// Business Logic Layer for Report generation
    /// </summary>
    public class ReportBLL
    {
        private readonly ReportDAL reportDAL;

        public ReportBLL()
        {
            reportDAL = new ReportDAL();
        }

        /// <summary>
        /// Generates employee list report
        /// </summary>
        public DataTable GenerateEmployeeListReport(int? departmentID = null, string status = null)
        {
            return reportDAL.GetEmployeeListReport(departmentID, status);
        }

        /// <summary>
        /// Generates department report
        /// </summary>
        public DataTable GenerateDepartmentReport(int? departmentID = null)
        {
            return reportDAL.GetDepartmentReport(departmentID);
        }

        /// <summary>
        /// Generates attendance report
        /// </summary>
        public DataTable GenerateAttendanceReport(DateTime startDate, DateTime endDate, int? employeeID = null)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be after end date");

            return reportDAL.GetAttendanceReport(startDate, endDate, employeeID);
        }

        /// <summary>
        /// Generates salary report
        /// </summary>
        public DataTable GenerateSalaryReport(int? departmentID = null, decimal? minSalary = null, decimal? maxSalary = null)
        {
            if (minSalary.HasValue && maxSalary.HasValue && minSalary > maxSalary)
                throw new ArgumentException("Minimum salary cannot be greater than maximum salary");

            return reportDAL.GetSalaryReport(departmentID, minSalary, maxSalary);
        }

        /// <summary>
        /// Gets dashboard statistics
        /// </summary>
        public DataTable GetDashboardStatistics()
        {
            return reportDAL.GetDashboardStatistics();
        }

        /// <summary>
        /// Exports report to CSV
        /// </summary>
        public string ExportToCSV(DataTable data, string[] columnNames = null)
        {
            return reportDAL.ExportToCSV(data, columnNames);
        }
    }
}