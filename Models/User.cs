using System;

namespace EmployeeManagementSystem.Models
{
    /// <summary>
    /// Represents a system user for authentication
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin or HR
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public bool IsAdmin => Role == "Admin";
        public bool IsHR => Role == "HR";
    }
}