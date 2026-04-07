using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Model.Email;
using UrGuide.Services.Email;

namespace UrGuide.UnitTests.Services;

public class EmailTemplateServiceTests : IDisposable
{
    private readonly UrGuideContext _context;
    private readonly EmailTemplateService _svc;

    public EmailTemplateServiceTests()
    {
        _context = CreateContext();
        _svc = new EmailTemplateService(_context, new LoggerFactory().CreateLogger<EmailTemplateService>());
    }

    public void Dispose() => _context.Dispose();
    // ------------------------------------------------------------------ //
    // Constructor guard tests                                             //
    // ------------------------------------------------------------------ //

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
    {
        var logger = new LoggerFactory().CreateLogger<EmailTemplateService>();

        var act = () => new EmailTemplateService(null!, logger);

        act.Should().Throw<ArgumentNullException>().WithParameterName("context");
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        using var tempContext = CreateContext();
        var act = () => new EmailTemplateService(tempContext, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    // ------------------------------------------------------------------ //
    // CreateTemplateAsync                                                  //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task CreateTemplateAsync_ReturnsTemplate_WhenRequestIsValid()
    {
        var request = new CreateEmailTemplateRequest
        {
            Name = "welcome",
            Subject = "Welcome to UrGuide",
            HtmlBody = "<p>Hello {{ToName}}</p>",
            Category = "onboarding",
            Language = "en",
            Variables = ["ToName"]
        };

        var result = await _svc.CreateTemplateAsync("user-1", request);

        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be("welcome");
        result.Value.Language.Should().Be("en");
        result.Value.Version.Should().Be(1);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateTemplateAsync_DefaultsLanguageToEn_WhenLanguageIsNull()
    {
        var request = new CreateEmailTemplateRequest
        {
            Name = "test-template",
            Subject = "Test",
            HtmlBody = "<p>Test</p>",
            Category = "test",
            Language = null
        };

        var result = await _svc.CreateTemplateAsync("user-1", request);

        result.IsError.Should().BeFalse();
        result.Value.Language.Should().Be("en");
    }

    // ------------------------------------------------------------------ //
    // UpdateTemplateAsync                                                  //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task UpdateTemplateAsync_ReturnsError_WhenTemplateNotFound()
    {

        var result = await _svc.UpdateTemplateAsync("user-1", "nonexistent-id",
            new UpdateEmailTemplateRequest { Subject = "New subject" });

        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateTemplateAsync_BumpsVersion_WhenSuccessful()
    {
        var createResult = await _svc.CreateTemplateAsync("user-1", BuildRequest("versioned", "en"));
        var templateId = createResult.Value.TemplateId;

        var updateResult = await _svc.UpdateTemplateAsync("user-1", templateId,
            new UpdateEmailTemplateRequest { Subject = "Updated subject" });

        updateResult.IsError.Should().BeFalse();
        updateResult.Value.Version.Should().Be(2);
    }

    // ------------------------------------------------------------------ //
    // GetTemplateAsync                                                     //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetTemplateAsync_ReturnsError_WhenNotFound()
    {

        var result = await _svc.GetTemplateAsync("missing-id");

        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task GetTemplateAsync_ReturnsTemplate_WhenFound()
    {
        var created = await _svc.CreateTemplateAsync("user-1", BuildRequest("get-test", "fr"));

        var result = await _svc.GetTemplateAsync(created.Value.TemplateId);

        result.IsError.Should().BeFalse();
        result.Value.Language.Should().Be("fr");
    }

    // ------------------------------------------------------------------ //
    // GetTemplatesAsync – pagination & filtering                          //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetTemplatesAsync_ReturnsAllTemplates_WhenNoFilter()
    {
        await _svc.CreateTemplateAsync("u", BuildRequest("t1", "en"));
        await _svc.CreateTemplateAsync("u", BuildRequest("t2", "fr"));

        var result = await _svc.GetTemplatesAsync(1, 10);

        result.IsError.Should().BeFalse();
        result.Value.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetTemplatesAsync_FiltersOnLanguage()
    {
        await _svc.CreateTemplateAsync("u", BuildRequest("lang-en", "en"));
        await _svc.CreateTemplateAsync("u", BuildRequest("lang-fr", "fr"));

        var result = await _svc.GetTemplatesAsync(1, 10, language: "fr");

        result.IsError.Should().BeFalse();
        result.Value.TotalCount.Should().Be(1);
        result.Value.Templates[0].Language.Should().Be("fr");
    }

    [Fact]
    public async Task GetTemplatesAsync_FiltersOnCategory()
    {
        await _svc.CreateTemplateAsync("u", BuildRequest("a", "en", category: "billing"));
        await _svc.CreateTemplateAsync("u", BuildRequest("b", "en", category: "onboarding"));

        var result = await _svc.GetTemplatesAsync(1, 10, category: "billing");

        result.IsError.Should().BeFalse();
        result.Value.TotalCount.Should().Be(1);
    }

    // ------------------------------------------------------------------ //
    // PreviewTemplateAsync – variable substitution                        //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task PreviewTemplateAsync_SubstitutesVariables_InSubjectAndBody()
    {
        var created = await _svc.CreateTemplateAsync("u", new CreateEmailTemplateRequest
        {
            Name = "preview-test",
            Subject = "Hello {{Name}}",
            HtmlBody = "<p>Dear {{Name}}, click {{Link}}</p>",
            Category = "test",
            Language = "en"
        });

        var previewResult = await _svc.PreviewTemplateAsync(new EmailPreviewRequest
        {
            TemplateId = created.Value.TemplateId,
            Variables = new Dictionary<string, string>
            {
                ["Name"] = "Alice",
                ["Link"] = "https://urguide.org"
            }
        });

        previewResult.IsError.Should().BeFalse();
        previewResult.Value.Subject.Should().Be("Hello Alice");
        previewResult.Value.HtmlBody.Should().Contain("Dear Alice");
        previewResult.Value.HtmlBody.Should().Contain("https://urguide.org");
    }

    [Fact]
    public async Task PreviewTemplateAsync_ReturnsError_WhenTemplateNotFound()
    {

        var result = await _svc.PreviewTemplateAsync(new EmailPreviewRequest
        {
            TemplateId = "does-not-exist"
        });

        result.IsError.Should().BeTrue();
    }

    // ------------------------------------------------------------------ //
    // RenderEmailAsync – multi-language + fallback                        //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task RenderEmailAsync_RendersCorrectLanguage()
    {
        await _svc.CreateTemplateAsync("u", new CreateEmailTemplateRequest
        {
            Name = "greeting",
            Subject = "Bienvenue {{ToName}}",
            HtmlBody = "<p>Bonjour {{ToName}}</p>",
            Category = "onboarding",
            Language = "fr"
        });

        var result = await _svc.RenderEmailAsync("greeting", "fr",
            new Dictionary<string, string> { ["ToName"] = "Marie" });

        result.IsError.Should().BeFalse();
        result.Value.Subject.Should().Be("Bienvenue Marie");
        result.Value.HtmlBody.Should().Contain("Bonjour Marie");
    }

    [Fact]
    public async Task RenderEmailAsync_FallsBackToEnglish_WhenLanguageNotFound()
    {
        // Only an English version exists
        await _svc.CreateTemplateAsync("u", new CreateEmailTemplateRequest
        {
            Name = "fallback-test",
            Subject = "Welcome {{ToName}}",
            HtmlBody = "<p>Hello {{ToName}}</p>",
            Category = "test",
            Language = "en"
        });

        // Request French - should fall back to the English template
        var result = await _svc.RenderEmailAsync("fallback-test", "fr",
            new Dictionary<string, string> { ["ToName"] = "Bob" });

        result.IsError.Should().BeFalse();
        result.Value.Subject.Should().Be("Welcome Bob");
    }

    [Fact]
    public async Task RenderEmailAsync_ReturnsError_WhenTemplateNotFoundInAnyLanguage()
    {

        var result = await _svc.RenderEmailAsync("nonexistent", "en",
            new Dictionary<string, string>());

        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task RenderEmailAsync_DoesNotRenderInactiveTemplate()
    {
        var created = await _svc.CreateTemplateAsync("u", BuildRequest("inactive-tmpl", "en"));
        await _svc.DeactivateTemplateAsync(created.Value.TemplateId);

        var result = await _svc.RenderEmailAsync("inactive-tmpl", "en", []);

        result.IsError.Should().BeTrue();
    }

    // ------------------------------------------------------------------ //
    // GetTemplateVersionsAsync                                            //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetTemplateVersionsAsync_ReturnsInitialVersion_AfterCreate()
    {
        var created = await _svc.CreateTemplateAsync("u", BuildRequest("versioned2", "en"));

        var versions = await _svc.GetTemplateVersionsAsync(created.Value.TemplateId);

        versions.IsError.Should().BeFalse();
        versions.Value.Should().HaveCount(1);
        versions.Value[0].VersionNumber.Should().Be(1);
    }

    [Fact]
    public async Task GetTemplateVersionsAsync_RecordsNewVersion_AfterUpdate()
    {
        var created = await _svc.CreateTemplateAsync("u", BuildRequest("versioned3", "en"));
        await _svc.UpdateTemplateAsync("u", created.Value.TemplateId,
            new UpdateEmailTemplateRequest { Subject = "v2" });

        var versions = await _svc.GetTemplateVersionsAsync(created.Value.TemplateId);

        versions.Value.Should().HaveCount(2);
        versions.Value.Select(v => v.VersionNumber).Should().BeEquivalentTo([2, 1]);
    }

    // ------------------------------------------------------------------ //
    // DeactivateTemplateAsync                                             //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task DeactivateTemplateAsync_SetsIsActiveToFalse()
    {
        var created = await _svc.CreateTemplateAsync("u", BuildRequest("deactivate-me", "en"));

        var deactivateResult = await _svc.DeactivateTemplateAsync(created.Value.TemplateId);
        var fetched = await _svc.GetTemplateAsync(created.Value.TemplateId);

        deactivateResult.IsError.Should().BeFalse();
        fetched.Value.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeactivateTemplateAsync_ReturnsError_WhenTemplateNotFound()
    {

        var result = await _svc.DeactivateTemplateAsync("ghost-id");

        result.IsError.Should().BeTrue();
    }

    // ------------------------------------------------------------------ //
    // Helpers                                                              //
    // ------------------------------------------------------------------ //

    private static UrGuideContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<UrGuideContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new UrGuideContext(options);
    }

    private static CreateEmailTemplateRequest BuildRequest(
        string name, string language, string category = "general") =>
        new()
        {
            Name = name,
            Subject = $"Subject for {name}",
            HtmlBody = $"<p>Body for {name}</p>",
            Category = category,
            Language = language
        };
}
