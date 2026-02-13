// Admin user management types matching backend API models

export interface AdminUserInfo {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  isGuide: boolean;
  emailConfirmed: boolean;
  twoFactorEnabled: boolean;
  lockoutEnabled: boolean;
  lockoutEnd: string | null;
  accessFailedCount: number;
  phoneNumber: string | null;
  roles: string[];
  postCount: number;
  tourCount: number;
}

export interface UpdateUserRolesModel {
  userId: string;
  roles: string[];
}

export interface UserActivityModel {
  userId: string;
  actionType: string;
  description: string;
  timestamp: string;
  ipAddress: string;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface SearchParameters {
  pageNumber?: number;
  pageSize?: number;
  term?: string;
}

export interface ApiResult<T> {
  success: boolean;
  data?: T;
  errors?: string[];
  message?: string;
}

// Auth types
export interface LoginRequest {
  email: string;
  password: string;
}

export interface TwoFactorRequest {
  userId: string;
  code: string;
}

export interface AuthTokens {
  accessToken: string;
  refreshToken?: string;
  expiresIn: number;
  tokenType: string;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

// UI State types
export interface UserFilters {
  role?: string;
  emailConfirmed?: boolean;
  isLocked?: boolean;
  searchTerm?: string;
}

export interface ConfirmDialogProps {
  open: boolean;
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  onConfirm: () => void;
  onCancel: () => void;
  severity?: 'warning' | 'error' | 'info';
}
