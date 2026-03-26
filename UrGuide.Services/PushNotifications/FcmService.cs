using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrGuide.Model.PushNotifications;

namespace UrGuide.Services.PushNotifications;

public class FcmService : IPushNotificationProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FcmService> _logger;

    public DevicePlatform Platform => DevicePlatform.Android;

    public FcmService(HttpClient httpClient, IConfiguration configuration, ILogger<FcmService> logger)
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
            var serverKey = _configuration["PushNotifications:FCM:ServerKey"];

            if (string.IsNullOrEmpty(serverKey))
            {
                _logger.LogWarning("FCM configuration is incomplete. ServerKey is required.");
                return new PushNotificationDeliveryResult
                {
                    Success = false,
                    ErrorMessage = "FCM configuration is incomplete.",
                    Status = DeliveryStatus.Failed
                };
            }

            var fcmUrl = "https://fcm.googleapis.com/fcm/send";

            var payload = BuildFcmPayload(deviceToken, title, body, imageUrl, actionUrl, data);

            var request = new HttpRequestMessage(HttpMethod.Post, fcmUrl)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };
            request.Headers.TryAddWithoutValidation("Authorization", $"key={serverKey}");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                return ParseLegacyFcmResponse(responseBody);
            }

            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("FCM notification failed with status {StatusCode}: {Error}", response.StatusCode, errorBody);

            return new PushNotificationDeliveryResult
            {
                Success = false,
                ErrorMessage = $"FCM error ({response.StatusCode}): {errorBody}",
                Status = DeliveryStatus.Failed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send FCM notification.");
            return new PushNotificationDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                Status = DeliveryStatus.Failed
            };
        }
    }

    private PushNotificationDeliveryResult ParseLegacyFcmResponse(string responseBody)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            var success = root.TryGetProperty("success", out var successProp) ? successProp.GetInt32() : 0;
            var failure = root.TryGetProperty("failure", out var failureProp) ? failureProp.GetInt32() : 0;

            if (success > 0 && failure == 0)
            {
                _logger.LogInformation("FCM notification sent successfully.");
                return new PushNotificationDeliveryResult
                {
                    Success = true,
                    Status = DeliveryStatus.Sent
                };
            }

            // Extract per-token error from results array
            var errorMessage = "FCM delivery failed.";
            if (root.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
            {
                var firstResult = results[0];
                if (firstResult.TryGetProperty("error", out var errorProp))
                {
                    errorMessage = errorProp.GetString() ?? errorMessage;

                    // NotRegistered/InvalidRegistration means the token is stale
                    if (errorMessage == "NotRegistered" || errorMessage == "InvalidRegistration")
                    {
                        return new PushNotificationDeliveryResult
                        {
                            Success = false,
                            ErrorMessage = errorMessage,
                            Status = DeliveryStatus.Expired
                        };
                    }
                }
            }

            _logger.LogWarning("FCM legacy response: success={Success}, failure={Failure}, error={Error}",
                success, failure, errorMessage);

            return new PushNotificationDeliveryResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                Status = DeliveryStatus.Failed
            };
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse FCM legacy response body.");
            // If we can't parse but got a 2xx, treat as sent
            return new PushNotificationDeliveryResult
            {
                Success = true,
                Status = DeliveryStatus.Sent
            };
        }
    }

    private static string BuildFcmPayload(string deviceToken, string title, string body, string imageUrl, string actionUrl, Dictionary<string, string> data)
    {
        var notification = new Dictionary<string, object>
        {
            ["title"] = title ?? string.Empty,
            ["body"] = body ?? string.Empty
        };

        if (!string.IsNullOrEmpty(imageUrl))
            notification["image"] = imageUrl;
        if (!string.IsNullOrEmpty(actionUrl))
            notification["click_action"] = actionUrl;

        var payload = new Dictionary<string, object>
        {
            ["to"] = deviceToken,
            ["notification"] = notification
        };

        var dataPayload = data != null ? new Dictionary<string, string>(data) : new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(actionUrl))
            dataPayload["action_url"] = actionUrl;
        if (dataPayload.Count > 0)
            payload["data"] = dataPayload;

        return JsonSerializer.Serialize(payload);
    }
}
