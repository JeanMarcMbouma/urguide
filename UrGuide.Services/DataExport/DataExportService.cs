using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Data.Entities.Users;
using UrGuide.Model.Auditing;
using UrGuide.Model.Messages;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.DataExport
{
    public class DataExportService : IDataExportService
    {
        private const int ExpirationDays = 7;
        
        public DataExportService(
            UrGuideContext context,
            IUserContext userContext,
            ILogger<DataExportService> logger,
            IMapper mapper,
            IEmailService emailService,
            IConfiguration configuration,
            IWebHelper webHelper)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            EmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            WebHelper = webHelper ?? throw new ArgumentNullException(nameof(webHelper));
        }

        public UrGuideContext Context { get; }
        public IUserContext UserContext { get; }
        public ILogger<DataExportService> Logger { get; }
        public IMapper Mapper { get; }
        public IEmailService EmailService { get; }
        public IConfiguration Configuration { get; }
        public IWebHelper WebHelper { get; }

        public async Task<Result<DataExportResponse>> RequestExportAsync(DataExportRequestModel request, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<DataExportResponse>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // Check if there's already a pending or processing request
                var existingRequest = await Context.DataExportRequests
                    .Where(x => x.UserId == UserContext.UserId)
                    .Where(x => x.Status == DataExportStatus.Pending || x.Status == DataExportStatus.Processing)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingRequest != null)
                {
                    return Result.Of<DataExportResponse>().WithErrors("You already have a pending export request. Please wait for it to complete.");
                }

                // Parse format
                var format = ParseFormat(request.Format);

                // Create new export request
                var exportRequest = new DataExportRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = UserContext.UserId,
                    Status = DataExportStatus.Pending,
                    Format = format,
                    RequestedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(ExpirationDays)
                };

                Context.DataExportRequests.Add(exportRequest);
                await Context.SaveChangesAsync(cancellationToken);

                Logger.LogInformation("Data export requested: RequestId={RequestId}, UserId={UserId}, Format={Format}",
                    exportRequest.Id, UserContext.UserId, format);

                return Result.Of(MapToResponse(exportRequest));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to request data export for user {UserId}", UserContext.UserId);
                return Result.Of<DataExportResponse>().WithErrors("Failed to create export request. Please try again later.");
            }
        }

        public async Task<Result<DataExportResponse>> GetExportStatusAsync(string requestId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of<DataExportResponse>().WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var exportRequest = await Context.DataExportRequests
                    .FirstOrDefaultAsync(x => x.Id == requestId && x.UserId == UserContext.UserId, cancellationToken);

                if (exportRequest == null)
                {
                    return Result.Of<DataExportResponse>().WithErrors("Export request not found.");
                }

                return Result.Of(MapToResponse(exportRequest));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to get export status: RequestId={RequestId}", requestId);
                return Result.Of<DataExportResponse>().WithErrors("Failed to get export status.");
            }
        }

        public async Task<Result<(string FilePath, string FileName, long FileSize)>> DownloadExportAsync(string token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var exportRequest = await Context.DataExportRequests
                    .FirstOrDefaultAsync(x => x.DownloadToken == token, cancellationToken);

                if (exportRequest == null)
                {
                    return Result.Of<(string, string, long)>().WithErrors("Invalid or expired download link.");
                }

                // Check if expired
                if (exportRequest.Status == DataExportStatus.Expired || exportRequest.ExpiresAt < DateTime.UtcNow)
                {
                    return Result.Of<(string, string, long)>().WithErrors("This download link has expired.");
                }

                // Check if completed
                if (exportRequest.Status != DataExportStatus.Completed)
                {
                    return Result.Of<(string, string, long)>().WithErrors("Export is not yet available for download.");
                }

                // Check if file exists
                if (string.IsNullOrEmpty(exportRequest.FilePath) || !File.Exists(exportRequest.FilePath))
                {
                    Logger.LogWarning("Export file not found: RequestId={RequestId}, FilePath={FilePath}",
                        exportRequest.Id, exportRequest.FilePath);
                    return Result.Of<(string, string, long)>().WithErrors("Export file not found.");
                }

                var fileName = $"urguide-data-export-{exportRequest.RequestedAt:yyyyMMdd}.{GetFileExtension(exportRequest.Format)}";
                var fileSize = exportRequest.FileSizeBytes ?? new FileInfo(exportRequest.FilePath).Length;

                return Result.Of((exportRequest.FilePath, fileName, fileSize));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to download export: Token={Token}", token);
                return Result.Of<(string, string, long)>().WithErrors("Failed to download export.");
            }
        }

        public async Task<Result<bool>> CancelExportAsync(string requestId, CancellationToken cancellationToken)
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var exportRequest = await Context.DataExportRequests
                    .FirstOrDefaultAsync(x => x.Id == requestId && x.UserId == UserContext.UserId, cancellationToken);

                if (exportRequest == null)
                {
                    return Result.Of(false).WithErrors("Export request not found.");
                }

                if (exportRequest.Status != DataExportStatus.Pending && exportRequest.Status != DataExportStatus.Processing)
                {
                    return Result.Of(false).WithErrors("Cannot cancel a completed or failed export.");
                }

                Context.DataExportRequests.Remove(exportRequest);
                await Context.SaveChangesAsync(cancellationToken);

                Logger.LogInformation("Data export cancelled: RequestId={RequestId}, UserId={UserId}",
                    requestId, UserContext.UserId);

                return Result.Of(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to cancel export: RequestId={RequestId}", requestId);
                return Result.Of(false).WithErrors("Failed to cancel export request.");
            }
        }

        public async Task<int> ProcessPendingExportsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var pendingExports = await Context.DataExportRequests
                    .Include(x => x.User)
                    .Where(x => x.Status == DataExportStatus.Pending)
                    .Take(10) // Process max 10 at a time
                    .ToListAsync(cancellationToken);

                if (!pendingExports.Any())
                {
                    return 0;
                }

                Logger.LogInformation("Processing {Count} pending data exports", pendingExports.Count);

                foreach (var exportRequest in pendingExports)
                {
                    try
                    {
                        await ProcessExportAsync(exportRequest, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Failed to process export: RequestId={RequestId}", exportRequest.Id);
                        exportRequest.Status = DataExportStatus.Failed;
                    }
                }

                await Context.SaveChangesAsync(cancellationToken);
                return pendingExports.Count;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to process pending exports");
                return 0;
            }
        }

        public async Task<int> CleanupExpiredExportsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var expiredExports = await Context.DataExportRequests
                    .Where(x => x.ExpiresAt < DateTime.UtcNow && x.Status != DataExportStatus.Expired)
                    .ToListAsync(cancellationToken);

                if (!expiredExports.Any())
                {
                    return 0;
                }

                Logger.LogInformation("Cleaning up {Count} expired data exports", expiredExports.Count);

                foreach (var exportRequest in expiredExports)
                {
                    try
                    {
                        // Delete the file if it exists
                        if (!string.IsNullOrEmpty(exportRequest.FilePath) && File.Exists(exportRequest.FilePath))
                        {
                            File.Delete(exportRequest.FilePath);
                            Logger.LogInformation("Deleted expired export file: {FilePath}", exportRequest.FilePath);
                        }

                        exportRequest.Status = DataExportStatus.Expired;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Failed to delete export file: RequestId={RequestId}, FilePath={FilePath}",
                            exportRequest.Id, exportRequest.FilePath);
                    }
                }

                await Context.SaveChangesAsync(cancellationToken);
                return expiredExports.Count;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to cleanup expired exports");
                return 0;
            }
        }

        private async Task ProcessExportAsync(DataExportRequest exportRequest, CancellationToken cancellationToken)
        {
            exportRequest.Status = DataExportStatus.Processing;
            await Context.SaveChangesAsync(cancellationToken);

            // Collect user data
            var userData = await CollectUserDataAsync(exportRequest.UserId, cancellationToken);

            // Generate export file
            var exportPath = GetExportPath();
            Directory.CreateDirectory(exportPath);

            string filePath;
            if (exportRequest.Format == DataExportFormat.Json)
            {
                filePath = await GenerateJsonExportAsync(userData, exportPath, exportRequest.UserId, cancellationToken);
            }
            else
            {
                filePath = await GenerateCsvExportAsync(userData, exportPath, exportRequest.UserId, cancellationToken);
            }

            // Generate secure download token
            var downloadToken = GenerateSecureToken();

            // Update export request
            exportRequest.Status = DataExportStatus.Completed;
            exportRequest.CompletedAt = DateTime.UtcNow;
            exportRequest.FilePath = filePath;
            exportRequest.FileSizeBytes = new FileInfo(filePath).Length;
            exportRequest.DownloadToken = downloadToken;

            await Context.SaveChangesAsync(cancellationToken);

            // Send email notification
            await SendExportReadyEmailAsync(exportRequest, cancellationToken);

            Logger.LogInformation("Data export completed: RequestId={RequestId}, UserId={UserId}, Size={Size} bytes",
                exportRequest.Id, exportRequest.UserId, exportRequest.FileSizeBytes);
        }

        private async Task<UserDataExport> CollectUserDataAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await Context.Users
                .Include(u => u.ProfileImage)
                .Include(u => u.Attributes)
                .Include(u => u.Feedback)
                .Include(u => u.Notifications)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException($"User not found: {userId}");
            }

            var export = new UserDataExport
            {
                ExportDate = DateTime.UtcNow,
                ExportVersion = "1.0",
                Profile = new UserInfo
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName?.ToString(),
                    City = user.Attributes.FirstOrDefault(a => a.Name == "City")?.Value,
                    Country = user.Attributes.FirstOrDefault(a => a.Name == "Country")?.Value,
                    Description = user.Attributes.FirstOrDefault(a => a.Name == "Description")?.Value,
                    ProfileImage = user.ProfileImage?.ImageUrl
                },
                Attributes = user.Attributes.ToDictionary(a => a.Name, a => a.Value),
                Account = new AccountMetadata
                {
                    UserId = user.Id,
                    Email = user.Email,
                    LastActivityDate = user.LastActivityDate,
                    CreatedDate = ParseDateAttribute(user.Attributes.FirstOrDefault(a => a.Name == "CreatedDate")?.Value),
                    IsGuide = user.Attributes.Any(a => a.Name == "IsGuide" && a.Value == "True"),
                    IsPremium = user.Attributes.Any(a => a.Name == "IsPremium" && a.Value == "True")
                }
            };

            // Get given feedback (feedback authored by this user)
            var givenFeedback = await Context.Posts
                .Include(p => p.Feedback)
                    .ThenInclude(f => f.Author)
                .Where(p => p.Feedback.Any(f => f.Author.Id == userId))
                .SelectMany(p => p.Feedback.Where(f => f.Author.Id == userId))
                .ToListAsync(cancellationToken);

            export.GivenFeedback = givenFeedback.Select(f => new AuthoredFeedback
            {
                Text = f.Text,
                Rating = f.Rating,
                PublicationDate = f.Created.ToString("O"),
                AuthorId = userId,
                AuthorFullName = user.FullName?.ToString()
            }).ToList();

            // Get received feedback (feedback on posts owned by this user)
            var receivedFeedback = await Context.Posts
                .Include(p => p.Feedback)
                    .ThenInclude(f => f.Author)
                .Include(p => p.User)
                .Where(p => p.User.Id == userId)
                .SelectMany(p => p.Feedback)
                .ToListAsync(cancellationToken);

            export.ReceivedFeedback = receivedFeedback.Select(f => new AuthoredFeedback
            {
                Text = f.Text,
                Rating = f.Rating,
                PublicationDate = f.Created.ToString("O"),
                AuthorId = f.Author?.Id,
                AuthorFullName = $"{f.Author?.FirstName} {f.Author?.LastName}",
                AuthorImage = f.Author?.ProfileImage?.ImageUrl
            }).ToList();

            // Get activity history
            var activities = await Context.AuditEvents
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Created)
                .Take(1000) // Limit to last 1000 activities
                .ToListAsync(cancellationToken);

            export.ActivityHistory = activities.Select(a => new
            {
                EventCode = a.EventCode.ToString(),
                a.ReferenceId,
                Created = a.Created
            }).Cast<object>().ToList();

            // Get bids
            var bids = await Context.Posts
                .Include(p => p.Bid)
                    .ThenInclude(b => b.Author)
                .Where(p => p.Bid != null && p.Bid.Author.Id == userId)
                .Select(p => new
                {
                    PostId = p.Id,
                    PostTitle = p.Text,
                    BidValue = p.Bid.NewValue,
                    p.Bid.LastUpdated
                })
                .ToListAsync(cancellationToken);

            export.Bids = bids.Cast<object>().ToList();

            // Get tour requests
            var tourRequests = await Context.TourRequests
                .Include(t => t.Region)
                .Where(t => t.RequesterId == userId)
                .Select(t => new
                {
                    t.TourRequestId,
                    t.Title,
                    t.Description,
                    t.PreferredDate,
                    t.MaxParticipants,
                    t.MaxBudget,
                    t.Tags,
                    Status = t.Status.ToString(),
                    Region = t.Region.Name,
                    t.CreatedAt
                })
                .ToListAsync(cancellationToken);

            export.TourRequests = tourRequests.Cast<object>().ToList();

            // Get posts
            var posts = await Context.Posts
                .Include(p => p.Catalog)
                .Where(p => p.User.Id == userId)
                .Select(p => new
                {
                    p.Id,
                    Title = p.Text,
                    p.Description,
                    p.Cost,
                    p.StartDate,
                    p.EndDate,
                    p.AllocatedSeats,
                    p.ReservedSeats,
                    p.Rating,
                    p.Reviews,
                    p.Likes,
                    p.Dislikes,
                    p.Tags,
                    p.DateOfPublication
                })
                .ToListAsync(cancellationToken);

            export.Posts = posts.Cast<object>().ToList();

            // Get galleries/catalogs
            var galleries = await Context.ImageCatalogs
                .Include(c => c.Images)
                .Where(c => c.User.Id == userId)
                .Select(c => new
                {
                    c.Id,
                    Title = c.Attributes.FirstOrDefault(a => a.Name == "Title").Value ?? string.Empty,
                    Description = c.Attributes.FirstOrDefault(a => a.Name == "Description").Value ?? string.Empty,
                    ImageCount = c.Images.Count,
                    c.Created
                })
                .ToListAsync(cancellationToken);

            export.Galleries = galleries.Cast<object>().ToList();

            // Get notifications
            export.Notifications = user.Notifications.Select(n => new
            {
                n.Id,
                n.Content,
                n.Read,
                n.Created
            }).Cast<object>().ToList();

            return export;
        }

        private async Task<string> GenerateJsonExportAsync(UserDataExport userData, string exportPath, string userId, CancellationToken cancellationToken)
        {
            var fileName = $"user-data-{userId}-{DateTime.UtcNow:yyyyMMddHHmmss}.json";
            var filePath = Path.Combine(exportPath, fileName);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(userData, options);
            await File.WriteAllTextAsync(filePath, json, cancellationToken);

            return filePath;
        }

        private async Task<string> GenerateCsvExportAsync(UserDataExport userData, string exportPath, string userId, CancellationToken cancellationToken)
        {
            // For CSV, we'll create a ZIP file with multiple CSV files
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var zipFileName = $"user-data-{userId}-{timestamp}.zip";
            var zipFilePath = Path.Combine(exportPath, zipFileName);
            
            // Create a temporary directory for CSV files
            var tempDir = Path.Combine(exportPath, $"temp-{userId}-{timestamp}");
            Directory.CreateDirectory(tempDir);

            try
            {
                // Generate individual CSV files
                await GenerateProfileCsvAsync(userData, tempDir, cancellationToken);
                await GenerateActivityCsvAsync(userData, tempDir, cancellationToken);
                await GenerateFeedbackCsvAsync(userData, tempDir, cancellationToken);
                await GenerateToursCsvAsync(userData, tempDir, cancellationToken);
                await GenerateBidsCsvAsync(userData, tempDir, cancellationToken);

                // Create ZIP file
                System.IO.Compression.ZipFile.CreateFromDirectory(tempDir, zipFilePath);

                return zipFilePath;
            }
            finally
            {
                // Clean up temporary directory
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        private async Task GenerateProfileCsvAsync(UserDataExport userData, string outputDir, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(outputDir, "profile.csv");
            var csv = new StringBuilder();
            csv.AppendLine("Field,Value");
            csv.AppendLine($"UserId,\"{userData.Account.UserId}\"");
            csv.AppendLine($"Email,\"{userData.Account.Email}\"");
            csv.AppendLine($"FirstName,\"{userData.Profile.FirstName}\"");
            csv.AppendLine($"LastName,\"{userData.Profile.LastName}\"");
            csv.AppendLine($"City,\"{userData.Profile.City}\"");
            csv.AppendLine($"Country,\"{userData.Profile.Country}\"");
            csv.AppendLine($"Description,\"{EscapeCsv(userData.Profile.Description)}\"");
            csv.AppendLine($"IsGuide,\"{userData.Account.IsGuide}\"");
            csv.AppendLine($"IsPremium,\"{userData.Account.IsPremium}\"");
            csv.AppendLine($"CreatedDate,\"{userData.Account.CreatedDate:O}\"");
            csv.AppendLine($"LastActivityDate,\"{userData.Account.LastActivityDate:O}\"");

            await File.WriteAllTextAsync(filePath, csv.ToString(), cancellationToken);
        }

        private async Task GenerateActivityCsvAsync(UserDataExport userData, string outputDir, CancellationToken cancellationToken)
        {
            if (!userData.ActivityHistory.Any()) return;

            var filePath = Path.Combine(outputDir, "activity_history.csv");
            var csv = new StringBuilder();
            csv.AppendLine("EventCode,ReferenceId,Created");

            foreach (dynamic activity in userData.ActivityHistory)
            {
                csv.AppendLine($"\"{activity.EventCode}\",\"{activity.ReferenceId}\",\"{activity.Created:O}\"");
            }

            await File.WriteAllTextAsync(filePath, csv.ToString(), cancellationToken);
        }

        private async Task GenerateFeedbackCsvAsync(UserDataExport userData, string outputDir, CancellationToken cancellationToken)
        {
            // Given feedback
            if (userData.GivenFeedback.Any())
            {
                var filePath = Path.Combine(outputDir, "feedback_given.csv");
                var csv = new StringBuilder();
                csv.AppendLine("Text,Rating,PublicationDate");

                foreach (var feedback in userData.GivenFeedback)
                {
                    csv.AppendLine($"\"{EscapeCsv(feedback.Text)}\",{feedback.Rating},\"{feedback.PublicationDate}\"");
                }

                await File.WriteAllTextAsync(filePath, csv.ToString(), cancellationToken);
            }

            // Received feedback
            if (userData.ReceivedFeedback.Any())
            {
                var filePath = Path.Combine(outputDir, "feedback_received.csv");
                var csv = new StringBuilder();
                csv.AppendLine("Text,Rating,AuthorName,PublicationDate");

                foreach (var feedback in userData.ReceivedFeedback)
                {
                    csv.AppendLine($"\"{EscapeCsv(feedback.Text)}\",{feedback.Rating},\"{feedback.AuthorFullName}\",\"{feedback.PublicationDate}\"");
                }

                await File.WriteAllTextAsync(filePath, csv.ToString(), cancellationToken);
            }
        }

        private async Task GenerateToursCsvAsync(UserDataExport userData, string outputDir, CancellationToken cancellationToken)
        {
            // Posts (tours)
            if (userData.Posts.Any())
            {
                var filePath = Path.Combine(outputDir, "tours_posts.csv");
                var csv = new StringBuilder();
                csv.AppendLine("Id,Title,Description,Cost,StartDate,EndDate,AllocatedSeats,ReservedSeats,Rating,Reviews,Likes,Dislikes,Tags,DateOfPublication");

                foreach (dynamic post in userData.Posts)
                {
                    csv.AppendLine($"\"{post.Id}\",\"{EscapeCsv(post.Title)}\",\"{EscapeCsv(post.Description)}\",\"{post.Cost}\",\"{post.StartDate:O}\",\"{post.EndDate:O}\",{post.AllocatedSeats},{post.ReservedSeats},{post.Rating},{post.Reviews},{post.Likes},{post.Dislikes},\"{post.Tags}\",\"{post.DateOfPublication:O}\"");
                }

                await File.WriteAllTextAsync(filePath, csv.ToString(), cancellationToken);
            }

            // Tour requests
            if (userData.TourRequests.Any())
            {
                var filePath = Path.Combine(outputDir, "tour_requests.csv");
                var csv = new StringBuilder();
                csv.AppendLine("TourRequestId,Title,Description,PreferredDate,MaxParticipants,MaxBudget,Tags,Status,Region,CreatedAt");

                foreach (dynamic request in userData.TourRequests)
                {
                    csv.AppendLine($"\"{request.TourRequestId}\",\"{EscapeCsv(request.Title)}\",\"{EscapeCsv(request.Description)}\",\"{request.PreferredDate:O}\",{request.MaxParticipants},{request.MaxBudget},\"{request.Tags}\",\"{request.Status}\",\"{request.Region}\",\"{request.CreatedAt:O}\"");
                }

                await File.WriteAllTextAsync(filePath, csv.ToString(), cancellationToken);
            }
        }

        private async Task GenerateBidsCsvAsync(UserDataExport userData, string outputDir, CancellationToken cancellationToken)
        {
            if (!userData.Bids.Any()) return;

            var filePath = Path.Combine(outputDir, "bids.csv");
            var csv = new StringBuilder();
            csv.AppendLine("PostId,PostTitle,BidValue,LastUpdated");

            foreach (dynamic bid in userData.Bids)
            {
                csv.AppendLine($"\"{bid.PostId}\",\"{EscapeCsv(bid.PostTitle)}\",\"{bid.BidValue}\",\"{bid.LastUpdated:O}\"");
            }

            await File.WriteAllTextAsync(filePath, csv.ToString(), cancellationToken);
        }

        private async Task SendExportReadyEmailAsync(DataExportRequest exportRequest, CancellationToken cancellationToken)
        {
            try
            {
                var user = await Context.Users.FindAsync(new[] { exportRequest.UserId }, cancellationToken);
                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    Logger.LogWarning("Cannot send export ready email: User not found or email missing: UserId={UserId}", exportRequest.UserId);
                    return;
                }

                var downloadUrl = $"{GetBaseUrl()}/api/dataexport/download/{exportRequest.DownloadToken}";
                var expiryDate = exportRequest.ExpiresAt.ToString("MMMM dd, yyyy");

                var message = new SendDirectMessageCommand
                {
                    To = user.Email,
                    ToName = user.FullName?.ToString() ?? user.Email,
                    Subject = "Your Data Export is Ready",
                    Content = $@"
Your data export request has been completed and is now ready for download.

The export includes:
- Your profile information
- Activity history
- Reviews and feedback
- Tour history
- Posts and galleries

Download your data using the secure link below. This link will expire on {expiryDate}.
",
                    Link = downloadUrl,
                    LinkText = "Download My Data"
                };

                await EmailService.SendAsync(message);

                Logger.LogInformation("Export ready email sent: UserId={UserId}, RequestId={RequestId}",
                    exportRequest.UserId, exportRequest.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to send export ready email: RequestId={RequestId}", exportRequest.Id);
            }
        }

        private DataExportResponse MapToResponse(DataExportRequest request)
        {
            var response = new DataExportResponse
            {
                RequestId = request.Id,
                Status = request.Status.ToString(),
                Format = request.Format.ToString(),
                RequestedAt = request.RequestedAt,
                CompletedAt = request.CompletedAt,
                ExpiresAt = request.ExpiresAt,
                FileSizeBytes = request.FileSizeBytes
            };

            if (request.Status == DataExportStatus.Completed && !string.IsNullOrEmpty(request.DownloadToken))
            {
                response.DownloadUrl = $"{GetBaseUrl()}/api/dataexport/download/{request.DownloadToken}";
            }

            return response;
        }

        private DataExportFormat ParseFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                return DataExportFormat.Json;

            return format.ToUpperInvariant() switch
            {
                "JSON" => DataExportFormat.Json,
                "CSV" => DataExportFormat.Csv,
                _ => DataExportFormat.Json
            };
        }

        private string GetFileExtension(DataExportFormat format)
        {
            return format switch
            {
                DataExportFormat.Json => "json",
                DataExportFormat.Csv => "zip",
                _ => "json"
            };
        }

        private string GetExportPath()
        {
            var basePath = Configuration.GetSection("DataExport:BasePath")?.Value ?? Path.Combine(Path.GetTempPath(), "urguide-exports");
            return basePath;
        }
        
        private string GetBaseUrl()
        {
            // Get base URL from configuration or use default
            var baseUrl = Configuration.GetSection("IdentityServer:ApplicationUri")?.Value ?? "https://localhost:5001";
            return baseUrl;
        }

        private string GenerateSecureToken()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("\"", "\"\"");
        }

        private DateTime ParseDateAttribute(string value)
        {
            if (string.IsNullOrEmpty(value))
                return DateTime.MinValue;

            return DateTime.TryParse(value, out var date) ? date : DateTime.MinValue;
        }
    }
}
