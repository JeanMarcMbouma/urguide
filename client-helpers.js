/**
 * UrGuide 2FA and Passkey Client-Side Helper
 * 
 * This file provides JavaScript helper functions for integrating with the
 * UrGuide 2FA and Passkey APIs from a web client.
 */

// ============================================================================
// TOTP Two-Factor Authentication Helpers
// ============================================================================

/**
 * Enable 2FA and display QR code to user
 * @param {string} authToken - Bearer authentication token
 * @returns {Promise<{secret: string, qrCode: string, backupCodes: string[]}>}
 */
async function enable2FA(authToken) {
    try {
        // Step 1: Get QR code
        const enableResponse = await fetch('/api/account/2fa/enable', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            }
        });
        
        if (!enableResponse.ok) {
            throw new Error('Failed to enable 2FA');
        }
        
        const { secret, qrCodeBase64, manualEntryKey } = await enableResponse.json();
        
        // Display QR code to user (example using img element)
        const qrImage = document.createElement('img');
        qrImage.src = qrCodeBase64;
        qrImage.alt = 'Scan this QR code with your authenticator app';
        
        // Display manual entry key as fallback
        console.log('Manual entry key:', manualEntryKey);
        
        // Prompt user to enter code from their authenticator app
        const code = prompt('Enter the 6-digit code from your authenticator app:');
        
        // Step 2: Verify the code
        const verifyResponse = await fetch('/api/account/2fa/verify', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ code })
        });
        
        if (!verifyResponse.ok) {
            throw new Error('Invalid verification code');
        }
        
        const { success, backupCodes } = await verifyResponse.json();
        
        if (success) {
            // IMPORTANT: Display backup codes to user and ensure they save them
            alert('2FA enabled! Please save your backup codes securely:\n' + backupCodes.join('\n'));
            return { secret, qrCode: qrCodeBase64, backupCodes };
        }
        
    } catch (error) {
        console.error('Error enabling 2FA:', error);
        throw error;
    }
}

/**
 * Verify 2FA code during login
 * @param {string} code - The 6-digit TOTP code or backup code
 * @param {boolean} isBackupCode - Whether this is a backup code
 * @param {string} authToken - Bearer authentication token
 * @returns {Promise<boolean>}
 */
async function verify2FACode(code, isBackupCode, authToken) {
    try {
        const response = await fetch('/api/account/2fa/verify-code', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ code, isBackupCode })
        });
        
        if (!response.ok) {
            throw new Error('Invalid 2FA code');
        }
        
        return true;
    } catch (error) {
        console.error('Error verifying 2FA code:', error);
        return false;
    }
}

/**
 * Disable 2FA for the current user
 * @param {string} authToken - Bearer authentication token
 * @returns {Promise<boolean>}
 */
async function disable2FA(authToken) {
    try {
        const response = await fetch('/api/account/2fa/disable', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            }
        });
        
        return response.ok;
    } catch (error) {
        console.error('Error disabling 2FA:', error);
        return false;
    }
}

// ============================================================================
// Passkey/WebAuthn Helpers
// ============================================================================

/**
 * Convert ArrayBuffer to Base64 string
 */
function arrayBufferToBase64(buffer) {
    const bytes = new Uint8Array(buffer);
    let binary = '';
    for (let i = 0; i < bytes.byteLength; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return btoa(binary);
}

/**
 * Convert Base64 string to ArrayBuffer
 */
function base64ToArrayBuffer(base64) {
    const binary = atob(base64);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes.buffer;
}

/**
 * Register a new passkey for the current user
 * @param {string} authToken - Bearer authentication token
 * @param {string} friendlyName - A friendly name for this passkey (e.g., "iPhone 14")
 * @returns {Promise<boolean>}
 */
async function registerPasskey(authToken, friendlyName) {
    try {
        // Check if WebAuthn is supported
        if (!window.PublicKeyCredential) {
            throw new Error('WebAuthn is not supported in this browser');
        }
        
        // Step 1: Get registration options from server
        const optionsResponse = await fetch('/api/account/passkey/register/options', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ friendlyName })
        });
        
        if (!optionsResponse.ok) {
            throw new Error('Failed to get registration options');
        }
        
        const { options } = await optionsResponse.json();
        
        // Convert base64 fields to ArrayBuffer
        options.challenge = base64ToArrayBuffer(options.challenge);
        options.user.id = base64ToArrayBuffer(options.user.id);
        
        // Step 2: Create credential using WebAuthn API
        const credential = await navigator.credentials.create({
            publicKey: options
        });
        
        // Step 3: Send attestation response to server
        const attestationResponse = {
            id: credential.id,
            rawId: arrayBufferToBase64(credential.rawId),
            type: credential.type,
            response: {
                attestationObject: arrayBufferToBase64(credential.response.attestationObject),
                clientDataJSON: arrayBufferToBase64(credential.response.clientDataJSON)
            }
        };
        
        const completeResponse = await fetch('/api/account/passkey/register/complete', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ attestationResponse })
        });
        
        if (!completeResponse.ok) {
            throw new Error('Failed to complete passkey registration');
        }
        
        const { success } = await completeResponse.json();
        return success;
        
    } catch (error) {
        console.error('Error registering passkey:', error);
        throw error;
    }
}

/**
 * Login using a passkey
 * @param {string} userName - User's email/username
 * @returns {Promise<{success: boolean, userId: string}>}
 */
async function loginWithPasskey(userName) {
    try {
        // Check if WebAuthn is supported
        if (!window.PublicKeyCredential) {
            throw new Error('WebAuthn is not supported in this browser');
        }
        
        // Step 1: Get assertion options from server
        const optionsResponse = await fetch('/api/account/passkey/login/options', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ userName })
        });
        
        if (!optionsResponse.ok) {
            throw new Error('Failed to get login options');
        }
        
        const { options } = await optionsResponse.json();
        
        // Convert base64 fields to ArrayBuffer
        options.challenge = base64ToArrayBuffer(options.challenge);
        options.allowCredentials = options.allowCredentials.map(cred => ({
            ...cred,
            id: base64ToArrayBuffer(cred.id)
        }));
        
        // Step 2: Get assertion using WebAuthn API
        const assertion = await navigator.credentials.get({
            publicKey: options
        });
        
        // Step 3: Send assertion response to server
        const assertionResponse = {
            id: assertion.id,
            rawId: arrayBufferToBase64(assertion.rawId),
            type: assertion.type,
            response: {
                authenticatorData: arrayBufferToBase64(assertion.response.authenticatorData),
                clientDataJSON: arrayBufferToBase64(assertion.response.clientDataJSON),
                signature: arrayBufferToBase64(assertion.response.signature),
                userHandle: assertion.response.userHandle ? 
                    arrayBufferToBase64(assertion.response.userHandle) : null
            }
        };
        
        const completeResponse = await fetch('/api/account/passkey/login/complete', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ assertionResponse })
        });
        
        if (!completeResponse.ok) {
            throw new Error('Failed to complete passkey login');
        }
        
        return await completeResponse.json();
        
    } catch (error) {
        console.error('Error logging in with passkey:', error);
        throw error;
    }
}

/**
 * List all registered passkeys for the current user
 * @param {string} authToken - Bearer authentication token
 * @returns {Promise<Array<{id: string, friendlyName: string, createdAt: string, lastUsedAt: string}>>}
 */
async function listPasskeys(authToken) {
    try {
        const response = await fetch('/api/account/passkey/list', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        if (!response.ok) {
            throw new Error('Failed to list passkeys');
        }
        
        return await response.json();
    } catch (error) {
        console.error('Error listing passkeys:', error);
        return [];
    }
}

/**
 * Delete a passkey
 * @param {string} authToken - Bearer authentication token
 * @param {string} credentialId - The ID of the passkey to delete
 * @returns {Promise<boolean>}
 */
async function deletePasskey(authToken, credentialId) {
    try {
        const response = await fetch(`/api/account/passkey/${credentialId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        return response.ok;
    } catch (error) {
        console.error('Error deleting passkey:', error);
        return false;
    }
}

// ============================================================================
// Example Usage
// ============================================================================

/**
 * Example: Complete 2FA setup flow
 */
async function example2FASetup(authToken) {
    try {
        // Enable 2FA and get backup codes
        const result = await enable2FA(authToken);
        console.log('2FA enabled successfully!');
        console.log('Backup codes:', result.backupCodes);
        
        // Later, verify a code
        const code = prompt('Enter your 2FA code:');
        const isValid = await verify2FACode(code, false, authToken);
        console.log('Code valid:', isValid);
    } catch (error) {
        console.error('2FA setup failed:', error);
    }
}

/**
 * Example: Register and use passkey
 */
async function examplePasskeyFlow(authToken) {
    try {
        // Register a passkey
        const registered = await registerPasskey(authToken, 'My Device');
        if (registered) {
            console.log('Passkey registered successfully!');
        }
        
        // Later, login with passkey (no authToken needed)
        const loginResult = await loginWithPasskey('user@example.com');
        if (loginResult.success) {
            console.log('Logged in with passkey! User ID:', loginResult.userId);
        }
        
        // List all passkeys
        const passkeys = await listPasskeys(authToken);
        console.log('Registered passkeys:', passkeys);
    } catch (error) {
        console.error('Passkey flow failed:', error);
    }
}

// Export functions for use in modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        // 2FA functions
        enable2FA,
        verify2FACode,
        disable2FA,
        
        // Passkey functions
        registerPasskey,
        loginWithPasskey,
        listPasskeys,
        deletePasskey,
        
        // Helper functions
        arrayBufferToBase64,
        base64ToArrayBuffer
    };
}
