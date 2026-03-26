using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrGuide.Model.PushNotifications;

namespace UrGuide.Services.PushNotifications;

public class ApnsService : IPushNotificationProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApnsService> _logger;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private string _cachedJwtToken = string.Empty;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public DevicePlatform Platform => DevicePlatform.iOS;

    public ApnsService(HttpClient httpClient, IConfiguration configuration, ILogger<ApnsService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PushNotificationDeliveryResult> SendAsync(
        string deviceToken, string title, string body, string imageUrl,
        string actionUrl, Dictionary<string, string> data, CancellationToken cancellationToken)
    {
        try
        {
            var teamId = _configuration["PushNotifications:APNs:TeamId"];
            var keyId = _configuration["PushNotifications:APNs:KeyId"];
            var bundleId = _configuration["PushNotifications:APNs:BundleId"];
            var useSandbox = _configuration.GetValue<bool>("PushNotifications:APNs:UseSandbox", true);

            if (string.IsNullOrEmpty(teamId) || string.IsNullOrEmpty(keyId) || string.IsNullOrEmpty(bundleId))
            {
                _logger.LogWarning("APNs configuration is incomplete. TeamId, KeyId, and BundleId are required.");
                return new PushNotificationDeliveryResult
                {
                    Success = false,
                    ErrorMessage = "APNs configuration is incomplete.",
                    Status = DeliveryStatus.Failed
                };
            }

            var jwtToken = GetOrCreateJwtToken(teamId, keyId);
            var host = useSandbox
                ? "https://api.sandbox.push.apple.com"
                : "https://api.push.apple.com";

            var payload = BuildApnsPayload(title, body, imageUrl, actionUrl, data);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{host}/3/device/{deviceToken}")
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json"),
                Version = new Version(2, 0)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", jwtToken);
            request.Headers.TryAddWithoutValidation("apns-topic", bundleId);
            request.Headers.TryAddWithoutValidation("apns-push-type", "alert");
            request.Headers.TryAddWithoutValidation("apns-priority", "10");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("APNs notification sent successfully to device {DeviceToken}", deviceToken[..Math.Min(8, deviceToken.Length)] + "...");
                return new PushNotificationDeliveryResult
                {
                    Success = true,
                    Status = DeliveryStatus.Sent
                };
            }

            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("APNs notification failed with status {StatusCode}: {Error}", response.StatusCode, errorBody);

            return new PushNotificationDeliveryResult
            {
                Success = false,
                ErrorMessage = $"APNs error ({response.StatusCode}): {errorBody}",
                Status = response.StatusCode == System.Net.HttpStatusCode.Gone
                    ? DeliveryStatus.Expired
                    : DeliveryStatus.Failed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send APNs notification to device {DeviceToken}", deviceToken[..Math.Min(8, deviceToken.Length)] + "...");
            return new PushNotificationDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                Status = DeliveryStatus.Failed
            };
        }
    }

    private string GetOrCreateJwtToken(string teamId, string keyId)
    {
        _tokenLock.Wait();
        try
        {
            if (!string.IsNullOrEmpty(_cachedJwtToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedJwtToken;
            }

            var keyPath = _configuration["PushNotifications:APNs:PrivateKeyPath"];
            if (string.IsNullOrEmpty(keyPath))
            {
                _logger.LogWarning("APNs private key path is not configured.");
                return string.Empty;
            }

            try
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var header = JsonSerializer.Serialize(new { alg = "ES256", kid = keyId });
                var claims = JsonSerializer.Serialize(new { iss = teamId, iat = now });

                var headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(header));
                var claimsBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(claims));
                var unsignedToken = $"{headerBase64}.{claimsBase64}";

                var privateKeyText = System.IO.File.ReadAllText(keyPath);
                using var ecdsa = ECDsa.Create();
                ecdsa.ImportFromPem(privateKeyText);

                var signature = ecdsa.SignData(
                    Encoding.UTF8.GetBytes(unsignedToken),
                    HashAlgorithmName.SHA256);

                _cachedJwtToken = $"{unsignedToken}.{Base64UrlEncode(signature)}";
                _tokenExpiry = DateTime.UtcNow.AddMinutes(50); // APNs tokens valid for 1 hour
                return _cachedJwtToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create APNs JWT token.");
                return string.Empty;
            }
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private static string BuildApnsPayload(string title, string body, string imageUrl, string actionUrl, Dictionary<string, string> data)
    {
        var aps = new Dictionary<string, object>
        {
            ["alert"] = new Dictionary<string, string>
            {
                ["title"] = title ?? string.Empty,
                ["body"] = body ?? string.Empty
            },
            ["sound"] = "default"
        };

        var payload = new Dictionary<string, object> { ["aps"] = aps };

        if (!string.IsNullOrEmpty(imageUrl))
            payload["image_url"] = imageUrl;
        if (!string.IsNullOrEmpty(actionUrl))
            payload["action_url"] = actionUrl;
        if (data != null)
        {
            foreach (var kvp in data)
                payload[kvp.Key] = kvp.Value;
        }

        return JsonSerializer.Serialize(payload);
    }

    private static string Base64UrlEncode(byte[] data)
    {
        return Convert.ToBase64String(data)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
