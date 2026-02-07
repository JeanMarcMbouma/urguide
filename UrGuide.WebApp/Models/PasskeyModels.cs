using Fido2NetLib;
using System;

namespace UrGuide.WebApp.Models
{
    /// <summary>
    /// Request to start passkey registration
    /// </summary>
    public class PasskeyRegistrationStartRequest
    {
        public string FriendlyName { get; set; }
    }
    
    /// <summary>
    /// Response with registration options for the client
    /// </summary>
    public class PasskeyRegistrationStartResponse
    {
        public CredentialCreateOptions Options { get; set; }
    }
    
    /// <summary>
    /// Request to complete passkey registration
    /// </summary>
    public class PasskeyRegistrationCompleteRequest
    {
        public AuthenticatorAttestationRawResponse AttestationResponse { get; set; }
    }
    
    /// <summary>
    /// Response after completing passkey registration
    /// </summary>
    public class PasskeyRegistrationCompleteResponse
    {
        public bool Success { get; set; }
        public string CredentialId { get; set; }
    }
    
    /// <summary>
    /// Request to start passkey login
    /// </summary>
    public class PasskeyLoginStartRequest
    {
        public string UserName { get; set; }
    }
    
    /// <summary>
    /// Response with assertion options for the client
    /// </summary>
    public class PasskeyLoginStartResponse
    {
        public AssertionOptions Options { get; set; }
    }
    
    /// <summary>
    /// Request to complete passkey login
    /// </summary>
    public class PasskeyLoginCompleteRequest
    {
        public AuthenticatorAssertionRawResponse AssertionResponse { get; set; }
    }
    
    /// <summary>
    /// Response after completing passkey login
    /// </summary>
    public class PasskeyLoginCompleteResponse
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
    }
    
    /// <summary>
    /// Information about a registered passkey
    /// </summary>
    public class PasskeyInfo
    {
        public string Id { get; set; }
        public string FriendlyName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
    }
}
