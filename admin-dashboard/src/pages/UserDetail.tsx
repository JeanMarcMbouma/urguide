import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
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
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from '@mui/material';
import {
  ArrowBack as BackIcon,
  Edit as EditIcon,
  Block as BlockIcon,
  CheckCircle as ActivateIcon,
  Delete as DeleteIcon,
  AcUnit as FreezeIcon,
  Whatshot as UnfreezeIcon,
} from '@mui/icons-material';
import { adminApi } from '../services/adminApi';
import ConfirmDialog from '../components/shared/ConfirmDialog';

const UserDetail = () => {
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { t } = useTranslation();
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
  const [freezeDialog, setFreezeDialog] = useState(false);
  const [freezeReason, setFreezeReason] = useState('');
  const [freezeDuration, setFreezeDuration] = useState<number | ''>('');
  const [unfreezeDialog, setUnfreezeDialog] = useState(false);
  const [unfreezeReason, setUnfreezeReason] = useState('');

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
      setSnackbar({ open: true, message: t('userDetail.rolesUpdated'), severity: 'success' });
    },
    onError: () => {
      setSnackbar({ open: true, message: t('userDetail.rolesUpdateError'), severity: 'error' });
    },
  });

  const suspendMutation = useMutation({
    mutationFn: () => adminApi.suspendUser(userId!, 30),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user', userId] });
      setSnackbar({ open: true, message: t('userDetail.suspendSuccess'), severity: 'success' });
    },
  });

  const activateMutation = useMutation({
    mutationFn: () => adminApi.activateUser(userId!),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user', userId] });
      setSnackbar({ open: true, message: t('userDetail.activateSuccess'), severity: 'success' });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: () => adminApi.deleteUser(userId!),
    onSuccess: () => {
      setSnackbar({ open: true, message: t('userDetail.deleteSuccess'), severity: 'success' });
      setTimeout(() => navigate('/users'), 1500);
    },
  });

  // Freeze / Unfreeze mutations
  const { data: freezeHistory } = useQuery({
    queryKey: ['freezeHistory', userId],
    queryFn: () => adminApi.getFreezeHistory(userId!),
    enabled: !!userId,
  });

  const freezeMutation = useMutation({
    mutationFn: () =>
      adminApi.freezeAccount({
        userId: userId!,
        reason: freezeReason,
        durationDays: freezeDuration ? Number(freezeDuration) : undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user', userId] });
      queryClient.invalidateQueries({ queryKey: ['freezeHistory', userId] });
      setFreezeDialog(false);
      setFreezeReason('');
      setFreezeDuration('');
      setSnackbar({ open: true, message: t('userDetail.freezeSuccess'), severity: 'success' });
    },
    onError: () => {
      setSnackbar({ open: true, message: t('userDetail.freezeError'), severity: 'error' });
    },
  });

  const unfreezeMutation = useMutation({
    mutationFn: () =>
      adminApi.unfreezeAccount({ userId: userId!, reason: unfreezeReason || undefined }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user', userId] });
      queryClient.invalidateQueries({ queryKey: ['freezeHistory', userId] });
      setUnfreezeDialog(false);
      setUnfreezeReason('');
      setSnackbar({ open: true, message: t('userDetail.unfreezeSuccess'), severity: 'success' });
    },
    onError: () => {
      setSnackbar({ open: true, message: t('userDetail.unfreezeError'), severity: 'error' });
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
        <Alert severity="error">{t('userDetail.loadError')}</Alert>
        <Button startIcon={<BackIcon />} onClick={() => navigate('/users')} sx={{ mt: 2 }}>
          {t('userDetail.backToUsers')}
        </Button>
      </Box>
    );
  }

  const isLocked = user.lockoutEnd && new Date(user.lockoutEnd) > new Date();
  const activeFreeze = freezeHistory?.items?.find((f) => f.status === 'Active');
  const isFrozen = !!activeFreeze;

  return (
    <Box>
      <Button startIcon={<BackIcon />} onClick={() => navigate('/users')} sx={{ mb: 2 }}>
        {t('userDetail.backToUsers')}
      </Button>

      <Typography variant="h4" gutterBottom>
        {t('userDetail.title')}
      </Typography>

      <Grid container spacing={3}>
        {/* User Information */}
        <Grid size={{ xs: 12, md: 8 }}>
          <Paper sx={{ p: 3 }}>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
              <Typography variant="h6">{t('userDetail.profileInfo')}</Typography>
              <Chip
                label={isLocked ? t('userDetail.suspended') : t('userDetail.active')}
                color={isLocked ? 'error' : 'success'}
              />
            </Box>

            <Grid container spacing={2}>
              <Grid size={{ xs: 12, sm: 6 }}>
                <Typography variant="body2" color="text.secondary">
                  {t('userDetail.email')}
                </Typography>
                <Typography variant="body1" fontWeight="medium">
                  {user.email}
                </Typography>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <Typography variant="body2" color="text.secondary">
                  {t('userDetail.name')}
                </Typography>
                <Typography variant="body1" fontWeight="medium">
                  {user.firstName} {user.lastName}
                </Typography>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <Typography variant="body2" color="text.secondary">
                  {t('userDetail.phoneNumber')}
                </Typography>
                <Typography variant="body1" fontWeight="medium">
                  {user.phoneNumber || t('common.na')}
                </Typography>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <Typography variant="body2" color="text.secondary">
                  {t('userDetail.userType')}
                </Typography>
                <Chip label={user.isGuide ? t('userDetail.guide') : t('userDetail.user')} size="small" />
              </Grid>
            </Grid>

            <Box mt={3}>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                {t('userDetail.roles')}
              </Typography>
              {editingRoles ? (
                <Box>
                  <FormControl fullWidth sx={{ mb: 2 }}>
                    <InputLabel>{t('userDetail.roles')}</InputLabel>
                    <Select
                      multiple
                      value={selectedRoles}
                      onChange={(e) => setSelectedRoles(e.target.value as string[])}
                      input={<OutlinedInput label={t('userDetail.roles')} />}
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
                      {t('common.save')}
                    </Button>
                    <Button variant="outlined" onClick={() => setEditingRoles(false)}>
                      {t('common.cancel')}
                    </Button>
                  </Box>
                </Box>
              ) : (
                <Box display="flex" gap={1} alignItems="center">
                  {user.roles.map((role) => (
                    <Chip key={role} label={role} />
                  ))}
                  <Button startIcon={<EditIcon />} size="small" onClick={handleStartEditRoles}>
                    {t('common.edit')}
                  </Button>
                </Box>
              )}
            </Box>
          </Paper>

          {/* Activity Statistics */}
          <Paper sx={{ p: 3, mt: 3 }}>
            <Typography variant="h6" gutterBottom>
              {t('userDetail.activityStats')}
            </Typography>
            <Grid container spacing={2}>
              <Grid size={{ xs: 6, sm: 3 }}>
                <Card variant="outlined">
                  <CardContent>
                    <Typography variant="body2" color="text.secondary">
                      {t('userDetail.posts')}
                    </Typography>
                    <Typography variant="h5">{user.postCount}</Typography>
                  </CardContent>
                </Card>
              </Grid>
              <Grid size={{ xs: 6, sm: 3 }}>
                <Card variant="outlined">
                  <CardContent>
                    <Typography variant="body2" color="text.secondary">
                      {t('userDetail.tours')}
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
              {t('userDetail.viewActivityLog')}
            </Button>
          </Paper>
        </Grid>

        {/* Security & Actions */}
        <Grid size={{ xs: 12, md: 4 }}>
          <Paper sx={{ p: 3, mb: 3 }}>
            <Typography variant="h6" gutterBottom>
              {t('userDetail.securityStatus')}
            </Typography>
            <Box display="flex" flexDirection="column" gap={1}>
              <Box display="flex" justifyContent="space-between">
                <Typography variant="body2">{t('userDetail.emailVerified')}</Typography>
                <Chip
                  label={user.emailConfirmed ? t('common.yes') : t('common.no')}
                  color={user.emailConfirmed ? 'success' : 'default'}
                  size="small"
                />
              </Box>
              <Box display="flex" justifyContent="space-between">
                <Typography variant="body2">{t('userDetail.twoFaEnabled')}</Typography>
                <Chip
                  label={user.twoFactorEnabled ? t('common.yes') : t('common.no')}
                  color={user.twoFactorEnabled ? 'success' : 'default'}
                  size="small"
                />
              </Box>
              <Box display="flex" justifyContent="space-between">
                <Typography variant="body2">{t('userDetail.failedAttempts')}</Typography>
                <Typography variant="body2" fontWeight="medium">
                  {user.accessFailedCount}
                </Typography>
              </Box>
              {isLocked && (
                <Box display="flex" flexDirection="column" mt={1}>
                  <Typography variant="body2" color="error">
                    {t('userDetail.lockedUntil')}
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
              {t('userDetail.actions')}
            </Typography>
            <Box display="flex" flexDirection="column" gap={1}>
              {isLocked ? (
                <Button
                  variant="contained"
                  color="success"
                  startIcon={<ActivateIcon />}
                  onClick={() => setConfirmDialog({ open: true, action: 'activate' })}
                >
                  {t('userDetail.activateAccount')}
                </Button>
              ) : (
                <Button
                  variant="outlined"
                  color="warning"
                  startIcon={<BlockIcon />}
                  onClick={() => setConfirmDialog({ open: true, action: 'suspend' })}
                >
                  {t('userDetail.suspendAccount')}
                </Button>
              )}
              {isFrozen ? (
                <Button
                  variant="contained"
                  color="info"
                  startIcon={<UnfreezeIcon />}
                  onClick={() => setUnfreezeDialog(true)}
                >
                  {t('userDetail.unfreezeAccount')}
                </Button>
              ) : (
                <Button
                  variant="outlined"
                  color="secondary"
                  startIcon={<FreezeIcon />}
                  onClick={() => setFreezeDialog(true)}
                >
                  {t('userDetail.freezeAccount')}
                </Button>
              )}
              <Button
                variant="outlined"
                color="error"
                startIcon={<DeleteIcon />}
                onClick={() => setConfirmDialog({ open: true, action: 'delete' })}
              >
                {t('userDetail.deleteAccount')}
              </Button>
            </Box>
            {isFrozen && activeFreeze && (
              <Alert severity="info" sx={{ mt: 2 }}>
                <Typography variant="body2" fontWeight="bold">{t('userDetail.accountFrozen')}</Typography>
                <Typography variant="body2">{t('userDetail.reason')}: {activeFreeze.reason}</Typography>
                {activeFreeze.expiresAt && (
                  <Typography variant="body2">
                    {t('userDetail.expires')}: {new Date(activeFreeze.expiresAt).toLocaleString()}
                  </Typography>
                )}
              </Alert>
            )}
          </Paper>

          {/* Freeze History */}
          {freezeHistory && freezeHistory.items.length > 0 && (
            <Paper sx={{ p: 3, mt: 3 }}>
              <Typography variant="h6" gutterBottom>
                {t('userDetail.freezeHistory')}
              </Typography>
              <TableContainer>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>{t('userDetail.status')}</TableCell>
                      <TableCell>{t('userDetail.reason')}</TableCell>
                      <TableCell>{t('userDetail.frozenAt')}</TableCell>
                      <TableCell>{t('userDetail.expires')}</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {freezeHistory.items.map((record) => (
                      <TableRow key={record.id}>
                        <TableCell>
                          <Chip
                            label={record.status}
                            color={record.status === 'Active' ? 'error' : record.status === 'Unfrozen' ? 'success' : 'default'}
                            size="small"
                          />
                        </TableCell>
                        <TableCell>{record.reason}</TableCell>
                        <TableCell>{new Date(record.frozenAt).toLocaleString()}</TableCell>
                        <TableCell>{record.expiresAt ? new Date(record.expiresAt).toLocaleString() : t('common.indefinite')}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </Paper>
          )}
        </Grid>
      </Grid>

      <ConfirmDialog
        open={confirmDialog.open}
        title={`${confirmDialog.action === 'delete' ? t('userDetail.deleteUser') : confirmDialog.action === 'suspend' ? t('userDetail.suspendUser') : t('userDetail.activateUser')}`}
        message={t('userDetail.confirmAction', { action: confirmDialog.action })}
        confirmText={confirmDialog.action === 'delete' ? t('common.delete') : t('common.confirm')}
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

      {/* Freeze Dialog */}
      <Dialog open={freezeDialog} onClose={() => setFreezeDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('userDetail.freezeDialogTitle')}</DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            {t('userDetail.freezeDialogDesc')}
          </Typography>
          <TextField
            label={t('userDetail.reasonLabel')}
            fullWidth
            required
            multiline
            rows={3}
            value={freezeReason}
            onChange={(e) => setFreezeReason(e.target.value)}
            inputProps={{ maxLength: 2000 }}
            helperText={`${freezeReason.length}/2000`}
            sx={{ mb: 2 }}
          />
          <TextField
            label={t('userDetail.durationLabel')}
            type="number"
            fullWidth
            value={freezeDuration}
            onChange={(e) => setFreezeDuration(e.target.value ? Number(e.target.value) : '')}
            helperText={t('userDetail.durationHelp')}
            inputProps={{ min: 1, max: 3650 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setFreezeDialog(false)}>{t('common.cancel')}</Button>
          <Button
            variant="contained"
            color="secondary"
            disabled={!freezeReason.trim() || freezeMutation.isPending}
            onClick={() => freezeMutation.mutate()}
          >
            {freezeMutation.isPending ? t('userDetail.freezing') : t('userDetail.freeze')}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Unfreeze Dialog */}
      <Dialog open={unfreezeDialog} onClose={() => setUnfreezeDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('userDetail.unfreezeDialogTitle')}</DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            {t('userDetail.unfreezeDialogDesc')}
          </Typography>
          <TextField
            label={t('userDetail.reasonOptionalLabel')}
            fullWidth
            multiline
            rows={2}
            value={unfreezeReason}
            onChange={(e) => setUnfreezeReason(e.target.value)}
            inputProps={{ maxLength: 2000 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setUnfreezeDialog(false)}>{t('common.cancel')}</Button>
          <Button
            variant="contained"
            color="info"
            disabled={unfreezeMutation.isPending}
            onClick={() => unfreezeMutation.mutate()}
          >
            {unfreezeMutation.isPending ? t('userDetail.unfreezing') : t('userDetail.unfreeze')}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default UserDetail;
