using System.Collections.Generic;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Data.Entities.Media;
using UrGuide.Model.Media;

namespace UrGuide.Services.Media
{
    public interface IImageProcessingService
    {
        Task<Outcome<ImageProcessingResult>> ProcessImageAsync(ImageProcessingRequest request);
        Task<Outcome<ImageVariantsDto>> GetImageVariantsAsync(string imageId);
        Task<Outcome<ImageProcessingStatus>> GetProcessingStatusAsync(string imageId);
        Task<Outcome<string>> GenerateThumbnailUrlAsync(string originalUrl, int width, int height);
        Task<Outcome<string>> GenerateWebPUrlAsync(string originalUrl);
        Task<Outcome<ImageProcessingResult>> ApplyWatermarkAsync(string imageId);
        Task<Outcome<Dictionary<string, string>>> ExtractExifDataAsync(string imageId);
        Task<Outcome<string>> GetCdnUrlAsync(string originalUrl, string cdnProvider);
    }
}
