using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmployeeManagementSystem.DAL;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.BLL
{
    /// <summary>
    /// Business Logic Layer for Employee operations
    /// Handles validation and business rules
    /// </summary>
    public class EmployeeBLL
    {
        private readonly EmployeeDAL employeeDAL;

        public EmployeeBLL()
        {
            employeeDAL = new EmployeeDAL();
        }

        /// <summary>
        /// Gets all active employees
        /// </summary>
        public List<Employee> GetAllEmployees()
        {
            return employeeDAL.GetAllEmployees();
        }

        /// <summary>
        /// Gets employee by ID
        /// </summary>
        public Employee GetEmployeeByID(int employeeID)
        {
            if (employeeID <= 0)
                throw new ArgumentException("Invalid Employee ID");

            return employeeDAL.GetEmployeeByID(employeeID);
        }

        /// <summary>
        /// Searches employees with validation
        /// </summary>
        public List<Employee> SearchEmployees(string searchTerm, int? departmentID = null, string jobTitle = null, string status = null)
        {
            return employeeDAL.SearchEmployees(searchTerm, departmentID, jobTitle, status);
        }

        /// <summary>
        /// Adds a new employee with business validation
        /// </summary>
        public int AddEmployee(Employee employee, string currentUserRole)
        {
            ValidateEmployee(employee);

            // Set defaults
            employee.EmploymentStatus = employee.EmploymentStatus ?? "Active";
            employee.DateOfJoining = employee.DateOfJoining == default ? DateTime.Now : employee.DateOfJoining;

            return employeeDAL.AddEmployee(employee);
        }

        /// <summary>
        /// Updates employee with validation
        /// </summary>
        public bool UpdateEmployee(Employee employee, string currentUserRole)
        {
            if (employee.EmployeeID <= 0)
                throw new ArgumentException("Invalid Employee ID");

            ValidateEmployee(employee);
            return employeeDAL.UpdateEmployee(employee);
        }

        /// <summary>
        /// Deletes employee (Admin only)
        /// </summary>
        public bool DeleteEmployee(int employeeID, string currentUserRole)
        {
            if (currentUserRole != "Admin")
                throw new UnauthorizedAccessException("Only Admin can delete employees");

            if (employeeID <= 0)
                throw new ArgumentException("Invalid Employee ID");

            // Check if employee exists
            var emp = employeeDAL.GetEmployeeByID(employeeID);
            if (emp == null)
                throw new Exception("Employee not found");

            return employeeDAL.DeleteEmployee(employeeID);
        }

        /// <summary>
        /// Gets employees by department
        /// </summary>
        public List<Employee> GetEmployeesByDepartment(int departmentID)
        {
            return employeeDAL.GetEmployeesByDepartment(departmentID);
        }

        /// <summary>
        /// Gets total employee count
        /// </summary>
        public int GetEmployeeCount()
        {
            return employeeDAL.GetEmployeeCount();
        }

        /// <summary>
        /// Validates employee data
        /// </summary>
        private void ValidateEmployee(Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.FirstName))
                throw new ArgumentException("First name is required");

            if (string.IsNullOrWhiteSpace(employee.LastName))
                throw new ArgumentException("Last name is required");

            if (employee.FirstName.Length > 50)
                throw new ArgumentException("First name cannot exceed 50 characters");

            if (employee.LastName.Length > 50)
                throw new ArgumentException("Last name cannot exceed 50 characters");

            if (!string.IsNullOrEmpty(employee.Email) && !IsValidEmail(employee.Email))
                throw new ArgumentException("Invalid email format");

            if (!string.IsNullOrEmpty(employee.Phone) && !IsValidPhone(employee.Phone))
                throw new ArgumentException("Invalid phone format");

            if (employee.Salary < 0)
                throw new ArgumentException("Salary cannot be negative");

            if (employee.DateOfJoining > DateTime.Now)
                throw new ArgumentException("Date of joining cannot be in the future");

            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value > DateTime.Now.AddYears(-18))
                throw new ArgumentException("Employee must be at least 18 years old");
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsValidPhone(string phone)
        {
            // Allow empty (optional field) or any reasonable phone format
            if (string.IsNullOrWhiteSpace(phone)) return true;

            // Minimum 7 digits, allows spaces, dashes, +, (, )
            string pattern = @"^[\d\s\-\+\(\)]{7,25}$";
            return Regex.IsMatch(phone, pattern);
        }
    }
}