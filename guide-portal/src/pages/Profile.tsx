import { useState, useEffect } from 'react';
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
  CircularProgress,
} from '@mui/material';
import { Save as SaveIcon } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import { useAuth } from '../hooks/useAuth';
import type { UpdateGuideProfileRequest } from '../types/guide.types';

const SPECIALIZATION_OPTIONS = ['Cultural Tours', 'Adventure', 'Food & Cuisine', 'Historical', 'Nature', 'Photography', 'Wildlife', 'City Tours', 'Hiking', 'Water Sports'];
const LANGUAGE_OPTIONS = ['English', 'Spanish', 'French', 'German', 'Chinese', 'Japanese', 'Portuguese', 'Arabic', 'Italian', 'Russian'];

const Profile = () => {
  const { t } = useTranslation();
  const { user } = useAuth();
  const [form, setForm] = useState<UpdateGuideProfileRequest>({
    id: '',
    firstName: '',
    lastName: '',
    address: '',
    country: '',
    city: '',
    phone: '',
    description: '',
    gender: '',
    profileImage: '',
  });
  const [specializations, setSpecializations] = useState<string[]>([]);
  const [languages, setLanguages] = useState<string[]>([]);
  const [pricePerHour, setPricePerHour] = useState(0);
  const [pricePerDay, setPricePerDay] = useState(0);
  const [loading, setLoading] = useState(true);
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    guideApi.getProfile()
      .then((profile) => {
        setForm({
          id: profile.id,
          firstName: profile.firstName ?? '',
          lastName: profile.lastName ?? '',
          address: profile.address ?? '',
          country: profile.country ?? '',
          city: profile.city ?? '',
          phone: profile.phoneNumber ?? '',
          description: profile.description ?? '',
        });
        setSpecializations(profile.specializations ?? []);
        setLanguages(profile.languages ?? []);
        setPricePerHour(profile.pricePerHour ?? 0);
        setPricePerDay(profile.pricePerDay ?? 0);
      })
      .catch(() => setError(t('profile.loadError')))
      .finally(() => setLoading(false));
  }, [t]);

  const handleChange = (field: keyof UpdateGuideProfileRequest, value: unknown) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleSave = async () => {
    setSuccess('');
    setError('');
    setIsSaving(true);
    try {
      await guideApi.updateProfile({ ...form, id: form.id || user?.id || '' });
      setSuccess(t('profile.saveSuccess'));
    } catch {
      setError(t('profile.saveError'));
    } finally {
      setIsSaving(false);
    }
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
        <CircularProgress />
        <Typography sx={{ ml: 2 }}>{t('profile.loading')}</Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>{t('profile.title')}</Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>{t('profile.subtitle')}</Typography>

      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>{t('profile.personalInfo')}</Typography>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label={t('profile.firstName')} value={form.firstName}
                  onChange={(e) => handleChange('firstName', e.target.value)} />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label={t('profile.lastName')} value={form.lastName}
                  onChange={(e) => handleChange('lastName', e.target.value)} />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label={t('profile.phone')} value={form.phone}
                  onChange={(e) => handleChange('phone', e.target.value)} />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label={t('profile.city')} value={form.city}
                  onChange={(e) => handleChange('city', e.target.value)} />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label={t('profile.country')} value={form.country}
                  onChange={(e) => handleChange('country', e.target.value)} />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label={t('profile.address')} value={form.address}
                  onChange={(e) => handleChange('address', e.target.value)} />
              </Grid>
              <Grid item xs={12}>
                <TextField fullWidth label={t('profile.description')} multiline rows={4}
                  value={form.description}
                  onChange={(e) => handleChange('description', e.target.value)} />
              </Grid>
            </Grid>
          </Paper>
        </Grid>

        <Grid item xs={12} md={4}>
          <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
            <Typography variant="h6" gutterBottom>{t('profile.pricing')}</Typography>
            <TextField fullWidth label={t('profile.pricePerHour')} type="number"
              value={pricePerHour}
              onChange={(e) => setPricePerHour(Number(e.target.value))}
              inputProps={{ min: 0 }} sx={{ mb: 2 }} />
            <TextField fullWidth label={t('profile.pricePerDay')} type="number"
              value={pricePerDay}
              onChange={(e) => setPricePerDay(Number(e.target.value))}
              inputProps={{ min: 0 }} />
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>{t('profile.specializations')}</Typography>
            <Autocomplete
              multiple
              options={SPECIALIZATION_OPTIONS}
              value={specializations}
              onChange={(_, value) => setSpecializations(value)}
              renderTags={(value, getTagProps) =>
                value.map((option, index) => (
                  <Chip label={option} {...getTagProps({ index })} key={option} />
                ))
              }
              renderInput={(params) => <TextField {...params} placeholder="Select or type..." />}
              freeSolo
            />
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>{t('profile.languages')}</Typography>
            <Autocomplete
              multiple
              options={LANGUAGE_OPTIONS}
              value={languages}
              onChange={(_, value) => setLanguages(value)}
              renderTags={(value, getTagProps) =>
                value.map((option, index) => (
                  <Chip label={option} {...getTagProps({ index })} key={option} />
                ))
              }
              renderInput={(params) => <TextField {...params} placeholder="Select or type..." />}
              freeSolo
            />
          </Paper>
        </Grid>

        <Grid item xs={12}>
          <Divider sx={{ my: 1 }} />
          <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
            <Button variant="contained" startIcon={<SaveIcon />} onClick={handleSave}
              disabled={isSaving} size="large">
              {isSaving ? t('profile.saving') : t('profile.save')}
            </Button>
          </Box>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Profile;
