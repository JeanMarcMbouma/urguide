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
  const [actionLoading, setActionLoading] = useState<string | null>(null);
  const [rejectTarget, setRejectTarget] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      if (!tourRequestId) return;
      try {
        const [requestData, bidsData] = await Promise.all([
          getTourRequest(tourRequestId),
          getBidsForPost(tourRequestId).catch(() => []),
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

  const handleAcceptBid = async (postId: string) => {
    setActionLoading(postId);
    try {
      await acceptBid(postId);
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
      if (tourRequestId) {
        const updatedBids = await getBidsForPost(tourRequestId).catch(() => []);
        setBids(updatedBids);
      }
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
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <CalendarMonth color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">Preferred Date</Typography>
                <Typography variant="body2">
                  {new Date(request.preferredDate).toLocaleDateString()}
                </Typography>
              </Box>
            </Box>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <People color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">Max Participants</Typography>
                <Typography variant="body2">{request.maxParticipants} people</Typography>
              </Box>
            </Box>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <AttachMoney color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">Max Budget</Typography>
                <Typography variant="body2">
                  ${request.maxBudget}
                </Typography>
              </Box>
            </Box>
          </Grid>
        </Grid>
        {request.tags && (
          <Box sx={{ mt: 2 }}>
            {request.tags.split(',').map((tag) => (
              <Chip key={tag.trim()} label={tag.trim()} size="small" sx={{ mr: 0.5 }} />
            ))}
          </Box>
        )}
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
          {bids.map((bid, index) => (
            <Grid key={index} size={{ xs: 12, md: 6 }}>
              <Card>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <Avatar src={bid.authorImage} sx={{ mr: 2, bgcolor: 'primary.main' }}>
                      {bid.author?.[0] || '?'}
                    </Avatar>
                    <Box sx={{ flexGrow: 1 }}>
                      <Typography variant="h6">{bid.author}</Typography>
                      <Typography variant="caption" color="text.secondary">
                        {new Date(bid.created).toLocaleDateString()}
                      </Typography>
                    </Box>
                    <Typography variant="h5" color="primary">
                      ${bid.value}
                    </Typography>
                  </Box>
                  <Chip
                    label={bid.isActive ? 'Active' : 'Inactive'}
                    size="small"
                    color={bid.isActive ? 'success' : 'default'}
                  />
                </CardContent>
                {bid.isActive && (
                  <CardActions sx={{ justifyContent: 'flex-end' }}>
                    <Button
                      size="small"
                      color="error"
                      onClick={() => setRejectTarget(tourRequestId || '')}
                      disabled={actionLoading === tourRequestId}
                    >
                      Reject
                    </Button>
                    <Button
                      size="small"
                      variant="contained"
                      onClick={() => handleAcceptBid(tourRequestId || '')}
                      disabled={actionLoading === tourRequestId}
                    >
                      {actionLoading === tourRequestId ? <CircularProgress size={20} /> : 'Accept'}
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
