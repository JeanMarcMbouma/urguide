import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Paper,
  TextField,
  Button,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Box,
  Alert,
  CircularProgress,
  Chip,
} from '@mui/material';
import { createTourRequest } from '../services/touristApi';
import type { CreateTourRequestData } from '../types/tourist.types';

const CURRENCIES = ['USD', 'EUR', 'GBP', 'JPY', 'AUD', 'CAD'];
const LANGUAGES = ['English', 'French', 'Spanish', 'German', 'Italian', 'Portuguese', 'Arabic', 'Chinese', 'Japanese', 'Korean'];

const CreateTourRequest = () => {
  const navigate = useNavigate();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  const [form, setForm] = useState<CreateTourRequestData>({
    title: '',
    description: '',
    regionId: 0,
    startDate: '',
    endDate: '',
    numberOfPeople: 1,
    budgetMin: 0,
    budgetMax: 0,
    currency: 'USD',
    languages: [],
    specialRequirements: '',
  });

  const handleChange = (field: keyof CreateTourRequestData, value: string | number | string[]) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsSubmitting(true);
    try {
      const result = await createTourRequest(form);
      setSuccess(true);
      setTimeout(() => navigate(`/tours/${result.id}`), 1500);
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
              type="number"
              label="Region ID"
              value={form.regionId || ''}
              onChange={(e) => handleChange('regionId', parseInt(e.target.value) || 0)}
              slotProps={{ htmlInput: { min: 1 } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              required
              fullWidth
              type="date"
              label="Start Date"
              value={form.startDate}
              onChange={(e) => handleChange('startDate', e.target.value)}
              slotProps={{ inputLabel: { shrink: true } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              required
              fullWidth
              type="date"
              label="End Date"
              value={form.endDate}
              onChange={(e) => handleChange('endDate', e.target.value)}
              slotProps={{ inputLabel: { shrink: true } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              required
              fullWidth
              type="number"
              label="Number of People"
              value={form.numberOfPeople}
              onChange={(e) => handleChange('numberOfPeople', parseInt(e.target.value) || 1)}
              slotProps={{ htmlInput: { min: 1, max: 50 } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <FormControl fullWidth required>
              <InputLabel>Currency</InputLabel>
              <Select
                value={form.currency}
                label="Currency"
                onChange={(e) => handleChange('currency', e.target.value)}
              >
                {CURRENCIES.map((c) => (
                  <MenuItem key={c} value={c}>{c}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              required
              fullWidth
              type="number"
              label="Minimum Budget"
              value={form.budgetMin || ''}
              onChange={(e) => handleChange('budgetMin', parseFloat(e.target.value) || 0)}
              slotProps={{ htmlInput: { min: 0 } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              required
              fullWidth
              type="number"
              label="Maximum Budget"
              value={form.budgetMax || ''}
              onChange={(e) => handleChange('budgetMax', parseFloat(e.target.value) || 0)}
              slotProps={{ htmlInput: { min: 0 } }}
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <FormControl fullWidth>
              <InputLabel>Preferred Languages</InputLabel>
              <Select
                multiple
                value={form.languages}
                label="Preferred Languages"
                onChange={(e) => handleChange('languages', e.target.value as string[])}
                renderValue={(selected) => (
                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                    {(selected as string[]).map((value) => (
                      <Chip key={value} label={value} size="small" />
                    ))}
                  </Box>
                )}
              >
                {LANGUAGES.map((lang) => (
                  <MenuItem key={lang} value={lang}>{lang}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid size={{ xs: 12 }}>
            <TextField
              fullWidth
              multiline
              rows={2}
              label="Special Requirements"
              placeholder="Any accessibility needs, dietary restrictions, etc."
              value={form.specialRequirements}
              onChange={(e) => handleChange('specialRequirements', e.target.value)}
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
                disabled={isSubmitting || !form.title || !form.description || !form.startDate || !form.endDate || !form.regionId || form.budgetMin <= 0 || form.budgetMax <= 0 || form.budgetMin >= form.budgetMax}
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
