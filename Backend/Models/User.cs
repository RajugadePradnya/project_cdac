using System;
using System.Collections.Generic;

namespace RapidReachApi.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
