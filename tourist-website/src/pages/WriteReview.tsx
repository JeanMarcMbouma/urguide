import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Paper,
  TextField,
  Button,
  Box,
  Rating,
  Alert,
  CircularProgress,
} from '@mui/material';
import { submitReview } from '../services/touristApi';

const WriteReview = () => {
  const { postId } = useParams<{ postId: string }>();
  const navigate = useNavigate();
  const [rating, setRating] = useState<number | null>(null);
  const [text, setText] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!rating || !postId) return;
    setIsSubmitting(true);
    setError('');
    try {
      await submitReview({
        postId,
        rating,
        text,
      });
      setSuccess(true);
      setTimeout(() => navigate('/reviews'), 1500);
    } catch {
      setError('Failed to submit review. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>Write a Review</Typography>

      {success && <Alert severity="success" sx={{ mb: 3 }}>Review submitted! Redirecting...</Alert>}
      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      <Paper component="form" onSubmit={handleSubmit} sx={{ p: 4 }}>
        <Typography variant="h6" gutterBottom>How was your experience?</Typography>

        <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
          <Typography sx={{ mr: 2 }}>Rating:</Typography>
          <Rating
            value={rating}
            onChange={(_e, value) => setRating(value)}
            size="large"
          />
          {rating && (
            <Typography sx={{ ml: 1 }} color="text.secondary">
              {rating}/5
            </Typography>
          )}
        </Box>

        <TextField
          fullWidth
          multiline
          rows={5}
          label="Your Review"
          placeholder="Share your experience with other tourists..."
          value={text}
          onChange={(e) => setText(e.target.value)}
          sx={{ mb: 3 }}
          required
        />

        <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
          <Button variant="outlined" onClick={() => navigate(-1)}>Cancel</Button>
          <Button
            type="submit"
            variant="contained"
            disabled={isSubmitting || !rating || !text}
          >
            {isSubmitting ? <CircularProgress size={24} /> : 'Submit Review'}
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default WriteReview;
