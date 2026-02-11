using System;

namespace UrGuide.WebApp.Models
{
    /// <summary>
    /// Request to enable TOTP-based 2FA
    /// </summary>
    public class Enable2FARequest
    {
        // No parameters needed - will use authenticated user
    }
    
    /// <summary>
    /// Response with QR code and setup information
    /// </summary>
    public class Enable2FAResponse
    {
        public string Secret { get; set; } = string.Empty;
        public string QRCodeBase64 { get; set; } = string.Empty;
        public string ManualEntryKey { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Request to verify and complete 2FA setup
    /// </summary>
    public class Verify2FASetupRequest
    {
        public string? Code { get; set; }
    }
    
    /// <summary>
    /// Response after completing 2FA setup
    /// </summary>
    public class Verify2FASetupResponse
    {
        public bool Success { get; set; }
        public string[] BackupCodes { get; set; } = Array.Empty<string>();
    }
    
    /// <summary>
    /// Request to verify 2FA code during login
    /// </summary>
    public class Verify2FACodeRequest
    {
        public string? Code { get; set; }
        public bool IsBackupCode { get; set; }
    }
    
    /// <summary>
    /// 2FA status information
    /// </summary>
    public class TwoFactorStatusResponse
    {
        public bool IsEnabled { get; set; }
        public DateTime? EnabledAt { get; set; }
        public int RemainingBackupCodes { get; set; }
        public int PasskeyCount { get; set; }
    }
    
    /// <summary>
    /// Request to generate new backup codes
    /// </summary>
    public class GenerateBackupCodesRequest
    {
        // No parameters needed - will use authenticated user
    }
    
    /// <summary>
    /// Response with new backup codes
    /// </summary>
    public class GenerateBackupCodesResponse
    {
        public string[] BackupCodes { get; set; } = Array.Empty<string>();
    }
}
