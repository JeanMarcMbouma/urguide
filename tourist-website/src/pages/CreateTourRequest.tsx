import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Paper,
  TextField,
  Button,
  Grid,
  Box,
  Alert,
  CircularProgress,
} from '@mui/material';
import { createTourRequest } from '../services/touristApi';
import type { CreateTourRequestData } from '../types/tourist.types';

const CreateTourRequest = () => {
  const navigate = useNavigate();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  const [form, setForm] = useState<CreateTourRequestData>({
    title: '',
    description: '',
    preferredDate: '',
    maxParticipants: 1,
    maxBudget: 0,
    tags: '',
    regionId: '',
  });

  const handleChange = (field: keyof CreateTourRequestData, value: string | number) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsSubmitting(true);
    try {
      const result = await createTourRequest(form);
      setSuccess(true);
      setTimeout(() => navigate(`/tours/${result.tourRequestId}`), 1500);
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setError(axiosErr.response?.data?.message || 'Failed to create tour request. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>
        Create Tour Request
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Describe your ideal tour and local guides will bid to be your guide.
      </Typography>

      {success && <Alert severity="success" sx={{ mb: 3 }}>Tour request created successfully! Redirecting...</Alert>}
      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      <Paper component="form" onSubmit={handleSubmit} sx={{ p: 4 }}>
        <Grid container spacing={3}>
          <Grid size={{ xs: 12 }}>
            <TextField
              required
              fullWidth
              label="Tour Title"
              placeholder="e.g., 3-Day Paris Cultural Tour"
              value={form.title}
              onChange={(e) => handleChange('title', e.target.value)}
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <TextField
              required
              fullWidth
              multiline
              rows={4}
              label="Description"
              placeholder="Describe what you'd like to see and experience..."
              value={form.description}
              onChange={(e) => handleChange('description', e.target.value)}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              required
              fullWidth
              label="Region ID"
              placeholder="e.g., paris-01"
              value={form.regionId}
              onChange={(e) => handleChange('regionId', e.target.value)}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              required
              fullWidth
              type="date"
              label="Preferred Date"
              value={form.preferredDate}
              onChange={(e) => handleChange('preferredDate', e.target.value)}
              slotProps={{ inputLabel: { shrink: true } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              required
              fullWidth
              type="number"
              label="Max Participants"
              value={form.maxParticipants}
              onChange={(e) => handleChange('maxParticipants', parseInt(e.target.value) || 1)}
              slotProps={{ htmlInput: { min: 1, max: 50 } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              required
              fullWidth
              type="number"
              label="Maximum Budget"
              value={form.maxBudget || ''}
              onChange={(e) => handleChange('maxBudget', parseFloat(e.target.value) || 0)}
              slotProps={{ htmlInput: { min: 0 } }}
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <TextField
              fullWidth
              label="Tags"
              placeholder="e.g., culture, history, food (comma-separated)"
              value={form.tags}
              onChange={(e) => handleChange('tags', e.target.value)}
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
              <Button variant="outlined" onClick={() => navigate(-1)}>
                Cancel
              </Button>
              <Button
                type="submit"
                variant="contained"
                disabled={isSubmitting || !form.title || !form.description || !form.preferredDate || !form.regionId || form.maxBudget <= 0}
              >
                {isSubmitting ? <CircularProgress size={24} /> : 'Create Request'}
              </Button>
            </Box>
          </Grid>
        </Grid>
      </Paper>
    </Container>
  );
};

export default CreateTourRequest;
