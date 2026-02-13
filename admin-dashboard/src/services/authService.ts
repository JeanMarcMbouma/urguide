import axios from 'axios';
import type { LoginRequest, TwoFactorRequest, AuthTokens, User } from '../types/admin.types';

class AuthService {
  private baseURL = '/api/auth';

  async login(credentials: LoginRequest): Promise<{ requiresTwoFactor: boolean; userId?: string }> {
    const { data } = await axios.post(`${this.baseURL}/login`, credentials);
    
    if (data.requiresTwoFactor) {
      return { requiresTwoFactor: true, userId: data.userId };
    }
    
    // Store token
    this.setToken(data.accessToken);
    return { requiresTwoFactor: false };
  }

  async verifyTwoFactor(request: TwoFactorRequest): Promise<AuthTokens> {
    const { data } = await axios.post<AuthTokens>(`${this.baseURL}/verify-2fa`, request);
    this.setToken(data.accessToken);
    return data;
  }

  async getCurrentUser(): Promise<User> {
    const token = this.getToken();
    if (!token) {
      throw new Error('No authentication token found');
    }

    const { data } = await axios.get<User>(`${this.baseURL}/me`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return data;
  }

  async logout(): Promise<void> {
    this.removeToken();
    window.location.href = '/login';
  }

  // Token management
  setToken(token: string): void {
    localStorage.setItem('adminToken', token);
  }

  getToken(): string | null {
    return localStorage.getItem('adminToken');
  }

  removeToken(): void {
    localStorage.removeItem('adminToken');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  // Decode JWT to check role (simple implementation)
  hasAdminRole(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const roles = payload.role || payload.roles || [];
      return Array.isArray(roles) ? roles.includes('Admin') : roles === 'Admin';
    } catch {
      return false;
    }
  }
}

export const authService = new AuthService();
export default authService;
