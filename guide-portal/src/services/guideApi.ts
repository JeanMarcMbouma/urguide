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
  UpdateBidRequest,
  BidHistory,
  AvailabilitySlot,
  BlockDatesRequest,
  RecurringPattern,
  EarningsSummary,
  EarningsDataPoint,
  MonthlyEarnings,
  TransactionItem,
  PayoutItem,
  CreatePayoutRequest,
  PaymentMethod,
  Review,
  ReviewFilters,
  SubmitReviewResponseRequest,
  ReviewStats,
  Conversation,
  Message,
  SendMessageRequest,
  PerformanceMetrics,
  TourStatistics,
  AnalyticsPeriod,
  PagedResult,
} from '../types/guide.types';

class GuideApiService {
  private guidesBase = '/api/guides';
  private apiBase = '/api';

  private authHeader() {
    const token = authService.getToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
  }

  // Profile (Issue #172)
  async getProfile(guideId: string): Promise<GuideProfile> {
    const { data } = await axios.get<GuideProfile>(`${this.guidesBase}/${guideId}/profile`, {
      headers: this.authHeader(),
    });
    return data;
  }

  async updateProfile(guideId: string, request: UpdateGuideProfileRequest): Promise<GuideProfile> {
    const { data } = await axios.put<GuideProfile>(
      `${this.guidesBase}/${guideId}/profile`,
      request,
      { headers: this.authHeader() }
    );
    return data;
  }

  // Gallery (Issue #172)
  async getGalleries(userId: string): Promise<Gallery[]> {
    const { data } = await axios.get<Gallery[]>(`${this.guidesBase}/${userId}/catalogs`, {
      headers: this.authHeader(),
    });
    return data;
  }

  async getGallery(catalogId: string): Promise<Gallery> {
    const { data } = await axios.get<Gallery>(`${this.guidesBase}/catalogs/${catalogId}`, {
      headers: this.authHeader(),
    });
    return data;
  }

  async createGallery(request: CreateGalleryRequest): Promise<Gallery> {
    const { data } = await axios.post<Gallery>(`${this.guidesBase}/catalogs`, request, {
      headers: this.authHeader(),
    });
    return data;
  }

  async deleteGallery(catalogId: string): Promise<void> {
    await axios.delete(`${this.guidesBase}/catalogs/${catalogId}`, {
      headers: this.authHeader(),
    });
  }

  async addImageToGallery(
    catalogId: string,
    imageFile: { fileBase64: string; fileName: string; description?: string }
  ): Promise<GalleryItem> {
    const { data } = await axios.post<GalleryItem>(
      `${this.guidesBase}/catalogs/${catalogId}/images`,
      imageFile,
      { headers: this.authHeader() }
    );
    return data;
  }

  async removeImageFromGallery(catalogId: string, imageId: string): Promise<void> {
    await axios.delete(`${this.guidesBase}/catalogs/${catalogId}/images/${imageId}`, {
      headers: this.authHeader(),
    });
  }

  // KYC Verification (Issue #172)
  async getVerificationStatus(guideId: string): Promise<KycVerificationStatus> {
    const { data } = await axios.get<KycVerificationStatus>(
      `${this.guidesBase}/${guideId}/verification`,
      { headers: this.authHeader() }
    );
    return data;
  }

  async submitVerificationDocument(
    guideId: string,
    request: SubmitVerificationRequest
  ): Promise<VerificationDocument> {
    const { data } = await axios.post<VerificationDocument>(
      `${this.guidesBase}/${guideId}/verification/documents`,
      request,
      { headers: this.authHeader() }
    );
    return data;
  }

  // Tour Requests (Issue #173)
  async getTourRequests(
    filters?: TourRequestFilters,
    page = 1
  ): Promise<PagedResult<TourRequest>> {
    const { data } = await axios.get<PagedResult<TourRequest>>(`${this.apiBase}/posts`, {
      headers: this.authHeader(),
      params: { ...filters, page, pageSize: 10 },
    });
    return data;
  }

  async getTourRequest(requestId: string): Promise<TourRequest> {
    const { data } = await axios.get<TourRequest>(`${this.apiBase}/posts/${requestId}`, {
      headers: this.authHeader(),
    });
    return data;
  }

  // Bids (Issue #173)
  async createBid(request: CreateBidRequest): Promise<Bid> {
    const { data } = await axios.post<Bid>(`${this.apiBase}/bids`, request, {
      headers: this.authHeader(),
    });
    return data;
  }

  async updateBid(request: UpdateBidRequest): Promise<Bid> {
    const { data } = await axios.put<Bid>(`${this.apiBase}/bids/${request.bidId}`, request, {
      headers: this.authHeader(),
    });
    return data;
  }

  async withdrawBid(bidId: string): Promise<void> {
    await axios.delete(`${this.apiBase}/bids/${bidId}`, {
      headers: this.authHeader(),
    });
  }

  async getMyBids(page = 1): Promise<PagedResult<Bid>> {
    const { data } = await axios.get<PagedResult<Bid>>(`${this.apiBase}/bids/my`, {
      headers: this.authHeader(),
      params: { page, pageSize: 10 },
    });
    return data;
  }

  async getBidHistory(postId: string): Promise<BidHistory> {
    const { data } = await axios.get<BidHistory>(`${this.apiBase}/bids/post/${postId}`, {
      headers: this.authHeader(),
    });
    return data;
  }

  // Availability (Issue #173)
  async getAvailability(
    guideId: string,
    startDate: string,
    endDate: string
  ): Promise<AvailabilitySlot[]> {
    const { data } = await axios.get<AvailabilitySlot[]>(
      `${this.guidesBase}/${guideId}/availability`,
      { headers: this.authHeader(), params: { startDate, endDate } }
    );
    return data;
  }

  async blockDates(guideId: string, request: BlockDatesRequest): Promise<void> {
    await axios.post(`${this.guidesBase}/${guideId}/availability/block`, request, {
      headers: this.authHeader(),
    });
  }

  async unblockDates(guideId: string, startDate: string, endDate: string): Promise<void> {
    await axios.delete(`${this.guidesBase}/${guideId}/availability/block`, {
      headers: this.authHeader(),
      params: { startDate, endDate },
    });
  }

  async setRecurringPattern(guideId: string, pattern: RecurringPattern): Promise<void> {
    await axios.post(`${this.guidesBase}/${guideId}/availability/recurring`, pattern, {
      headers: this.authHeader(),
    });
  }

  // Earnings (Issue #174)
  async getEarningsSummary(guideId: string): Promise<EarningsSummary> {
    const { data } = await axios.get<EarningsSummary>(
      `${this.guidesBase}/${guideId}/earnings/summary`,
      { headers: this.authHeader() }
    );
    return data;
  }

  async getEarningsTrend(
    guideId: string,
    period: AnalyticsPeriod
  ): Promise<EarningsDataPoint[]> {
    const { data } = await axios.get<EarningsDataPoint[]>(
      `${this.guidesBase}/${guideId}/earnings/trend`,
      { headers: this.authHeader(), params: { period } }
    );
    return data;
  }

  async getMonthlyEarnings(guideId: string, year: number): Promise<MonthlyEarnings[]> {
    const { data } = await axios.get<MonthlyEarnings[]>(
      `${this.guidesBase}/${guideId}/earnings/monthly`,
      { headers: this.authHeader(), params: { year } }
    );
    return data;
  }

  async getTransactions(guideId: string, page = 1): Promise<PagedResult<TransactionItem>> {
    const { data } = await axios.get<PagedResult<TransactionItem>>(
      `${this.guidesBase}/${guideId}/transactions`,
      { headers: this.authHeader(), params: { page, pageSize: 10 } }
    );
    return data;
  }

  // Payouts (Issue #174)
  async getPayouts(guideId: string, page = 1): Promise<PagedResult<PayoutItem>> {
    const { data } = await axios.get<PagedResult<PayoutItem>>(
      `${this.guidesBase}/${guideId}/payouts`,
      { headers: this.authHeader(), params: { page, pageSize: 10 } }
    );
    return data;
  }

  async getAvailableBalance(guideId: string): Promise<number> {
    const { data } = await axios.get<{ balance: number }>(
      `${this.guidesBase}/${guideId}/payouts/balance`,
      { headers: this.authHeader() }
    );
    return data.balance;
  }

  async createPayout(request: CreatePayoutRequest): Promise<PayoutItem> {
    const { data } = await axios.post<PayoutItem>(`${this.guidesBase}/payouts`, request, {
      headers: this.authHeader(),
    });
    return data;
  }

  async getPaymentMethods(guideId: string): Promise<PaymentMethod[]> {
    const { data } = await axios.get<PaymentMethod[]>(
      `${this.guidesBase}/${guideId}/payment-methods`,
      { headers: this.authHeader() }
    );
    return data;
  }

  // Reviews (Issue #175)
  async getReviews(
    guideId: string,
    filters?: ReviewFilters,
    page = 1
  ): Promise<PagedResult<Review>> {
    const { data } = await axios.get<PagedResult<Review>>(
      `${this.guidesBase}/${guideId}/reviews`,
      { headers: this.authHeader(), params: { ...filters, page, pageSize: 10 } }
    );
    return data;
  }

  async getReviewStats(guideId: string): Promise<ReviewStats> {
    const { data } = await axios.get<ReviewStats>(
      `${this.guidesBase}/${guideId}/reviews/stats`,
      { headers: this.authHeader() }
    );
    return data;
  }

  async submitReviewResponse(request: SubmitReviewResponseRequest): Promise<Review> {
    const { data } = await axios.post<Review>(
      `${this.apiBase}/reviews/${request.reviewId}/response`,
      { response: request.response },
      { headers: this.authHeader() }
    );
    return data;
  }

  // Messages (Issue #175)
  async getConversations(page = 1): Promise<PagedResult<Conversation>> {
    const { data } = await axios.get<PagedResult<Conversation>>(
      `${this.apiBase}/messages/conversations`,
      { headers: this.authHeader(), params: { page, pageSize: 20 } }
    );
    return data;
  }

  async getMessages(conversationId: string, page = 1): Promise<PagedResult<Message>> {
    const { data } = await axios.get<PagedResult<Message>>(
      `${this.apiBase}/messages/conversations/${conversationId}`,
      { headers: this.authHeader(), params: { page, pageSize: 50 } }
    );
    return data;
  }

  async sendMessage(request: SendMessageRequest): Promise<Message> {
    const { data } = await axios.post<Message>(`${this.apiBase}/messages`, request, {
      headers: this.authHeader(),
    });
    return data;
  }

  async markConversationRead(conversationId: string): Promise<void> {
    await axios.put(
      `${this.apiBase}/messages/conversations/${conversationId}/read`,
      {},
      { headers: this.authHeader() }
    );
  }

  // Analytics (Issue #175)
  async getPerformanceMetrics(
    guideId: string,
    period: AnalyticsPeriod
  ): Promise<PerformanceMetrics> {
    const { data } = await axios.get<PerformanceMetrics>(
      `${this.guidesBase}/${guideId}/analytics/performance`,
      { headers: this.authHeader(), params: { period } }
    );
    return data;
  }

  async getTourStatistics(guideId: string): Promise<TourStatistics> {
    const { data } = await axios.get<TourStatistics>(
      `${this.guidesBase}/${guideId}/analytics/tours`,
      { headers: this.authHeader() }
    );
    return data;
  }
}

export const guideApi = new GuideApiService();
export default guideApi;
