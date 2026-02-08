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
 * Convert ArrayBuffer to Base64URL string (WebAuthn compatible)
 */
function arrayBufferToBase64URL(buffer) {
    const bytes = new Uint8Array(buffer);
    let binary = '';
    for (let i = 0; i < bytes.byteLength; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    // Convert to base64 and then to base64url
    return btoa(binary)
        .replace(/\+/g, '-')
        .replace(/\//g, '_')
        .replace(/=/g, '');
}

/**
 * Convert Base64URL string to ArrayBuffer (WebAuthn compatible)
 */
function base64URLToArrayBuffer(base64url) {
    // Convert base64url to base64
    let base64 = base64url
        .replace(/-/g, '+')
        .replace(/_/g, '/');
    
    // Add padding if needed
    const padding = (4 - (base64.length % 4)) % 4;
    base64 += '='.repeat(padding);
    
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
        options.challenge = base64URLToArrayBuffer(options.challenge);
        options.user.id = base64URLToArrayBuffer(options.user.id);
        
        // Step 2: Create credential using WebAuthn API
        const credential = await navigator.credentials.create({
            publicKey: options
        });
        
        // Step 3: Send attestation response to server
        const attestationResponse = {
            id: credential.id,
            rawId: arrayBufferToBase64URL(credential.rawId),
            type: credential.type,
            response: {
                attestationObject: arrayBufferToBase64URL(credential.response.attestationObject),
                clientDataJSON: arrayBufferToBase64URL(credential.response.clientDataJSON)
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
        options.challenge = base64URLToArrayBuffer(options.challenge);
        options.allowCredentials = options.allowCredentials.map(cred => ({
            ...cred,
            id: base64URLToArrayBuffer(cred.id)
        }));
        
        // Step 2: Get assertion using WebAuthn API
        const assertion = await navigator.credentials.get({
            publicKey: options
        });
        
        // Step 3: Send assertion response to server
        const assertionResponse = {
            id: assertion.id,
            rawId: arrayBufferToBase64URL(assertion.rawId),
            type: assertion.type,
            response: {
                authenticatorData: arrayBufferToBase64URL(assertion.response.authenticatorData),
                clientDataJSON: arrayBufferToBase64URL(assertion.response.clientDataJSON),
                signature: arrayBufferToBase64URL(assertion.response.signature),
                userHandle: assertion.response.userHandle ? 
                    arrayBufferToBase64URL(assertion.response.userHandle) : null
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
        arrayBufferToBase64URL,
        base64URLToArrayBuffer
    };
}
