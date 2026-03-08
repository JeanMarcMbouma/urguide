import axios from 'axios';
import { authService } from './authService';
import type {
  GuideProfile,
  UpdateGuideProfileRequest,
  Gallery,
  GalleryItem,
  CreateGalleryRequest,
  KycVerificationStatus,
  VerificationDocument,
  SubmitVerificationRequest,
  TourRequest,
  TourRequestFilters,
  Bid,
  CreateBidRequest,
  BidHistory,
  AvailabilitySlot,
  BlockDatesRequest,
  RecurringPattern,
  TransactionHistoryResponse,
  PayoutItem,
  PayoutListResponse,
  CreatePayoutRequest,
  Review,
  AuthoredFeedback,
  ReviewFilters,
  Conversation,
  Message,
  SendMessageRequest,
  GuideDashboard,
  PerformanceMetrics,
  TourStatistics,
  AnalyticsPeriod,
  PagedResult,
} from '../types/guide.types';

class GuideApiService {
  private authHeader() {
    const token = authService.getToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
  }

  // ── Dashboard ──────────────────────────────────────────────────────────────
  async getDashboard(): Promise<GuideDashboard> {
    const { data } = await axios.get<GuideDashboard>('/api/guide/dashboard', {
      headers: this.authHeader(),
    });
    return data;
  }

  // ── Profile ────────────────────────────────────────────────────────────────
  async getProfile(): Promise<GuideProfile> {
    const { data } = await axios.get<GuideProfile>('/getdetails', {
      headers: this.authHeader(),
    });
    return data;
  }

  async updateProfile(request: UpdateGuideProfileRequest): Promise<GuideProfile> {
    const { data } = await axios.post<GuideProfile>('/updateguide', request, {
      headers: this.authHeader(),
    });
    return data;
  }

  // ── Gallery ────────────────────────────────────────────────────────────────
  async getGalleries(userId: string): Promise<Gallery[]> {
    const { data } = await axios.get<Gallery[]>(`/api/catalogs/${userId}/all`, {
      headers: this.authHeader(),
    });
    return data;
  }

  async getGallery(catalogId: string): Promise<Gallery> {
    const { data } = await axios.get<Gallery>(`/api/catalogs/${catalogId}/retrieve`, {
      headers: this.authHeader(),
    });
    return data;
  }

  async createGallery(request: CreateGalleryRequest): Promise<Gallery> {
    const { data } = await axios.post<Gallery>('/api/catalogs/create', request, {
      headers: this.authHeader(),
    });
    return data;
  }

  async deleteGallery(catalogId: string): Promise<void> {
    await axios.delete(`/api/catalogs/${catalogId}/remove`, {
      headers: this.authHeader(),
    });
  }

  async addImageToGallery(
    catalogId: string,
    imageFile: { fileBase64: string; fileName: string; description?: string }
  ): Promise<GalleryItem> {
    const { data } = await axios.put<GalleryItem>(
      `/api/catalogs/update/${catalogId}/addimage`,
      imageFile,
      { headers: this.authHeader() }
    );
    return data;
  }

  async removeImageFromGallery(catalogId: string, imageId: string): Promise<void> {
    await axios.put(
      `/api/catalogs/update/${catalogId}/images/${imageId}/remove`,
      {},
      { headers: this.authHeader() }
    );
  }

  // ── KYC Verification ───────────────────────────────────────────────────────
  async getVerificationStatus(): Promise<KycVerificationStatus> {
    const { data } = await axios.get<KycVerificationStatus>('/api/guide-verification/status', {
      headers: this.authHeader(),
    });
    return data;
  }

  async submitVerificationDocument(
    request: SubmitVerificationRequest
  ): Promise<VerificationDocument> {
    const { data } = await axios.post<VerificationDocument>(
      '/api/guide-verification/documents',
      request,
      { headers: this.authHeader() }
    );
    return data;
  }

  // ── Tour Requests ──────────────────────────────────────────────────────────
  async getTourRequests(
    filters?: TourRequestFilters,
    page = 1
  ): Promise<PagedResult<TourRequest>> {
    const params: Record<string, unknown> = { PageNumber: page, PageSize: 10 };
    if (filters?.status && filters.status !== 'all') params.Status = filters.status;
    if (filters?.searchTerm) params.SearchTerm = filters.searchTerm;

    const { data } = await axios.get<PagedResult<TourRequest>>('/api/tour-requests', {
      headers: this.authHeader(),
      params,
    });
    // Normalise id
    const items = (data.items ?? []).map((r) => ({
      ...r,
      id: r.id ?? (r as TourRequest & { tourRequestId?: string }).tourRequestId ?? '',
    }));
    return { ...data, items };
  }

  async getTourRequest(requestId: string): Promise<TourRequest> {
    const { data } = await axios.get<TourRequest>(`/api/tour-requests/${requestId}`, {
      headers: this.authHeader(),
    });
    return { ...data, id: data.id ?? (data as TourRequest & { tourRequestId?: string }).tourRequestId ?? requestId };
  }

  // ── Bids ───────────────────────────────────────────────────────────────────
  async createBid(request: CreateBidRequest): Promise<Bid> {
    const { data } = await axios.post<Bid>(`/api/bid/${request.postId}/newbid`, request, {
      headers: this.authHeader(),
    });
    return data;
  }

  async getBidHistory(postId: string): Promise<BidHistory> {
    const { data } = await axios.post<BidHistory>(
      `/api/bid/${postId}/history`,
      {},
      { headers: this.authHeader() }
    );
    return data;
  }

  // ── Transactions / Earnings ────────────────────────────────────────────────
  async getTransactions(page = 1, pageSize = 10): Promise<TransactionHistoryResponse> {
    const { data } = await axios.get<TransactionHistoryResponse>('/api/payment/transactions', {
      headers: this.authHeader(),
      params: { page, pageSize },
    });
    return data;
  }

  // ── Payouts ────────────────────────────────────────────────────────────────
  async createPayout(request: CreatePayoutRequest): Promise<PayoutItem> {
    const { data } = await axios.post<PayoutItem>('/api/payout', request, {
      headers: this.authHeader(),
    });
    return data;
  }

  async getPayouts(guideId: string, page = 1, pageSize = 10): Promise<PayoutListResponse> {
    const { data } = await axios.get<PayoutListResponse>(`/api/payout/guide/${guideId}`, {
      headers: this.authHeader(),
      params: { page, pageSize },
    });
    return data;
  }

  async getAvailableBalance(guideId: string): Promise<number> {
    const { data } = await axios.get<{ guideId: string; availableBalance: number }>(
      `/api/payout/guide/${guideId}/balance`,
      { headers: this.authHeader() }
    );
    return data.availableBalance;
  }

  // ── Reviews ────────────────────────────────────────────────────────────────
  async getReviews(
    userId: string,
    filters?: ReviewFilters,
    page = 1
  ): Promise<PagedResult<Review>> {
    const params: Record<string, unknown> = { PageNumber: page };
    if (filters?.rating) params.rating = filters.rating;

    const { data } = await axios.get<PagedResult<AuthoredFeedback>>(
      `/feedback/users/${userId}`,
      { headers: this.authHeader(), params }
    );

    // Map AuthoredFeedback → Review
    const items: Review[] = (data.items ?? []).map((fb) => ({
      id: fb.id ?? `${fb.authorId}-${fb.publicationDate}`,
      touristId: fb.authorId,
      touristName: fb.authorFullName,
      touristAvatar: fb.authorImage ?? '',
      rating: fb.rating,
      comment: fb.text,
      guideResponse: fb.guideResponse,
      createdAt: fb.publicationDate,
    }));

    return { ...data, items };
  }

  async submitReviewResponse(feedbackId: string, response: string): Promise<void> {
    await axios.post(
      `/feedback/${feedbackId}/respond`,
      { response },
      { headers: this.authHeader() }
    );
  }

  // ── Availability ───────────────────────────────────────────────────────────
  async getAvailability(startDate: string, endDate: string): Promise<AvailabilitySlot[]> {
    const { data } = await axios.get<{ slots: AvailabilitySlot[]; startDate: string; endDate: string }>(
      '/api/availability',
      { headers: this.authHeader(), params: { startDate, endDate } }
    );
    return data.slots ?? [];
  }

  async blockDates(request: BlockDatesRequest): Promise<void> {
    await axios.post('/api/availability/block', request, {
      headers: this.authHeader(),
    });
  }

  async unblockDates(startDate: string, endDate: string): Promise<void> {
    await axios.delete('/api/availability/block', {
      headers: this.authHeader(),
      params: { startDate, endDate },
    });
  }

  async setRecurringPattern(pattern: RecurringPattern): Promise<void> {
    await axios.post('/api/availability/recurring', pattern, {
      headers: this.authHeader(),
    });
  }

  // ── Messages ───────────────────────────────────────────────────────────────
  async getConversations(page = 1, pageSize = 20): Promise<PagedResult<Conversation>> {
    const { data } = await axios.get<PagedResult<Conversation>>(
      '/api/messages/conversations',
      { headers: this.authHeader(), params: { page, pageSize } }
    );
    return data;
  }

  async getMessages(conversationId: string, page = 1, pageSize = 50): Promise<PagedResult<Message>> {
    const { data } = await axios.get<PagedResult<Message>>(
      `/api/messages/conversations/${conversationId}`,
      { headers: this.authHeader(), params: { page, pageSize } }
    );
    return data;
  }

  async sendMessage(request: SendMessageRequest): Promise<Message> {
    const { data } = await axios.post<Message>('/api/messages', request, {
      headers: this.authHeader(),
    });
    return data;
  }

  async markConversationRead(conversationId: string): Promise<void> {
    await axios.put(
      `/api/messages/conversations/${conversationId}/read`,
      {},
      { headers: this.authHeader() }
    );
  }

  // ── Analytics (reuses dashboard; guide-specific analytics not yet available) ─
  async getPerformanceMetrics(_guideId: string, _period: AnalyticsPeriod): Promise<PerformanceMetrics> {
    // Return placeholder until a dedicated analytics endpoint is available
    return {
      responseRate: 0,
      responseTimeAvg: 0,
      completionRate: 0,
      cancellationRate: 0,
      repeatClientRate: 0,
    };
  }

  async getTourStatistics(_guideId: string): Promise<TourStatistics> {
    return {
      totalTours: 0,
      completedTours: 0,
      cancelledTours: 0,
      averageDuration: 0,
      topDestinations: [],
    };
  }
}

export const guideApi = new GuideApiService();
export default guideApi;
