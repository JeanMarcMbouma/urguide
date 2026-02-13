import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Chip,
  Button,
  Card,
  CardContent,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  OutlinedInput,
  Alert,
  Snackbar,
  CircularProgress,
} from '@mui/material';
import {
  ArrowBack as BackIcon,
  Edit as EditIcon,
  Block as BlockIcon,
  CheckCircle as ActivateIcon,
  Delete as DeleteIcon,
} from '@mui/icons-material';
import { adminApi } from '../services/adminApi';
import ConfirmDialog from '../components/shared/ConfirmDialog';

const UserDetail = () => {
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [editingRoles, setEditingRoles] = useState(false);
  const [selectedRoles, setSelectedRoles] = useState<string[]>([]);
  const [confirmDialog, setConfirmDialog] = useState<{
    open: boolean;
    action: 'suspend' | 'activate' | 'delete' | null;
  }>({
    open: false,
    action: null,
  });
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  // Fetch user details
  const { data: user, isLoading, error } = useQuery({
    queryKey: ['user', userId],
    queryFn: () => adminApi.getUserDetail(userId!),
    enabled: !!userId,
  });

  // Fetch available roles
  const { data: allRoles } = useQuery({
    queryKey: ['roles'],
    queryFn: () => adminApi.getAllRoles(),
  });

  // Mutations
  const updateRolesMutation = useMutation({
    mutationFn: () => adminApi.updateUserRoles({ userId: userId!, roles: selectedRoles }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user', userId] });
      setEditingRoles(false);
      setSnackbar({ open: true, message: 'Roles updated successfully', severity: 'success' });
    },
    onError: () => {
      setSnackbar({ open: true, message: 'Failed to update roles', severity: 'error' });
    },
  });

  const suspendMutation = useMutation({
    mutationFn: () => adminApi.suspendUser(userId!, 30),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user', userId] });
      setSnackbar({ open: true, message: 'User suspended successfully', severity: 'success' });
    },
  });

  const activateMutation = useMutation({
    mutationFn: () => adminApi.activateUser(userId!),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user', userId] });
      setSnackbar({ open: true, message: 'User activated successfully', severity: 'success' });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: () => adminApi.deleteUser(userId!),
    onSuccess: () => {
      setSnackbar({ open: true, message: 'User deleted successfully', severity: 'success' });
      setTimeout(() => navigate('/users'), 1500);
    },
  });

  const handleStartEditRoles = () => {
    setSelectedRoles(user?.roles || []);
    setEditingRoles(true);
  };

  const handleConfirm = () => {
    const { action } = confirmDialog;
    if (action === 'suspend') suspendMutation.mutate();
    if (action === 'activate') activateMutation.mutate();
    if (action === 'delete') deleteMutation.mutate();
    setConfirmDialog({ open: false, action: null });
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" mt={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !user) {
    return (
      <Box>
        <Alert severity="error">Failed to load user details</Alert>
        <Button startIcon={<BackIcon />} onClick={() => navigate('/users')} sx={{ mt: 2 }}>
          Back to Users
        </Button>
      </Box>
    );
  }

  const isLocked = user.lockoutEnd && new Date(user.lockoutEnd) > new Date();

  return (
    <Box>
      <Button startIcon={<BackIcon />} onClick={() => navigate('/users')} sx={{ mb: 2 }}>
        Back to Users
      </Button>

      <Typography variant="h4" gutterBottom>
        User Details
      </Typography>

      <Grid container spacing={3}>
        {/* User Information */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3 }}>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
              <Typography variant="h6">Profile Information</Typography>
              <Chip
                label={isLocked ? 'Suspended' : 'Active'}
                color={isLocked ? 'error' : 'success'}
              />
            </Box>

            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Typography variant="body2" color="text.secondary">
                  Email
                </Typography>
                <Typography variant="body1" fontWeight="medium">
                  {user.email}
                </Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="body2" color="text.secondary">
                  Name
                </Typography>
                <Typography variant="body1" fontWeight="medium">
                  {user.firstName} {user.lastName}
                </Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="body2" color="text.secondary">
                  Phone Number
                </Typography>
                <Typography variant="body1" fontWeight="medium">
                  {user.phoneNumber || 'N/A'}
                </Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="body2" color="text.secondary">
                  User Type
                </Typography>
                <Chip label={user.isGuide ? 'Guide' : 'User'} size="small" />
              </Grid>
            </Grid>

            <Box mt={3}>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Roles
              </Typography>
              {editingRoles ? (
                <Box>
                  <FormControl fullWidth sx={{ mb: 2 }}>
                    <InputLabel>Roles</InputLabel>
                    <Select
                      multiple
                      value={selectedRoles}
                      onChange={(e) => setSelectedRoles(e.target.value as string[])}
                      input={<OutlinedInput label="Roles" />}
                      renderValue={(selected) => (
                        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                          {selected.map((value) => (
                            <Chip key={value} label={value} size="small" />
                          ))}
                        </Box>
                      )}
                    >
                      {allRoles?.map((role) => (
                        <MenuItem key={role} value={role}>
                          {role}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                  <Box display="flex" gap={1}>
                    <Button variant="contained" onClick={() => updateRolesMutation.mutate()}>
                      Save
                    </Button>
                    <Button variant="outlined" onClick={() => setEditingRoles(false)}>
                      Cancel
                    </Button>
                  </Box>
                </Box>
              ) : (
                <Box display="flex" gap={1} alignItems="center">
                  {user.roles.map((role) => (
                    <Chip key={role} label={role} />
                  ))}
                  <Button startIcon={<EditIcon />} size="small" onClick={handleStartEditRoles}>
                    Edit
                  </Button>
                </Box>
              )}
            </Box>
          </Paper>

          {/* Activity Statistics */}
          <Paper sx={{ p: 3, mt: 3 }}>
            <Typography variant="h6" gutterBottom>
              Activity Statistics
            </Typography>
            <Grid container spacing={2}>
              <Grid item xs={6} sm={3}>
                <Card variant="outlined">
                  <CardContent>
                    <Typography variant="body2" color="text.secondary">
                      Posts
                    </Typography>
                    <Typography variant="h5">{user.postCount}</Typography>
                  </CardContent>
                </Card>
              </Grid>
              <Grid item xs={6} sm={3}>
                <Card variant="outlined">
                  <CardContent>
                    <Typography variant="body2" color="text.secondary">
                      Tours
                    </Typography>
                    <Typography variant="h5">{user.tourCount}</Typography>
                  </CardContent>
                </Card>
              </Grid>
            </Grid>
            <Button
              variant="outlined"
              fullWidth
              sx={{ mt: 2 }}
              onClick={() => navigate(`/users/${userId}/activity`)}
            >
              View Activity Log
            </Button>
          </Paper>
        </Grid>

        {/* Security & Actions */}
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3, mb: 3 }}>
            <Typography variant="h6" gutterBottom>
              Security Status
            </Typography>
            <Box display="flex" flexDirection="column" gap={1}>
              <Box display="flex" justifyContent="space-between">
                <Typography variant="body2">Email Verified</Typography>
                <Chip
                  label={user.emailConfirmed ? 'Yes' : 'No'}
                  color={user.emailConfirmed ? 'success' : 'default'}
                  size="small"
                />
              </Box>
              <Box display="flex" justifyContent="space-between">
                <Typography variant="body2">2FA Enabled</Typography>
                <Chip
                  label={user.twoFactorEnabled ? 'Yes' : 'No'}
                  color={user.twoFactorEnabled ? 'success' : 'default'}
                  size="small"
                />
              </Box>
              <Box display="flex" justifyContent="space-between">
                <Typography variant="body2">Failed Attempts</Typography>
                <Typography variant="body2" fontWeight="medium">
                  {user.accessFailedCount}
                </Typography>
              </Box>
              {isLocked && (
                <Box display="flex" flexDirection="column" mt={1}>
                  <Typography variant="body2" color="error">
                    Locked Until
                  </Typography>
                  <Typography variant="body2" fontWeight="medium">
                    {new Date(user.lockoutEnd!).toLocaleString()}
                  </Typography>
                </Box>
              )}
            </Box>
          </Paper>

          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Actions
            </Typography>
            <Box display="flex" flexDirection="column" gap={1}>
              {isLocked ? (
                <Button
                  variant="contained"
                  color="success"
                  startIcon={<ActivateIcon />}
                  onClick={() => setConfirmDialog({ open: true, action: 'activate' })}
                >
                  Activate Account
                </Button>
              ) : (
                <Button
                  variant="outlined"
                  color="warning"
                  startIcon={<BlockIcon />}
                  onClick={() => setConfirmDialog({ open: true, action: 'suspend' })}
                >
                  Suspend Account
                </Button>
              )}
              <Button
                variant="outlined"
                color="error"
                startIcon={<DeleteIcon />}
                onClick={() => setConfirmDialog({ open: true, action: 'delete' })}
              >
                Delete Account
              </Button>
            </Box>
          </Paper>
        </Grid>
      </Grid>

      <ConfirmDialog
        open={confirmDialog.open}
        title={`${confirmDialog.action === 'delete' ? 'Delete' : confirmDialog.action === 'suspend' ? 'Suspend' : 'Activate'} User`}
        message={`Are you sure you want to ${confirmDialog.action} this user?`}
        confirmText={confirmDialog.action === 'delete' ? 'Delete' : 'Confirm'}
        severity={confirmDialog.action === 'delete' ? 'error' : 'warning'}
        onConfirm={handleConfirm}
        onCancel={() => setConfirmDialog({ open: false, action: null })}
      />

      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
      >
        <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
};

export default UserDetail;
