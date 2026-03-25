import { initializeApp, getApps, getApp } from 'firebase/app';
import { getMessaging, getToken } from 'firebase/messaging';

/**
 * Firebase Cloud Messaging (FCM) push notification service.
 *
 * Environment variables required (set in .env.local or deployment config):
 *   VITE_FIREBASE_API_KEY, VITE_FIREBASE_AUTH_DOMAIN, VITE_FIREBASE_PROJECT_ID,
 *   VITE_FIREBASE_STORAGE_BUCKET, VITE_FIREBASE_MESSAGING_SENDER_ID,
 *   VITE_FIREBASE_APP_ID, VITE_FIREBASE_VAPID_KEY
 *
 * See docs/guides/PUSH_NOTIFICATIONS_GUIDE.md for full setup instructions.
 */

export interface PushSubscriptionResult {
  token: string | null;
  error?: string;
}

/**
 * Requests notification permission and returns the FCM registration token.
 * Returns null if FCM is not configured or permission is denied.
 */
export async function subscribeToPushNotifications(): Promise<PushSubscriptionResult> {
  const vapidKey = import.meta.env.VITE_FIREBASE_VAPID_KEY as string | undefined;
  if (!vapidKey) {
    console.info('[FCM] VITE_FIREBASE_VAPID_KEY not set — push notifications disabled.');
    return { token: null };
  }

  if (!('Notification' in window)) {
    return { token: null, error: 'Notifications are not supported in this browser.' };
  }

  if (Notification.permission === 'denied') {
    return { token: null, error: 'Notification permission was denied.' };
  }

  const permission = await Notification.requestPermission();
  if (permission !== 'granted') {
    return { token: null, error: 'Notification permission was not granted.' };
  }

  try {
    const firebaseApp = getApps().length
      ? getApp()
      : initializeApp({
          apiKey: import.meta.env.VITE_FIREBASE_API_KEY as string,
          authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN as string,
          projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID as string,
          storageBucket: import.meta.env.VITE_FIREBASE_STORAGE_BUCKET as string,
          messagingSenderId: import.meta.env.VITE_FIREBASE_MESSAGING_SENDER_ID as string,
          appId: import.meta.env.VITE_FIREBASE_APP_ID as string,
        });
    const messaging = getMessaging(firebaseApp);
    const token = await getToken(messaging, { vapidKey });
    return { token };
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Unknown error';
    console.error('[FCM] Error getting push token:', message);
    return { token: null, error: message };
  }
}

/**
 * Sends the FCM token to the backend so it can send targeted push notifications.
 *
 * @param token The FCM registration token for this browser.
 * @param accessToken Optional bearer token for authenticating with the backend API.
 */
export async function registerPushTokenWithServer(
  token: string,
  accessToken?: string
): Promise<void> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  };

  if (accessToken) {
    headers['Authorization'] = `Bearer ${accessToken}`;
  }

  const response = await fetch('/api/notifications', {
    method: 'POST',
    headers,
    body: JSON.stringify({ token, platform: 'web' }),
  });

  if (!response.ok) {
    let errorText: string | undefined;
    try {
      errorText = await response.text();
    } catch {
      // Ignore errors while reading the response body.
    }
    console.error(
      '[FCM] Failed to register push token with server:',
      response.status,
      response.statusText,
      errorText ?? ''
    );
    throw new Error(`Failed to register push token (status ${response.status})`);
  }
}
