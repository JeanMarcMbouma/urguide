/**
 * Firebase Cloud Messaging (FCM) push notification service.
 *
 * To enable push notifications:
 * 1. Create a Firebase project at https://console.firebase.google.com
 * 2. Add a Web app and copy the config values to your environment variables:
 *    VITE_FIREBASE_API_KEY, VITE_FIREBASE_AUTH_DOMAIN, VITE_FIREBASE_PROJECT_ID,
 *    VITE_FIREBASE_STORAGE_BUCKET, VITE_FIREBASE_MESSAGING_SENDER_ID,
 *    VITE_FIREBASE_APP_ID, VITE_FIREBASE_VAPID_KEY
 * 3. Install firebase: npm install firebase
 * 4. Uncomment the implementation below.
 *
 * See docs/guides/push-notifications.md for full setup instructions.
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

  const vapidKey = import.meta.env.VITE_FIREBASE_VAPID_KEY as string | undefined;
  if (!vapidKey) {
    console.info('[FCM] VITE_FIREBASE_VAPID_KEY not set — push notifications disabled.');
    return { token: null };
  }

  try {
    /*
     * Uncomment after installing firebase and setting environment variables:
     *
     * import { initializeApp } from 'firebase/app';
     * import { getMessaging, getToken } from 'firebase/messaging';
     *
     * const firebaseApp = initializeApp({
     *   apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
     *   authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN,
     *   projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
     *   storageBucket: import.meta.env.VITE_FIREBASE_STORAGE_BUCKET,
     *   messagingSenderId: import.meta.env.VITE_FIREBASE_MESSAGING_SENDER_ID,
     *   appId: import.meta.env.VITE_FIREBASE_APP_ID,
     * });
     * const messaging = getMessaging(firebaseApp);
     * const token = await getToken(messaging, { vapidKey });
     * return { token };
     */
    return { token: null };
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Unknown error';
    console.error('[FCM] Error getting push token:', message);
    return { token: null, error: message };
  }
}

/**
 * Sends the FCM token to the backend so it can send targeted push notifications.
 */
export async function registerPushTokenWithServer(token: string): Promise<void> {
  await fetch('/api/notifications/push-token', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ token, platform: 'web' }),
  });
}
