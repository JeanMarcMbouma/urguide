# Push Notifications Setup Guide

This guide explains how to enable Firebase Cloud Messaging (FCM) push notifications for UrGuide web applications.

## Prerequisites

- A [Firebase project](https://console.firebase.google.com)
- Web app registration in your Firebase project
- VAPID key pair generated in Firebase Console → Project Settings → Cloud Messaging

## Environment Variables

Add the following variables to each frontend app's `.env.local` (or `.env.production`):

```env
VITE_FIREBASE_API_KEY=your_api_key
VITE_FIREBASE_AUTH_DOMAIN=your_project.firebaseapp.com
VITE_FIREBASE_PROJECT_ID=your_project_id
VITE_FIREBASE_STORAGE_BUCKET=your_project.appspot.com
VITE_FIREBASE_MESSAGING_SENDER_ID=your_sender_id
VITE_FIREBASE_APP_ID=your_app_id
VITE_FIREBASE_VAPID_KEY=your_vapid_key
```

## Enabling FCM in Frontend Apps

1. The Firebase SDK is already installed in all three apps (`firebase` package). No additional install step needed.

2. Set the environment variables above in `.env.local` (development) or your deployment environment. The service automatically enables itself when `VITE_FIREBASE_VAPID_KEY` is detected.

3. For background message handling (notifications when the browser tab is in the background), create a `firebase-messaging-sw.js` in the `public/` folder:

   ```js
   importScripts('https://www.gstatic.com/firebasejs/10.x.x/firebase-app-compat.js');
   importScripts('https://www.gstatic.com/firebasejs/10.x.x/firebase-messaging-compat.js');

   firebase.initializeApp({
     apiKey: 'YOUR_API_KEY',
     authDomain: 'YOUR_AUTH_DOMAIN',
     projectId: 'YOUR_PROJECT_ID',
     storageBucket: 'YOUR_STORAGE_BUCKET',
     messagingSenderId: 'YOUR_SENDER_ID',
     appId: 'YOUR_APP_ID',
   });

   const messaging = firebase.messaging();

   messaging.onBackgroundMessage((payload) => {
     const { title, body, icon } = payload.notification ?? {};
     self.registration.showNotification(title ?? 'UrGuide', {
       body,
       icon: icon ?? '/pwa-192x192.svg',
     });
   });
   ```

4. Call `subscribeToPushNotifications()` after user login:

   ```ts
   import {
     subscribeToPushNotifications,
     registerPushTokenWithServer,
   } from '../services/pushNotificationService';

   const { token, error } = await subscribeToPushNotifications();
   if (token) {
     await registerPushTokenWithServer(token);
   }
   ```

## Backend Integration

The frontend sends the FCM token to `POST /api/notifications/push-token` with body:

```json
{ "token": "<fcm_token>", "platform": "web" }
```

Implement this endpoint to store the token associated with the authenticated user. Use the FCM Admin SDK to send targeted push notifications.

## Notification Permission Flow

- The `usePWA` hook exposes `notificationPermission` and `requestNotificationPermission`.
- Permission is requested explicitly — never automatically on page load.
- Typical flow: prompt the user after a meaningful interaction (e.g., after login).

## Cache Strategies

Workbox caching strategies configured via `vite-plugin-pwa`:

| Route Pattern | Strategy | Cache Name | TTL |
|---|---|---|---|
| Google Fonts | CacheFirst | `google-fonts-cache` | 1 year |
| `/api/guides` | NetworkFirst | `api-guides-cache` | 5 min |
| `/api/*` | NetworkFirst | `api-cache` | 2 min |
| App shell | StaleWhileRevalidate | (default) | — |

## Background Sync

Each app registers a background sync queue for offline form submissions:

- **tourist-website**: `tour-request-queue` — retries failed tour requests
- **guide-portal**: `bid-submission-queue` — retries failed bid submissions
- **admin-dashboard**: `admin-action-queue` — retries failed admin actions

Failed POST requests will be replayed when the user comes back online.
