using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.DAL
{
    /// <summary>
    /// Data Access Layer for User operations
    /// Handles authentication and user management
    /// </summary>
    public class UserDAL
    {
        private readonly DatabaseHelper dbHelper;

        public UserDAL()
        {
            dbHelper = new DatabaseHelper();
        }

        /// <summary>
        /// Hashes password using SHA256
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty");

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Authenticates user with username and password
        /// </summary>
        public User AuthenticateUser(string username, string password)
        {
            string hashedPassword = HashPassword(password);

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username },
                new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 256) { Value = hashedPassword }
            };

            DataTable dt = dbHelper.ExecuteStoredProcedure("sp_AuthenticateUser", parameters);

            if (dt.Rows.Count > 0)
            {
                return MapUserFromDataRow(dt.Rows[0]);
            }
            return null;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        public bool CreateUser(string username, string password, string role)
        {
            string query = @"INSERT INTO Users (Username, PasswordHash, Role) 
                            VALUES (@Username, @PasswordHash, @Role)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username },
                new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 256) { Value = HashPassword(password) },
                new SqlParameter("@Role", SqlDbType.NVarChar, 20) { Value = role }
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        public DataTable GetAllUsers()
        {
            string query = "SELECT UserID, Username, Role, CreatedDate, IsActive, LastLoginDate FROM Users ORDER BY CreatedDate DESC";
            return dbHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// Updates user active status
        /// </summary>
        public bool UpdateUserStatus(int userID, bool isActive)
        {
            string query = "UPDATE Users SET IsActive = @IsActive WHERE UserID = @UserID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SqlDbType.Int) { Value = userID },
                new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive }
            };
            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Changes user password
        /// </summary>
        public bool ChangePassword(int userID, string newPassword)
        {
            string query = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserID = @UserID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SqlDbType.Int) { Value = userID },
                new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 256) { Value = HashPassword(newPassword) }
            };
            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        private User MapUserFromDataRow(DataRow row)
        {
            return new User
            {
                UserID = Convert.ToInt32(row["UserID"]),
                Username = row["Username"].ToString(),
                Role = row["Role"].ToString(),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                LastLoginDate = row["LastLoginDate"] != DBNull.Value ? Convert.ToDateTime(row["LastLoginDate"]) : (DateTime?)null
            };
        }
    }
}