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

// Guide Profile types
export interface GuideProfile {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  bio: string;
  specializations: string[];
  languages: string[];
  pricePerHour: number;
  pricePerDay: number;
  yearsExperience: number;
  isVerified: boolean;
  verificationStatus: string;
  profileImageUrl: string;
  coverImageUrl: string;
  rating: number;
  reviewCount: number;
  location: string;
  createdAt: string;
}

export interface UpdateGuideProfileRequest {
  firstName?: string;
  lastName?: string;
  email?: string;
  phoneNumber?: string;
  bio?: string;
  specializations?: string[];
  languages?: string[];
  pricePerHour?: number;
  pricePerDay?: number;
  yearsExperience?: number;
  location?: string;
}

// Verification / KYC types
export interface VerificationDocument {
  id: string;
  type: string;
  fileName: string;
  fileUrl: string;
  uploadedAt: string;
  status: string;
}

export interface KycVerificationStatus {
  guideId: string;
  overallStatus: string;
  documents: VerificationDocument[];
  submittedAt: string;
  reviewedAt: string;
  notes: string;
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
  thumbnailUrl: string;
  title: string;
  description: string;
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

// Tour Request types
export interface TourRequest {
  id: string;
  touristId: string;
  touristName: string;
  touristAvatar: string;
  title: string;
  description: string;
  destination: string;
  startDate: string;
  endDate: string;
  groupSize: number;
  budget: number;
  status: string;
  createdAt: string;
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

// Availability types
export interface AvailabilitySlot {
  id: string;
  guideId: string;
  date: string;
  isAvailable: boolean;
  isBlocked: boolean;
  blockReason?: string;
  recurringPattern?: string;
}

export interface BlockDatesRequest {
  startDate: string;
  endDate: string;
  reason?: string;
}

export interface RecurringPattern {
  type: 'weekly' | 'monthly';
  dayOfWeek?: number;
  dayOfMonth?: number;
  endDate?: string;
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

export interface TransactionItem {
  id: string;
  type: string;
  description: string;
  amount: number;
  currency: string;
  status: string;
  date: string;
  relatedTourId?: string;
}

// Payout types
export interface PayoutItem {
  payoutId: string;
  guideId: string;
  amount: number;
  currencyCode: string;
  status: string;
  requestedAt: string;
  processedAt?: string;
  failureReason?: string;
  paymentMethod: string;
}

export interface CreatePayoutRequest {
  guideId: string;
  amount: number;
  currencyCode: string;
  paymentMethod: string;
}

export interface PaymentMethod {
  id: string;
  type: string;
  details: string;
  isDefault: boolean;
  createdAt: string;
}

// Review types
export interface Review {
  id: string;
  touristId: string;
  touristName: string;
  touristAvatar: string;
  rating: number;
  comment: string;
  guideResponse?: string;
  createdAt: string;
  tourId: string;
  tourTitle: string;
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

// Message types
export interface Conversation {
  id: string;
  touristId: string;
  touristName: string;
  touristAvatar: string;
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

// Shared generic types
export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
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
