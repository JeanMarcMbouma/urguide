using FluentAssertions;
using UrGuide.Data.Entities.Event;
using UrGuide.Model.Admin;

namespace UrGuide.UnitTests.Core;

public class EnhancedAuditTests
{
    [Fact]
    public void AuditEvent_has_enhanced_properties()
    {
        var auditEvent = new AuditEvent
        {
            UserId = "user-1",
            EventCode = EventCodes.AccountFrozen,
            ReferenceId = "target-user",
            IpAddress = "192.168.1.1",
            UserAgent = "Mozilla/5.0",
            Details = "Account frozen for spam",
            Category = "AccountManagement",
            Severity = AuditSeverity.Warning
        };

        auditEvent.IpAddress.Should().Be("192.168.1.1");
        auditEvent.UserAgent.Should().Be("Mozilla/5.0");
        auditEvent.Details.Should().Be("Account frozen for spam");
        auditEvent.Category.Should().Be("AccountManagement");
        auditEvent.Severity.Should().Be(AuditSeverity.Warning);
    }

    [Fact]
    public void AuditSeverity_has_expected_values()
    {
        ((int)AuditSeverity.Info).Should().Be(0);
        ((int)AuditSeverity.Warning).Should().Be(1);
        ((int)AuditSeverity.Critical).Should().Be(2);
    }

    [Fact]
    public void EventCodes_includes_account_management_codes()
    {
        ((int)EventCodes.AccountFrozen).Should().Be(4000);
        ((int)EventCodes.AccountUnfrozen).Should().Be(4001);
        ((int)EventCodes.AccountSuspended).Should().Be(4002);
        ((int)EventCodes.AccountActivated).Should().Be(4003);
        ((int)EventCodes.AccountDeleted).Should().Be(4004);
    }

    [Fact]
    public void EventCodes_includes_authentication_codes()
    {
        ((int)EventCodes.FailedLogin).Should().Be(1002);
        ((int)EventCodes.PasswordChanged).Should().Be(1003);
        ((int)EventCodes.PasswordReset).Should().Be(1004);
        ((int)EventCodes.TwoFactorEnabled).Should().Be(1005);
        ((int)EventCodes.TwoFactorDisabled).Should().Be(1006);
    }

    [Fact]
    public void EventCodes_includes_moderation_codes()
    {
        ((int)EventCodes.GuideVerificationApproved).Should().Be(5000);
        ((int)EventCodes.GuideVerificationRejected).Should().Be(5001);
        ((int)EventCodes.TourApproved).Should().Be(5002);
        ((int)EventCodes.TourRejected).Should().Be(5003);
    }

    [Fact]
    public void EventCodes_includes_financial_codes()
    {
        ((int)EventCodes.PaymentProcessed).Should().Be(6000);
        ((int)EventCodes.RefundIssued).Should().Be(6001);
        ((int)EventCodes.PayoutProcessed).Should().Be(6002);
    }

    [Fact]
    public void EventCodes_preserves_existing_codes()
    {
        ((int)EventCodes.Login).Should().Be(1000);
        ((int)EventCodes.Logout).Should().Be(1001);
        ((int)EventCodes.Register).Should().Be(2000);
        ((int)EventCodes.DeleteAccount).Should().Be(2001);
        ((int)EventCodes.CreatePost).Should().Be(3000);
        ((int)EventCodes.EditPost).Should().Be(3001);
        ((int)EventCodes.Maintenance).Should().Be(10000);
    }

    [Fact]
    public void AdminAuditLogItem_has_enhanced_fields()
    {
        var item = new AdminAuditLogItem
        {
            Id = "log-1",
            EventCode = "AccountFrozen",
            UserId = "admin-1",
            UserEmail = "admin@example.com",
            ReferenceId = "user-1",
            Created = DateTime.UtcNow,
            IpAddress = "10.0.0.1",
            UserAgent = "AdminBrowser/1.0",
            Details = "Frozen for TOS violation",
            Category = "AccountManagement",
            Severity = "Warning"
        };

        item.IpAddress.Should().Be("10.0.0.1");
        item.UserAgent.Should().Be("AdminBrowser/1.0");
        item.Details.Should().Be("Frozen for TOS violation");
        item.Category.Should().Be("AccountManagement");
        item.Severity.Should().Be("Warning");
    }

    [Fact]
    public void AuditLogFilterParameters_has_enhanced_filters()
    {
        var filters = new AuditLogFilterParameters
        {
            PageNumber = 1,
            PageSize = 50,
            UserId = "user-1",
            EventCode = "AccountFrozen",
            Category = "AccountManagement",
            Severity = "Warning"
        };

        filters.Category.Should().Be("AccountManagement");
        filters.Severity.Should().Be("Warning");
    }

    [Fact]
    public void AuditLogFilterParameters_defaults_are_correct()
    {
        var filters = new AuditLogFilterParameters();
        filters.PageNumber.Should().Be(1);
        filters.PageSize.Should().Be(50);
        filters.UserId.Should().BeNull();
        filters.Category.Should().BeNull();
        filters.Severity.Should().BeNull();
    }
}
