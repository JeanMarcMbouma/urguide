namespace UrGuide.Data.Entities.Event
{
    public enum EventCodes
    {
        Login = 1000,
        Logout = 1001,
        FailedLogin = 1002,
        PasswordChanged = 1003,
        PasswordReset = 1004,
        TwoFactorEnabled = 1005,
        TwoFactorDisabled = 1006,
        Register = 2000,
        DeleteAccount = 2001,
        ProfileUpdated = 2002,
        EmailChanged = 2003,
        RolesUpdated = 2004,
        CreatePost = 3000,
        EditPost = 3001,
        EditCatalog = 3002,
        DeleteCatalog = 3003,
        DeletePost = 3004,
        CreateCalalog = 3005,
        AccountFrozen = 4000,
        AccountUnfrozen = 4001,
        AccountSuspended = 4002,
        AccountActivated = 4003,
        AccountDeleted = 4004,
        GuideVerificationApproved = 5000,
        GuideVerificationRejected = 5001,
        TourApproved = 5002,
        TourRejected = 5003,
        PaymentProcessed = 6000,
        RefundIssued = 6001,
        PayoutProcessed = 6002,
        SettingsUpdated = 7000,
        Maintenance = 10000
    }
}