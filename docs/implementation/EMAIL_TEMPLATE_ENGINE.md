# Email Template Engine

## Overview

UrGuide uses a **proprietary database-backed email template engine** rather than an external third-party service such as SendGrid. All email templates are stored in the database and managed through the admin dashboard. The engine supports full multi-language localisation.

---

## Architecture

```
Admin Dashboard ──── CRUD ────► EmailTemplate (DB)
                                     │
                                     │ RenderEmailAsync
                                     ▼
                           IEmailTemplateService
                                     │
                                     │ rendered subject + HTML body
                                     ▼
                             IEmailService.SendAsync
                                     │
                                     │ SMTP
                                     ▼
                               Recipient Inbox
```

### Key Components

| Component | Location | Responsibility |
|---|---|---|
| `EmailTemplate` entity | `UrGuide.Data/Entities/Email/` | Persisted template (subject, HTML, plain-text, language) |
| `EmailTemplateVersion` entity | `UrGuide.Data/Entities/Email/` | Immutable version history per template |
| `IEmailTemplateService` | `UrGuide.Services/Email/` | CRUD, preview, rendering, version retrieval |
| `EmailTemplateService` | `UrGuide.Services/Email/` | Concrete implementation against EF Core |
| `EmailTemplateController` | `UrGuide.WebApp/Controllers/` | REST API consumed by the admin dashboard |
| `IEmailService` | `UrGuide.Shared/Contracts/` | Send email (raw content or rendered template) |
| `EmailService` | `UrGuide.WebApp/Services/` | MailKit SMTP delivery; delegates rendering to `IEmailTemplateService` |

---

## Template Syntax

Templates use **double-brace substitution** (`{{VariableName}}`). All three fields support variables:

| Field | Example |
|---|---|
| Subject | `Welcome to UrGuide, {{ToName}}!` |
| HtmlBody | `<p>Hello <strong>{{ToName}}</strong>, please <a href="{{Link}}">{{LinkText}}</a>.</p>` |
| PlainTextBody | `Hello {{ToName}}, please visit: {{Link}}` |

### Built-in Variables

The following variables are automatically populated from `SendDirectMessageCommand` when a template is rendered:

| Variable | Source |
|---|---|
| `{{ToName}}` | `SendDirectMessageCommand.ToName` |
| `{{Link}}` | `SendDirectMessageCommand.Link` |
| `{{LinkText}}` | `SendDirectMessageCommand.LinkText` |
| `{{Content}}` | `SendDirectMessageCommand.Content` |

Additional variables can be supplied via `SendDirectMessageCommand.TemplateVariables`.

---

## Multi-Language Support

Every template record has a `Language` field (BCP 47 tag, e.g. `"en"`, `"fr"`, `"es"`, `"de"`, `"ar"`). The rendering algorithm:

1. Looks up the template by `Name` **and** `Language`.
2. If no match is found it **falls back to the English (`"en"`) variant**.
3. If neither exists it returns an error and the caller falls back to raw `Content`.

Callers should populate `SendDirectMessageCommand.Language` with the recipient's preferred language, for example derived from the user's profile or the `Accept-Language` header.

---

## SMTP Configuration

Email delivery uses **MailKit** (v4+), a modern, actively maintained SMTP library. MailKit provides full async support, proper TLS handling, and is the recommended replacement for the obsolete `System.Net.Mail.SmtpClient`. Configure SMTP via `appsettings.json` or environment variables:

```json
{
  "Smtp": {
    "Host": "mail.example.com",
    "Port": 587,
    "EnableSsl": true,
    "FromEmail": "noreply@urguide.org",
    "FromName": "UrGuide"
  }
}
```

| Setting | Environment variable | Default |
|---|---|---|
| `Smtp:Host` | `Smtp__Host` | `localhost` |
| `Smtp:Port` | `Smtp__Port` | `587` |
| `Smtp:EnableSsl` | `Smtp__EnableSsl` | `true` |
| `Smtp:Username` | `Smtp__Username` | *(empty – unauthenticated relay)* |
| `Smtp:Password` | `Smtp__Password` | *(empty)* |
| `Smtp:FromEmail` | `Smtp__FromEmail` | `noreply@urguide.org` |
| `Smtp:FromName` | `Smtp__FromName` | `UrGuide` |

> **Security**: Never commit credentials to source control. Use environment variables or Docker secrets.

---

## Admin Dashboard API

All endpoints require authentication (`[Authorize]`).

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/email-templates` | Create a new template |
| `GET` | `/api/email-templates` | List templates (supports `?page`, `?pageSize`, `?category`, `?language`) |
| `GET` | `/api/email-templates/{id}` | Retrieve a specific template |
| `PUT` | `/api/email-templates/{id}` | Update a template (creates a new version automatically) |
| `DELETE` | `/api/email-templates/{id}` | Deactivate a template (soft-delete) |
| `POST` | `/api/email-templates/preview` | Preview rendered output with variable substitution |
| `GET` | `/api/email-templates/{id}/versions` | List version history for a template |

### Create Template – Request Body

```json
{
  "name": "password_reset",
  "subject": "Reset your password, {{ToName}}",
  "htmlBody": "<p>Click <a href=\"{{Link}}\">{{LinkText}}</a> to reset your password.</p>",
  "plainTextBody": "Visit {{Link}} to reset your password.",
  "category": "account",
  "language": "en",
  "variables": ["ToName", "Link", "LinkText"]
}
```

### Preview Request

```json
{
  "templateId": "<template-guid>",
  "variables": {
    "ToName": "Alice",
    "Link": "https://urguide.org/reset?token=abc123",
    "LinkText": "Reset password"
  }
}
```

---

## Sending Email with a Template

```csharp
await emailService.SendAsync(new SendDirectMessageCommand
{
    To      = user.Email,
    ToName  = user.FirstName,
    Subject = "Password reset",          // Overridden by the template's subject
    TemplateName = "password_reset",
    Language     = user.PreferredLanguage ?? "en",
    Link     = resetUrl,
    LinkText = "Reset your password"
});
```

If the named template is not found for the requested language the engine falls back to English; if no English template exists it logs a warning and delivers the raw `Content` value instead.

---

## Seeding Default Templates

To seed initial templates for a fresh installation, add a migration or use the admin dashboard. A recommended baseline set of templates:

| Name | Category | Description |
|---|---|---|
| `email_confirmation` | `account` | Sent after registration |
| `password_reset` | `account` | Sent on password-reset request |
| `password_changed` | `account` | Notification after successful password change |
| `password_change_attempt` | `account` | Security alert for failed change attempt |
| `tour_booking_confirmed` | `bookings` | Booking confirmation to tourist |
| `tour_cancelled` | `bookings` | Cancellation notification |

Each template should be created in every supported language (`en`, `fr`, `es`, `de`, `ar`).

---

## Testing

The template engine is covered by:

- **Unit tests** (`tests/UrGuide.UnitTests/Services/EmailTemplateServiceTests.cs`) – 20 tests covering CRUD, variable substitution, language fallback, version history, and deactivation.
- **Integration tests** (`tests/UrGuide.IntegrationTests/Controllers/EmailTemplateControllerTests.cs`) – 14 tests covering HTTP responses for all controller actions.

Run tests:

```bash
dotnet test tests/UrGuide.UnitTests/UrGuide.UnitTests.csproj
dotnet test tests/UrGuide.IntegrationTests/UrGuide.IntegrationTests.csproj
```
