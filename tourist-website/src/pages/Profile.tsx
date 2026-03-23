import { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Paper,
  TextField,
  Button,
  Grid,
  Box,
  Avatar,
  CircularProgress,
  Alert,
  Divider,
} from '@mui/material';
import { Save as SaveIcon } from '@mui/icons-material';
import { getUserProfile, updateProfile } from '../services/touristApi';
import type { UpdateProfileRequest } from '../types/tourist.types';

const Profile = () => {
  const [profile, setProfile] = useState<UpdateProfileRequest>({
    id: '',
    firstName: '',
    lastName: '',
    profileImage: '',
  });
  const [email, setEmail] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const data = await getUserProfile();
        setEmail(data.email);
        setProfile({
          id: data.id,
          firstName: data.firstName || '',
          lastName: data.lastName || '',
        });
      } catch {
        setError('Failed to load profile.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchProfile();
  }, []);

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSaving(true);
    setError('');
    setSuccess('');
    try {
      await updateProfile(profile);
      setSuccess('Profile updated successfully!');
    } catch {
      setError('Failed to update profile.');
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" py={6}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>My Profile</Typography>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 3 }}>{success}</Alert>}

      <Paper component="form" onSubmit={handleSave} sx={{ p: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
          <Avatar sx={{ width: 80, height: 80, mr: 3, bgcolor: 'primary.main', fontSize: 32 }}>
            {profile.firstName?.[0]}{profile.lastName?.[0]}
          </Avatar>
          <Box>
            <Typography variant="h6">{profile.firstName} {profile.lastName}</Typography>
            <Typography variant="body2" color="text.secondary">{email}</Typography>
          </Box>
        </Box>

        <Divider sx={{ mb: 3 }} />

        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              fullWidth
              label="First Name"
              value={profile.firstName}
              onChange={(e) => setProfile({ ...profile, firstName: e.target.value })}
              required
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              fullWidth
              label="Last Name"
              value={profile.lastName}
              onChange={(e) => setProfile({ ...profile, lastName: e.target.value })}
              required
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
              <Button
                type="submit"
                variant="contained"
                startIcon={isSaving ? <CircularProgress size={20} /> : <SaveIcon />}
                disabled={isSaving}
              >
                {isSaving ? 'Saving...' : 'Save Changes'}
              </Button>
            </Box>
          </Grid>
        </Grid>
      </Paper>
    </Container>
  );
};

export default Profile;
