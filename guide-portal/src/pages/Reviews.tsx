import { useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Avatar,
  Rating,
  Chip,
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
  LinearProgress,
  Divider,
  Alert,
} from '@mui/material';
import { Reply as ReplyIcon } from '@mui/icons-material';
import type { Review } from '../types/guide.types';

const SAMPLE_REVIEWS: Review[] = [
  {
    id: 'r1',
    touristId: 't1',
    touristName: 'Alice Johnson',
    touristAvatar: '',
    rating: 5,
    comment: 'Absolutely amazing tour! Marco was incredibly knowledgeable about Roman history and took us to hidden gems we never would have found on our own. Highly recommend!',
    createdAt: '2024-02-15T10:00:00Z',
    tourId: 'tour1',
    tourTitle: 'Cultural Tour of Old City',
  },
  {
    id: 'r2',
    touristId: 't2',
    touristName: 'Bob Martinez',
    touristAvatar: '',
    rating: 4,
    comment: 'Great food tour experience. Lots of delicious stops and good storytelling. Slightly rushed at the end but overall very enjoyable.',
    guideResponse: 'Thank you for your feedback Bob! I\'ll make sure to better pace the final section. Hope to see you again!',
    createdAt: '2024-02-22T09:00:00Z',
    tourId: 'tour2',
    tourTitle: 'Food & Wine Experience',
  },
  {
    id: 'r3',
    touristId: 't3',
    touristName: 'Carol Smith',
    touristAvatar: '',
    rating: 5,
    comment: 'Perfect day trip! Everything was well-organized and Marco\'s passion for the subject was infectious. We learned so much!',
    createdAt: '2024-03-01T14:00:00Z',
    tourId: 'tour3',
    tourTitle: 'Nature Hike & Photography',
  },
];

const ratingDistribution: Record<number, number> = { 5: 28, 4: 10, 3: 3, 2: 1, 1: 0 };

const Reviews = () => {
  const [reviews] = useState<Review[]>(SAMPLE_REVIEWS);
  const [ratingFilter, setRatingFilter] = useState('all');
  const [sortBy, setSortBy] = useState('newest');
  const [replyReview, setReplyReview] = useState<Review | null>(null);
  const [replyText, setReplyText] = useState('');
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  const totalReviews = Object.values(ratingDistribution).reduce((a, b) => a + b, 0);
  const averageRating = (
    Object.entries(ratingDistribution).reduce((sum, [r, c]) => sum + Number(r) * c, 0) / totalReviews
  ).toFixed(1);

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 4000);
  };

  const handleSubmitResponse = async () => {
    if (!replyReview || !replyText.trim()) return;
    try {
      // In production: await guideApi.submitReviewResponse({ reviewId: replyReview.id, response: replyText });
      setReplyReview(null);
      setReplyText('');
      showAlert('success', 'Response submitted successfully.');
    } catch {
      showAlert('error', 'Failed to submit response.');
    }
  };

  const filtered = reviews
    .filter((r) => ratingFilter === 'all' || r.rating === Number(ratingFilter))
    .sort((a, b) =>
      sortBy === 'newest'
        ? new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        : new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
    );

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Reviews & Ratings
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        View and respond to tourist reviews.
      </Typography>

      {alert && (
        <Alert severity={alert.type} sx={{ mb: 2 }}>
          {alert.message}
        </Alert>
      )}

      <Grid container spacing={3} sx={{ mb: 3 }}>
        {/* Overall Rating */}
        <Grid item xs={12} md={4}>
          <Paper elevation={2} sx={{ p: 3, textAlign: 'center' }}>
            <Typography variant="h2" fontWeight="bold" color="primary.main">
              {averageRating}
            </Typography>
            <Rating value={Number(averageRating)} precision={0.1} readOnly size="large" />
            <Typography variant="body2" color="text.secondary">
              Based on {totalReviews} reviews
            </Typography>
          </Paper>
        </Grid>

        {/* Rating Distribution */}
        <Grid item xs={12} md={8}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Rating Distribution
            </Typography>
            {[5, 4, 3, 2, 1].map((star) => (
              <Box key={star} sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                <Typography variant="body2" sx={{ minWidth: 20 }}>
                  {star}★
                </Typography>
                <LinearProgress
                  variant="determinate"
                  value={totalReviews > 0 ? (ratingDistribution[star] / totalReviews) * 100 : 0}
                  sx={{ flexGrow: 1, height: 8, borderRadius: 4 }}
                />
                <Typography variant="body2" sx={{ minWidth: 30 }}>
                  {ratingDistribution[star]}
                </Typography>
              </Box>
            ))}
          </Paper>
        </Grid>
      </Grid>

      {/* Filters */}
      <Paper elevation={2} sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6}>
            <FormControl fullWidth size="small">
              <InputLabel>Filter by Rating</InputLabel>
              <Select
                value={ratingFilter}
                label="Filter by Rating"
                onChange={(e) => setRatingFilter(e.target.value)}
              >
                <MenuItem value="all">All Ratings</MenuItem>
                {[5, 4, 3, 2, 1].map((r) => (
                  <MenuItem key={r} value={String(r)}>
                    {r} Stars
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={6}>
            <FormControl fullWidth size="small">
              <InputLabel>Sort By</InputLabel>
              <Select value={sortBy} label="Sort By" onChange={(e) => setSortBy(e.target.value)}>
                <MenuItem value="newest">Newest First</MenuItem>
                <MenuItem value="oldest">Oldest First</MenuItem>
              </Select>
            </FormControl>
          </Grid>
        </Grid>
      </Paper>

      {/* Review Cards */}
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        {filtered.map((review) => (
          <Paper key={review.id} elevation={2} sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
                <Avatar src={review.touristAvatar}>{review.touristName[0]}</Avatar>
                <Box>
                  <Typography variant="subtitle1" fontWeight="bold">
                    {review.touristName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {new Date(review.createdAt).toLocaleDateString()}
                  </Typography>
                </Box>
              </Box>
              <Box sx={{ textAlign: 'right' }}>
                <Rating value={review.rating} readOnly size="small" />
                <Chip label={review.tourTitle} size="small" variant="outlined" sx={{ mt: 0.5 }} />
              </Box>
            </Box>

            <Typography variant="body1" sx={{ mb: 1 }}>
              {review.comment}
            </Typography>

            {review.guideResponse && (
              <Box sx={{ bgcolor: 'grey.50', p: 2, borderRadius: 1, mt: 1, borderLeft: '3px solid', borderColor: 'primary.main' }}>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  Your response:
                </Typography>
                <Typography variant="body2">{review.guideResponse}</Typography>
              </Box>
            )}

            {!review.guideResponse && (
              <Button
                startIcon={<ReplyIcon />}
                size="small"
                sx={{ mt: 1 }}
                onClick={() => {
                  setReplyReview(review);
                  setReplyText('');
                }}
              >
                Reply
              </Button>
            )}
          </Paper>
        ))}

        {filtered.length === 0 && (
          <Paper elevation={1} sx={{ p: 4, textAlign: 'center' }}>
            <Typography color="text.secondary">No reviews found.</Typography>
          </Paper>
        )}
      </Box>

      {/* Reply Dialog */}
      <Dialog open={!!replyReview} onClose={() => setReplyReview(null)} maxWidth="sm" fullWidth>
        <DialogTitle>Reply to Review</DialogTitle>
        <DialogContent>
          {replyReview && (
            <Box sx={{ mb: 2, p: 2, bgcolor: 'grey.50', borderRadius: 1 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                <Rating value={replyReview.rating} readOnly size="small" />
                <Typography variant="body2" fontWeight="bold">
                  {replyReview.touristName}
                </Typography>
              </Box>
              <Typography variant="body2">{replyReview.comment}</Typography>
            </Box>
          )}
          <Divider sx={{ mb: 2 }} />
          <TextField
            autoFocus
            fullWidth
            label="Your Response"
            multiline
            rows={4}
            value={replyText}
            onChange={(e) => setReplyText(e.target.value)}
            placeholder="Thank the tourist and address any points raised..."
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setReplyReview(null)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleSubmitResponse}
            disabled={!replyText.trim()}
          >
            Submit Response
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Reviews;
