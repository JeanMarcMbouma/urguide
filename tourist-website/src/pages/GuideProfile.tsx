import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Container,
  Typography,
  Button,
  Grid,
  Paper,
  Rating,
  Chip,
  Avatar,
  Divider,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  LinearProgress,
  CircularProgress,
  Alert,
} from '@mui/material';
import {
  VerifiedUser,
  Language as LanguageIcon,
  LocationOn,
  WorkHistory,
  Star as StarIcon,
} from '@mui/icons-material';
import { getGuideProfile, getGuideReviews } from '../services/touristApi';
import type { GuideDetail, ReviewItem } from '../types/tourist.types';

const GuideProfile = () => {
  const { guideId } = useParams<{ guideId: string }>();
  const navigate = useNavigate();
  const [guide, setGuide] = useState<GuideDetail | null>(null);
  const [reviews, setReviews] = useState<ReviewItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      if (!guideId) return;
      try {
        const [guideData, reviewData] = await Promise.all([
          getGuideProfile(guideId),
          getGuideReviews(guideId),
        ]);
        setGuide(guideData);
        setReviews(reviewData.items || []);
      } catch {
        setError('Failed to load guide profile.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [guideId]);

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  if (error || !guide) {
    return (
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <Alert severity="error">{error || 'Guide not found'}</Alert>
        <Button sx={{ mt: 2 }} onClick={() => navigate('/search')}>Back to Search</Button>
      </Container>
    );
  }

  const ratingDistribution = [5, 4, 3, 2, 1].map((star) => {
    const count = reviews.filter((r) => Math.round(r.rating) === star).length;
    const percentage = reviews.length > 0 ? (count / reviews.length) * 100 : 0;
    return { star, count, percentage };
  });

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      {/* Guide Header */}
      <Paper sx={{ p: 4, mb: 4 }}>
        <Grid container spacing={4}>
          <Grid size={{ xs: 12, md: 4 }} sx={{ textAlign: 'center' }}>
            <Avatar
              sx={{ width: 120, height: 120, mx: 'auto', mb: 2, bgcolor: 'primary.main', fontSize: 48 }}
            >
              {guide.firstName?.[0]}{guide.lastName?.[0]}
            </Avatar>
            <Typography variant="h5" gutterBottom>
              {guide.firstName} {guide.lastName}
              {guide.verified && (
                <VerifiedUser color="primary" sx={{ ml: 1, verticalAlign: 'middle' }} />
              )}
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', mb: 1 }}>
              <Rating value={guide.rating} precision={0.5} readOnly />
              <Typography variant="body2" sx={{ ml: 1 }}>
                ({guide.reviewCount} reviews)
              </Typography>
            </Box>
            <Typography variant="h6" color="primary" gutterBottom>
              ${guide.pricePerHour}/hr
            </Typography>
            <Button
              variant="contained"
              fullWidth
              sx={{ mt: 2 }}
              onClick={() => navigate('/tours/create')}
            >
              Request a Tour
            </Button>
          </Grid>
          <Grid size={{ xs: 12, md: 8 }}>
            <Box sx={{ mb: 2 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                <LocationOn color="action" sx={{ mr: 1 }} />
                <Typography>{guide.location}</Typography>
              </Box>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                <WorkHistory color="action" sx={{ mr: 1 }} />
                <Typography>{guide.experience} years of experience</Typography>
              </Box>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <LanguageIcon color="action" sx={{ mr: 1 }} />
                <Box>
                  {guide.languages?.map((lang) => (
                    <Chip key={lang} label={lang} size="small" sx={{ mr: 0.5 }} />
                  ))}
                </Box>
              </Box>
            </Box>
            <Divider sx={{ my: 2 }} />
            <Typography variant="h6" gutterBottom>About</Typography>
            <Typography variant="body1" color="text.secondary">
              {guide.bio || 'No bio available.'}
            </Typography>
            {guide.specialties && guide.specialties.length > 0 && (
              <Box sx={{ mt: 2 }}>
                <Typography variant="subtitle2" gutterBottom>Specialties</Typography>
                {guide.specialties.map((s) => (
                  <Chip key={s} label={s} sx={{ mr: 0.5, mb: 0.5 }} color="secondary" variant="outlined" />
                ))}
              </Box>
            )}
          </Grid>
        </Grid>
      </Paper>

      {/* Gallery Preview */}
      {guide.galleries && guide.galleries.length > 0 && (
        <Paper sx={{ p: 3, mb: 4 }}>
          <Typography variant="h6" gutterBottom>Photo Gallery</Typography>
          <Grid container spacing={2}>
            {guide.galleries.map((gallery) => (
              <Grid key={gallery.id} size={{ xs: 6, sm: 4, md: 3 }}>
                <Paper
                  sx={{
                    height: 120,
                    bgcolor: 'grey.200',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    borderRadius: 2,
                  }}
                >
                  <Typography variant="body2" color="text.secondary">
                    {gallery.title} ({gallery.imageCount})
                  </Typography>
                </Paper>
              </Grid>
            ))}
          </Grid>
        </Paper>
      )}

      {/* Reviews */}
      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>Reviews</Typography>
        <Grid container spacing={4}>
          <Grid size={{ xs: 12, md: 4 }}>
            <Box sx={{ textAlign: 'center', mb: 2 }}>
              <Typography variant="h3">{guide.rating?.toFixed(1) || '0.0'}</Typography>
              <Rating value={guide.rating} precision={0.5} readOnly />
              <Typography variant="body2" color="text.secondary">
                {guide.reviewCount} reviews
              </Typography>
            </Box>
            {ratingDistribution.map(({ star, count, percentage }) => (
              <Box key={star} sx={{ display: 'flex', alignItems: 'center', mb: 0.5 }}>
                <Typography variant="body2" sx={{ minWidth: 20 }}>{star}</Typography>
                <StarIcon sx={{ fontSize: 16, color: 'gold', mr: 1 }} />
                <LinearProgress
                  variant="determinate"
                  value={percentage}
                  sx={{ flexGrow: 1, mr: 1, height: 8, borderRadius: 4 }}
                />
                <Typography variant="body2" sx={{ minWidth: 20 }}>{count}</Typography>
              </Box>
            ))}
          </Grid>
          <Grid size={{ xs: 12, md: 8 }}>
            {reviews.length > 0 ? (
              <List>
                {reviews.map((review) => (
                  <ListItem key={review.id} alignItems="flex-start" sx={{ px: 0 }}>
                    <ListItemAvatar>
                      <Avatar>{review.guideName?.[0] || 'U'}</Avatar>
                    </ListItemAvatar>
                    <ListItemText
                      primary={
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Rating value={review.rating} size="small" readOnly />
                          <Typography variant="caption" color="text.secondary">
                            {new Date(review.createdAt).toLocaleDateString()}
                          </Typography>
                        </Box>
                      }
                      secondary={
                        <>
                          <Typography variant="body2" sx={{ mt: 0.5 }}>
                            {review.comment}
                          </Typography>
                          {review.guideResponse && (
                            <Paper sx={{ p: 1.5, mt: 1, bgcolor: 'grey.50' }}>
                              <Typography variant="caption" fontWeight="bold">Guide Response:</Typography>
                              <Typography variant="body2">{review.guideResponse}</Typography>
                            </Paper>
                          )}
                        </>
                      }
                    />
                  </ListItem>
                ))}
              </List>
            ) : (
              <Typography color="text.secondary" align="center" sx={{ py: 4 }}>
                No reviews yet.
              </Typography>
            )}
          </Grid>
        </Grid>
      </Paper>
    </Container>
  );
};

export default GuideProfile;
