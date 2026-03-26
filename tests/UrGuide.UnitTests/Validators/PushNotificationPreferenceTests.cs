using FluentAssertions;
using UrGuide.Model.PushNotifications;

namespace UrGuide.UnitTests.Validators;

public class PushNotificationPreferenceTests
{
    [Fact]
    public void NotificationPreferenceDto_has_correct_defaults()
    {
        var dto = new NotificationPreferenceDto();

        dto.UserId.Should().BeEmpty();
        dto.PushEnabled.Should().BeTrue();
        dto.TourUpdatesEnabled.Should().BeTrue();
        dto.BookingAlertsEnabled.Should().BeTrue();
        dto.ChatMessagesEnabled.Should().BeTrue();
        dto.PromotionalEnabled.Should().BeFalse();
        dto.SystemAlertsEnabled.Should().BeTrue();
    }

    [Fact]
    public void UpdateNotificationPreferenceRequest_has_correct_defaults()
    {
        var request = new UpdateNotificationPreferenceRequest();

        request.PushEnabled.Should().BeTrue();
        request.TourUpdatesEnabled.Should().BeTrue();
        request.BookingAlertsEnabled.Should().BeTrue();
        request.ChatMessagesEnabled.Should().BeTrue();
        request.PromotionalEnabled.Should().BeFalse();
        request.SystemAlertsEnabled.Should().BeTrue();
    }
}
