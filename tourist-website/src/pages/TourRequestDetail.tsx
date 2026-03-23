import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Paper,
  Grid,
  Button,
  Chip,
  Box,
  Divider,
  Avatar,
  Rating,
  CircularProgress,
  Alert,
  Card,
  CardContent,
  CardActions,
} from '@mui/material';
import {
  CalendarMonth,
  People,
  AttachMoney,
  Language as LanguageIcon,
} from '@mui/icons-material';
import { getTourRequest, getBidsForPost, acceptBid, rejectBid } from '../services/touristApi';
import ConfirmDialog from '../components/shared/ConfirmDialog';
import type { TourRequest, Bid } from '../types/tourist.types';

const TourRequestDetail = () => {
  const { tourRequestId } = useParams<{ tourRequestId: string }>();
  const navigate = useNavigate();
  const [request, setRequest] = useState<TourRequest | null>(null);
  const [bids, setBids] = useState<Bid[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [actionLoading, setActionLoading] = useState<number | null>(null);
  const [rejectTarget, setRejectTarget] = useState<number | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      if (!tourRequestId) return;
      try {
        const id = parseInt(tourRequestId);
        const [requestData, bidsData] = await Promise.all([
          getTourRequest(id),
          getBidsForPost(id).catch(() => []),
        ]);
        setRequest(requestData);
        setBids(bidsData);
      } catch {
        setError('Failed to load tour request details.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [tourRequestId]);

  const handleAcceptBid = async (bidPostId: number) => {
    setActionLoading(bidPostId);
    try {
      await acceptBid(bidPostId);
      navigate('/bookings');
    } catch {
      setError('Failed to accept bid.');
    } finally {
      setActionLoading(null);
    }
  };

  const handleRejectBid = async () => {
    if (rejectTarget === null) return;
    setActionLoading(rejectTarget);
    try {
      await rejectBid(rejectTarget);
      setRejectTarget(null);
      const id = parseInt(tourRequestId!);
      const updatedBids = await getBidsForPost(id).catch(() => []);
      setBids(updatedBids);
    } catch {
      setError('Failed to reject bid.');
      setRejectTarget(null);
    } finally {
      setActionLoading(null);
    }
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  if (!request) {
    return (
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <Alert severity="error">Tour request not found.</Alert>
        <Button sx={{ mt: 2 }} onClick={() => navigate('/tours/my')}>Back to My Requests</Button>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Button variant="text" onClick={() => navigate('/tours/my')} sx={{ mb: 2 }}>
        ← Back to My Requests
      </Button>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {/* Request Details */}
      <Paper sx={{ p: 4, mb: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
          <Typography variant="h4">{request.title}</Typography>
          <Chip label={request.status} color="primary" />
        </Box>
        <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
          {request.description}
        </Typography>
        <Divider sx={{ my: 2 }} />
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <CalendarMonth color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">Dates</Typography>
                <Typography variant="body2">
                  {new Date(request.startDate).toLocaleDateString()} - {new Date(request.endDate).toLocaleDateString()}
                </Typography>
              </Box>
            </Box>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <People color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">Group Size</Typography>
                <Typography variant="body2">{request.numberOfPeople} people</Typography>
              </Box>
            </Box>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <AttachMoney color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">Budget</Typography>
                <Typography variant="body2">
                  {request.currency} {request.budgetMin} - {request.budgetMax}
                </Typography>
              </Box>
            </Box>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <LanguageIcon color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">Languages</Typography>
                <Box>
                  {request.languages?.map((lang) => (
                    <Chip key={lang} label={lang} size="small" sx={{ mr: 0.5 }} />
                  ))}
                </Box>
              </Box>
            </Box>
          </Grid>
        </Grid>
      </Paper>

      {/* Bids Section */}
      <Typography variant="h5" gutterBottom>
        Bids ({bids.length})
      </Typography>
      {bids.length === 0 ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Typography color="text.secondary">
            No bids received yet. Guides will review your request and submit their proposals.
          </Typography>
        </Paper>
      ) : (
        <Grid container spacing={3}>
          {bids.map((bid) => (
            <Grid key={bid.id} size={{ xs: 12, md: 6 }}>
              <Card>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <Avatar sx={{ mr: 2, bgcolor: 'primary.main' }}>
                      {bid.guideName?.[0] || '?'}
                    </Avatar>
                    <Box sx={{ flexGrow: 1 }}>
                      <Typography variant="h6">{bid.guideName}</Typography>
                      <Box sx={{ display: 'flex', alignItems: 'center' }}>
                        <Rating value={bid.guideRating} size="small" readOnly />
                        <Typography variant="caption" sx={{ ml: 0.5 }}>
                          ({bid.guideReviewCount} reviews)
                        </Typography>
                      </Box>
                    </Box>
                    <Typography variant="h5" color="primary">
                      {bid.currency} {bid.amount}
                    </Typography>
                  </Box>
                  <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                    {bid.message}
                  </Typography>
                  <Chip
                    label={bid.status}
                    size="small"
                    color={bid.status === 'pending' ? 'warning' : bid.status === 'accepted' ? 'success' : 'default'}
                  />
                </CardContent>
                {bid.status === 'pending' && (
                  <CardActions sx={{ justifyContent: 'flex-end' }}>
                    <Button
                      size="small"
                      color="error"
                      onClick={() => setRejectTarget(bid.postId)}
                      disabled={actionLoading === bid.postId}
                    >
                      Reject
                    </Button>
                    <Button
                      size="small"
                      variant="contained"
                      onClick={() => handleAcceptBid(bid.postId)}
                      disabled={actionLoading === bid.postId}
                    >
                      {actionLoading === bid.postId ? <CircularProgress size={20} /> : 'Accept'}
                    </Button>
                  </CardActions>
                )}
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
      <ConfirmDialog
        open={rejectTarget !== null}
        title="Reject Bid"
        message="Are you sure you want to reject this bid?"
        confirmText="Reject"
        onConfirm={handleRejectBid}
        onCancel={() => setRejectTarget(null)}
        severity="error"
      />
    </Container>
  );
};

export default TourRequestDetail;
