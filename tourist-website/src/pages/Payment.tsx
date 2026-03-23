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
} from '@mui/material';
import { Lock as LockIcon } from '@mui/icons-material';
import { createPayment, confirmPayment } from '../services/touristApi';
import type { PaymentInfo } from '../types/tourist.types';

const Payment = () => {
  const { bookingId } = useParams<{ bookingId: string }>();
  const navigate = useNavigate();
  const [payment, setPayment] = useState<PaymentInfo | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isProcessing, setIsProcessing] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    // Check if there's an existing payment for this booking
    if (!bookingId) return;
    setIsLoading(true);
    // No direct "get payment by booking" endpoint, so we start fresh
    setIsLoading(false);
  }, [bookingId]);

  const handleCreatePayment = async () => {
    if (!bookingId) return;
    setIsProcessing(true);
    setError('');
    try {
      const paymentData = await createPayment({
        bookingId,
        amount: 0, // Server calculates actual amount from booking
        currencyCode: 'USD',
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
      await confirmPayment(payment.paymentId);
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

      <Paper sx={{ p: 4 }}>
        <Typography variant="h6" gutterBottom>Order Summary</Typography>
        <Box sx={{ mb: 2 }}>
          <Typography variant="body2" color="text.secondary">
            Booking ID: {bookingId}
          </Typography>
        </Box>
        <Divider sx={{ my: 2 }} />

        {!payment ? (
          <>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Click below to initiate the payment process. Your payment will be securely processed.
            </Typography>
            <Button
              fullWidth
              variant="contained"
              size="large"
              startIcon={isProcessing ? <CircularProgress size={20} /> : <LockIcon />}
              onClick={handleCreatePayment}
              disabled={isProcessing || success}
            >
              {isProcessing ? 'Processing...' : 'Initiate Payment'}
            </Button>
          </>
        ) : (
          <>
            <Box sx={{ mb: 2 }}>
              <Typography variant="body2">Amount: {payment.currencyCode} {payment.amount}</Typography>
              <Typography variant="body2">Status: {payment.status}</Typography>
            </Box>
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
    </Container>
  );
};

export default Payment;
