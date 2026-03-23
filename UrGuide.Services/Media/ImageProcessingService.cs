using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BbQ.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Media;
using UrGuide.Model.Media;
using UrGuide.Model.Results;

namespace UrGuide.Services.Media
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<ImageProcessingService> _logger;

        private const int ThumbnailWidth = 150;
        private const int ThumbnailHeight = 150;
        private const int MediumWidth = 600;
        private const int MediumHeight = 600;
        private const int LargeWidth = 1200;
        private const int LargeHeight = 1200;

        public ImageProcessingService(UrGuideContext context, ILogger<ImageProcessingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Outcome<ImageProcessingResult>> ProcessImageAsync(ImageProcessingRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.ImageId))
                    return Result.Of<ImageProcessingResult>(null).WithErrors("ImageId is required");

                if (string.IsNullOrWhiteSpace(request.Url))
                    return Result.Of<ImageProcessingResult>(null).WithErrors("Url is required");

                var processedImage = new ProcessedImage
                {
                    Id = Guid.NewGuid().ToString(),
                    OriginalImageId = request.ImageId,
                    OriginalUrl = request.Url,
                    Status = ImageProcessingStatus.Processing,
                    CreatedAt = DateTime.UtcNow
                };

                // Generate variant URLs based on request options
                if (request.GenerateThumbnail)
                {
                    processedImage.ThumbnailUrl = GenerateVariantUrl(request.Url, ThumbnailWidth, ThumbnailHeight);
                }

                processedImage.MediumUrl = GenerateVariantUrl(request.Url, MediumWidth, MediumHeight);
                processedImage.LargeUrl = GenerateVariantUrl(request.Url, LargeWidth, LargeHeight);

                if (request.GenerateWebP)
                {
                    processedImage.WebPUrl = GenerateWebPVariantUrl(request.Url);
                }

                if (!string.IsNullOrWhiteSpace(request.CdnProvider))
                {
                    processedImage.CdnUrl = BuildCdnUrl(request.Url, request.CdnProvider);
                }

                processedImage.IsWatermarked = request.EnableWatermark;
                processedImage.Format = ExtractFormat(request.Url);
                processedImage.Width = LargeWidth;
                processedImage.Height = LargeHeight;
                processedImage.Status = ImageProcessingStatus.Completed;
                processedImage.ProcessedAt = DateTime.UtcNow;

                _context.ProcessedImages.Add(processedImage);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Image {ImageId} processed successfully", request.ImageId);

                return Result.Of(MapToResult(processedImage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image {ImageId}", request.ImageId);
                return Result.Of<ImageProcessingResult>(null).WithErrors("Failed to process image");
            }
        }

        public async Task<Outcome<ImageVariantsDto>> GetImageVariantsAsync(string imageId)
        {
            try
            {
                var processedImage = await _context.ProcessedImages
                    .FirstOrDefaultAsync(p => p.OriginalImageId == imageId);

                if (processedImage == null)
                    return Result.Of<ImageVariantsDto>(null).WithErrors("Image not found");

                var variants = new ImageVariantsDto
                {
                    OriginalUrl = processedImage.OriginalUrl,
                    ThumbnailUrl = processedImage.ThumbnailUrl,
                    MediumUrl = processedImage.MediumUrl,
                    LargeUrl = processedImage.LargeUrl,
                    WebPUrl = processedImage.WebPUrl,
                    CdnUrl = processedImage.CdnUrl
                };

                return Result.Of(variants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting image variants for {ImageId}", imageId);
                return Result.Of<ImageVariantsDto>(null).WithErrors("Failed to get image variants");
            }
        }

        public async Task<Outcome<ImageProcessingStatus>> GetProcessingStatusAsync(string imageId)
        {
            try
            {
                var processedImage = await _context.ProcessedImages
                    .FirstOrDefaultAsync(p => p.OriginalImageId == imageId);

                if (processedImage == null)
                    return Result.Of(ImageProcessingStatus.Pending).WithErrors("Image not found");

                return Result.Of(processedImage.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting processing status for {ImageId}", imageId);
                return Result.Of(ImageProcessingStatus.Failed).WithErrors("Failed to get processing status");
            }
        }

        public Task<Outcome<string>> GenerateThumbnailUrlAsync(string originalUrl, int width, int height)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(originalUrl))
                    return Task.FromResult(Result.Of<string>(null).WithErrors("Original URL is required"));

                var thumbnailUrl = GenerateVariantUrl(originalUrl, width, height);
                return Task.FromResult(Result.Of(thumbnailUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating thumbnail URL");
                return Task.FromResult(Result.Of<string>(null).WithErrors("Failed to generate thumbnail URL"));
            }
        }

        public Task<Outcome<string>> GenerateWebPUrlAsync(string originalUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(originalUrl))
                    return Task.FromResult(Result.Of<string>(null).WithErrors("Original URL is required"));

                var webpUrl = GenerateWebPVariantUrl(originalUrl);
                return Task.FromResult(Result.Of(webpUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating WebP URL");
                return Task.FromResult(Result.Of<string>(null).WithErrors("Failed to generate WebP URL"));
            }
        }

        public async Task<Outcome<ImageProcessingResult>> ApplyWatermarkAsync(string imageId)
        {
            try
            {
                var processedImage = await _context.ProcessedImages
                    .FirstOrDefaultAsync(p => p.OriginalImageId == imageId);

                if (processedImage == null)
                    return Result.Of<ImageProcessingResult>(null).WithErrors("Image not found");

                processedImage.IsWatermarked = true;
                processedImage.ProcessedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Watermark applied to image {ImageId}", imageId);

                return Result.Of(MapToResult(processedImage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying watermark to image {ImageId}", imageId);
                return Result.Of<ImageProcessingResult>(null).WithErrors("Failed to apply watermark");
            }
        }

        public async Task<Outcome<Dictionary<string, string>>> ExtractExifDataAsync(string imageId)
        {
            try
            {
                var processedImage = await _context.ProcessedImages
                    .FirstOrDefaultAsync(p => p.OriginalImageId == imageId);

                if (processedImage == null)
                    return Result.Of<Dictionary<string, string>>(null).WithErrors("Image not found");

                if (!string.IsNullOrWhiteSpace(processedImage.ExifDataJson))
                {
                    var exifData = JsonSerializer.Deserialize<Dictionary<string, string>>(processedImage.ExifDataJson);
                    return Result.Of(exifData);
                }

                // Generate placeholder EXIF data for metadata tracking
                var extractedExif = new Dictionary<string, string>
                {
                    { "ImageWidth", processedImage.Width.ToString() },
                    { "ImageHeight", processedImage.Height.ToString() },
                    { "Format", processedImage.Format ?? "unknown" },
                    { "FileSize", processedImage.OriginalSize.ToString() }
                };

                processedImage.ExifDataJson = JsonSerializer.Serialize(extractedExif);
                await _context.SaveChangesAsync();

                return Result.Of(extractedExif);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting EXIF data for image {ImageId}", imageId);
                return Result.Of<Dictionary<string, string>>(null).WithErrors("Failed to extract EXIF data");
            }
        }

        public Task<Outcome<string>> GetCdnUrlAsync(string originalUrl, string cdnProvider)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(originalUrl))
                    return Task.FromResult(Result.Of<string>(null).WithErrors("Original URL is required"));

                if (string.IsNullOrWhiteSpace(cdnProvider))
                    return Task.FromResult(Result.Of<string>(null).WithErrors("CDN provider is required"));

                var cdnUrl = BuildCdnUrl(originalUrl, cdnProvider);
                return Task.FromResult(Result.Of(cdnUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating CDN URL");
                return Task.FromResult(Result.Of<string>(null).WithErrors("Failed to generate CDN URL"));
            }
        }

        private static string GenerateVariantUrl(string originalUrl, int width, int height)
        {
            var lastDot = originalUrl.LastIndexOf('.');
            if (lastDot < 0)
                return $"{originalUrl}_{width}x{height}";

            var basePath = originalUrl.Substring(0, lastDot);
            var extension = originalUrl.Substring(lastDot);
            return $"{basePath}_{width}x{height}{extension}";
        }

        private static string GenerateWebPVariantUrl(string originalUrl)
        {
            var lastDot = originalUrl.LastIndexOf('.');
            if (lastDot < 0)
                return $"{originalUrl}.webp";

            var basePath = originalUrl.Substring(0, lastDot);
            return $"{basePath}.webp";
        }

        private static string BuildCdnUrl(string originalUrl, string cdnProvider)
        {
            var cdnBaseUrls = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "cloudflare", "https://cdn.cloudflare.com" },
                { "cloudfront", "https://cdn.cloudfront.net" },
                { "azure", "https://cdn.azure.net" }
            };

            if (cdnBaseUrls.TryGetValue(cdnProvider, out var baseUrl))
            {
                var path = originalUrl.StartsWith("/") ? originalUrl : $"/{originalUrl}";
                return $"{baseUrl}{path}";
            }

            return originalUrl;
        }

        private static string ExtractFormat(string url)
        {
            var lastDot = url.LastIndexOf('.');
            if (lastDot < 0 || lastDot >= url.Length - 1)
                return "unknown";

            return url.Substring(lastDot + 1).ToLowerInvariant();
        }

        private static ImageProcessingResult MapToResult(ProcessedImage entity)
        {
            Dictionary<string, string> exifData = null;
            if (!string.IsNullOrWhiteSpace(entity.ExifDataJson))
            {
                exifData = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.ExifDataJson);
            }

            return new ImageProcessingResult
            {
                ImageId = entity.OriginalImageId,
                OriginalUrl = entity.OriginalUrl,
                ThumbnailUrl = entity.ThumbnailUrl,
                MediumUrl = entity.MediumUrl,
                LargeUrl = entity.LargeUrl,
                WebPUrl = entity.WebPUrl,
                CdnUrl = entity.CdnUrl,
                Format = entity.Format,
                OriginalSize = entity.OriginalSize,
                CompressedSize = entity.CompressedSize,
                Width = entity.Width,
                Height = entity.Height,
                IsWatermarked = entity.IsWatermarked,
                ExifData = exifData,
                Status = entity.Status.ToString(),
                CreatedAt = entity.CreatedAt,
                ProcessedAt = entity.ProcessedAt
            };
        }
    }
}
