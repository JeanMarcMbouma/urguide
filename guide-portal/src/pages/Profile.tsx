import { useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  TextField,
  Button,
  Chip,
  Alert,
  Divider,
  Autocomplete,
} from '@mui/material';
import { Save as SaveIcon } from '@mui/icons-material';
import { guideApi } from '../services/guideApi';
import type { UpdateGuideProfileRequest } from '../types/guide.types';

const SPECIALIZATION_OPTIONS = [
  'Cultural Tours',
  'Adventure',
  'Food & Cuisine',
  'Historical',
  'Nature',
  'Photography',
  'Wildlife',
  'City Tours',
  'Hiking',
  'Water Sports',
];

const LANGUAGE_OPTIONS = [
  'English',
  'Spanish',
  'French',
  'German',
  'Chinese',
  'Japanese',
  'Portuguese',
  'Arabic',
  'Italian',
  'Russian',
];

const Profile = () => {
  const [form, setForm] = useState<UpdateGuideProfileRequest>({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    bio: '',
    location: '',
    yearsExperience: 0,
    specializations: [],
    languages: [],
    pricePerHour: 0,
    pricePerDay: 0,
  });
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');
  const [isSaving, setIsSaving] = useState(false);

  const handleChange = (field: keyof UpdateGuideProfileRequest, value: unknown) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleSave = async () => {
    setSuccess('');
    setError('');
    setIsSaving(true);

    try {
      // In a real app, guideId would come from useAuth
      await guideApi.updateProfile('me', form);
      setSuccess('Profile updated successfully.');
    } catch {
      setError('Failed to update profile. Please try again.');
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Profile Management
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Manage your guide profile information, specializations, and pricing.
      </Typography>

      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Personal Information
            </Typography>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="First Name"
                  value={form.firstName}
                  onChange={(e) => handleChange('firstName', e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Last Name"
                  value={form.lastName}
                  onChange={(e) => handleChange('lastName', e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Email"
                  type="email"
                  value={form.email}
                  onChange={(e) => handleChange('email', e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Phone Number"
                  value={form.phoneNumber}
                  onChange={(e) => handleChange('phoneNumber', e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Location"
                  value={form.location}
                  onChange={(e) => handleChange('location', e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Years of Experience"
                  type="number"
                  value={form.yearsExperience}
                  onChange={(e) => handleChange('yearsExperience', Number(e.target.value))}
                  inputProps={{ min: 0 }}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Bio"
                  multiline
                  rows={4}
                  value={form.bio}
                  onChange={(e) => handleChange('bio', e.target.value)}
                  placeholder="Tell tourists about yourself, your expertise, and what makes your tours special..."
                />
              </Grid>
            </Grid>
          </Paper>
        </Grid>

        <Grid item xs={12} md={4}>
          <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
            <Typography variant="h6" gutterBottom>
              Pricing
            </Typography>
            <TextField
              fullWidth
              label="Price Per Hour ($)"
              type="number"
              value={form.pricePerHour}
              onChange={(e) => handleChange('pricePerHour', Number(e.target.value))}
              inputProps={{ min: 0 }}
              sx={{ mb: 2 }}
            />
            <TextField
              fullWidth
              label="Price Per Day ($)"
              type="number"
              value={form.pricePerDay}
              onChange={(e) => handleChange('pricePerDay', Number(e.target.value))}
              inputProps={{ min: 0 }}
            />
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Specializations
            </Typography>
            <Autocomplete
              multiple
              options={SPECIALIZATION_OPTIONS}
              value={form.specializations ?? []}
              onChange={(_, value) => handleChange('specializations', value)}
              renderTags={(value, getTagProps) =>
                value.map((option, index) => (
                  <Chip label={option} {...getTagProps({ index })} key={option} />
                ))
              }
              renderInput={(params) => (
                <TextField {...params} label="Add specializations" placeholder="Select or type..." />
              )}
              freeSolo
            />
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Languages
            </Typography>
            <Autocomplete
              multiple
              options={LANGUAGE_OPTIONS}
              value={form.languages ?? []}
              onChange={(_, value) => handleChange('languages', value)}
              renderTags={(value, getTagProps) =>
                value.map((option, index) => (
                  <Chip label={option} {...getTagProps({ index })} key={option} />
                ))
              }
              renderInput={(params) => (
                <TextField {...params} label="Add languages" placeholder="Select or type..." />
              )}
              freeSolo
            />
          </Paper>
        </Grid>

        <Grid item xs={12}>
          <Divider sx={{ my: 1 }} />
          <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
            <Button
              variant="contained"
              startIcon={<SaveIcon />}
              onClick={handleSave}
              disabled={isSaving}
              size="large"
            >
              {isSaving ? 'Saving...' : 'Save Profile'}
            </Button>
          </Box>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Profile;
