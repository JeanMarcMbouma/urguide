import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Paper,
  Button,
  Box,
  Divider,
  CircularProgress,
  Alert,
  TextField,
  Grid,
} from '@mui/material';
import { Lock as LockIcon } from '@mui/icons-material';
import { getTourRequest, createPayment, confirmPayment } from '../services/touristApi';
import type { TourRequest, PaymentInfo } from '../types/tourist.types';

const Payment = () => {
  const { tourRequestId } = useParams<{ tourRequestId: string }>();
  const navigate = useNavigate();
  const [tourRequest, setTourRequest] = useState<TourRequest | null>(null);
  const [payment, setPayment] = useState<PaymentInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isProcessing, setIsProcessing] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      if (!tourRequestId) return;
      try {
        const data = await getTourRequest(parseInt(tourRequestId));
        setTourRequest(data);
      } catch {
        setError('Failed to load tour details.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [tourRequestId]);

  const handleCreatePayment = async () => {
    if (!tourRequest) return;
    setIsProcessing(true);
    setError('');
    try {
      const paymentData = await createPayment({
        tourRequestId: tourRequest.id,
        amount: tourRequest.budgetMax,
        currency: tourRequest.currency,
      });
      setPayment(paymentData);
    } catch {
      setError('Failed to initiate payment. Please try again.');
    } finally {
      setIsProcessing(false);
    }
  };

  const handleConfirmPayment = async () => {
    if (!payment) return;
    setIsProcessing(true);
    setError('');
    try {
      await confirmPayment(payment.id);
      setSuccess(true);
      setTimeout(() => navigate('/bookings'), 2000);
    } catch {
      setError('Payment confirmation failed. Please try again.');
    } finally {
      setIsProcessing(false);
    }
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>
        Payment
      </Typography>

      {success && (
        <Alert severity="success" sx={{ mb: 3 }}>
          Payment successful! Redirecting to your bookings...
        </Alert>
      )}
      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {tourRequest && (
        <Paper sx={{ p: 4 }}>
          <Typography variant="h6" gutterBottom>Order Summary</Typography>
          <Box sx={{ mb: 2 }}>
            <Typography variant="body1" fontWeight="bold">{tourRequest.title}</Typography>
            <Typography variant="body2" color="text.secondary">
              {new Date(tourRequest.startDate).toLocaleDateString()} - {new Date(tourRequest.endDate).toLocaleDateString()}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {tourRequest.numberOfPeople} people
            </Typography>
          </Box>
          <Divider sx={{ my: 2 }} />
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
            <Typography variant="h6">Total</Typography>
            <Typography variant="h6" color="primary">
              {tourRequest.currency} {tourRequest.budgetMax}
            </Typography>
          </Box>

          {!payment ? (
            <>
              <Typography variant="subtitle2" gutterBottom>Payment Details</Typography>
              <Grid container spacing={2} sx={{ mb: 3 }}>
                <Grid size={{ xs: 12 }}>
                  <TextField fullWidth label="Card Number" placeholder="4242 4242 4242 4242" disabled={isProcessing} />
                </Grid>
                <Grid size={{ xs: 6 }}>
                  <TextField fullWidth label="Expiry" placeholder="MM/YY" disabled={isProcessing} />
                </Grid>
                <Grid size={{ xs: 6 }}>
                  <TextField fullWidth label="CVC" placeholder="123" disabled={isProcessing} />
                </Grid>
              </Grid>
              <Button
                fullWidth
                variant="contained"
                size="large"
                startIcon={isProcessing ? <CircularProgress size={20} /> : <LockIcon />}
                onClick={handleCreatePayment}
                disabled={isProcessing || success}
              >
                {isProcessing ? 'Processing...' : `Pay ${tourRequest.currency} ${tourRequest.budgetMax}`}
              </Button>
            </>
          ) : (
            <>
              <Alert severity="info" sx={{ mb: 2 }}>
                Payment initiated. Please confirm to complete.
              </Alert>
              <Button
                fullWidth
                variant="contained"
                size="large"
                color="success"
                onClick={handleConfirmPayment}
                disabled={isProcessing || success}
              >
                {isProcessing ? <CircularProgress size={24} /> : 'Confirm Payment'}
              </Button>
            </>
          )}

          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 2, textAlign: 'center' }}>
            <LockIcon sx={{ fontSize: 14, verticalAlign: 'middle', mr: 0.5 }} />
            Payments are securely processed via Stripe
          </Typography>
        </Paper>
      )}
    </Container>
  );
};

export default Payment;
