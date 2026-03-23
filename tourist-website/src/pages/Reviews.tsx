import { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Paper,
  Box,
  Rating,
  CircularProgress,
  Alert,
  Card,
  CardContent,
  Pagination,
  Chip,
} from '@mui/material';
import { getMyReviews } from '../services/touristApi';
import type { ReviewItem } from '../types/tourist.types';

const Reviews = () => {
  const [reviews, setReviews] = useState<ReviewItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchReviews = async () => {
      setIsLoading(true);
      try {
        const data = await getMyReviews(page, 10);
        setReviews(data.items || []);
        setTotalCount(data.totalCount || 0);
      } catch {
        setError('Failed to load reviews.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchReviews();
  }, [page]);

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>My Reviews</Typography>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {isLoading ? (
        <Box display="flex" justifyContent="center" py={6}>
          <CircularProgress />
        </Box>
      ) : reviews.length === 0 ? (
        <Paper sx={{ p: 6, textAlign: 'center' }}>
          <Typography color="text.secondary">
            You haven&apos;t written any reviews yet.
          </Typography>
        </Paper>
      ) : (
        <>
          {reviews.map((review) => (
            <Card key={review.id} sx={{ mb: 2 }}>
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="h6">{review.tourTitle}</Typography>
                  <Typography variant="caption" color="text.secondary">
                    {new Date(review.createdAt).toLocaleDateString()}
                  </Typography>
                </Box>
                <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                  Guide: {review.guideName}
                </Typography>
                <Rating value={review.rating} readOnly size="small" sx={{ mb: 1 }} />
                <Typography variant="body1" sx={{ mb: 1 }}>
                  {review.comment}
                </Typography>
                {review.photos?.length > 0 && (
                  <Box sx={{ display: 'flex', gap: 1, mb: 1 }}>
                    {review.photos.map((photo, i) => (
                      <Chip key={i} label={`Photo ${i + 1}`} size="small" variant="outlined" />
                    ))}
                  </Box>
                )}
                {review.guideResponse && (
                  <Paper sx={{ p: 2, bgcolor: 'grey.50', mt: 1 }}>
                    <Typography variant="caption" fontWeight="bold">Guide Response:</Typography>
                    <Typography variant="body2">{review.guideResponse}</Typography>
                    {review.guideRespondedAt && (
                      <Typography variant="caption" color="text.secondary">
                        {new Date(review.guideRespondedAt).toLocaleDateString()}
                      </Typography>
                    )}
                  </Paper>
                )}
              </CardContent>
            </Card>
          ))}
          {totalCount > 10 && (
            <Box display="flex" justifyContent="center" sx={{ mt: 3 }}>
              <Pagination
                count={Math.ceil(totalCount / 10)}
                page={page}
                onChange={(_e, p) => setPage(p)}
                color="primary"
              />
            </Box>
          )}
        </>
      )}
    </Container>
  );
};

export default Reviews;
