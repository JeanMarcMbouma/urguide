import { useState } from 'react';
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
import { changePassword } from '../services/touristApi';

const Settings = () => {
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isChangingPassword, setIsChangingPassword] = useState(false);
  const [passwordError, setPasswordError] = useState('');
  const [passwordSuccess, setPasswordSuccess] = useState('');

  const [notifications, setNotifications] = useState({
    emailNotifications: true,
    bidUpdates: true,
    tourReminders: true,
    promotionalEmails: false,
    reviewReminders: true,
  });

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
      await changePassword(currentPassword, newPassword);
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
        <List>
          <ListItem>
            <FormControlLabel
              control={
                <Switch
                  checked={notifications.emailNotifications}
                  onChange={(e) => setNotifications({ ...notifications, emailNotifications: e.target.checked })}
                />
              }
              label="Email Notifications"
            />
          </ListItem>
          <ListItem>
            <FormControlLabel
              control={
                <Switch
                  checked={notifications.bidUpdates}
                  onChange={(e) => setNotifications({ ...notifications, bidUpdates: e.target.checked })}
                />
              }
              label="Bid Updates"
            />
          </ListItem>
          <ListItem>
            <FormControlLabel
              control={
                <Switch
                  checked={notifications.tourReminders}
                  onChange={(e) => setNotifications({ ...notifications, tourReminders: e.target.checked })}
                />
              }
              label="Tour Reminders"
            />
          </ListItem>
          <ListItem>
            <FormControlLabel
              control={
                <Switch
                  checked={notifications.promotionalEmails}
                  onChange={(e) => setNotifications({ ...notifications, promotionalEmails: e.target.checked })}
                />
              }
              label="Promotional Emails"
            />
          </ListItem>
          <ListItem>
            <FormControlLabel
              control={
                <Switch
                  checked={notifications.reviewReminders}
                  onChange={(e) => setNotifications({ ...notifications, reviewReminders: e.target.checked })}
                />
              }
              label="Review Reminders"
            />
          </ListItem>
        </List>
      </Paper>
    </Container>
  );
};

export default Settings;
