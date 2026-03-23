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
  refreshToken: string;
  expiration: string;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  profileImageUrl?: string;
  roles: string[];
}

// Guide discovery types (#168)
export interface GuideListItem {
  id: string;
  firstName: string;
  lastName: string;
  profileImageUrl?: string;
  rating: number;
  reviewCount: number;
  languages: string[];
  specialties: string[];
  location: string;
  pricePerHour: number;
  currency: string;
  verified: boolean;
}

export interface GuideDetail extends GuideListItem {
  bio: string;
  experience: number;
  galleries: GalleryPreview[];
  recentReviews: ReviewItem[];
  availability: boolean;
}

export interface GalleryPreview {
  id: number;
  title: string;
  coverImageUrl: string;
  imageCount: number;
}

export interface SearchFilters {
  query?: string;
  location?: string;
  minPrice?: number;
  maxPrice?: number;
  minRating?: number;
  languages?: string[];
  specialties?: string[];
  sortBy?: 'rating' | 'price' | 'reviews';
  sortOrder?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

export interface SearchResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface FeaturedContent {
  featuredGuides: GuideListItem[];
  popularDestinations: Destination[];
  recentTours: TourPreview[];
}

export interface Destination {
  id: number;
  name: string;
  country: string;
  imageUrl: string;
  guideCount: number;
}

export interface TourPreview {
  id: number;
  title: string;
  description: string;
  imageUrl?: string;
  guideName: string;
  rating: number;
  price: number;
  currency: string;
}

// Tour Request & Bidding types (#169)
export interface CreateTourRequestData {
  title: string;
  description: string;
  regionId: number;
  startDate: string;
  endDate: string;
  numberOfPeople: number;
  budgetMin: number;
  budgetMax: number;
  currency: string;
  languages: string[];
  specialRequirements?: string;
}

export interface TourRequest {
  id: number;
  title: string;
  description: string;
  regionId: number;
  regionName: string;
  startDate: string;
  endDate: string;
  numberOfPeople: number;
  budgetMin: number;
  budgetMax: number;
  currency: string;
  languages: string[];
  specialRequirements?: string;
  status: string;
  bidCount: number;
  createdAt: string;
  userId: string;
}

export interface Bid {
  id: number;
  postId: number;
  guideId: string;
  guideName: string;
  guideProfileImage?: string;
  guideRating: number;
  guideReviewCount: number;
  amount: number;
  currency: string;
  message: string;
  estimatedDuration: string;
  status: string;
  createdAt: string;
}

export interface Booking {
  id: number;
  tourRequestId: number;
  tourTitle: string;
  guideName: string;
  guideProfileImage?: string;
  startDate: string;
  endDate: string;
  amount: number;
  currency: string;
  status: string;
  paymentStatus: string;
  createdAt: string;
}

// Payment types (#170)
export interface PaymentRequest {
  tourRequestId: number;
  amount: number;
  currency: string;
  paymentMethodId?: string;
}

export interface PaymentInfo {
  id: number;
  tourRequestId: number;
  amount: number;
  currency: string;
  status: string;
  stripePaymentIntentId?: string;
  clientSecret?: string;
  createdAt: string;
}

export interface TransactionItem {
  id: number;
  type: string;
  amount: number;
  currency: string;
  description: string;
  status: string;
  createdAt: string;
  tourTitle?: string;
  guideName?: string;
}

export interface TransactionHistoryResponse {
  transactions: TransactionItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface UserProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  profileImageUrl?: string;
  address?: string;
  city?: string;
  country?: string;
  preferredCurrency: string;
  preferredLanguage: string;
  twoFactorEnabled: boolean;
  createdAt: string;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  country?: string;
  preferredCurrency: string;
  preferredLanguage: string;
}

export interface NotificationPreferences {
  emailNotifications: boolean;
  pushNotifications: boolean;
  bidUpdates: boolean;
  tourReminders: boolean;
  promotionalEmails: boolean;
  reviewReminders: boolean;
}

// Review types (#171)
export interface ReviewItem {
  id: number;
  postId: number;
  tourTitle: string;
  guideName: string;
  rating: number;
  comment: string;
  photos: string[];
  createdAt: string;
  guideResponse?: string;
  guideRespondedAt?: string;
}

export interface WriteReviewData {
  postId: number;
  rating: number;
  comment: string;
  photos?: File[];
}

export interface ReviewStats {
  averageRating: number;
  totalReviews: number;
  distribution: { [key: number]: number };
}

// Notification types (#171)
export interface NotificationItem {
  id: number;
  type: string;
  title: string;
  message: string;
  isRead: boolean;
  data?: Record<string, string>;
  createdAt: string;
}

// Messages types (#171)
export interface Conversation {
  id: number;
  participantId: string;
  participantName: string;
  participantProfileImage?: string;
  lastMessage: string;
  lastMessageAt: string;
  unreadCount: number;
}

export interface Message {
  id: number;
  conversationId: number;
  senderId: string;
  senderName: string;
  content: string;
  isRead: boolean;
  createdAt: string;
}

export interface SendMessageRequest {
  conversationId: number;
  content: string;
}

// Shared utility types
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface ApiResult<T> {
  data: T;
  success: boolean;
  errors?: string[];
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
