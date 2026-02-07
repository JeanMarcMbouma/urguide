using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace UrGuide.WebApp.Entities
{
    public class UrGuideUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsGuide { get; set; }
        
        // Two-Factor Authentication properties
        public string? TwoFactorSecret { get; set; }
        public DateTime? TwoFactorEnabledAt { get; set; }
        public string? BackupCodes { get; set; } // JSON array of backup codes
        
        // Passkey/WebAuthn properties (stored as collection)
        public ICollection<PasskeyCredential> PasskeyCredentials { get; set; } = new List<PasskeyCredential>();
    }
}
