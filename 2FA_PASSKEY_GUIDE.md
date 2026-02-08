# Two-Factor Authentication (2FA) and Passkey Usage Guide

This document provides examples of how to use the newly implemented 2FA and Passkey authentication features in the UrGuide API.

## Table of Contents
- [TOTP Two-Factor Authentication](#totp-two-factor-authentication)
- [Passkey/WebAuthn Authentication](#passkeywebauthn-authentication)
- [Security Best Practices](#security-best-practices)

---

## TOTP Two-Factor Authentication

### 1. Check 2FA Status

**Endpoint:** `GET /api/account/2fa/status`

**Authorization:** Required (Bearer token)

**Response:**
```json
{
  "isEnabled": false,
  "enabledAt": null,
  "remainingBackupCodes": 0,
  "passkeyCount": 0
}
```

### 2. Enable 2FA

**Step 1: Get QR Code**

**Endpoint:** `POST /api/account/2fa/enable`

**Authorization:** Required

**Response:**
```json
{
  "secret": "JBSWY3DPEHPK3PXP",
  "qrCodeBase64": "data:image/png;base64,iVBORw0KG...",
  "manualEntryKey": "JBSWY3DPEHPK3PXP"
}
```

**Usage:**
- Display the QR code to the user to scan with Google Authenticator, Authy, or any TOTP app
- Alternatively, provide the `manualEntryKey` for manual entry

**Step 2: Verify Setup**

**Endpoint:** `POST /api/account/2fa/verify`

**Authorization:** Required

**Request Body:**
```json
{
  "code": "123456"
}
```

**Response:**
```json
{
  "success": true,
  "backupCodes": [
    "a1b2c3d4",
    "e5f6g7h8",
    "i9j0k1l2",
    "m3n4o5p6",
    "q7r8s9t0",
    "u1v2w3x4",
    "y5z6a7b8",
    "c9d0e1f2",
    "g3h4i5j6",
    "k7l8m9n0"
  ]
}
```

**Important:** Save backup codes securely! They are shown only once.

### 3. Verify 2FA Code During Login

**Endpoint:** `POST /api/account/2fa/verify-code`

**Request Body:**
```json
{
  "code": "123456",
  "isBackupCode": false
}
```

**Or with backup code:**
```json
{
  "code": "a1b2c3d4",
  "isBackupCode": true
}
```

**Response:**
```json
{
  "message": "Code verified successfully"
}
```

### 4. Generate New Backup Codes

**Endpoint:** `POST /api/account/2fa/backup-codes/generate`

**Authorization:** Required

**Response:**
```json
{
  "backupCodes": [
    "n1o2p3q4",
    "r5s6t7u8",
    "v9w0x1y2",
    "z3a4b5c6",
    "d7e8f9g0",
    "h1i2j3k4",
    "l5m6n7o8",
    "p9q0r1s2",
    "t3u4v5w6",
    "x7y8z9a0"
  ]
}
```

**Note:** Old backup codes are invalidated when new ones are generated.

### 5. Disable 2FA

**Endpoint:** `POST /api/account/2fa/disable`

**Authorization:** Required

**Response:**
```json
{
  "message": "2FA disabled successfully"
}
```

---

## Passkey/WebAuthn Authentication

Passkeys provide passwordless authentication using biometrics, security keys, or device PIN.

### 1. Register a Passkey

**Step 1: Get Registration Options**

**Endpoint:** `POST /api/account/passkey/register/options`

**Authorization:** Required

**Request Body:**
```json
{
  "friendlyName": "iPhone 14 Pro"
}
```

**Response:**
```json
{
  "options": {
    "challenge": "base64-encoded-challenge",
    "rp": {
      "name": "UrGuide",
      "id": "urguide.com"
    },
    "user": {
      "id": "base64-encoded-user-id",
      "name": "user@example.com",
      "displayName": "John Doe"
    },
    "pubKeyCredParams": [...],
    "authenticatorSelection": {
      "requireResidentKey": false,
      "userVerification": "preferred"
    },
    "timeout": 60000,
    "attestation": "none"
  }
}
```

**Step 2: Complete Registration (Client-side WebAuthn API)**

```javascript
// Client-side JavaScript example
const registrationOptions = response.options;

// Call WebAuthn API
const credential = await navigator.credentials.create({
  publicKey: registrationOptions
});

// Send attestation response to server
const attestationResponse = {
  id: credential.id,
  rawId: arrayBufferToBase64(credential.rawId),
  type: credential.type,
  response: {
    attestationObject: arrayBufferToBase64(credential.response.attestationObject),
    clientDataJSON: arrayBufferToBase64(credential.response.clientDataJSON)
  }
};
```

**Endpoint:** `POST /api/account/passkey/register/complete`

**Authorization:** Required

**Request Body:**
```json
{
  "attestationResponse": {
    // WebAuthn attestation response from client
  }
}
```

**Response:**
```json
{
  "success": true,
  "credentialId": "base64-credential-id"
}
```

### 2. Login with Passkey

**Step 1: Get Login Options**

**Endpoint:** `POST /api/account/passkey/login/options`

**Request Body:**
```json
{
  "userName": "user@example.com"
}
```

**Response:**
```json
{
  "options": {
    "challenge": "base64-encoded-challenge",
    "timeout": 60000,
    "rpId": "urguide.com",
    "allowCredentials": [
      {
        "id": "base64-credential-id",
        "type": "public-key"
      }
    ],
    "userVerification": "preferred"
  }
}
```

**Step 2: Complete Login (Client-side WebAuthn API)**

```javascript
// Client-side JavaScript example
const assertionOptions = response.options;

// Call WebAuthn API
const assertion = await navigator.credentials.get({
  publicKey: assertionOptions
});

// Send assertion response to server
const assertionResponse = {
  id: assertion.id,
  rawId: arrayBufferToBase64(assertion.rawId),
  type: assertion.type,
  response: {
    authenticatorData: arrayBufferToBase64(assertion.response.authenticatorData),
    clientDataJSON: arrayBufferToBase64(assertion.response.clientDataJSON),
    signature: arrayBufferToBase64(assertion.response.signature),
    userHandle: arrayBufferToBase64(assertion.response.userHandle)
  }
};
```

**Endpoint:** `POST /api/account/passkey/login/complete`

**Request Body:**
```json
{
  "assertionResponse": {
    // WebAuthn assertion response from client
  }
}
```

**Response:**
```json
{
  "success": true,
  "userId": "user-id"
}
```

### 3. List Registered Passkeys

**Endpoint:** `GET /api/account/passkey/list`

**Authorization:** Required

**Response:**
```json
[
  {
    "id": "credential-id-1",
    "friendlyName": "iPhone 14 Pro",
    "createdAt": "2024-01-15T10:30:00Z",
    "lastUsedAt": "2024-02-07T14:22:00Z"
  },
  {
    "id": "credential-id-2",
    "friendlyName": "YubiKey 5",
    "createdAt": "2024-01-20T09:15:00Z",
    "lastUsedAt": null
  }
]
```

### 4. Delete a Passkey

**Endpoint:** `DELETE /api/account/passkey/{id}`

**Authorization:** Required

**Response:**
```json
{
  "message": "Passkey deleted successfully"
}
```

---

## Security Best Practices

### For TOTP 2FA:

1. **Store Backup Codes Securely**
   - Users should save backup codes in a secure password manager
   - Never store them in plain text or share them

2. **Time Synchronization**
   - Ensure device clocks are synchronized (TOTP is time-based)
   - Codes are valid for 30 seconds with a 1-step tolerance window

3. **Rate Limiting**
   - Implement rate limiting on verification endpoints to prevent brute force

4. **Account Recovery**
   - Always provide backup codes during setup
   - Have a secure account recovery process

### For Passkey/WebAuthn:

1. **Device Security**
   - Passkeys are bound to devices with biometric/PIN protection
   - Lost devices should have passkeys revoked immediately

2. **Multiple Passkeys**
   - Encourage users to register multiple passkeys (phone, security key, etc.)
   - Provides backup if one device is lost

3. **HTTPS Required**
   - WebAuthn only works over HTTPS
   - Ensure production environment uses valid SSL certificates

4. **User Verification**
   - Always require user verification (biometric, PIN)
   - Configured as "preferred" in the implementation

### General Security:

1. **Enforce 2FA for Sensitive Accounts**
   - Consider requiring 2FA for guide accounts
   - Add configuration for mandatory 2FA on specific roles

2. **Audit Logging**
   - Log all 2FA enablement/disablement
   - Track failed authentication attempts
   - Monitor backup code usage

3. **User Education**
   - Provide clear instructions on setup
   - Explain the importance of backup codes
   - Guide users through passkey registration

---

## Error Handling

### Common Error Responses:

**2FA Not Enabled:**
```json
{
  "errors": ["2FA is not enabled"]
}
```

**Invalid Code:**
```json
{
  "errors": ["Invalid verification code"]
}
```

**User Not Found:**
```json
{
  "errors": ["User not found"]
}
```

**Passkey Registration Failed:**
```json
{
  "errors": ["Failed to register passkey"]
}
```

---

## Testing

### Test 2FA Locally:

1. Enable 2FA for a test account
2. Use a TOTP app like Google Authenticator or Authy
3. Scan the QR code or enter the manual key
4. Verify the 6-digit code
5. Test backup codes
6. Test disabling and re-enabling

### Test Passkeys:

1. Use a modern browser (Chrome 108+, Safari 16+, Edge 108+)
2. Test with Touch ID/Face ID on Mac/iOS
3. Test with Windows Hello on Windows
4. Test with FIDO2 security keys (YubiKey, etc.)
5. Verify multiple passkeys can be registered
6. Test passkey deletion

---

## Browser Compatibility

**Passkey Support:**
- Chrome/Edge 108+
- Safari 16+
- Firefox 122+
- iOS Safari 16+
- Android Chrome 108+

**Requirements:**
- HTTPS connection
- Compatible authenticator (biometric, security key, or platform authenticator)

---

For more information, see the API documentation at `/swagger` when running the application.
