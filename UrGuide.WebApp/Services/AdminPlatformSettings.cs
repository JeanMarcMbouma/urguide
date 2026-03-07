using UrGuide.Model.Admin;

namespace UrGuide.WebApp.Services
{
    /// <summary>
    /// In-memory singleton store for platform feature toggles and settings.
    /// Settings persist for the lifetime of the application process.
    /// </summary>
    public class AdminPlatformSettings
    {
        private readonly object _lock = new();

        private PlatformSettings _settings = new()
        {
            MaintenanceMode = false,
            RegistrationEnabled = true,
            GuideApplicationsEnabled = true,
            TourBookingEnabled = true,
            PaymentsEnabled = true,
            EmailNotificationsEnabled = true,
            PlatformFeePercentage = 10.0m,
            MaxImagesPerPost = 10,
            MinBookingDaysAdvance = 1
        };

        public PlatformSettings Get()
        {
            lock (_lock)
            {
                return new PlatformSettings
                {
                    MaintenanceMode = _settings.MaintenanceMode,
                    RegistrationEnabled = _settings.RegistrationEnabled,
                    GuideApplicationsEnabled = _settings.GuideApplicationsEnabled,
                    TourBookingEnabled = _settings.TourBookingEnabled,
                    PaymentsEnabled = _settings.PaymentsEnabled,
                    EmailNotificationsEnabled = _settings.EmailNotificationsEnabled,
                    PlatformFeePercentage = _settings.PlatformFeePercentage,
                    MaxImagesPerPost = _settings.MaxImagesPerPost,
                    MinBookingDaysAdvance = _settings.MinBookingDaysAdvance
                };
            }
        }

        public void Update(PlatformSettings newSettings)
        {
            lock (_lock)
            {
                _settings = new PlatformSettings
                {
                    MaintenanceMode = newSettings.MaintenanceMode,
                    RegistrationEnabled = newSettings.RegistrationEnabled,
                    GuideApplicationsEnabled = newSettings.GuideApplicationsEnabled,
                    TourBookingEnabled = newSettings.TourBookingEnabled,
                    PaymentsEnabled = newSettings.PaymentsEnabled,
                    EmailNotificationsEnabled = newSettings.EmailNotificationsEnabled,
                    PlatformFeePercentage = newSettings.PlatformFeePercentage,
                    MaxImagesPerPost = newSettings.MaxImagesPerPost,
                    MinBookingDaysAdvance = newSettings.MinBookingDaysAdvance
                };
            }
        }
    }
}
