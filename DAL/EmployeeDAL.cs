using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.DAL
{
    /// <summary>
    /// Data Access Layer for Employee operations
    /// Handles all CRUD operations for employees
    /// </summary>
    public class EmployeeDAL
    {
        private readonly DatabaseHelper dbHelper;

        public EmployeeDAL()
        {
            dbHelper = new DatabaseHelper();
        }

        /// <summary>
        /// Gets all employees with department details
        /// </summary>
        public List<Employee> GetAllEmployees()
        {
            List<Employee> employees = new List<Employee>();
            DataTable dt = dbHelper.ExecuteQuery("SELECT * FROM vw_EmployeeDetails ORDER BY FullName");

            foreach (DataRow row in dt.Rows)
            {
                employees.Add(MapEmployeeFromDataRow(row));
            }
            return employees;
        }

        /// <summary>
        /// Gets employee by ID
        /// </summary>
        public Employee GetEmployeeByID(int employeeID)
        {
            string query = "SELECT * FROM vw_EmployeeDetails WHERE EmployeeID = @EmployeeID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = employeeID }
            };

            DataTable dt = dbHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
                return MapEmployeeFromDataRow(dt.Rows[0]);
            return null;
        }

        /// <summary>
        /// Searches employees by various criteria
        /// </summary>
        public List<Employee> SearchEmployees(string searchTerm, int? departmentID = null, string jobTitle = null, string status = null)
        {
            List<Employee> employees = new List<Employee>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 100) { Value = (object)searchTerm ?? DBNull.Value },
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = (object)departmentID ?? DBNull.Value },
                new SqlParameter("@JobTitle", SqlDbType.NVarChar, 100) { Value = (object)jobTitle ?? DBNull.Value },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = (object)status ?? DBNull.Value }
            };

            DataTable dt = dbHelper.ExecuteStoredProcedure("sp_SearchEmployees", parameters);

            foreach (DataRow row in dt.Rows)
            {
                employees.Add(MapEmployeeFromDataRow(row));
            }
            return employees;
        }

        /// <summary>
        /// Adds a new employee
        /// </summary>
        public int AddEmployee(Employee employee)
        {
            string query = @"INSERT INTO Employees (FirstName, LastName, DateOfBirth, Gender, Address, Phone, Email, 
                            DepartmentID, JobTitle, DateOfJoining, Salary, EmploymentStatus, EmergencyContact, EmergencyPhone)
                            VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @Address, @Phone, @Email, 
                            @DepartmentID, @JobTitle, @DateOfJoining, @Salary, @EmploymentStatus, @EmergencyContact, @EmergencyPhone);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = GetEmployeeParameters(employee);
            object result = dbHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Updates an existing employee
        /// </summary>
        public bool UpdateEmployee(Employee employee)
        {
            string query = @"UPDATE Employees SET 
                            FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, 
                            Gender = @Gender, Address = @Address, Phone = @Phone, Email = @Email,
                            DepartmentID = @DepartmentID, JobTitle = @JobTitle, DateOfJoining = @DateOfJoining, 
                            Salary = @Salary, EmploymentStatus = @EmploymentStatus, 
                            EmergencyContact = @EmergencyContact, EmergencyPhone = @EmergencyPhone,
                            ModifiedDate = GETDATE()
                            WHERE EmployeeID = @EmployeeID";

            List<SqlParameter> paramList = new List<SqlParameter>(GetEmployeeParameters(employee));
            paramList.Add(new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = employee.EmployeeID });

            return dbHelper.ExecuteNonQuery(query, paramList.ToArray()) > 0;
        }

        /// <summary>
        /// Deletes an employee (and related records)
        /// </summary>
        public bool DeleteEmployee(int employeeID)
        {
            // Related records will be deleted by cascade
            string query = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = employeeID }
            };
            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Gets employees by department
        /// </summary>
        public List<Employee> GetEmployeesByDepartment(int departmentID)
        {
            List<Employee> employees = new List<Employee>();
            string query = "SELECT * FROM vw_EmployeeDetails WHERE DepartmentID = @DepartmentID ORDER BY FullName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = departmentID }
            };

            DataTable dt = dbHelper.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
            {
                employees.Add(MapEmployeeFromDataRow(row));
            }
            return employees;
        }

        /// <summary>
        /// Gets total employee count
        /// </summary>
        public int GetEmployeeCount()
        {
            object result = dbHelper.ExecuteScalar("SELECT COUNT(*) FROM Employees WHERE EmploymentStatus = 'Active'");
            return Convert.ToInt32(result);
        }

        private SqlParameter[] GetEmployeeParameters(Employee emp)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@FirstName", SqlDbType.NVarChar, 50) { Value = emp.FirstName },
                new SqlParameter("@LastName", SqlDbType.NVarChar, 50) { Value = emp.LastName },
                new SqlParameter("@DateOfBirth", SqlDbType.Date) { Value = (object)emp.DateOfBirth ?? DBNull.Value },
                new SqlParameter("@Gender", SqlDbType.NVarChar, 10) { Value = (object)emp.Gender ?? DBNull.Value },
                new SqlParameter("@Address", SqlDbType.NVarChar, 255) { Value = (object)emp.Address ?? DBNull.Value },
                new SqlParameter("@Phone", SqlDbType.NVarChar, 20) { Value = (object)emp.Phone ?? DBNull.Value },
                new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = (object)emp.Email ?? DBNull.Value },
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = (object)emp.DepartmentID ?? DBNull.Value },
                new SqlParameter("@JobTitle", SqlDbType.NVarChar, 100) { Value = (object)emp.JobTitle ?? DBNull.Value },
                new SqlParameter("@DateOfJoining", SqlDbType.Date) { Value = emp.DateOfJoining },
                new SqlParameter("@Salary", SqlDbType.Decimal) { Value = emp.Salary },
                new SqlParameter("@EmploymentStatus", SqlDbType.NVarChar, 20) { Value = emp.EmploymentStatus },
                new SqlParameter("@EmergencyContact", SqlDbType.NVarChar, 100) { Value = (object)emp.EmergencyContact ?? DBNull.Value },
                new SqlParameter("@EmergencyPhone", SqlDbType.NVarChar, 20) { Value = (object)emp.EmergencyPhone ?? DBNull.Value }
            };
        }

        private Employee MapEmployeeFromDataRow(DataRow row)
        {
            return new Employee
            {
                EmployeeID = Convert.ToInt32(row["EmployeeID"]),
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                DateOfBirth = row["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(row["DateOfBirth"]) : (DateTime?)null,
                Gender = row["Gender"] != DBNull.Value ? row["Gender"].ToString() : null,
                Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : null,
                Phone = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : null,
                Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : null,
                DepartmentID = row["DepartmentID"] != DBNull.Value ? Convert.ToInt32(row["DepartmentID"]) : (int?)null,
                DepartmentName = row["DepartmentName"] != DBNull.Value ? row["DepartmentName"].ToString() : null,
                JobTitle = row["JobTitle"] != DBNull.Value ? row["JobTitle"].ToString() : null,
                DateOfJoining = Convert.ToDateTime(row["DateOfJoining"]),
                Salary = Convert.ToDecimal(row["Salary"]),
                EmploymentStatus = row["EmploymentStatus"].ToString(),
                EmergencyContact = row["EmergencyContact"] != DBNull.Value ? row["EmergencyContact"].ToString() : null,
                EmergencyPhone = row["EmergencyPhone"] != DBNull.Value ? row["EmergencyPhone"].ToString() : null,
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                ModifiedDate = Convert.ToDateTime(row["ModifiedDate"])
            };
        }
    }
}