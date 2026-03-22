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
  expiresIn?: number;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

// Guide Profile types – matches backend User model from GET /getdetails
export interface GuideProfile {
  id: string;
  firstName: string;
  lastName: string;
  city: string;
  country: string;
  address: string;
  description: string;
  phoneNumber: string;
  profileImage: string;
  rating: number;
  isGuide: boolean;
  // extended fields kept for UI use
  email?: string;
  specializations?: string[];
  languages?: string[];
  pricePerHour?: number;
  pricePerDay?: number;
}

// Matches backend UpdateGuideModel for POST /updateguide
export interface UpdateGuideProfileRequest {
  id: string;
  firstName: string;
  lastName: string;
  address: string;
  country: string;
  city: string;
  gender?: string;
  phone: string;
  birthDay?: string;
  description: string;
  profileImage?: string;
}

// Verification / KYC types
export interface VerificationDocument {
  id: string;
  type: string;
  fileName: string;
  fileUrl?: string;
  uploadedAt: string;
  status: string;
}

export interface KycVerificationStatus {
  guideId: string;
  overallStatus: string;
  documents: VerificationDocument[];
  submittedAt?: string;
  reviewedAt?: string;
  notes?: string;
}

export interface SubmitVerificationRequest {
  documentType: string;
  fileBase64: string;
  fileName: string;
}

// Gallery types
export interface GalleryItem {
  id: string;
  catalogId: string;
  imageUrl: string;
  thumbnailUrl?: string;
  title?: string;
  description?: string;
  uploadedAt: string;
}

export interface Gallery {
  id: string;
  userId: string;
  name: string;
  description: string;
  images: GalleryItem[];
  createdAt: string;
}

export interface CreateGalleryRequest {
  name: string;
  description: string;
}

// Tour Request types – matches backend TourRequestModel
export interface TourRequest {
  // Normalized id used by frontend (maps from tourRequestId)
  id: string;
  tourRequestId?: string;
  title: string;
  description: string;
  preferredDate?: string;
  maxParticipants?: number;
  maxBudget?: number;
  status: string;
  requesterId?: string;
  requesterName?: string;
  regionName?: string;
  // Legacy fields kept for compatibility
  touristId?: string;
  touristName?: string;
  touristAvatar?: string;
  destination?: string;
  startDate?: string;
  endDate?: string;
  groupSize?: number;
  budget?: number;
  createdAt?: string;
}

export interface TourRequestFilters {
  status?: string;
  destination?: string;
  startDate?: string;
  endDate?: string;
  searchTerm?: string;
}

// Bid types
export interface Bid {
  id: string;
  postId: string;
  guideId: string;
  amount: number;
  currency: string;
  message: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBidRequest {
  postId: string;
  amount: number;
  currency: string;
  message: string;
}

export interface UpdateBidRequest {
  bidId: string;
  amount: number;
  message: string;
}

export interface BidHistory {
  bids: Bid[];
  totalCount: number;
}

// Availability types – matches backend AvailabilitySlot
export interface AvailabilitySlot {
  id?: string;
  guideId?: string;
  date: string;
  isAvailable?: boolean;
  isBlocked: boolean;
  blockReason?: string;
  recurringPattern?: string;
}

export interface BlockDatesRequest {
  startDate: string;
  endDate: string;
  reason?: string;
  timezone?: string;
}

export interface RecurringPattern {
  type: 'weekly' | 'monthly';
  dayOfWeek?: number;
  dayOfMonth?: number;
  endDate?: string;
}

export interface AvailabilityResponse {
  slots: AvailabilitySlot[];
  startDate: string;
  endDate: string;
  timezone: string;
}

export interface ICalImportRequest {
  iCalContent: string;
  reason?: string;
}

export interface ICalImportResponse {
  datesImported: number;
  datesSkipped: number;
  importedDates: string[];
}

export interface ConflictCheckResponse {
  date: string;
  hasConflict: boolean;
  conflictReason?: string;
}

export interface GoogleCalendarStatusResponse {
  isConnected: boolean;
  scope?: string;
  expiresAt?: string;
}

export interface GoogleCalendarSyncResponse {
  datesBlocked: number;
  datesSkipped: number;
  blockedDates: string[];
}

// Earnings types
export interface EarningsSummary {
  totalEarnings: number;
  currentMonthEarnings: number;
  previousMonthEarnings: number;
  availableBalance: number;
  pendingEarnings: number;
  totalWithdrawn: number;
}

export interface EarningsDataPoint {
  date: string;
  amount: number;
  tourCount: number;
}

export interface MonthlyEarnings {
  year: number;
  month: number;
  amount: number;
  tourCount: number;
  refundedAmount: number;
}

// TransactionItem – matches backend TransactionItem
export interface TransactionItem {
  transactionId?: string;
  id?: string;
  type: string;
  description: string;
  amount: number;
  currencyCode?: string;
  currency?: string;
  status: string;
  createdAt?: string;
  date?: string;
}

export interface TransactionHistoryResponse {
  transactions: TransactionItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}

// Payout types – matches backend PayoutResponse
export interface PayoutItem {
  payoutId: string;
  guideId: string;
  amount: number;
  currencyCode: string;
  status: string;
  requestedAt: string;
  processedAt?: string;
  failureReason?: string;
  paymentMethod?: string;
}

export interface PayoutListResponse {
  payouts: PayoutItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface CreatePayoutRequest {
  guideId: string;
  amount: number;
  currencyCode: string;
}

export interface PaymentMethod {
  id: string;
  type: string;
  details: string;
  isDefault: boolean;
  createdAt: string;
}

// Review types – matches backend AuthoredFeedback
export interface AuthoredFeedback {
  id?: string;
  text: string;
  rating: number;
  publicationDate: string;
  authorId: string;
  authorImage?: string;
  authorFullName: string;
  guideResponse?: string;
}

// Mapped review type for UI
export interface Review {
  id: string;
  touristId: string;
  touristName: string;
  touristAvatar: string;
  rating: number;
  comment: string;
  guideResponse?: string;
  createdAt: string;
  tourId?: string;
  tourTitle?: string;
}

export interface ReviewFilters {
  rating?: number;
  hasResponse?: boolean;
  sortBy?: string;
}

export interface SubmitReviewResponseRequest {
  reviewId: string;
  response: string;
}

export interface ReviewStats {
  averageRating: number;
  totalReviews: number;
  ratingDistribution: Record<number, number>;
}

// Message types – matches backend ConversationSummary / MessageItem
export interface Conversation {
  id: string;
  participantId?: string;
  participantName?: string;
  // Legacy fields
  touristId?: string;
  touristName?: string;
  touristAvatar?: string;
  lastMessage: string;
  lastMessageAt: string;
  unreadCount: number;
}

export interface Message {
  id: string;
  conversationId: string;
  senderId: string;
  senderName: string;
  content: string;
  sentAt: string;
  isRead: boolean;
}

export interface SendMessageRequest {
  conversationId: string;
  content: string;
}

// Dashboard stats – matches backend GET /api/guide/dashboard
export interface GuideDashboard {
  availableBalance: number;
  averageRating: number;
  reviewCount: number;
  openTourRequests: number;
}

// Analytics types
export interface PerformanceMetrics {
  responseRate: number;
  responseTimeAvg: number;
  completionRate: number;
  cancellationRate: number;
  repeatClientRate: number;
}

export interface TourStatistics {
  totalTours: number;
  completedTours: number;
  cancelledTours: number;
  averageDuration: number;
  topDestinations: string[];
}

export type AnalyticsPeriod = 'week' | 'month' | 'year';

// Activity feed types
export interface ActivityItem {
  type: string;
  description: string;
  timestamp: string;
  icon: string;
}

// Shared generic types
export interface PagedResult<T> {
  items: T[];
  pageNumber?: number;
  pageSize?: number;
  totalCount: number;
  hasNextPage?: boolean;
  hasPreviousPage?: boolean;
}

export interface ApiResult<T> {
  success: boolean;
  data?: T;
  errors?: string[];
  message?: string;
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
