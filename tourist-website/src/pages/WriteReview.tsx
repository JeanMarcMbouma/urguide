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
  IconButton,
} from '@mui/material';
import {
  PhotoCamera,
  Close as CloseIcon,
} from '@mui/icons-material';
import { submitReview } from '../services/touristApi';

const WriteReview = () => {
  const { postId } = useParams<{ postId: string }>();
  const navigate = useNavigate();
  const [rating, setRating] = useState<number | null>(null);
  const [comment, setComment] = useState('');
  const [photos, setPhotos] = useState<File[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const handlePhotoAdd = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setPhotos((prev) => [...prev, ...Array.from(e.target.files!)].slice(0, 5));
    }
  };

  const handlePhotoRemove = (index: number) => {
    setPhotos((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!rating || !postId) return;
    setIsSubmitting(true);
    setError('');
    try {
      await submitReview({
        postId: parseInt(postId),
        rating,
        comment,
        photos: photos.length > 0 ? photos : undefined,
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
          value={comment}
          onChange={(e) => setComment(e.target.value)}
          sx={{ mb: 3 }}
          required
        />

        {/* Photo Upload */}
        <Box sx={{ mb: 3 }}>
          <Typography variant="subtitle2" gutterBottom>Photos (optional, max 5)</Typography>
          <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', alignItems: 'center' }}>
            {photos.map((photo, index) => (
              <Box
                key={index}
                sx={{
                  position: 'relative',
                  width: 80,
                  height: 80,
                  bgcolor: 'grey.200',
                  borderRadius: 1,
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                }}
              >
                <Typography variant="caption" noWrap sx={{ maxWidth: 70, px: 0.5 }}>
                  {photo.name}
                </Typography>
                <IconButton
                  size="small"
                  sx={{ position: 'absolute', top: -8, right: -8, bgcolor: 'white' }}
                  onClick={() => handlePhotoRemove(index)}
                >
                  <CloseIcon fontSize="small" />
                </IconButton>
              </Box>
            ))}
            {photos.length < 5 && (
              <Button
                component="label"
                variant="outlined"
                sx={{ width: 80, height: 80 }}
              >
                <PhotoCamera />
                <input type="file" hidden accept="image/*" onChange={handlePhotoAdd} />
              </Button>
            )}
          </Box>
        </Box>

        <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
          <Button variant="outlined" onClick={() => navigate(-1)}>Cancel</Button>
          <Button
            type="submit"
            variant="contained"
            disabled={isSubmitting || !rating || !comment}
          >
            {isSubmitting ? <CircularProgress size={24} /> : 'Submit Review'}
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default WriteReview;
