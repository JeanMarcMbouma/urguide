import axios from 'axios';
import { authService } from './authService';
import type {
  SearchFilters,
  SearchResult,
  GuideListItem,
  GuideDetail,
  FeaturedContent,
  TourPreview,
  CreateTourRequestData,
  TourRequest,
  Bid,
  Booking,
  PaymentRequest,
  PaymentInfo,
  TransactionHistoryResponse,
  UserProfile,
  UpdateProfileRequest,
  NotificationPreferences,
  WriteReviewData,
  ReviewItem,
  ReviewStats,
  NotificationItem,
  Conversation,
  Message,
  SendMessageRequest,
  PagedResult,
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

const mapToTourPreview = (item: Record<string, unknown>): TourPreview => ({
  id: Number(item.id) || 0,
  title: String(item.title || ''),
  description: String(item.description || ''),
  imageUrl: item.imageUrl ? String(item.imageUrl) : undefined,
  guideName: String(item.guideName || ''),
  rating: Number(item.rating) || 0,
  price: Number(item.price) || 0,
  currency: String(item.currency || 'USD'),
});

export const getFeaturedContent = async (): Promise<FeaturedContent> => {
  const [guides, popular] = await Promise.all([
    api.post('/search/posts', { pageSize: 6, sortBy: 'rating' }),
    api.get('/recommendation/popular', { params: { count: 6 } }),
  ]);

  const combined = [...(guides.data.items || []), ...(popular.data || [])];
  return {
    featuredGuides: [],
    popularDestinations: [],
    recentTours: combined.slice(0, 6).map(mapToTourPreview),
  };
};

export const searchGuides = async (filters: SearchFilters): Promise<SearchResult<GuideListItem>> => {
  const { data } = await api.post('/search/posts', {
    query: filters.query,
    location: filters.location,
    minPrice: filters.minPrice,
    maxPrice: filters.maxPrice,
    minRating: filters.minRating,
    languages: filters.languages,
    specialties: filters.specialties,
    sortBy: filters.sortBy,
    sortOrder: filters.sortOrder,
    page: filters.page || 1,
    pageSize: filters.pageSize || 12,
  });
  return data;
};

export const getSearchSuggestions = async (query: string): Promise<string[]> => {
  const { data } = await api.post('/search/autocomplete', { query });
  return data.suggestions || [];
};

export const getGuideProfile = async (guideId: string): Promise<GuideDetail> => {
  const { data } = await api.get(`/posts/${guideId}/retrieve`);
  return data;
};

export const getGuideReviews = async (guideId: string, page = 1): Promise<PagedResult<ReviewItem>> => {
  const { data } = await api.get(`/feedback/users/${guideId}`, { params: { page, pageSize: 10 } });
  return data;
};

export const getPopularTours = async (count = 10): Promise<TourPreview[]> => {
  const { data } = await api.get('/recommendation/popular', { params: { count } });
  return data;
};

export const getRecommendations = async (lat?: number, lng?: number): Promise<TourPreview[]> => {
  const { data } = await api.get('/recommendation', { params: { lat, lng } });
  return data;
};

// ==========================================
// Tour Booking & Bidding (#169)
// ==========================================

export const createTourRequest = async (request: CreateTourRequestData): Promise<TourRequest> => {
  const { data } = await api.post('/tour-requests', request);
  return data;
};

export const getMyTourRequests = async (page = 1, pageSize = 10): Promise<PagedResult<TourRequest>> => {
  const { data } = await api.get('/tour-requests/my', { params: { page, pageSize } });
  return data;
};

export const getTourRequest = async (tourRequestId: number): Promise<TourRequest> => {
  const { data } = await api.get(`/tour-requests/${tourRequestId}`);
  return data;
};

export const cancelTourRequest = async (tourRequestId: number): Promise<void> => {
  await api.post(`/tour-requests/${tourRequestId}/cancel`);
};

export const getBidsForPost = async (postId: number): Promise<Bid[]> => {
  const { data } = await api.post(`/bid/${postId}/history`);
  return data;
};

export const acceptBid = async (postId: number): Promise<void> => {
  await api.post(`/bid/${postId}/accept`);
};

export const rejectBid = async (postId: number): Promise<void> => {
  await api.post(`/bid/${postId}/reject`);
};

export const getBookings = async (page = 1, pageSize = 10): Promise<PagedResult<Booking>> => {
  const { data } = await api.post('/posts/owned', { page, pageSize });
  return data;
};

export const reserveTour = async (postId: number, seats: number): Promise<void> => {
  await api.post(`/posts/${postId}/makereservation`, { seats });
};

export const cancelReservation = async (postId: number): Promise<void> => {
  await api.post(`/posts/${postId}/cancelreservation`);
};

// ==========================================
// Payment & User Profile (#170)
// ==========================================

export const createPayment = async (payment: PaymentRequest): Promise<PaymentInfo> => {
  const { data } = await api.post('/payment', payment);
  return data;
};

export const getPayment = async (paymentId: number): Promise<PaymentInfo> => {
  const { data } = await api.get(`/payment/${paymentId}`);
  return data;
};

export const confirmPayment = async (paymentId: number): Promise<void> => {
  await api.post(`/payment/${paymentId}/confirm`);
};

export const cancelPayment = async (paymentId: number): Promise<void> => {
  await api.post(`/payment/${paymentId}/cancel`);
};

export const getTransactionHistory = async (page = 1, pageSize = 10): Promise<TransactionHistoryResponse> => {
  const { data } = await api.get('/payment/transactions', { params: { page, pageSize } });
  return data;
};

export const getUserProfile = async (): Promise<UserProfile> => {
  const { data } = await api.get('/auth/me');
  return data;
};

export const updateProfile = async (profile: UpdateProfileRequest): Promise<void> => {
  await api.post('/account/updateuser', profile);
};

export const changePassword = async (currentPassword: string, newPassword: string): Promise<void> => {
  await api.post('/account/changepassword', { currentPassword, newPassword });
};

export const getNotificationPreferences = async (): Promise<NotificationPreferences> => {
  const { data } = await api.get('/recommendation/preferences');
  return data;
};

export const updateNotificationPreferences = async (prefs: NotificationPreferences): Promise<void> => {
  await api.put('/recommendation/preferences', prefs);
};

// ==========================================
// Reviews & Communication (#171)
// ==========================================

export const submitReview = async (review: WriteReviewData): Promise<void> => {
  const formData = new FormData();
  formData.append('rating', review.rating.toString());
  formData.append('comment', review.comment);
  if (review.photos) {
    review.photos.forEach((photo) => {
      formData.append('photos', photo);
    });
  }
  await api.post(`/posts/${review.postId}/feedback`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
};

export const getMyReviews = async (page = 1, pageSize = 10): Promise<PagedResult<ReviewItem>> => {
  const { data } = await api.post('/posts/owned', { page, pageSize });
  return data;
};

export const getReviewStats = async (guideId: string): Promise<ReviewStats> => {
  const { data } = await api.get(`/feedback/users/${guideId}`);
  return data;
};

export const getNotifications = async (page = 1, pageSize = 20): Promise<PagedResult<NotificationItem>> => {
  const { data } = await api.get('/notifications/all', { params: { page, pageSize } });
  return data;
};

export const getUnreadNotifications = async (): Promise<PagedResult<NotificationItem>> => {
  const { data } = await api.get('/notifications/unread');
  return data;
};

export const markNotificationRead = async (notificationId: number): Promise<void> => {
  await api.put(`/notifications/${notificationId}/mark_as_read`);
};

export const getConversations = async (page = 1, pageSize = 20): Promise<PagedResult<Conversation>> => {
  const { data } = await api.get('/messages/conversations', { params: { page, pageSize } });
  return data;
};

export const getMessages = async (conversationId: number, page = 1, pageSize = 50): Promise<PagedResult<Message>> => {
  const { data } = await api.get(`/messages/conversations/${conversationId}`, { params: { page, pageSize } });
  return data;
};

export const sendMessage = async (request: SendMessageRequest): Promise<Message> => {
  const { data } = await api.post('/messages', request);
  return data;
};

export const markConversationRead = async (conversationId: number): Promise<void> => {
  await api.put(`/messages/conversations/${conversationId}/read`);
};
