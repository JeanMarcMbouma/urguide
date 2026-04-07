using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using BbQ.MockLite;
using BbQ.Outcome;
using UrGuide.Model.Email;
using UrGuide.Model.Results;
using UrGuide.Services.Email;
using UrGuide.WebApp.Controllers;
using UrGuide.WebApp.Resources;
using Microsoft.Extensions.Logging;

namespace UrGuide.IntegrationTests.Controllers;

public class EmailTemplateControllerTests
{
    private readonly Mock<IEmailTemplateService> _templateServiceMock;
    private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
    private readonly EmailTemplateController _controller;

    public EmailTemplateControllerTests()
    {
        _templateServiceMock = Mock.Create<IEmailTemplateService>();
        _localizerMock = Mock.Create<IStringLocalizer<SharedResource>>();
        _localizerMock.Setup(l => l[It.IsAny<string>()])
            .Returns(new LocalizedString("key", "value"));

        var logger = new LoggerFactory().CreateLogger<EmailTemplateController>();
        _controller = new EmailTemplateController(
            _templateServiceMock.Object, logger, _localizerMock.Object);
    }

    // ------------------------------------------------------------------ //
    // GetTemplates                                                         //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetTemplates_ReturnsOk_WhenServiceSucceeds()
    {
        var list = new EmailTemplateListResponse
        {
            Templates = [],
            TotalCount = 0,
            Page = 1,
            PageSize = 20
        };
        _templateServiceMock
            .Setup(s => s.GetTemplatesAsync(1, 20, null, null))
            .ReturnsAsync(Result.Of(list));

        var result = await _controller.GetTemplates();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetTemplates_FiltersByLanguage()
    {
        var list = new EmailTemplateListResponse
        {
            Templates = [new EmailTemplateListItem { Language = "fr" }],
            TotalCount = 1,
            Page = 1,
            PageSize = 20
        };
        _templateServiceMock
            .Setup(s => s.GetTemplatesAsync(1, 20, null, "fr"))
            .ReturnsAsync(Result.Of(list));

        var result = await _controller.GetTemplates(language: "fr");

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<EmailTemplateListResponse>().Subject;
        response.Templates[0].Language.Should().Be("fr");
    }

    // ------------------------------------------------------------------ //
    // GetTemplate                                                          //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetTemplate_ReturnsOk_WhenFound()
    {
        var dto = new EmailTemplateDto { TemplateId = "t1", Name = "welcome", Language = "en" };
        _templateServiceMock
            .Setup(s => s.GetTemplateAsync("t1"))
            .ReturnsAsync(Result.Of(dto));

        var result = await _controller.GetTemplate("t1");

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<EmailTemplateDto>()
            .Which.Language.Should().Be("en");
    }

    [Fact]
    public async Task GetTemplate_ReturnsNotFound_WhenNotFound()
    {
        _templateServiceMock
            .Setup(s => s.GetTemplateAsync("missing"))
            .ReturnsAsync(Result.Of<EmailTemplateDto>().WithErrors("Not found"));

        var result = await _controller.GetTemplate("missing");

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ------------------------------------------------------------------ //
    // PreviewTemplate                                                      //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task PreviewTemplate_ReturnsOk_WithRenderedContent()
    {
        var preview = new EmailPreviewResult
        {
            Subject = "Hello Alice",
            HtmlBody = "<p>Dear Alice</p>"
        };
        var request = new EmailPreviewRequest
        {
            TemplateId = "t1",
            Variables = new Dictionary<string, string> { ["ToName"] = "Alice" }
        };
        _templateServiceMock
            .Setup(s => s.PreviewTemplateAsync(request))
            .ReturnsAsync(Result.Of(preview));

        var result = await _controller.PreviewTemplate(request);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<EmailPreviewResult>()
            .Which.Subject.Should().Be("Hello Alice");
    }

    [Fact]
    public async Task PreviewTemplate_ReturnsBadRequest_WhenServiceFails()
    {
        var request = new EmailPreviewRequest { TemplateId = "missing" };
        _templateServiceMock
            .Setup(s => s.PreviewTemplateAsync(request))
            .ReturnsAsync(Result.Of<EmailPreviewResult>().WithErrors("Template not found"));

        var result = await _controller.PreviewTemplate(request);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ------------------------------------------------------------------ //
    // GetTemplateVersions                                                  //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task GetTemplateVersions_ReturnsOk_WithVersionList()
    {
        var versions = new List<EmailTemplateVersionDto>
        {
            new() { VersionNumber = 2, Subject = "v2" },
            new() { VersionNumber = 1, Subject = "v1" }
        };
        _templateServiceMock
            .Setup(s => s.GetTemplateVersionsAsync("t1"))
            .ReturnsAsync(Result.Of(versions));

        var result = await _controller.GetTemplateVersions("t1");

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<List<EmailTemplateVersionDto>>()
            .Which.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTemplateVersions_ReturnsNotFound_WhenTemplateNotFound()
    {
        _templateServiceMock
            .Setup(s => s.GetTemplateVersionsAsync("ghost"))
            .ReturnsAsync(Result.Of<List<EmailTemplateVersionDto>>().WithErrors("Not found"));

        var result = await _controller.GetTemplateVersions("ghost");

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ------------------------------------------------------------------ //
    // DeactivateTemplate                                                   //
    // ------------------------------------------------------------------ //

    [Fact]
    public async Task DeactivateTemplate_ReturnsOk_WhenSuccessful()
    {
        _templateServiceMock
            .Setup(s => s.DeactivateTemplateAsync("t1"))
            .ReturnsAsync(Result.Of(true));

        var result = await _controller.DeactivateTemplate("t1");

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeactivateTemplate_ReturnsNotFound_WhenNotFound()
    {
        _templateServiceMock
            .Setup(s => s.DeactivateTemplateAsync("ghost"))
            .ReturnsAsync(Result.Of(false).WithErrors("Not found"));

        var result = await _controller.DeactivateTemplate("ghost");

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
