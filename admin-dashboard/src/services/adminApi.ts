import axios, { AxiosInstance, AxiosError } from 'axios';
import type {
  AdminUserInfo,
  UpdateUserRolesModel,
  UserActivityModel,
  PagedResult,
  SearchParameters,
  ApiResult,
  AdminTransactionListResponse,
  AdminPayoutListResponse,
  AdminRefundListResponse,
  FinancialFilterParameters,
  RevenueMetrics,
  DashboardSummary,
  SystemHealthStatus,
  AdminAuditLogResponse,
  AdminWebhookListResponse,
  PlatformSettings,
  AuditLogFilterParameters,
} from '../types/admin.types';

class AdminApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: '/api/admin',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Request interceptor to add auth token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('adminToken');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor for error handling
    this.api.interceptors.response.use(
      (response) => response,
      (error: AxiosError<ApiResult<unknown>>) => {
        if (error.response?.status === 401) {
          // Unauthorized - redirect to login
          localStorage.removeItem('adminToken');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Get all users with pagination and search
  async getUsers(params?: SearchParameters): Promise<PagedResult<AdminUserInfo>> {
    const { data } = await this.api.get<PagedResult<AdminUserInfo>>('/users', {
      params: {
        PageNumber: params?.pageNumber || 1,
        PageSize: params?.pageSize || 20,
        Term: params?.term || undefined,
      },
    });
    return data;
  }

  // Get user details by ID
  async getUserDetail(userId: string): Promise<AdminUserInfo> {
    const { data } = await this.api.get<AdminUserInfo>(`/users/${userId}`);
    return data;
  }

  // Suspend user account
  async suspendUser(userId: string, durationDays: number = 30): Promise<ApiResult<void>> {
    const { data } = await this.api.post<ApiResult<void>>(
      `/users/${userId}/suspend`,
      null,
      {
        params: { durationDays },
      }
    );
    return data;
  }

  // Activate user account
  async activateUser(userId: string): Promise<ApiResult<void>> {
    const { data } = await this.api.post<ApiResult<void>>(`/users/${userId}/activate`);
    return data;
  }

  // Delete user account
  async deleteUser(userId: string): Promise<ApiResult<void>> {
    const { data } = await this.api.delete<ApiResult<void>>(`/users/${userId}`);
    return data;
  }

  // Update user roles
  async updateUserRoles(model: UpdateUserRolesModel): Promise<ApiResult<void>> {
    const { data } = await this.api.put<ApiResult<void>>('/users/roles', model);
    return data;
  }

  // Get user activity log
  async getUserActivity(
    userId: string,
    params?: SearchParameters
  ): Promise<PagedResult<UserActivityModel>> {
    const { data } = await this.api.get<PagedResult<UserActivityModel>>(
      `/users/${userId}/activity`,
      {
        params: {
          PageNumber: params?.pageNumber || 1,
          PageSize: params?.pageSize || 50,
        },
      }
    );
    return data;
  }

  // Get all available roles
  async getAllRoles(): Promise<string[]> {
    const { data } = await this.api.get<string[]>('/roles');
    return data;
  }

  // Get pending guides for verification
  async getPendingGuides(pageNumber: number = 1): Promise<any> {
    const { data } = await this.api.get('/guides/pending', {
      params: { PageNumber: pageNumber, PageSize: 10 }
    });
    return data;
  }

  // Get guide verification details
  async getGuideVerificationDetail(userId: string): Promise<any> {
    const { data } = await this.api.get(`/guides/${userId}/verification`);
    return data;
  }

  // Process guide verification (approve/reject)
  async processGuideVerification(model: any): Promise<any> {
    const { data } = await this.api.post('/guides/verification', model);
    return data;
  }

  // Get pending tours for moderation
  async getPendingTours(pageNumber: number = 1): Promise<any> {
    const { data } = await this.api.get('/tours/pending', {
      params: { PageNumber: pageNumber, PageSize: 10 }
    });
    return data;
  }

  // Get tour moderation details
  async getTourModerationDetail(postId: string): Promise<any> {
    const { data } = await this.api.get(`/tours/${postId}/moderation`);
    return data;
  }

  // Process tour moderation (approve/reject)
  async processTourModeration(model: any): Promise<any> {
    const { data } = await this.api.post('/tours/moderation', model);
    return data;
  }

  // ── Financial Monitoring ────────────────────────────────────────────────────

  // Get all transactions (admin view)
  async getTransactions(params?: FinancialFilterParameters): Promise<AdminTransactionListResponse> {
    const { data } = await this.api.get<AdminTransactionListResponse>('/financial/transactions', {
      params: {
        PageNumber: params?.pageNumber || 1,
        PageSize: params?.pageSize || 20,
        StartDate: params?.startDate,
        EndDate: params?.endDate,
        Status: params?.status,
      },
    });
    return data;
  }

  // Get all payouts (admin view)
  async getPayouts(params?: FinancialFilterParameters): Promise<AdminPayoutListResponse> {
    const { data } = await this.api.get<AdminPayoutListResponse>('/financial/payouts', {
      params: {
        PageNumber: params?.pageNumber || 1,
        PageSize: params?.pageSize || 20,
        StartDate: params?.startDate,
        EndDate: params?.endDate,
        Status: params?.status,
      },
    });
    return data;
  }

  // Get all refunds (admin view)
  async getRefunds(params?: FinancialFilterParameters): Promise<AdminRefundListResponse> {
    const { data } = await this.api.get<AdminRefundListResponse>('/financial/refunds', {
      params: {
        PageNumber: params?.pageNumber || 1,
        PageSize: params?.pageSize || 20,
        StartDate: params?.startDate,
        EndDate: params?.endDate,
        Status: params?.status,
      },
    });
    return data;
  }

  // Get revenue metrics from analytics endpoint
  async getRevenueMetrics(startDate?: string, endDate?: string): Promise<RevenueMetrics> {
    const analyticsApi = axios.create({ baseURL: '/api/analytics' });
    const token = localStorage.getItem('adminToken');
    const { data } = await analyticsApi.get<RevenueMetrics>('/revenue-metrics', {
      headers: token ? { Authorization: `Bearer ${token}` } : {},
      params: { startDate, endDate },
    });
    return data;
  }

  // Get full analytics dashboard summary
  async getAnalyticsDashboard(startDate?: string, endDate?: string): Promise<DashboardSummary> {
    const analyticsApi = axios.create({ baseURL: '/api/analytics' });
    const token = localStorage.getItem('adminToken');
    const { data } = await analyticsApi.get<DashboardSummary>('/dashboard', {
      headers: token ? { Authorization: `Bearer ${token}` } : {},
      params: { startDate, endDate },
    });
    return data;
  }

  // Export analytics data (returns a Blob)
  async exportAnalyticsData(format: 'csv' | 'json' = 'csv', startDate?: string, endDate?: string): Promise<Blob> {
    const analyticsApi = axios.create({ baseURL: '/api/analytics' });
    const token = localStorage.getItem('adminToken');
    const response = await analyticsApi.get('/export', {
      headers: token ? { Authorization: `Bearer ${token}` } : {},
      params: { format, startDate, endDate },
      responseType: 'blob',
    });
    return response.data;
  }

  // ── System Monitoring ───────────────────────────────────────────────────────

  // Get system health status
  async getSystemHealth(): Promise<SystemHealthStatus> {
    const { data } = await this.api.get<SystemHealthStatus>('/system/health');
    return data;
  }

  // Get all audit log events
  async getAuditLogs(params?: AuditLogFilterParameters): Promise<AdminAuditLogResponse> {
    const { data } = await this.api.get<AdminAuditLogResponse>('/system/audit-logs', {
      params: {
        PageNumber: params?.pageNumber || 1,
        PageSize: params?.pageSize || 50,
        UserId: params?.userId,
        StartDate: params?.startDate,
        EndDate: params?.endDate,
        EventCode: params?.eventCode,
      },
    });
    return data;
  }

  // Get all webhook subscriptions
  async getWebhooks(pageNumber: number = 1): Promise<AdminWebhookListResponse> {
    const { data } = await this.api.get<AdminWebhookListResponse>('/system/webhooks', {
      params: { PageNumber: pageNumber, PageSize: 20 },
    });
    return data;
  }

  // Get platform settings
  async getPlatformSettings(): Promise<PlatformSettings> {
    const { data } = await this.api.get<PlatformSettings>('/system/settings');
    return data;
  }

  // Update platform settings
  async updatePlatformSettings(settings: PlatformSettings): Promise<ApiResult<void>> {
    const { data } = await this.api.put<ApiResult<void>>('/system/settings', settings);
    return data;
  }
}

// Export singleton instance
export const adminApi = new AdminApiService();
export const adminService = adminApi;
export default adminApi;
