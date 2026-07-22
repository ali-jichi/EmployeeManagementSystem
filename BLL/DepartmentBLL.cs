using System;
using System.Collections.Generic;
using System.Data;
using EmployeeManagementSystem.DAL;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.BLL
{
    /// <summary>
    /// Business Logic Layer for Department operations
    /// </summary>
    public class DepartmentBLL
    {
        private readonly DepartmentDAL departmentDAL;

        public DepartmentBLL()
        {
            departmentDAL = new DepartmentDAL();
        }

        /// <summary>
        /// Gets all active departments
        /// </summary>
        public List<Department> GetAllDepartments()
        {
            return departmentDAL.GetAllDepartments();
        }

        /// <summary>
        /// Gets department by ID
        /// </summary>
        public Department GetDepartmentByID(int departmentID)
        {
            return departmentDAL.GetDepartmentByID(departmentID);
        }

        /// <summary>
        /// Adds a new department
        /// </summary>
        public int AddDepartment(Department department, string currentUserRole)
        {
            if (currentUserRole != "Admin")
                throw new UnauthorizedAccessException("Only Admin can add departments");

            ValidateDepartment(department);
            return departmentDAL.AddDepartment(department);
        }

        /// <summary>
        /// Updates department
        /// </summary>
        public bool UpdateDepartment(Department department, string currentUserRole)
        {
            if (currentUserRole != "Admin")
                throw new UnauthorizedAccessException("Only Admin can update departments");

            ValidateDepartment(department);
            return departmentDAL.UpdateDepartment(department);
        }

        /// <summary>
        /// Deletes department (soft delete)
        /// </summary>
        public bool DeleteDepartment(int departmentID, string currentUserRole)
        {
            if (currentUserRole != "Admin")
                throw new UnauthorizedAccessException("Only Admin can delete departments");

            return departmentDAL.DeleteDepartment(departmentID);
        }

        /// <summary>
        /// Gets department summary report
        /// </summary>
        public DataTable GetDepartmentSummary()
        {
            return departmentDAL.GetDepartmentSummary();
        }

        /// <summary>
        /// Assigns manager to department
        /// </summary>
        public bool AssignManager(int departmentID, int managerID, string currentUserRole)
        {
            if (currentUserRole != "Admin")
                throw new UnauthorizedAccessException("Only Admin can assign managers");

            return departmentDAL.AssignManager(departmentID, managerID);
        }

        private void ValidateDepartment(Department department)
        {
            if (string.IsNullOrWhiteSpace(department.DepartmentName))
                throw new ArgumentException("Department name is required");

            if (department.DepartmentName.Length > 100)
                throw new ArgumentException("Department name cannot exceed 100 characters");
        }
    }
}