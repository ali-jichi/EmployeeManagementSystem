using System;

namespace EmployeeManagementSystem.Models
{
    /// <summary>
    /// Represents an employee in the system
    /// </summary>
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string JobTitle { get; set; }
        public DateTime DateOfJoining { get; set; }
        public decimal Salary { get; set; }
        public string EmploymentStatus { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyPhone { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public int YearsOfService => DateTime.Now.Year - DateOfJoining.Year;
    }
}