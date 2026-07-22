using System;

namespace EmployeeManagementSystem.Models
{
    /// <summary>
    /// Represents an audit log entry for security tracking
    /// </summary>
    public class AuditLog
    {
        public int AuditID { get; set; }
        public string TableName { get; set; }
        public string RecordID { get; set; }
        public string Action { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string UserName { get; set; }
        public DateTime ActionDate { get; set; }
        public string IPAddress { get; set; }
    }
}