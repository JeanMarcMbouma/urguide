import { useState, useEffect, useCallback } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Avatar,
  Rating,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Alert,
  CircularProgress,
  LinearProgress,
} from '@mui/material';
import { Reply as ReplyIcon } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import { useAuth } from '../hooks/useAuth';
import type { Review, ReviewFilters } from '../types/guide.types';

const Reviews = () => {
  const { t } = useTranslation();
  const { user } = useAuth();
  const [reviews, setReviews] = useState<Review[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [ratingFilter, setRatingFilter] = useState<number | ''>('');
  const [sortBy, setSortBy] = useState<'newest' | 'oldest'>('newest');
  const [replyReview, setReplyReview] = useState<Review | null>(null);
  const [replyText, setReplyText] = useState('');
  const [totalCount, setTotalCount] = useState(0);

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 4000);
  };

  const load = useCallback(async () => {
    if (!user?.id) return;
    setLoading(true);
    setError('');
    try {
      const filters: ReviewFilters = {};
      if (ratingFilter !== '') filters.rating = ratingFilter as number;
      const result = await guideApi.getReviews(user.id, filters);
      let items = result.items ?? [];
      items = [...items].sort((a, b) => {
        const ta = new Date(a.createdAt).getTime();
        const tb = new Date(b.createdAt).getTime();
        return sortBy === 'newest' ? tb - ta : ta - tb;
      });
      setReviews(items);
      setTotalCount(result.totalCount ?? items.length);
    } catch {
      setError(t('reviews.loadError'));
    } finally {
      setLoading(false);
    }
  }, [user?.id, ratingFilter, sortBy, t]);

  useEffect(() => { load(); }, [load]);

  // Compute rating distribution from loaded reviews
  const ratingDist: Record<number, number> = { 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 };
  reviews.forEach((r) => { if (r.rating >= 1 && r.rating <= 5) ratingDist[r.rating]++; });
  const avgRating = reviews.length > 0
    ? reviews.reduce((s, r) => s + r.rating, 0) / reviews.length
    : 0;

  const handleSubmitResponse = async () => {
    if (!replyReview || !replyText.trim()) return;
    try {
      await guideApi.submitReviewResponse(replyReview.id, replyText.trim());
      setReviews((prev) =>
        prev.map((r) => r.id === replyReview.id ? { ...r, guideResponse: replyText.trim() } : r)
      );
      setReplyReview(null);
      setReplyText('');
      showAlert('success', t('reviews.responseSuccess'));
    } catch {
      showAlert('error', t('reviews.responseError'));
    }
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
        <CircularProgress />
        <Typography sx={{ ml: 2 }}>{t('reviews.loading')}</Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>{t('reviews.title')}</Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>{t('reviews.subtitle')}</Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {alert && <Alert severity={alert.type} sx={{ mb: 2 }}>{alert.message}</Alert>}

      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} md={4}>
          <Paper elevation={2} sx={{ p: 3, textAlign: 'center' }}>
            <Typography variant="h2" fontWeight="bold">{avgRating.toFixed(1)}</Typography>
            <Rating value={avgRating} precision={0.1} readOnly />
            <Typography variant="body2" color="text.secondary">
              {t('reviews.basedOn', { count: totalCount })}
            </Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} md={8}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>{t('reviews.ratingDistribution')}</Typography>
            {[5, 4, 3, 2, 1].map((star) => (
              <Box key={star} sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 0.5 }}>
                <Typography variant="body2" sx={{ minWidth: 40 }}>{star} ★</Typography>
                <LinearProgress
                  variant="determinate"
                  value={totalCount > 0 ? (ratingDist[star] / totalCount) * 100 : 0}
                  sx={{ flexGrow: 1, height: 8, borderRadius: 1 }}
                />
                <Typography variant="body2" sx={{ minWidth: 24 }}>{ratingDist[star]}</Typography>
              </Box>
            ))}
          </Paper>
        </Grid>
      </Grid>

      <Paper elevation={2} sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={6}>
            <FormControl fullWidth size="small">
              <InputLabel>{t('reviews.filterByRating')}</InputLabel>
              <Select
                value={ratingFilter}
                label={t('reviews.filterByRating')}
                onChange={(e) => setRatingFilter(e.target.value as number | '')}
              >
                <MenuItem value="">{t('reviews.allRatings')}</MenuItem>
                {[5, 4, 3, 2, 1].map((s) => (
                  <MenuItem key={s} value={s}>{s} {t('reviews.stars')}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={6}>
            <FormControl fullWidth size="small">
              <InputLabel>{t('reviews.sortBy')}</InputLabel>
              <Select
                value={sortBy}
                label={t('reviews.sortBy')}
                onChange={(e) => setSortBy(e.target.value as 'newest' | 'oldest')}
              >
                <MenuItem value="newest">{t('reviews.newestFirst')}</MenuItem>
                <MenuItem value="oldest">{t('reviews.oldestFirst')}</MenuItem>
              </Select>
            </FormControl>
          </Grid>
        </Grid>
      </Paper>

      {reviews.length === 0 ? (
        <Paper elevation={1} sx={{ p: 4, textAlign: 'center' }}>
          <Typography color="text.secondary">{t('reviews.noReviews')}</Typography>
        </Paper>
      ) : (
        reviews.map((review) => (
          <Paper key={review.id} elevation={2} sx={{ p: 3, mb: 2 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <Avatar src={review.touristAvatar}>{review.touristName[0] ?? '?'}</Avatar>
                <Box>
                  <Typography variant="subtitle1" fontWeight="bold">{review.touristName}</Typography>
                  <Typography variant="caption" color="text.secondary">
                    {new Date(review.createdAt).toLocaleDateString()}
                  </Typography>
                </Box>
              </Box>
              <Rating value={review.rating} readOnly />
            </Box>
            <Typography variant="body1" sx={{ mb: 2 }}>{review.comment}</Typography>
            {review.guideResponse ? (
              <Box sx={{ bgcolor: 'grey.50', p: 2, borderRadius: 1, borderLeft: '3px solid', borderColor: 'primary.main' }}>
                <Typography variant="body2" fontWeight="bold" gutterBottom>
                  {t('reviews.yourResponse')}
                </Typography>
                <Typography variant="body2">{review.guideResponse}</Typography>
              </Box>
            ) : (
              <Button
                size="small"
                startIcon={<ReplyIcon />}
                onClick={() => { setReplyReview(review); setReplyText(''); }}
              >
                {t('reviews.reply')}
              </Button>
            )}
          </Paper>
        ))
      )}

      <Dialog open={!!replyReview} onClose={() => setReplyReview(null)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('reviews.replyToReview')}</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            multiline
            rows={4}
            label={t('reviews.yourResponseLabel')}
            value={replyText}
            onChange={(e) => setReplyText(e.target.value)}
            placeholder={t('reviews.responsePlaceholder')}
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setReplyReview(null)}>{t('reviews.cancel')}</Button>
          <Button variant="contained" onClick={handleSubmitResponse} disabled={!replyText.trim()}>
            {t('reviews.submitResponse')}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Reviews;
