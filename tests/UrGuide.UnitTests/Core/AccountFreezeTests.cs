using FluentAssertions;
using UrGuide.Data.Entities.Event;
using UrGuide.Data.Entities.Users;
using UrGuide.Model.Admin;

namespace UrGuide.UnitTests.Core;

public class AccountFreezeTests
{
    [Fact]
    public void AccountFreezeRecord_defaults_are_correct()
    {
        var record = new AccountFreezeRecord();
        record.Status.Should().Be(AccountFreezeStatus.Active);
        record.UnfrozenAt.Should().BeNull();
        record.UnfrozenByAdminId.Should().BeNull();
        record.UnfreezeReason.Should().BeNull();
        record.ExpiresAt.Should().BeNull();
    }

    [Fact]
    public void AccountFreezeStatus_has_expected_values()
    {
        ((int)AccountFreezeStatus.Active).Should().Be(0);
        ((int)AccountFreezeStatus.Expired).Should().Be(1);
        ((int)AccountFreezeStatus.Unfrozen).Should().Be(2);
    }

    [Fact]
    public void AccountFreezeRequest_can_set_all_properties()
    {
        var request = new AccountFreezeRequest
        {
            UserId = "user-1",
            Reason = "Violation of terms",
            DurationDays = 30
        };

        request.UserId.Should().Be("user-1");
        request.Reason.Should().Be("Violation of terms");
        request.DurationDays.Should().Be(30);
    }

    [Fact]
    public void AccountFreezeRequest_DurationDays_is_nullable()
    {
        var request = new AccountFreezeRequest
        {
            UserId = "user-1",
            Reason = "Permanent freeze"
        };

        request.DurationDays.Should().BeNull();
    }

    [Fact]
    public void AccountUnfreezeRequest_can_set_all_properties()
    {
        var request = new AccountUnfreezeRequest
        {
            UserId = "user-1",
            Reason = "Issue resolved"
        };

        request.UserId.Should().Be("user-1");
        request.Reason.Should().Be("Issue resolved");
    }

    [Fact]
    public void AccountFreezeInfo_maps_all_properties()
    {
        var info = new AccountFreezeInfo
        {
            Id = "freeze-1",
            UserId = "user-1",
            UserEmail = "user@example.com",
            Reason = "Spam",
            FrozenByAdminId = "admin-1",
            FrozenAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ExpiresAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            UnfrozenAt = null,
            UnfrozenByAdminId = null,
            UnfreezeReason = null,
            Status = "Active"
        };

        info.Id.Should().Be("freeze-1");
        info.UserId.Should().Be("user-1");
        info.UserEmail.Should().Be("user@example.com");
        info.Reason.Should().Be("Spam");
        info.FrozenByAdminId.Should().Be("admin-1");
        info.Status.Should().Be("Active");
        info.ExpiresAt.Should().NotBeNull();
        info.UnfrozenAt.Should().BeNull();
    }

    [Fact]
    public void AccountFreezeHistoryResponse_defaults_empty_list()
    {
        var response = new AccountFreezeHistoryResponse();
        response.Items.Should().NotBeNull();
        response.Items.Should().BeEmpty();
        response.TotalCount.Should().Be(0);
    }
}
