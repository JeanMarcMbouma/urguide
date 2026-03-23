using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using UrGuide.WebApp.Resources;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("api/localization")]
    public class LocalizationController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private static readonly string[] _supportedLanguages = ["en", "fr", "es", "de", "ar"];

        // ResourceManager for culture-specific lookups without touching ambient culture
        private static readonly ResourceManager _resourceManager = new ResourceManager(
            "UrGuide.WebApp.Resources.SharedResource",
            typeof(SharedResource).Assembly);

        public LocalizationController(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// Returns the list of supported languages.
        /// </summary>
        [HttpGet("languages")]
        [AllowAnonymous]
        public IActionResult GetSupportedLanguages()
        {
            var languages = new[]
            {
                new { code = "en", name = "English",    nativeName = "English"   },
                new { code = "fr", name = "French",     nativeName = "Français"  },
                new { code = "es", name = "Spanish",    nativeName = "Español"   },
                new { code = "de", name = "German",     nativeName = "Deutsch"   },
                new { code = "ar", name = "Arabic",     nativeName = "العربية"   },
            };
            return Ok(languages);
        }

        /// <summary>
        /// Returns all translation strings for a specific language.
        /// Admin-only endpoint for managing translations.
        /// </summary>
        [HttpGet("{language}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetTranslations(string language)
        {
            if (!_supportedLanguages.Contains(language))
                return BadRequest(new { error = $"Language '{language}' is not supported. Supported languages: {string.Join(", ", _supportedLanguages)}" });

            var requestedCulture = new CultureInfo(language);
            var resourceKeys = GetAllResourceKeys();
            var translations = LookupTranslations(resourceKeys, requestedCulture);

            return Ok(new
            {
                language,
                culture = requestedCulture.DisplayName,
                translations
            });
        }

        /// <summary>
        /// Returns all translations for all supported languages.
        /// Admin-only endpoint.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllTranslations()
        {
            var resourceKeys = GetAllResourceKeys();
            var translations = _supportedLanguages.ToDictionary(
                lang => lang,
                lang => LookupTranslations(resourceKeys, new CultureInfo(lang)));

            return Ok(new
            {
                supportedLanguages = _supportedLanguages,
                translations
            });
        }

        /// <summary>
        /// Looks up translations for the given keys in the given culture using ResourceManager
        /// directly, without modifying any ambient/thread culture.
        /// </summary>
        private static Dictionary<string, string?> LookupTranslations(
            IEnumerable<string> keys,
            CultureInfo culture)
        {
            return keys.ToDictionary(
                key => key,
                key => _resourceManager.GetString(key, culture));
        }

        /// <summary>
        /// Returns all resource keys defined in SharedResource.resx.
        /// </summary>
        private static List<string> GetAllResourceKeys()
        {
            // The default (English) resource file is the reference; retrieve all its data names
            // via the IStringLocalizer which exposes GetAllStrings on the neutral culture.
            var resourceType = typeof(SharedResource);
            var baseName = $"{resourceType.Namespace}.{resourceType.Name}";
            var assembly = resourceType.Assembly;

            using var stream = assembly.GetManifestResourceStream($"{baseName}.resources");
            if (stream != null)
            {
                using var reader = new System.Resources.ResourceReader(stream);
                var keys = new List<string>();
                var enumerator = reader.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    keys.Add(enumerator.Key!.ToString()!);
                }
                return keys;
            }

            // Fallback: return the well-known keys explicitly
            return
            [
                "Error_NotFound", "Error_Unauthorized", "Error_Forbidden", "Error_BadRequest",
                "Error_InternalServer", "Error_ValidationFailed", "Error_Conflict",
                "Auth_InvalidCredentials", "Auth_AccountLocked", "Auth_EmailNotConfirmed",
                "Auth_TokenExpired", "Auth_TokenInvalid", "Auth_PasswordMismatch",
                "Auth_EmailAlreadyRegistered", "Auth_TwoFactorRequired", "Auth_TwoFactorInvalid",
                "User_NotFound", "User_ProfileUpdateFailed", "User_PasswordChangeFailed",
                "Tour_NotFound", "Tour_AlreadyBooked", "Tour_Expired", "Tour_CapacityExceeded",
                "Bid_NotFound", "Bid_AlreadyPlaced", "Bid_CannotWithdraw",
                "Payment_Failed", "Payment_NotFound", "Payment_AlreadyProcessed",
                "Payment_InsufficientFunds", "Payout_RequestFailed", "Refund_Failed",
                "Review_NotFound", "Review_AlreadySubmitted",
                "File_UploadFailed", "File_InvalidFormat", "File_TooLarge",
                "EmailTemplate_NotFound", "EmailTemplate_RenderFailed",
                "Success_ProfileUpdated", "Success_PasswordChanged", "Success_EmailSent",
                "Success_BidPlaced", "Success_BidWithdrawn",
            ];
        }
    }
}
