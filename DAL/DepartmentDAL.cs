using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.DAL
{
    /// <summary>
    /// Data Access Layer for Department operations
    /// </summary>
    public class DepartmentDAL
    {
        private readonly DatabaseHelper dbHelper;

        public DepartmentDAL()
        {
            dbHelper = new DatabaseHelper();
        }

        /// <summary>
        /// Gets all departments
        /// </summary>
        public List<Department> GetAllDepartments()
        {
            List<Department> departments = new List<Department>();
            DataTable dt = dbHelper.ExecuteQuery("SELECT * FROM Departments WHERE IsActive = 1 ORDER BY DepartmentName");

            foreach (DataRow row in dt.Rows)
            {
                departments.Add(MapDepartmentFromDataRow(row));
            }
            return departments;
        }

        /// <summary>
        /// Gets department by ID
        /// </summary>
        public Department GetDepartmentByID(int departmentID)
        {
            string query = "SELECT * FROM Departments WHERE DepartmentID = @DepartmentID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = departmentID }
            };

            DataTable dt = dbHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
                return MapDepartmentFromDataRow(dt.Rows[0]);
            return null;
        }

        /// <summary>
        /// Adds a new department
        /// </summary>
        public int AddDepartment(Department department)
        {
            string query = @"INSERT INTO Departments (DepartmentName, Description, ManagerID) 
                            VALUES (@DepartmentName, @Description, @ManagerID);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 100) { Value = department.DepartmentName },
                new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Value = (object)department.Description ?? DBNull.Value },
                new SqlParameter("@ManagerID", SqlDbType.Int) { Value = (object)department.ManagerID ?? DBNull.Value }
            };

            return Convert.ToInt32(dbHelper.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Updates a department
        /// </summary>
        public bool UpdateDepartment(Department department)
        {
            string query = @"UPDATE Departments SET 
                            DepartmentName = @DepartmentName, 
                            Description = @Description, 
                            ManagerID = @ManagerID
                            WHERE DepartmentID = @DepartmentID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = department.DepartmentID },
                new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 100) { Value = department.DepartmentName },
                new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Value = (object)department.Description ?? DBNull.Value },
                new SqlParameter("@ManagerID", SqlDbType.Int) { Value = (object)department.ManagerID ?? DBNull.Value }
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Soft deletes a department (sets IsActive = 0)
        /// </summary>
        public bool DeleteDepartment(int departmentID)
        {
            string query = "DELETE FROM Departments WHERE DepartmentID = @DepartmentID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = departmentID }
            };
            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Gets department summary with employee counts
        /// </summary>
        public DataTable GetDepartmentSummary()
        {
            return dbHelper.ExecuteQuery("SELECT * FROM vw_DepartmentSummary ORDER BY DepartmentName");
        }

        /// <summary>
        /// Assigns manager to department
        /// </summary>
        public bool AssignManager(int departmentID, int managerID)
        {
            string query = "UPDATE Departments SET ManagerID = @ManagerID WHERE DepartmentID = @DepartmentID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = departmentID },
                new SqlParameter("@ManagerID", SqlDbType.Int) { Value = managerID }
            };
            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        private Department MapDepartmentFromDataRow(DataRow row)
        {
            return new Department
            {
                DepartmentID = Convert.ToInt32(row["DepartmentID"]),
                DepartmentName = row["DepartmentName"].ToString(),
                Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                ManagerID = row["ManagerID"] != DBNull.Value ? Convert.ToInt32(row["ManagerID"]) : (int?)null,
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                IsActive = Convert.ToBoolean(row["IsActive"])
            };
        }
    }
}