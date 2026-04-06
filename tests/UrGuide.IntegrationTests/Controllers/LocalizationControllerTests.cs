using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using BbQ.MockLite;
using UrGuide.WebApp.Controllers;
using UrGuide.WebApp.Resources;

namespace UrGuide.IntegrationTests.Controllers;

public class LocalizationControllerTests
{
    private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
    private readonly LocalizationController _controller;

    public LocalizationControllerTests()
    {
        _localizerMock = Mock.Create<IStringLocalizer<SharedResource>>();
        _localizerMock.Setup(l => l[It.IsAny<string>()])
            .Returns(new LocalizedString("Localization_LanguageNotSupported", "Language not supported"));
        _controller = new LocalizationController(_localizerMock.Object);
    }

    [Fact]
    public void GetSupportedLanguages_ReturnsOk_WithAllLanguages()
    {
        // Act
        var result = _controller.GetSupportedLanguages();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var languages = okResult.Value as IEnumerable<object>;
        languages.Should().NotBeNull();
        languages!.Should().HaveCount(5);
    }

    [Fact]
    public void GetSupportedLanguages_ContainsEnglish()
    {
        // Act
        var result = _controller.GetSupportedLanguages();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
        json.Should().Contain("\"en\"");
        json.Should().Contain("English");
    }

    [Fact]
    public void GetSupportedLanguages_ContainsAllExpectedLanguageCodes()
    {
        // Act
        var result = _controller.GetSupportedLanguages();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);

        var expectedCodes = new[] { "en", "fr", "es", "de", "ar" };
        foreach (var code in expectedCodes)
        {
            json.Should().Contain($"\"{code}\"", $"language code '{code}' should be present");
        }
    }

    [Fact]
    public void GetSupportedLanguages_ContainsNativeNames()
    {
        // Act
        var result = _controller.GetSupportedLanguages();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);

        json.Should().Match(s => s.Contains("Fran") && s.Contains("ais"));
        json.Should().Match(s => s.Contains("Espa") && s.Contains("ol"));
        json.Should().Contain("Deutsch");
    }

    [Fact]
    public void GetTranslations_ReturnsBadRequest_ForUnsupportedLanguage()
    {
        // Act
        var result = _controller.GetTranslations("zh");

        // Assert
        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var json = System.Text.Json.JsonSerializer.Serialize(badResult.Value);
        json.Should().Contain("not supported");
    }

    [Fact]
    public void GetTranslations_ReturnsBadRequest_ForEmptyLanguage()
    {
        // Act
        var result = _controller.GetTranslations("xx");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void GetTranslations_ReturnsOk_ForEnglish()
    {
        // Act
        var result = _controller.GetTranslations("en");

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();
        var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
        json.Should().Contain("\"language\"");
        json.Should().Contain("\"en\"");
    }

    [Fact]
    public void GetTranslations_ReturnsOk_ForFrench()
    {
        // Act
        var result = _controller.GetTranslations("fr");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetTranslations_ReturnsOk_ForArabic()
    {
        // Act
        var result = _controller.GetTranslations("ar");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetAllTranslations_ReturnsOk_WithAllLanguages()
    {
        // Act
        var result = _controller.GetAllTranslations();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();
        var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
        json.Should().Contain("supportedLanguages");
        json.Should().Contain("translations");
    }
}
