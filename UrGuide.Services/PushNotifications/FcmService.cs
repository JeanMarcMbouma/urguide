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
            var projectId = _configuration["PushNotifications:FCM:ProjectId"];
            var useV1Api = !string.IsNullOrEmpty(projectId);

            // V1 API uses OAuth2 access token; legacy API uses server key
            var authToken = useV1Api
                ? _configuration["PushNotifications:FCM:AccessToken"]
                : _configuration["PushNotifications:FCM:ServerKey"];

            if (string.IsNullOrEmpty(authToken))
            {
                var requiredKey = useV1Api ? "AccessToken" : "ServerKey";
                _logger.LogWarning("FCM configuration is incomplete. {Key} is required.", requiredKey);
                return new PushNotificationDeliveryResult
                {
                    Success = false,
                    ErrorMessage = "FCM configuration is incomplete.",
                    Status = DeliveryStatus.Failed
                };
            }

            var fcmUrl = useV1Api
                ? $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send"
                : "https://fcm.googleapis.com/fcm/send";

            var payload = BuildFcmPayload(deviceToken, title, body, imageUrl, data, useV1Api);

            var request = new HttpRequestMessage(HttpMethod.Post, fcmUrl)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            if (useV1Api)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }
            else
            {
                request.Headers.TryAddWithoutValidation("Authorization", $"key={authToken}");
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("FCM notification sent successfully.");
                return new PushNotificationDeliveryResult
                {
                    Success = true,
                    Status = DeliveryStatus.Sent
                };
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

    private static string BuildFcmPayload(string deviceToken, string title, string body, string imageUrl, Dictionary<string, string> data, bool useV1Api)
    {
        if (useV1Api)
        {
            var message = new Dictionary<string, object>
            {
                ["message"] = new Dictionary<string, object>
                {
                    ["token"] = deviceToken,
                    ["notification"] = new Dictionary<string, object>
                    {
                        ["title"] = title ?? string.Empty,
                        ["body"] = body ?? string.Empty
                    }
                }
            };

            var innerMessage = (Dictionary<string, object>)message["message"];
            if (!string.IsNullOrEmpty(imageUrl))
            {
                ((Dictionary<string, object>)innerMessage["notification"])["image"] = imageUrl;
            }
            if (data != null && data.Count > 0)
            {
                innerMessage["data"] = data;
            }

            return JsonSerializer.Serialize(message);
        }
        else
        {
            var payload = new Dictionary<string, object>
            {
                ["to"] = deviceToken,
                ["notification"] = new Dictionary<string, object>
                {
                    ["title"] = title ?? string.Empty,
                    ["body"] = body ?? string.Empty
                }
            };

            if (!string.IsNullOrEmpty(imageUrl))
            {
                ((Dictionary<string, object>)payload["notification"])["image"] = imageUrl;
            }
            if (data != null && data.Count > 0)
            {
                payload["data"] = data;
            }

            return JsonSerializer.Serialize(payload);
        }
    }
}
