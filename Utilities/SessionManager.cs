using System;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Utilities
{
    /// <summary>
    /// Manages user session throughout the application
    /// Singleton pattern implementation
    /// </summary>
    public sealed class SessionManager
    {
        private static readonly Lazy<SessionManager> lazy = 
            new Lazy<SessionManager>(() => new SessionManager());

        public static SessionManager Instance => lazy.Value;

        private SessionManager() { }

        public User CurrentUser { get; private set; }
        public DateTime LoginTime { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public void StartSession(User user)
        {
            CurrentUser = user;
            LoginTime = DateTime.Now;
        }

        public void EndSession()
        {
            CurrentUser = null;
            LoginTime = DateTime.MinValue;
        }

        public bool HasRole(string role)
        {
            return CurrentUser?.Role == role;
        }

        public bool IsAdmin => CurrentUser?.IsAdmin ?? false;
        public string CurrentUsername => CurrentUser?.Username ?? "Guest";
        public string CurrentRole => CurrentUser?.Role ?? "None";
    }
}