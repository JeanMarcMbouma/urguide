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
  userName: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

// Guide discovery types (#168)
// Mapped from PostSearchDocument / PostModel
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

// Mapped from PostModel
export interface GuideDetail extends GuideListItem {
  bio: string;
  experience: number;
  galleries: GalleryPreview[];
  recentReviews: ReviewItem[];
  availability: boolean;
}

export interface GalleryPreview {
  id: string;
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
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

// Matches backend SearchResponse<T>
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
  id: string;
  name: string;
  country: string;
  imageUrl: string;
  guideCount: number;
}

export interface TourPreview {
  id: string;
  title: string;
  description: string;
  imageUrl?: string;
  guideName: string;
  rating: number;
  price: number;
  currency: string;
}

// Tour Request & Bidding types (#169)
// Matches backend CreateTourRequestModel
export interface CreateTourRequestData {
  title: string;
  description: string;
  preferredDate: string;
  maxParticipants: number;
  maxBudget: number;
  tags: string;
  regionId: string;
}

// Matches backend TourRequestModel
export interface TourRequest {
  tourRequestId: string;
  title: string;
  description: string;
  preferredDate: string;
  maxParticipants: number;
  maxBudget: number;
  tags: string;
  regionId: string;
  regionName: string;
  status: string;
  requesterId: string;
  requesterName: string;
  createdAt: string;
  updatedAt: string;
}

// Matches backend BidHistoryModel
export interface Bid {
  value: string;
  author: string;
  authorImage: string;
  created: string;
  isActive: boolean;
}

// Mapped from PostModel for bookings view
export interface Booking {
  id: string;
  text: string;
  description: string;
  price: string;
  rating: string;
  location: string;
  status: string;
  startDate: string;
  endDate: string;
  seats: number;
  reservedSeats: number;
  authorId: string;
  author: string;
  authorAvatar: string;
  hasReserved: boolean;
  reviews: number;
}

// Payment types (#170)
// Matches backend CreatePaymentRequest
export interface PaymentRequest {
  bookingId: string;
  amount: number;
  currencyCode: string;
  description?: string;
  paymentMethodId?: string;
}

// Matches backend PaymentResponse
export interface PaymentInfo {
  paymentId: string;
  clientSecret: string;
  status: string;
  amount: number;
  currencyCode: string;
  platformFeeAmount: number;
  guidePayout: number;
  createdAt: string;
}

// Matches backend PaymentDetailsResponse
export interface PaymentDetails {
  paymentId: string;
  userId: string;
  bookingId: string;
  amount: number;
  currencyCode: string;
  status: string;
  paymentMethod: string;
  description: string;
  platformFeeAmount: number;
  guidePayout: number;
  createdAt: string;
  updatedAt: string;
}

export interface TransactionHistoryResponse {
  items: PaymentDetails[];
  totalCount: number;
  page: number;
  pageSize: number;
}

// Matches backend /api/auth/me response
export interface UserProfile {
  id: string;
  email: string;
  userName: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

// Matches backend UpdateUserModel
export interface UpdateProfileRequest {
  id: string;
  firstName: string;
  lastName: string;
  profileImage?: string;
}

// Matches backend SetPreferencesRequest / UserPreferenceDto
export interface UserPreferenceDto {
  preferenceType: string;
  preferenceValue: string;
  weight: number;
}

export interface NotificationPreferences {
  preferences: UserPreferenceDto[];
}

// Review types (#171)
// Matches backend AuthoredFeedback
export interface ReviewItem {
  id: string;
  text: string;
  rating: number;
  publicationDate: string;
  authorId: string;
  authorImage: string;
  authorFullName: string;
  guideResponse?: string;
}

// Matches backend FeedbackModel
export interface WriteReviewData {
  postId: string;
  rating: number;
  text: string;
}

export interface ReviewStats {
  averageRating: number;
  totalReviews: number;
  distribution: { [key: number]: number };
}

// Notification types (#171)
// Matches backend Notification entity
export interface NotificationItem {
  id: string;
  content: string;
  authorId: string;
  authorImage: string;
  referenceLink: string;
  created: string;
  read: boolean;
  isSystem: boolean;
}

// Messages types (#171)
// Matches backend ConversationSummary
export interface Conversation {
  id: string;
  participantId: string;
  participantName: string;
  lastMessage: string;
  lastMessageAt: string;
  unreadCount: number;
}

// Matches backend MessageItem
export interface Message {
  id: string;
  conversationId: string;
  senderId: string;
  senderName: string;
  content: string;
  sentAt: string;
  isRead: boolean;
}

// Matches backend SendMessageRequest
export interface SendMessageRequest {
  conversationId: string;
  content: string;
}

// Matches backend PagedList<T>
export interface PagedResult<T> {
  items: T[];
  itemsCount: number;
  pageNumber: number;
}

// Matches backend ConversationListResponse
export interface ConversationListResponse {
  conversations: Conversation[];
  totalCount: number;
  page: number;
  pageSize: number;
}

// Matches backend MessageListResponse
export interface MessageListResponse {
  messages: Message[];
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
