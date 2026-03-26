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

// ── Financial Monitoring types ────────────────────────────────────────────────

export interface AdminTransactionItem {
  paymentId: string;
  userId: string;
  userEmail: string;
  bookingId: string;
  amount: number;
  currencyCode: string;
  status: string;
  paymentMethod: string;
  description: string;
  platformFeeAmount: number;
  guidePayout: number;
  createdAt: string;
}

export interface AdminTransactionListResponse {
  items: AdminTransactionItem[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface AdminPayoutItem {
  payoutId: string;
  guideId: string;
  guideName: string;
  amount: number;
  currencyCode: string;
  status: string;
  requestedAt: string;
  processedAt: string | null;
  failureReason: string | null;
}

export interface AdminPayoutListResponse {
  items: AdminPayoutItem[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface AdminRefundItem {
  refundId: string;
  paymentId: string;
  amount: number;
  currencyCode: string;
  status: string;
  reason: string;
  requestedBy: string;
  requestedAt: string;
  processedAt: string | null;
}

export interface AdminRefundListResponse {
  items: AdminRefundItem[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface FinancialFilterParameters {
  pageNumber?: number;
  pageSize?: number;
  startDate?: string;
  endDate?: string;
  status?: string;
}

// ── Analytics types ───────────────────────────────────────────────────────────

export interface RevenueDataPoint {
  date: string;
  amount: number;
  platformFees: number;
  transactionCount: number;
}

export interface RevenueMetrics {
  totalRevenue: number;
  platformFees: number;
  guidePayout: number;
  refundedAmount: number;
  netRevenue: number;
  transactionCount: number;
  averageTransactionValue: number;
  trendData: RevenueDataPoint[];
}

export interface DashboardSummary {
  revenue: RevenueMetrics;
  userTrends: {
    totalUsers: number;
    newUsersInPeriod: number;
    growthRate: number;
  };
  bookingStats: {
    totalBookings: number;
    completedBookings: number;
    cancelledBookings: number;
    completionRate: number;
  };
}

// ── System Monitoring types ───────────────────────────────────────────────────

export interface ServiceHealthItem {
  serviceName: string;
  status: string;
  message: string;
  responseTimeMs: number;
}

export interface SystemHealthStatus {
  overallStatus: string;
  checkedAt: string;
  services: ServiceHealthItem[];
}

export interface AdminAuditLogItem {
  id: string;
  eventCode: string;
  userId: string;
  userEmail: string;
  referenceId: string;
  created: string;
  ipAddress: string | null;
  userAgent: string | null;
  details: string | null;
  category: string | null;
  severity: string | null;
}

export interface AdminAuditLogResponse {
  items: AdminAuditLogItem[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface AdminWebhookItem {
  id: string;
  userId: string;
  userEmail: string;
  url: string;
  isActive: boolean;
  description: string;
  successCount: number;
  failureCount: number;
  createdAt: string;
  lastTriggeredAt: string | null;
}

export interface AdminWebhookListResponse {
  items: AdminWebhookItem[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface PlatformSettings {
  maintenanceMode: boolean;
  registrationEnabled: boolean;
  guideApplicationsEnabled: boolean;
  tourBookingEnabled: boolean;
  paymentsEnabled: boolean;
  emailNotificationsEnabled: boolean;
  platformFeePercentage: number;
  maxImagesPerPost: number;
  minBookingDaysAdvance: number;
}

export interface AuditLogFilterParameters {
  pageNumber?: number;
  pageSize?: number;
  userId?: string;
  startDate?: string;
  endDate?: string;
  eventCode?: string;
  category?: string;
  severity?: string;
}

// ── Push Notification Templates ───────────────────────────────────────────────

export interface NotificationTemplateDto {
  id: string;
  name: string;
  category: string;
  language: string;
  version: number;
  titleTemplate: string;
  bodyTemplate: string;
  imageUrl: string;
  actionUrl: string;
  isActive: boolean;
  variantGroup: string;
  createdBy: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateNotificationTemplateRequest {
  name: string;
  category: string;
  language: string;
  titleTemplate: string;
  bodyTemplate: string;
  imageUrl?: string;
  actionUrl?: string;
  variantGroup?: string;
}

export interface UpdateNotificationTemplateRequest {
  titleTemplate: string;
  bodyTemplate: string;
  imageUrl?: string;
  actionUrl?: string;
  isActive: boolean;
  variantGroup?: string;
}

export interface TemplatePreviewResult {
  title: string;
  body: string;
}

// ── Account Freeze types ──────────────────────────────────────────────────────

export interface AccountFreezeRequest {
  userId: string;
  reason: string;
  durationDays?: number;
}

export interface AccountUnfreezeRequest {
  userId: string;
  reason?: string;
}

export interface AccountFreezeInfo {
  id: string;
  userId: string;
  userEmail: string;
  reason: string;
  frozenByAdminId: string;
  frozenAt: string;
  expiresAt: string | null;
  unfrozenAt: string | null;
  unfrozenByAdminId: string | null;
  unfreezeReason: string | null;
  status: string;
}

export interface AccountFreezeHistoryResponse {
  items: AccountFreezeInfo[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}
