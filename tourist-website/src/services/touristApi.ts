import axios from 'axios';
import { authService } from './authService';
import type {
  SearchFilters,
  SearchResult,
  GuideListItem,
  GuideDetail,
  TourPreview,
  CreateTourRequestData,
  TourRequest,
  Bid,
  Booking,
  PaymentRequest,
  PaymentInfo,
  PaymentDetails,
  TransactionHistoryResponse,
  UpdateProfileRequest,
  UserProfile,
  UserPreferenceDto,
  WriteReviewData,
  ReviewItem,
  NotificationItem,
  Message,
  SendMessageRequest,
  PagedResult,
  ConversationListResponse,
  MessageListResponse,
} from '../types/tourist.types';

const api = axios.create({
  baseURL: '/api',
});

api.interceptors.request.use((config) => {
  const token = authService.getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      authService.removeToken();
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// ==========================================
// Discovery & Search (#168)
// ==========================================

// Maps SearchResponse<PostSearchDocument> results to SearchResult<GuideListItem>
export const searchGuides = async (filters: SearchFilters): Promise<SearchResult<GuideListItem>> => {
  const { data } = await api.post('/search/posts', {
    query: filters.query,
    page: filters.page || 1,
    pageSize: filters.pageSize || 12,
    sortBy: filters.sortBy || 'relevance',
    sortOrder: filters.sortOrder || 'desc',
    filters: {
      location: filters.location,
      minPrice: filters.minPrice,
      maxPrice: filters.maxPrice,
      minRating: filters.minRating,
      tags: filters.specialties,
    },
  });
  // Map SearchResponse<PostSearchDocument> → SearchResult<GuideListItem>
  const guideItems = (data.results || []).map((item: Record<string, unknown>) => {
    const doc = (item as { document?: Record<string, unknown> }).document || item;
    return {
      id: String(doc.id || ''),
      firstName: String(doc.author || '').split(' ')[0] || '',
      lastName: String(doc.author || '').split(' ').slice(1).join(' ') || '',
      profileImageUrl: doc.authorAvatar ? String(doc.authorAvatar) : undefined,
      rating: Number(doc.rating) || 0,
      reviewCount: Number(doc.reviews) || 0,
      languages: Array.isArray(doc.categories) ? doc.categories.map(String) : [],
      specialties: Array.isArray(doc.tags) ? doc.tags.map(String) : [],
      location: String(doc.location || ''),
      pricePerHour: Number(doc.price) || 0,
      currency: 'USD',
      verified: false,
    } as GuideListItem;
  });
  return {
    items: guideItems,
    totalCount: Number(data.totalHits) || 0,
    page: Number(data.page) || 1,
    pageSize: Number(data.pageSize) || 12,
    totalPages: Number(data.totalPages) || 1,
  };
};

// Maps AutocompleteResponse → string[] (extracts .text from each suggestion)
export const getSearchSuggestions = async (query: string): Promise<string[]> => {
  const { data } = await api.post('/search/autocomplete', { query });
  const suggestions = data.suggestions || [];
  return suggestions.map((s: { text: string }) => s.text || String(s));
};

// Gets PostModel by postId and maps to GuideDetail
export const getGuideProfile = async (postId: string): Promise<GuideDetail> => {
  const { data } = await api.get(`/posts/${postId}/retrieve`);
  return {
    id: String(data.id || ''),
    firstName: String(data.author || '').split(' ')[0] || '',
    lastName: String(data.author || '').split(' ').slice(1).join(' ') || '',
    profileImageUrl: data.authorAvatar || undefined,
    rating: Number(data.rating) || 0,
    reviewCount: Number(data.reviews) || 0,
    languages: Array.isArray(data.categories) ? data.categories : [],
    specialties: [],
    location: String(data.location || ''),
    pricePerHour: Number(data.price) || 0,
    currency: 'USD',
    verified: false,
    bio: String(data.description || ''),
    experience: 0,
    galleries: (data.images || []).map((img: Record<string, unknown>, idx: number) => ({
      id: String(img.id || idx),
      title: String(img.fileName || `Photo ${idx + 1}`),
      coverImageUrl: String(img.url || ''),
      imageCount: 1,
    })),
    recentReviews: [],
    availability: data.status !== 'Closed',
  };
};

// Matches GET /feedback/users/{userId} → PagedList<AuthoredFeedback>
export const getGuideReviews = async (userId: string, page = 1): Promise<PagedResult<ReviewItem>> => {
  const { data } = await api.get(`/feedback/users/${userId}`, { params: { pageNumber: page } });
  return {
    items: (data.items || []).map((item: Record<string, unknown>) => ({
      id: String(item.id || ''),
      text: String(item.text || ''),
      rating: Number(item.rating) || 0,
      publicationDate: String(item.publicationDate || ''),
      authorId: String(item.authorId || ''),
      authorImage: String(item.authorImage || ''),
      authorFullName: String(item.authorFullName || ''),
      guideResponse: item.guideResponse ? String(item.guideResponse) : undefined,
    } as ReviewItem)),
    itemsCount: Number(data.itemsCount) || 0,
    pageNumber: Number(data.pageNumber) || 1,
  };
};

export const getPopularTours = async (count = 10): Promise<TourPreview[]> => {
  const { data } = await api.get('/recommendation/popular', { params: { count } });
  return (Array.isArray(data) ? data : []).map((item: Record<string, unknown>) => ({
    id: String(item.id || ''),
    title: String(item.text || item.title || ''),
    description: String(item.description || ''),
    imageUrl: undefined,
    guideName: String(item.author || ''),
    rating: Number(item.rating) || 0,
    price: Number(item.price) || 0,
    currency: 'USD',
  }));
};

export const getRecommendations = async (lat?: number, lng?: number): Promise<TourPreview[]> => {
  const { data } = await api.get('/recommendation', { params: { lat, lng } });
  return (Array.isArray(data) ? data : []).map((item: Record<string, unknown>) => ({
    id: String(item.id || ''),
    title: String(item.text || item.title || ''),
    description: String(item.description || ''),
    imageUrl: undefined,
    guideName: String(item.author || ''),
    rating: Number(item.rating) || 0,
    price: Number(item.price) || 0,
    currency: 'USD',
  }));
};

// ==========================================
// Tour Booking & Bidding (#169)
// ==========================================

// POST /api/tour-requests → TourRequestModel
export const createTourRequest = async (request: CreateTourRequestData): Promise<TourRequest> => {
  const { data } = await api.post('/tour-requests', request);
  return data;
};

// GET /api/tour-requests/my → PagedList<TourRequestModel>
export const getMyTourRequests = async (page = 1): Promise<PagedResult<TourRequest>> => {
  const { data } = await api.get('/tour-requests/my', { params: { pageNumber: page } });
  return data;
};

// GET /api/tour-requests/{tourRequestId} → TourRequestModel
export const getTourRequest = async (tourRequestId: string): Promise<TourRequest> => {
  const { data } = await api.get(`/tour-requests/${tourRequestId}`);
  return data;
};

// POST /api/tour-requests/{tourRequestId}/cancel
export const cancelTourRequest = async (tourRequestId: string): Promise<void> => {
  await api.post(`/tour-requests/${tourRequestId}/cancel`);
};

// POST /api/bid/{postId}/history → BidHistoryModel[]
export const getBidsForPost = async (postId: string): Promise<Bid[]> => {
  const { data } = await api.post(`/bid/${postId}/history`);
  return data;
};

// POST /api/bid/{postId}/accept
export const acceptBid = async (postId: string): Promise<void> => {
  await api.post(`/bid/${postId}/accept`);
};

// POST /api/bid/{postId}/reject
export const rejectBid = async (postId: string): Promise<void> => {
  await api.post(`/bid/${postId}/reject`);
};

// POST /api/posts/owned → PagedList<PostModel>, mapped to Booking[]
export const getBookings = async (page = 1): Promise<PagedResult<Booking>> => {
  const { data } = await api.post('/posts/owned', { pageNumber: page });
  return {
    items: (data.items || []).map((item: Record<string, unknown>) => ({
      id: String(item.id || ''),
      text: String(item.text || ''),
      description: String(item.description || ''),
      price: String(item.price || '0'),
      rating: String(item.rating || '0'),
      location: String(item.location || ''),
      status: String(item.status || ''),
      startDate: String(item.startDate || ''),
      endDate: String(item.endDate || ''),
      seats: Number(item.seats) || 0,
      reservedSeats: Number(item.reservedSeats) || 0,
      authorId: String(item.authorId || ''),
      author: String(item.author || ''),
      authorAvatar: String(item.authorAvatar || ''),
      hasReserved: Boolean(item.hasReserved),
      reviews: Number(item.reviews) || 0,
    } as Booking)),
    itemsCount: Number(data.itemsCount) || 0,
    pageNumber: Number(data.pageNumber) || 1,
  };
};

// POST /api/posts/{postId}/makereservation with SeatReservationModel
export const reserveTour = async (postId: string, seats: number): Promise<void> => {
  await api.post(`/posts/${postId}/makereservation`, { postId, seats });
};

export const cancelReservation = async (postId: string): Promise<void> => {
  await api.post(`/posts/${postId}/cancelreservation`);
};

// ==========================================
// Payment & User Profile (#170)
// ==========================================

// POST /api/payment → PaymentResponse
export const createPayment = async (payment: PaymentRequest): Promise<PaymentInfo> => {
  const { data } = await api.post('/payment', payment);
  return data;
};

// GET /api/payment/{paymentId} → PaymentDetailsResponse
export const getPayment = async (paymentId: string): Promise<PaymentDetails> => {
  const { data } = await api.get(`/payment/${paymentId}`);
  return data;
};

// POST /api/payment/{paymentId}/confirm
export const confirmPayment = async (paymentId: string): Promise<void> => {
  await api.post(`/payment/${paymentId}/confirm`);
};

// POST /api/payment/{paymentId}/cancel
export const cancelPayment = async (paymentId: string): Promise<void> => {
  await api.post(`/payment/${paymentId}/cancel`);
};

// GET /api/payment/transactions
export const getTransactionHistory = async (page = 1, pageSize = 10): Promise<TransactionHistoryResponse> => {
  const { data } = await api.get('/payment/transactions', { params: { page, pageSize } });
  return data;
};

// GET /api/auth/me → { id, email, userName, firstName, lastName, roles }
export const getUserProfile = async (): Promise<UserProfile> => {
  const { data } = await api.get('/auth/me');
  return data;
};

// POST /updateuser (absolute route, not under /api/account)
// Must bypass api instance's /api baseURL since this endpoint is at root
const rootApi = axios.create({ baseURL: '/' });
rootApi.interceptors.request.use((config) => {
  const token = authService.getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const updateProfile = async (profile: UpdateProfileRequest): Promise<void> => {
  await rootApi.post('/updateuser', profile);
};

// POST /api/account/changepassword → ChangePasswordModel
export const changePassword = async (
  email: string,
  currentPassword: string,
  newPassword: string,
  confirmPassword: string,
): Promise<void> => {
  await api.post('/account/changepassword', {
    email,
    currentPassword,
    password: newPassword,
    confirmPassword,
  });
};

// GET /api/recommendation/preferences → UserPreferenceDto[]
export const getNotificationPreferences = async (): Promise<UserPreferenceDto[]> => {
  const { data } = await api.get('/recommendation/preferences');
  return Array.isArray(data) ? data : (data.preferences || []);
};

// PUT /api/recommendation/preferences → SetPreferencesRequest
export const updateNotificationPreferences = async (preferences: UserPreferenceDto[]): Promise<void> => {
  await api.put('/recommendation/preferences', { preferences });
};

// ==========================================
// Reviews & Communication (#171)
// ==========================================

// POST /posts/{postId}/feedback → FeedbackModel (JSON, not multipart)
export const submitReview = async (review: WriteReviewData): Promise<void> => {
  await api.post(`/posts/${review.postId}/feedback`, {
    text: review.text,
    rating: review.rating,
  });
};

// GET /feedback/users/{userId} → PagedList<AuthoredFeedback>
export const getMyReviews = async (userId: string, page = 1): Promise<PagedResult<ReviewItem>> => {
  const { data } = await api.get(`/feedback/users/${userId}`, { params: { pageNumber: page } });
  return {
    items: (data.items || []).map((item: Record<string, unknown>) => ({
      id: String(item.id || ''),
      text: String(item.text || ''),
      rating: Number(item.rating) || 0,
      publicationDate: String(item.publicationDate || ''),
      authorId: String(item.authorId || ''),
      authorImage: String(item.authorImage || ''),
      authorFullName: String(item.authorFullName || ''),
      guideResponse: item.guideResponse ? String(item.guideResponse) : undefined,
    } as ReviewItem)),
    itemsCount: Number(data.itemsCount) || 0,
    pageNumber: Number(data.pageNumber) || 1,
  };
};

// GET /api/notifications/all → PagedList<Notification>
export const getNotifications = async (page = 1): Promise<PagedResult<NotificationItem>> => {
  const { data } = await api.get('/notifications/all', { params: { pageNumber: page } });
  return {
    items: (data.items || []).map((item: Record<string, unknown>) => ({
      id: String(item.id || ''),
      content: String(item.content || ''),
      authorId: String(item.authorId || ''),
      authorImage: String(item.authorImage || ''),
      referenceLink: String(item.referenceLink || ''),
      created: String(item.created || ''),
      read: Boolean(item.read),
      isSystem: Boolean(item.isSystem),
    } as NotificationItem)),
    itemsCount: Number(data.itemsCount) || 0,
    pageNumber: Number(data.pageNumber) || 1,
  };
};

// GET /api/notifications/unread → PagedList<Notification>
export const getUnreadNotifications = async (): Promise<PagedResult<NotificationItem>> => {
  const { data } = await api.get('/notifications/unread');
  return {
    items: (data.items || []).map((item: Record<string, unknown>) => ({
      id: String(item.id || ''),
      content: String(item.content || ''),
      authorId: String(item.authorId || ''),
      authorImage: String(item.authorImage || ''),
      referenceLink: String(item.referenceLink || ''),
      created: String(item.created || ''),
      read: Boolean(item.read),
      isSystem: Boolean(item.isSystem),
    } as NotificationItem)),
    itemsCount: Number(data.itemsCount) || 0,
    pageNumber: Number(data.pageNumber) || 1,
  };
};

// PUT /api/notifications/{id}/mark_as_read (string id)
export const markNotificationRead = async (notificationId: string): Promise<void> => {
  await api.put(`/notifications/${notificationId}/mark_as_read`);
};

// GET /api/messages/conversations → ConversationListResponse
export const getConversations = async (page = 1, pageSize = 20): Promise<ConversationListResponse> => {
  const { data } = await api.get('/messages/conversations', { params: { page, pageSize } });
  return data;
};

// GET /api/messages/conversations/{conversationId} → MessageListResponse
export const getMessages = async (conversationId: string, page = 1, pageSize = 50): Promise<MessageListResponse> => {
  const { data } = await api.get(`/messages/conversations/${conversationId}`, { params: { page, pageSize } });
  return data;
};

// POST /api/messages → MessageItem
export const sendMessage = async (request: SendMessageRequest): Promise<Message> => {
  const { data } = await api.post('/messages', request);
  return data;
};

// PUT /api/messages/conversations/{conversationId}/read
export const markConversationRead = async (conversationId: string): Promise<void> => {
  await api.put(`/messages/conversations/${conversationId}/read`);
};
