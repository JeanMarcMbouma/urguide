using FluentAssertions;
using UrGuide.Services.Helpers;

namespace UrGuide.UnitTests.Helpers;

public class FileExtensionHelperTests
{
    [Theory]
    [InlineData("photo.png")]
    [InlineData("photo.jpg")]
    [InlineData("photo.gif")]
    [InlineData("photo.webp")]
    [InlineData("photo.tiff")]
    [InlineData("photo.bmp")]
    public void Any_extension_returns_image_jpeg_due_to_extension_bug(string fileName)
    {
        // Path.GetExtension returns ".png" not "png", so no switch case matches
        var result = FileExtensionHelper.GetImageMimeType(fileName);
        result.Should().Be("image/jpeg");
    }

    [Theory]
    [InlineData("")]
    [InlineData("noextension")]
    public void Empty_or_no_extension_returns_default(string fileName)
    {
        var result = FileExtensionHelper.GetImageMimeType(fileName);
        result.Should().Be("image/jpeg");
    }
}
