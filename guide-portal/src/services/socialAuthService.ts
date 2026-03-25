import axios from 'axios';

export interface SocialLoginProvider {
  provider: string;
  email?: string;
  displayName?: string;
  avatarUrl?: string;
  linkedAt: string;
  lastLoginAt?: string;
}

export interface SocialLoginAuditLog {
  provider: string;
  action: string;
  details?: string;
  timestamp: string;
}

export interface AvailableSocialProvider {
  name: string;
  loginUrl: string;
  icon: string;
}

class SocialAuthService {
  private baseURL = '/api/social-auth';

  /**
   * Initiates social login by redirecting to the provider's consent screen.
   */
  initiateLogin(provider: string, returnUrl?: string): void {
    const params = returnUrl ? `?returnUrl=${encodeURIComponent(returnUrl)}` : '';
    window.location.href = `${this.baseURL}/login/${provider}${params}`;
  }

  /**
   * Initiates linking a social provider to the current user's account.
   */
  initiateLink(provider: string, returnUrl?: string): void {
    const params = returnUrl ? `?returnUrl=${encodeURIComponent(returnUrl)}` : '';
    window.location.href = `${this.baseURL}/link/${provider}${params}`;
  }

  /**
   * Unlinks a social provider from the current user's account.
   */
  async unlinkProvider(provider: string, token: string): Promise<{ message: string }> {
    const { data } = await axios.delete<{ message: string }>(
      `${this.baseURL}/unlink/${provider}`,
      { headers: { Authorization: `Bearer ${token}` } }
    );
    return data;
  }

  /**
   * Gets all linked social providers for the current user.
   */
  async getLinkedProviders(token: string): Promise<SocialLoginProvider[]> {
    const { data } = await axios.get<SocialLoginProvider[]>(
      `${this.baseURL}/providers`,
      { headers: { Authorization: `Bearer ${token}` } }
    );
    return data;
  }

  /**
   * Gets the audit log of social login activities for the current user.
   */
  async getAuditLog(token: string, take: number = 50): Promise<SocialLoginAuditLog[]> {
    const { data } = await axios.get<SocialLoginAuditLog[]>(
      `${this.baseURL}/audit-log?take=${take}`,
      { headers: { Authorization: `Bearer ${token}` } }
    );
    return data;
  }

  /**
   * Gets the list of available social login providers.
   */
  async getAvailableProviders(): Promise<AvailableSocialProvider[]> {
    const { data } = await axios.get<AvailableSocialProvider[]>(
      `${this.baseURL}/providers/available`
    );
    return data;
  }

  /**
   * Handles the callback after a social login redirect.
   * Extracts the token from the URL query parameters.
   */
  handleCallback(): { token: string | null; isNew: boolean } {
    const params = new URLSearchParams(window.location.search);
    const token = params.get('token');
    const isNew = params.get('isNew') === 'True' || params.get('isNew') === 'true';
    return { token, isNew };
  }
}

export const socialAuthService = new SocialAuthService();
export default socialAuthService;
