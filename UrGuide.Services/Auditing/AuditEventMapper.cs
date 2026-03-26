using System;
using System.Collections.Generic;
using UrGuide.Data.Entities.Event;
using UrGuide.Model.Auditing;
using UrGuide.Services.Helpers;

namespace UrGuide.Services.Auditing
{
    static class AuditEventMapper
    {
        private static readonly Dictionary<EventCodes, string> EventDescriptions = new Dictionary<EventCodes, string>
        {
            { EventCodes.Login, "User logged in" },
            { EventCodes.Logout, "User logged out" },
            { EventCodes.FailedLogin, "Failed login attempt" },
            { EventCodes.PasswordChanged, "Password changed" },
            { EventCodes.PasswordReset, "Password reset" },
            { EventCodes.TwoFactorEnabled, "Two-factor authentication enabled" },
            { EventCodes.TwoFactorDisabled, "Two-factor authentication disabled" },
            { EventCodes.Register, "User has registered" },
            { EventCodes.DeleteAccount, "User deleted his account" },
            { EventCodes.ProfileUpdated, "Profile updated" },
            { EventCodes.EmailChanged, "Email address changed" },
            { EventCodes.RolesUpdated, "User roles updated {0}" },
            { EventCodes.EditPost, "User edited a post {0}" },
            { EventCodes.EditCatalog, "User edited a catalog {0}" },
            { EventCodes.DeletePost, "User deleted a post {0}" },
            { EventCodes.DeleteCatalog, "User deleted a catalog {0}" },
            { EventCodes.CreatePost, "User created a post {0}" },
            { EventCodes.CreateCalalog, "User created a catalog {0}" },
            { EventCodes.AccountFrozen, "Account frozen {0}" },
            { EventCodes.AccountUnfrozen, "Account unfrozen {0}" },
            { EventCodes.AccountSuspended, "Account suspended {0}" },
            { EventCodes.AccountActivated, "Account activated {0}" },
            { EventCodes.AccountDeleted, "Account deleted {0}" },
            { EventCodes.GuideVerificationApproved, "Guide verification approved {0}" },
            { EventCodes.GuideVerificationRejected, "Guide verification rejected {0}" },
            { EventCodes.TourApproved, "Tour approved {0}" },
            { EventCodes.TourRejected, "Tour rejected {0}" },
            { EventCodes.PaymentProcessed, "Payment processed {0}" },
            { EventCodes.RefundIssued, "Refund issued {0}" },
            { EventCodes.PayoutProcessed, "Payout processed {0}" },
            { EventCodes.SettingsUpdated, "Platform settings updated" },
            { EventCodes.Maintenance, "A service maintenance has occurred" }
        };

        public static ActivityModel ToActivityModel(AuditEvent source)
        {
            string description;
            if (EventDescriptions.TryGetValue(source.EventCode, out var desc))
            {
                description = desc.Contains("{0}")
                    ? string.Format(desc, source.ReferenceId)
                    : desc;
            }
            else
            {
                description = $"Event: {source.EventCode}";
            }

            return new ActivityModel
            {
                Event = description,
                When = DateTimeHelper.GetDateTime(source.Created, DateTimeKind.Utc)
            };
        }
    }
}
