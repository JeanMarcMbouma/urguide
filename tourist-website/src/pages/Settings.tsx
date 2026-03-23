import { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Paper,
  TextField,
  Button,
  Box,
  Alert,
  CircularProgress,
  Switch,
  FormControlLabel,
  Divider,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
} from '@mui/material';
import {
  Lock as LockIcon,
  Security as SecurityIcon,
  Notifications as NotificationsIcon,
} from '@mui/icons-material';
import { changePassword, getNotificationPreferences, updateNotificationPreferences, getUserProfile } from '../services/touristApi';
import type { UserPreferenceDto } from '../types/tourist.types';

// Helper to get boolean-like preference from array
const getPrefValue = (prefs: UserPreferenceDto[], type: string): boolean => {
  const pref = prefs.find((p) => p.preferenceType === type);
  return pref ? pref.preferenceValue === 'true' : false;
};

// Helper to set a boolean-like preference in array
const setPrefValue = (prefs: UserPreferenceDto[], type: string, value: boolean): UserPreferenceDto[] => {
  const existing = prefs.findIndex((p) => p.preferenceType === type);
  const newPref: UserPreferenceDto = { preferenceType: type, preferenceValue: String(value), weight: 1 };
  if (existing >= 0) {
    const updated = [...prefs];
    updated[existing] = newPref;
    return updated;
  }
  return [...prefs, newPref];
};

const Settings = () => {
  const [email, setEmail] = useState('');
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isChangingPassword, setIsChangingPassword] = useState(false);
  const [passwordError, setPasswordError] = useState('');
  const [passwordSuccess, setPasswordSuccess] = useState('');

  const [preferences, setPreferences] = useState<UserPreferenceDto[]>([]);
  const [isSavingPrefs, setIsSavingPrefs] = useState(false);
  const [prefsError, setPrefsError] = useState('');
  const [prefsSuccess, setPrefsSuccess] = useState('');

  useEffect(() => {
    const init = async () => {
      try {
        const user = await getUserProfile();
        setEmail(user.email);
      } catch {
        // use empty email
      }
      try {
        const prefs = await getNotificationPreferences();
        setPreferences(prefs);
      } catch {
        // Use defaults if fetch fails
      }
    };
    init();
  }, []);

  const handlePasswordChange = async (e: React.FormEvent) => {
    e.preventDefault();
    setPasswordError('');
    setPasswordSuccess('');

    if (newPassword !== confirmPassword) {
      setPasswordError('Passwords do not match.');
      return;
    }

    if (newPassword.length < 8) {
      setPasswordError('Password must be at least 8 characters.');
      return;
    }

    setIsChangingPassword(true);
    try {
      await changePassword(email, currentPassword, newPassword, confirmPassword);
      setPasswordSuccess('Password changed successfully!');
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
    } catch {
      setPasswordError('Failed to change password. Check your current password.');
    } finally {
      setIsChangingPassword(false);
    }
  };

  const togglePref = (type: string) => {
    setPreferences((prev) => setPrefValue(prev, type, !getPrefValue(prev, type)));
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>Settings</Typography>

      {/* Change Password */}
      <Paper sx={{ p: 4, mb: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <LockIcon sx={{ mr: 1 }} color="action" />
          <Typography variant="h6">Change Password</Typography>
        </Box>

        {passwordError && <Alert severity="error" sx={{ mb: 2 }}>{passwordError}</Alert>}
        {passwordSuccess && <Alert severity="success" sx={{ mb: 2 }}>{passwordSuccess}</Alert>}

        <Box component="form" onSubmit={handlePasswordChange}>
          <TextField
            fullWidth
            type="password"
            label="Current Password"
            value={currentPassword}
            onChange={(e) => setCurrentPassword(e.target.value)}
            sx={{ mb: 2 }}
            required
          />
          <TextField
            fullWidth
            type="password"
            label="New Password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            sx={{ mb: 2 }}
            required
          />
          <TextField
            fullWidth
            type="password"
            label="Confirm New Password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            sx={{ mb: 2 }}
            required
          />
          <Button
            type="submit"
            variant="contained"
            disabled={isChangingPassword || !currentPassword || !newPassword || !confirmPassword}
          >
            {isChangingPassword ? <CircularProgress size={24} /> : 'Change Password'}
          </Button>
        </Box>
      </Paper>

      {/* Security */}
      <Paper sx={{ p: 4, mb: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <SecurityIcon sx={{ mr: 1 }} color="action" />
          <Typography variant="h6">Security</Typography>
        </Box>
        <List>
          <ListItem>
            <ListItemText
              primary="Two-Factor Authentication"
              secondary="Add an extra layer of security to your account"
            />
            <ListItemSecondaryAction>
              <Button variant="outlined" size="small">
                Setup 2FA
              </Button>
            </ListItemSecondaryAction>
          </ListItem>
          <Divider />
          <ListItem>
            <ListItemText
              primary="Active Sessions"
              secondary="Manage your active login sessions"
            />
            <ListItemSecondaryAction>
              <Button variant="outlined" size="small" color="error">
                Sign Out All
              </Button>
            </ListItemSecondaryAction>
          </ListItem>
        </List>
      </Paper>

      {/* Notification Preferences */}
      <Paper sx={{ p: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <NotificationsIcon sx={{ mr: 1 }} color="action" />
          <Typography variant="h6">Notification Preferences</Typography>
        </Box>

        {prefsError && <Alert severity="error" sx={{ mb: 2 }}>{prefsError}</Alert>}
        {prefsSuccess && <Alert severity="success" sx={{ mb: 2 }}>{prefsSuccess}</Alert>}

        <List>
          {['emailNotifications', 'bidUpdates', 'tourReminders', 'promotionalEmails', 'reviewReminders'].map((type) => (
            <ListItem key={type}>
              <FormControlLabel
                control={
                  <Switch
                    checked={getPrefValue(preferences, type)}
                    onChange={() => togglePref(type)}
                  />
                }
                label={type.replace(/([A-Z])/g, ' $1').replace(/^./, (s) => s.toUpperCase())}
              />
            </ListItem>
          ))}
        </List>
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 2 }}>
          <Button
            variant="contained"
            disabled={isSavingPrefs}
            onClick={async () => {
              setIsSavingPrefs(true);
              setPrefsError('');
              setPrefsSuccess('');
              try {
                await updateNotificationPreferences(preferences);
                setPrefsSuccess('Preferences saved!');
              } catch {
                setPrefsError('Failed to save preferences.');
              } finally {
                setIsSavingPrefs(false);
              }
            }}
          >
            {isSavingPrefs ? <CircularProgress size={24} /> : 'Save Preferences'}
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default Settings;
