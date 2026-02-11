using System;

namespace UrGuide.WebApp.Entities
{
    /// <summary>
    /// Represents a WebAuthn/FIDO2 Passkey credential for a user
    /// </summary>
    public class PasskeyCredential
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public UrGuideUser User { get; set; } = null!;
        
        /// <summary>
        /// The credential ID from the authenticator
        /// </summary>
        public byte[] CredentialId { get; set; } = Array.Empty<byte>();
        
        /// <summary>
        /// The public key from the authenticator
        /// </summary>
        public byte[] PublicKey { get; set; } = Array.Empty<byte>();
        
        /// <summary>
        /// The credential descriptor in JSON format
        /// </summary>
        public string CredentialDescriptor { get; set; } = string.Empty;
        
        /// <summary>
        /// Counter value for replay protection
        /// </summary>
        public uint SignatureCounter { get; set; }
        
        /// <summary>
        /// Friendly name for the credential (e.g., "iPhone", "YubiKey")
        /// </summary>
        public string FriendlyName { get; set; } = string.Empty;
        
        /// <summary>
        /// When the credential was registered
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Last time the credential was used
        /// </summary>
        public DateTime? LastUsedAt { get; set; }
        
        /// <summary>
        /// Authenticator Attestation GUID (AAGUID)
        /// </summary>
        public byte[] AaGuid { get; set; } = Array.Empty<byte>();
    }
}
