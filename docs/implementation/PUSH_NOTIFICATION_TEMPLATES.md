# Push Notification Templates

Reusable push notification templates with variable substitution, multi-language support, automatic versioning, and A/B testing via variant groups.

## Overview

Templates allow admins to define consistent notification content once and reuse it across the platform. When `SendPushNotificationRequest.TemplateId` is set, the `PushNotificationService` automatically resolves the template, substitutes variables, and uses the rendered title/body.

## Variable Substitution

Use `{{variable_name}}` placeholders in `TitleTemplate` and `BodyTemplate`. Variables are provided as a `Dictionary<string, string>` in `TemplateVariables` on the send request.

**Example:**
```
TitleTemplate: "New booking for {{tour_name}}!"
BodyTemplate:  "Hi {{guide_name}}, {{tourist_name}} has confirmed the tour on {{date}}."

Variables: { "tour_name": "Paris Walking Tour", "guide_name": "Jean", "tourist_name": "Alice", "date": "Dec 25" }
```

Unresolved placeholders (variable not supplied) are left as-is in the rendered output.

## Multi-Language Support

Each `Name` + `Language` combination is an independent template record. When sending a notification, the service can look up the template by name and language. If the requested language is not available, it falls back to English (`en`).

## Versioning

Every `PUT /api/notification-templates/{id}` call creates a **new** template record with `Version + 1` and marks the old record as inactive. Previous versions remain in the database for audit history.

## A/B Testing

Set `VariantGroup` to `"A"` or `"B"` (or any label) to mark templates as part of an A/B test. The calling code is responsible for selecting which variant to use; the template system stores and returns the group label.

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET`  | `/api/notification-templates` | List templates (filter by `category` and/or `language`) |
| `GET`  | `/api/notification-templates/{id}` | Get template by ID |
| `GET`  | `/api/notification-templates/by-name/{name}?language=en` | Get active template by name + language |
| `POST` | `/api/notification-templates` | Create new template |
| `PUT`  | `/api/notification-templates/{id}` | Update template (creates new versioned record) |
| `DELETE` | `/api/notification-templates/{id}` | Deactivate template |
| `POST` | `/api/notification-templates/{id}/preview` | Preview rendered template with variables |

All endpoints require `Admin` role.

## Using Templates when Sending Notifications

```json
POST /api/push-notifications/send
{
  "userId": "<user-id>",
  "templateId": "<template-id>",
  "templateVariables": {
    "tour_name": "Paris Walking Tour",
    "date": "Dec 25"
  },
  "category": "tour_updates"
}
```

If `templateId` is provided and found, the resolved `title` and `body` override the request's `Title`/`Body` fields. If the template is not found, the request falls back to any `Title`/`Body` provided directly.

## Admin Dashboard

Navigate to **Notification Templates** in the admin dashboard sidebar to:
- Create templates with category, language, title/body, and optional image/action URLs
- Edit templates (each save creates a new version)
- Preview templates by entering JSON variable values
- Deactivate templates

## Database Schema

Table: `ug.notification_templates`

| Column | Type | Description |
|--------|------|-------------|
| `Id` | `varchar(50)` | Primary key (GUID) |
| `Name` | `varchar(100)` | Logical name |
| `Category` | `varchar(100)` | Notification category |
| `Language` | `varchar(10)` | ISO 639-1 code |
| `Version` | `int` | Version number |
| `TitleTemplate` | `varchar(200)` | Title with placeholders |
| `BodyTemplate` | `varchar(4000)` | Body with placeholders |
| `ImageUrl` | `varchar(2000)` | Optional image URL |
| `ActionUrl` | `varchar(2000)` | Optional deep-link URL |
| `IsActive` | `bit` | Whether this is the current version |
| `VariantGroup` | `varchar(50)` | A/B variant label |
| `CreatedBy` | `varchar(450)` | Admin user ID |
| `CreatedAt` | `datetime2` | Creation timestamp |
| `UpdatedAt` | `datetime2` | Last update timestamp |
