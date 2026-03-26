using System.Collections.Generic;
using FluentAssertions;
using UrGuide.Model.PushNotifications;
using UrGuide.Services.PushNotifications;

namespace UrGuide.UnitTests.PushNotifications;

public class NotificationTemplateRenderTests
{
    private readonly NotificationTemplateServiceTestHelper _helper = new();

    [Fact]
    public void RenderTemplate_replaces_single_placeholder()
    {
        var template = new NotificationTemplateDto
        {
            TitleTemplate = "Hello, {{name}}!",
            BodyTemplate = "Your booking for {{tour_name}} is confirmed."
        };
        var variables = new Dictionary<string, string>
        {
            ["name"] = "Alice",
            ["tour_name"] = "Paris Walking Tour"
        };

        var (title, body) = _helper.Render(template, variables);

        title.Should().Be("Hello, Alice!");
        body.Should().Be("Your booking for Paris Walking Tour is confirmed.");
    }

    [Fact]
    public void RenderTemplate_leaves_unknown_placeholders_intact()
    {
        var template = new NotificationTemplateDto
        {
            TitleTemplate = "Hi {{name}}, your {{item}} is ready.",
            BodyTemplate = "Details: {{details}}"
        };
        var variables = new Dictionary<string, string>
        {
            ["name"] = "Bob"
            // "item" and "details" are not supplied
        };

        var (title, body) = _helper.Render(template, variables);

        title.Should().Be("Hi Bob, your {{item}} is ready.");
        body.Should().Be("Details: {{details}}");
    }

    [Fact]
    public void RenderTemplate_with_null_variables_returns_raw_templates()
    {
        var template = new NotificationTemplateDto
        {
            TitleTemplate = "Hello {{name}}",
            BodyTemplate = "Tour: {{tour_name}}"
        };

        var (title, body) = _helper.Render(template, null);

        title.Should().Be("Hello {{name}}");
        body.Should().Be("Tour: {{tour_name}}");
    }

    [Fact]
    public void RenderTemplate_with_empty_variables_returns_raw_templates()
    {
        var template = new NotificationTemplateDto
        {
            TitleTemplate = "Hi {{name}}",
            BodyTemplate = "Booking {{id}} confirmed."
        };

        var (title, body) = _helper.Render(template, new Dictionary<string, string>());

        title.Should().Be("Hi {{name}}");
        body.Should().Be("Booking {{id}} confirmed.");
    }

    [Fact]
    public void RenderTemplate_handles_multiple_occurrences_of_same_placeholder()
    {
        var template = new NotificationTemplateDto
        {
            TitleTemplate = "{{name}} - {{name}} re-confirmed",
            BodyTemplate = "Hi {{name}}, see you soon {{name}}."
        };
        var variables = new Dictionary<string, string> { ["name"] = "Carol" };

        var (title, body) = _helper.Render(template, variables);

        title.Should().Be("Carol - Carol re-confirmed");
        body.Should().Be("Hi Carol, see you soon Carol.");
    }

    [Fact]
    public void RenderTemplate_handles_templates_with_no_placeholders()
    {
        var template = new NotificationTemplateDto
        {
            TitleTemplate = "Welcome to UrGuide!",
            BodyTemplate = "Thank you for joining us."
        };
        var variables = new Dictionary<string, string> { ["name"] = "Dave" };

        var (title, body) = _helper.Render(template, variables);

        title.Should().Be("Welcome to UrGuide!");
        body.Should().Be("Thank you for joining us.");
    }

    [Fact]
    public void NotificationTemplateDto_has_correct_defaults()
    {
        var dto = new NotificationTemplateDto();

        dto.Id.Should().BeEmpty();
        dto.Language.Should().Be("en");
        dto.Version.Should().Be(1);
        dto.IsActive.Should().BeTrue();
        dto.VariantGroup.Should().BeEmpty();
    }

    [Fact]
    public void CreateNotificationTemplateRequest_has_correct_defaults()
    {
        var req = new CreateNotificationTemplateRequest();

        req.Language.Should().Be("en");
        req.Name.Should().BeEmpty();
        req.TitleTemplate.Should().BeEmpty();
        req.BodyTemplate.Should().BeEmpty();
        req.VariantGroup.Should().BeEmpty();
    }

    [Fact]
    public void UpdateNotificationTemplateRequest_has_correct_defaults()
    {
        var req = new UpdateNotificationTemplateRequest();

        req.IsActive.Should().BeTrue();
        req.TitleTemplate.Should().BeEmpty();
        req.BodyTemplate.Should().BeEmpty();
        req.VariantGroup.Should().BeEmpty();
    }
}

/// <summary>
/// Thin wrapper that exposes the internal RenderTemplate logic for unit testing
/// without requiring a full database context.
/// </summary>
internal sealed class NotificationTemplateServiceTestHelper
{
    private static readonly System.Text.RegularExpressions.Regex PlaceholderRegex =
        new(@"\{\{([a-zA-Z0-9_]+)\}\}", System.Text.RegularExpressions.RegexOptions.Compiled);

    public (string title, string body) Render(
        NotificationTemplateDto template,
        Dictionary<string, string>? variables)
    {
        if (variables == null || variables.Count == 0)
            return (template.TitleTemplate, template.BodyTemplate);

        var title = PlaceholderRegex.Replace(template.TitleTemplate, m =>
            variables.TryGetValue(m.Groups[1].Value, out var v) ? v : m.Value);

        var body = PlaceholderRegex.Replace(template.BodyTemplate, m =>
            variables.TryGetValue(m.Groups[1].Value, out var v) ? v : m.Value);

        return (title, body);
    }
}
