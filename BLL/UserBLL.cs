using System;
using EmployeeManagementSystem.DAL;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.BLL
{
    /// <summary>
    /// Business Logic Layer for User operations
    /// Handles authentication logic and user management rules
    /// </summary>
    public class UserBLL
    {
        private readonly UserDAL userDAL;

        public UserBLL()
        {
            userDAL = new UserDAL();
        }

        /// <summary>
        /// Authenticates user and returns user object if successful
        /// </summary>
        public User AuthenticateUser(string username, string password)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required");

            if (password.Length < 5)
                throw new ArgumentException("Password must be at least 5 characters");

            return userDAL.AuthenticateUser(username.Trim(), password);
        }

        /// <summary>
        /// Creates a new user (Admin only)
        /// </summary>
        public bool CreateUser(string username, string password, string role, string currentUserRole)
        {
            // Authorization check
            if (currentUserRole != "Admin")
                throw new UnauthorizedAccessException("Only Admin can create users");

            // Validation
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required");

            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");

            if (role != "Admin" && role != "HR")
                throw new ArgumentException("Role must be Admin or HR");

            return userDAL.CreateUser(username.Trim(), password, role);
        }

        /// <summary>
        /// Changes user password
        /// </summary>
        public bool ChangePassword(int userID, string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password is required");

            if (newPassword.Length < 5)
                throw new ArgumentException("New password must be at least 5 characters");

            if (newPassword != confirmPassword)
                throw new ArgumentException("Passwords do not match");

            return userDAL.ChangePassword(userID, newPassword);
        }

        /// <summary>
        /// Gets all users for management
        /// </summary>
        public System.Data.DataTable GetAllUsers(string currentUserRole)
        {
            if (currentUserRole != "Admin")
                throw new UnauthorizedAccessException("Only Admin can view all users");

            return userDAL.GetAllUsers();
        }
    }
}