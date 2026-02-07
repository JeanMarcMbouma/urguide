using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UrGuide.WebApp.Data;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Services
{
    public interface IPasskeyService
    {
        Task<CredentialCreateOptions> StartRegistrationAsync(UrGuideUser user, string friendlyName);
        Task<bool> CompleteRegistrationAsync(UrGuideUser user, AuthenticatorAttestationRawResponse attestationResponse, string friendlyName);
        Task<AssertionOptions> StartLoginAsync(string userName);
        Task<(bool success, UrGuideUser user)> CompleteLoginAsync(AuthenticatorAssertionRawResponse assertionResponse);
        Task<List<PasskeyCredential>> GetUserPasskeysAsync(string userId);
        Task<bool> DeletePasskeyAsync(string userId, string credentialId);
    }
    
    public class PasskeyService : IPasskeyService
    {
        private readonly IFido2 _fido2;
        private readonly UrGuideAuthContext _context;
        private readonly UserManager<UrGuideUser> _userManager;
        private readonly Dictionary<string, CredentialCreateOptions> _registrationOptions = new();
        private readonly Dictionary<string, AssertionOptions> _assertionOptions = new();
        
        public PasskeyService(IFido2 fido2, UrGuideAuthContext context, UserManager<UrGuideUser> userManager)
        {
            _fido2 = fido2 ?? throw new ArgumentNullException(nameof(fido2));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        
        public async Task<CredentialCreateOptions> StartRegistrationAsync(UrGuideUser user, string friendlyName)
        {
            // Get existing credentials for the user
            var existingCredentials = await _context.PasskeyCredentials
                .Where(c => c.UserId == user.Id)
                .Select(c => new PublicKeyCredentialDescriptor(c.CredentialId))
                .ToListAsync();
            
            // Create credential creation options
            var authenticatorSelection = new AuthenticatorSelection
            {
                RequireResidentKey = false,
                UserVerification = UserVerificationRequirement.Preferred
            };
            
            var exts = new AuthenticationExtensionsClientInputs
            {
                Extensions = true,
                UserVerificationMethod = true
            };
            
            var options = _fido2.RequestNewCredential(
                new Fido2User
                {
                    Name = user.Email,
                    Id = Encoding.UTF8.GetBytes(user.Id),
                    DisplayName = $"{user.FirstName} {user.LastName}"
                },
                existingCredentials,
                authenticatorSelection,
                AttestationConveyancePreference.None,
                exts
            );
            
            // Store options for verification
            _registrationOptions[user.Id] = options;
            
            return options;
        }
        
        public async Task<bool> CompleteRegistrationAsync(UrGuideUser user, AuthenticatorAttestationRawResponse attestationResponse, string friendlyName)
        {
            try
            {
                // Get stored options
                if (!_registrationOptions.TryGetValue(user.Id, out var options))
                {
                    return false;
                }
                
                // Verify the attestation response
                var success = await _fido2.MakeNewCredentialAsync(
                    attestationResponse,
                    options,
                    async (args, cancellationToken) =>
                    {
                        // Check if credential already exists
                        var exists = await _context.PasskeyCredentials
                            .AnyAsync(c => c.CredentialId == args.CredentialId, cancellationToken);
                        return !exists;
                    }
                );
                
                if (success.Result == null)
                {
                    return false;
                }
                
                // Store the credential
                var credential = new PasskeyCredential
                {
                    UserId = user.Id,
                    CredentialId = success.Result.CredentialId,
                    PublicKey = success.Result.PublicKey,
                    SignatureCounter = success.Result.Counter,
                    FriendlyName = friendlyName ?? "Passkey",
                    AaGuid = success.Result.Aaguid.ToByteArray(),
                    CredentialDescriptor = JsonSerializer.Serialize(new PublicKeyCredentialDescriptor(success.Result.CredentialId))
                };
                
                _context.PasskeyCredentials.Add(credential);
                await _context.SaveChangesAsync();
                
                // Clean up stored options
                _registrationOptions.Remove(user.Id);
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<AssertionOptions> StartLoginAsync(string userName)
        {
            // Find user by username/email
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return null;
            }
            
            // Get user's credentials
            var credentials = await _context.PasskeyCredentials
                .Where(c => c.UserId == user.Id)
                .Select(c => new PublicKeyCredentialDescriptor(c.CredentialId))
                .ToListAsync();
            
            if (!credentials.Any())
            {
                return null;
            }
            
            var exts = new AuthenticationExtensionsClientInputs
            {
                Extensions = true,
                UserVerificationMethod = true
            };
            
            // Create assertion options
            var options = _fido2.GetAssertionOptions(
                credentials,
                UserVerificationRequirement.Preferred,
                exts
            );
            
            // Store options for verification
            _assertionOptions[user.Id] = options;
            
            return options;
        }
        
        public async Task<(bool success, UrGuideUser user)> CompleteLoginAsync(AuthenticatorAssertionRawResponse assertionResponse)
        {
            try
            {
                // Find credential by credentialId
                var credentialIdBytes = assertionResponse.Id;
                var credential = await _context.PasskeyCredentials
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CredentialId == credentialIdBytes);
                
                if (credential == null)
                {
                    return (false, null);
                }
                
                // Get stored options
                if (!_assertionOptions.TryGetValue(credential.UserId, out var options))
                {
                    return (false, null);
                }
                
                // Verify the assertion
                var result = await _fido2.MakeAssertionAsync(
                    assertionResponse,
                    options,
                    credential.PublicKey,
                    credential.SignatureCounter,
                    async (args, cancellationToken) =>
                    {
                        // Verify user handle matches
                        var userHandle = Encoding.UTF8.GetString(args.UserHandle);
                        return userHandle == credential.UserId;
                    }
                );
                
                if (result.Status != "ok")
                {
                    return (false, null);
                }
                
                // Update credential counter and last used time
                credential.SignatureCounter = result.Counter;
                credential.LastUsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                // Clean up stored options
                _assertionOptions.Remove(credential.UserId);
                
                return (true, credential.User);
            }
            catch
            {
                return (false, null);
            }
        }
        
        public async Task<List<PasskeyCredential>> GetUserPasskeysAsync(string userId)
        {
            return await _context.PasskeyCredentials
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<bool> DeletePasskeyAsync(string userId, string credentialId)
        {
            var credential = await _context.PasskeyCredentials
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == credentialId);
            
            if (credential == null)
            {
                return false;
            }
            
            _context.PasskeyCredentials.Remove(credential);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
