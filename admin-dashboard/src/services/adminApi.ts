import axios, { AxiosInstance, AxiosError } from 'axios';
import type {
  AdminUserInfo,
  UpdateUserRolesModel,
  UserActivityModel,
  PagedResult,
  SearchParameters,
  ApiResult,
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
}

// Export singleton instance
export const adminApi = new AdminApiService();
export const adminService = adminApi;
export default adminApi;
