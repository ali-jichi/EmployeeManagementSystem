using System;

namespace EmployeeManagementSystem.Models
{
    /// <summary>
    /// Represents a department in the organization
    /// </summary>
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public int? ManagerID { get; set; }
        public string ManagerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public int EmployeeCount { get; set; }
    }
}