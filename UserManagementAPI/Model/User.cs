    
using System;

namespace UserManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        // Basic properties — extend as needed
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}