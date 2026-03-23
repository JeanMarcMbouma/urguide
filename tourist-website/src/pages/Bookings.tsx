import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Paper,
  Grid,
  Button,
  Chip,
  Box,
  Avatar,
  CircularProgress,
  Alert,
  Card,
  CardContent,
  CardActions,
  Tabs,
  Tab,
} from '@mui/material';
import {
  CalendarMonth,
  Payment as PaymentIcon,
} from '@mui/icons-material';
import { getBookings } from '../services/touristApi';
import type { Booking } from '../types/tourist.types';

const Bookings = () => {
  const navigate = useNavigate();
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [tab, setTab] = useState(0);

  useEffect(() => {
    const fetchBookings = async () => {
      try {
        const data = await getBookings(1, 50);
        setBookings(data.items || []);
      } catch {
        setError('Failed to load bookings.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchBookings();
  }, []);

  const getStatusColor = (status: string): 'default' | 'primary' | 'success' | 'warning' | 'error' => {
    switch (status?.toLowerCase()) {
      case 'confirmed': return 'success';
      case 'pending': return 'warning';
      case 'completed': return 'success';
      case 'cancelled': return 'error';
      default: return 'default';
    }
  };

  const activeBookings = bookings.filter(
    (b) => ['confirmed', 'pending'].includes(b.status?.toLowerCase())
  );
  const pastBookings = bookings.filter(
    (b) => ['completed', 'cancelled'].includes(b.status?.toLowerCase())
  );
  const displayedBookings = tab === 0 ? activeBookings : pastBookings;

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>
        My Bookings
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      <Tabs value={tab} onChange={(_e, v) => setTab(v)} sx={{ mb: 3 }}>
        <Tab label={`Active (${activeBookings.length})`} />
        <Tab label={`Past (${pastBookings.length})`} />
      </Tabs>

      {isLoading ? (
        <Box display="flex" justifyContent="center" py={6}>
          <CircularProgress />
        </Box>
      ) : displayedBookings.length === 0 ? (
        <Paper sx={{ p: 6, textAlign: 'center' }}>
          <Typography variant="h6" color="text.secondary" gutterBottom>
            {tab === 0 ? 'No active bookings' : 'No past bookings'}
          </Typography>
          {tab === 0 && (
            <Button variant="contained" onClick={() => navigate('/tours/create')} sx={{ mt: 2 }}>
              Create Tour Request
            </Button>
          )}
        </Paper>
      ) : (
        <Grid container spacing={3}>
          {displayedBookings.map((booking) => (
            <Grid key={booking.id} size={{ xs: 12, md: 6 }}>
              <Card>
                <CardContent>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                    <Typography variant="h6">{booking.tourTitle}</Typography>
                    <Chip label={booking.status} size="small" color={getStatusColor(booking.status)} />
                  </Box>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <Avatar sx={{ mr: 1, width: 32, height: 32 }}>
                      {booking.guideName?.[0] || '?'}
                    </Avatar>
                    <Typography variant="body2">{booking.guideName}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                    <CalendarMonth fontSize="small" color="action" />
                    <Typography variant="body2">
                      {new Date(booking.startDate).toLocaleDateString()} - {new Date(booking.endDate).toLocaleDateString()}
                    </Typography>
                  </Box>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <PaymentIcon fontSize="small" color="action" />
                    <Typography variant="body2">
                      {booking.currency} {booking.amount}
                    </Typography>
                    <Chip
                      label={booking.paymentStatus}
                      size="small"
                      variant="outlined"
                      color={booking.paymentStatus === 'paid' ? 'success' : 'warning'}
                    />
                  </Box>
                </CardContent>
                <CardActions>
                  {booking.paymentStatus !== 'paid' && booking.status === 'confirmed' && (
                    <Button
                      size="small"
                      variant="contained"
                      onClick={() => navigate(`/payment/${booking.tourRequestId}`)}
                    >
                      Pay Now
                    </Button>
                  )}
                  {booking.status === 'completed' && (
                    <Button
                      size="small"
                      onClick={() => navigate(`/reviews/write/${booking.tourRequestId}`)}
                    >
                      Write Review
                    </Button>
                  )}
                </CardActions>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
    </Container>
  );
};

export default Bookings;
