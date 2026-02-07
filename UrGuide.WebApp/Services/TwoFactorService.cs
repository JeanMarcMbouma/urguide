using Microsoft.AspNetCore.Identity;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Services
{
    public interface ITwoFactorService
    {
        Task<(string secret, string qrCode, string manualKey)> GenerateQRCodeAsync(UrGuideUser user);
        Task<bool> VerifyTotpCodeAsync(UrGuideUser user, string code);
        Task<bool> EnableTwoFactorAsync(UrGuideUser user, string secret);
        Task<bool> DisableTwoFactorAsync(UrGuideUser user);
        Task<string[]> GenerateBackupCodesAsync(UrGuideUser user);
        Task<bool> VerifyBackupCodeAsync(UrGuideUser user, string code);
        int GetRemainingBackupCodesCount(UrGuideUser user);
    }
    
    public class TwoFactorService : ITwoFactorService
    {
        private readonly UserManager<UrGuideUser> _userManager;
        private const int BackupCodesCount = 10;
        private const int TotpWindow = 1; // Allow 1 step before/after current time
        
        public TwoFactorService(UserManager<UrGuideUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        
        public async Task<(string secret, string qrCode, string manualKey)> GenerateQRCodeAsync(UrGuideUser user)
        {
            // Generate a random secret (Base32 encoded)
            var secret = GenerateSecret();
            
            // Create the authenticator URI
            var authenticatorUri = $"otpauth://totp/UrGuide:{user.Email}?secret={secret}&issuer=UrGuide&algorithm=SHA1&digits=6&period=30";
            
            // Generate QR code
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(authenticatorUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);
            var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
            
            return (secret, $"data:image/png;base64,{qrCodeBase64}", secret);
        }
        
        public Task<bool> VerifyTotpCodeAsync(UrGuideUser user, string code)
        {
            if (string.IsNullOrEmpty(user.TwoFactorSecret) || string.IsNullOrEmpty(code))
            {
                return Task.FromResult(false);
            }
            
            var isValid = ValidateTotpCode(user.TwoFactorSecret, code);
            return Task.FromResult(isValid);
        }
        
        public async Task<bool> EnableTwoFactorAsync(UrGuideUser user, string secret)
        {
            user.TwoFactorSecret = secret;
            user.TwoFactorEnabled = true;
            user.TwoFactorEnabledAt = DateTime.UtcNow;
            
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        
        public async Task<bool> DisableTwoFactorAsync(UrGuideUser user)
        {
            user.TwoFactorSecret = null;
            user.TwoFactorEnabled = false;
            user.TwoFactorEnabledAt = null;
            user.BackupCodes = null;
            
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        
        public async Task<string[]> GenerateBackupCodesAsync(UrGuideUser user)
        {
            var backupCodes = new List<string>();
            
            for (int i = 0; i < BackupCodesCount; i++)
            {
                var code = GenerateBackupCode();
                backupCodes.Add(code);
            }
            
            // Store hashed backup codes
            var hashedCodes = backupCodes.Select(HashBackupCode).ToList();
            user.BackupCodes = JsonSerializer.Serialize(hashedCodes);
            
            await _userManager.UpdateAsync(user);
            
            return backupCodes.ToArray();
        }
        
        public async Task<bool> VerifyBackupCodeAsync(UrGuideUser user, string code)
        {
            if (string.IsNullOrEmpty(user.BackupCodes) || string.IsNullOrEmpty(code))
            {
                return false;
            }
            
            var hashedCodes = JsonSerializer.Deserialize<List<string>>(user.BackupCodes);
            if (hashedCodes == null || hashedCodes.Count == 0)
            {
                return false;
            }
            
            var hashedInput = HashBackupCode(code);
            
            if (hashedCodes.Contains(hashedInput))
            {
                // Remove the used backup code
                hashedCodes.Remove(hashedInput);
                user.BackupCodes = JsonSerializer.Serialize(hashedCodes);
                await _userManager.UpdateAsync(user);
                return true;
            }
            
            return false;
        }
        
        public int GetRemainingBackupCodesCount(UrGuideUser user)
        {
            if (string.IsNullOrEmpty(user.BackupCodes))
            {
                return 0;
            }
            
            var hashedCodes = JsonSerializer.Deserialize<List<string>>(user.BackupCodes);
            return hashedCodes?.Count ?? 0;
        }
        
        private string GenerateSecret()
        {
            var bytes = new byte[20]; // 160 bits
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Base32Encode(bytes);
        }
        
        private string GenerateBackupCode()
        {
            var bytes = new byte[4]; // 8 characters when hex encoded
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToHexString(bytes).ToLower();
        }
        
        private string HashBackupCode(string code)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(code);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        
        private bool ValidateTotpCode(string secret, string code)
        {
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
            
            // Check current window and adjacent windows
            for (int i = -TotpWindow; i <= TotpWindow; i++)
            {
                var timestamp = currentTimestamp + i;
                var expectedCode = GenerateTotpCode(secret, timestamp);
                
                if (expectedCode == code)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        private string GenerateTotpCode(string secret, long timestamp)
        {
            var key = Base32Decode(secret);
            var timestampBytes = BitConverter.GetBytes(timestamp);
            
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }
            
            using var hmac = new HMACSHA1(key);
            var hash = hmac.ComputeHash(timestampBytes);
            
            var offset = hash[hash.Length - 1] & 0x0F;
            var binaryCode = ((hash[offset] & 0x7F) << 24)
                           | ((hash[offset + 1] & 0xFF) << 16)
                           | ((hash[offset + 2] & 0xFF) << 8)
                           | (hash[offset + 3] & 0xFF);
            
            var code = binaryCode % 1000000;
            return code.ToString("D6");
        }
        
        private static string Base32Encode(byte[] data)
        {
            const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var result = new StringBuilder((data.Length * 8 + 4) / 5);
            
            int buffer = data[0];
            int next = 1;
            int bitsLeft = 8;
            
            while (bitsLeft > 0 || next < data.Length)
            {
                if (bitsLeft < 5)
                {
                    if (next < data.Length)
                    {
                        buffer <<= 8;
                        buffer |= data[next++] & 0xFF;
                        bitsLeft += 8;
                    }
                    else
                    {
                        int pad = 5 - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }
                
                int index = 0x1F & (buffer >> (bitsLeft - 5));
                bitsLeft -= 5;
                result.Append(base32Chars[index]);
            }
            
            return result.ToString();
        }
        
        private static byte[] Base32Decode(string base32)
        {
            const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var bits = base32.ToUpper().TrimEnd('=').Select(c => base32Chars.IndexOf(c)).ToList();
            
            var result = new List<byte>();
            int buffer = 0;
            int bitsInBuffer = 0;
            
            foreach (var bit in bits)
            {
                buffer = (buffer << 5) | bit;
                bitsInBuffer += 5;
                
                if (bitsInBuffer >= 8)
                {
                    result.Add((byte)(buffer >> (bitsInBuffer - 8)));
                    bitsInBuffer -= 8;
                }
            }
            
            return result.ToArray();
        }
    }
}
