using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.PushNotifications;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("api/push-notifications")]
    [Authorize]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    public class PushNotificationController : Controller
    {
        public PushNotificationController(IPushNotificationService pushNotificationService)
        {
            PushNotificationService = pushNotificationService ?? throw new ArgumentNullException(nameof(pushNotificationService));
        }

        public IPushNotificationService PushNotificationService { get; }

        [HttpPost("devices")]
        [ProducesDefaultResponseType(typeof(DeviceRegistrationDto))]
        public async Task<IActionResult> RegisterDevice([FromBody] DeviceRegistrationRequest request, CancellationToken cancellationToken)
        {
            var result = await PushNotificationService.RegisterDeviceAsync(request, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        [HttpDelete("devices/{deviceId}")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> UnregisterDevice(string deviceId, CancellationToken cancellationToken)
        {
            var result = await PushNotificationService.UnregisterDeviceAsync(deviceId, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        [HttpGet("devices")]
        [ProducesDefaultResponseType(typeof(List<DeviceRegistrationDto>))]
        public async Task<IActionResult> GetUserDevices(CancellationToken cancellationToken)
        {
            var result = await PushNotificationService.GetUserDevicesAsync(cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        [HttpPost("send")]
        [Authorize(Roles = "Admin")]
        [ProducesDefaultResponseType(typeof(List<PushNotificationResultDto>))]
        public async Task<IActionResult> SendPushNotification([FromBody] SendPushNotificationRequest request, CancellationToken cancellationToken)
        {
            var result = await PushNotificationService.SendPushNotificationAsync(request, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        [HttpGet("delivery/{notificationId}")]
        [ProducesDefaultResponseType(typeof(PushNotificationResultDto))]
        public async Task<IActionResult> GetDeliveryStatus(string notificationId, CancellationToken cancellationToken)
        {
            var result = await PushNotificationService.GetDeliveryStatusAsync(notificationId, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        [HttpGet("preferences")]
        [ProducesDefaultResponseType(typeof(NotificationPreferenceDto))]
        public async Task<IActionResult> GetNotificationPreferences(CancellationToken cancellationToken)
        {
            var result = await PushNotificationService.GetNotificationPreferencesAsync(cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        [HttpPut("preferences")]
        [ProducesDefaultResponseType(typeof(NotificationPreferenceDto))]
        public async Task<IActionResult> UpdateNotificationPreferences([FromBody] UpdateNotificationPreferenceRequest request, CancellationToken cancellationToken)
        {
            var result = await PushNotificationService.UpdateNotificationPreferencesAsync(request, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }
    }
}
