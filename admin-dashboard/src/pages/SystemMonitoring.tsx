import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Grid,
  Paper,
  Tab,
  Tabs,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  CircularProgress,
  Alert,
  TextField,
  MenuItem,
  Switch,
  FormControlLabel,
  Button,
  Pagination,
  Stack,
  Snackbar,
  InputAdornment,
  Divider,
} from '@mui/material';
import {
  CheckCircle as HealthyIcon,
  Error as UnhealthyIcon,
  Warning as DegradedIcon,
  Refresh as RefreshIcon,
  Webhook as WebhookIcon,
  Settings as SettingsIcon,
  Save as SaveIcon,
  Search as SearchIcon,
} from '@mui/icons-material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApi } from '../services/adminApi';
import type {
  AdminAuditLogItem,
  AdminWebhookItem,
  PlatformSettings,
  AuditLogFilterParameters,
} from '../types/admin.types';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

const TabPanel = ({ children, value, index }: TabPanelProps) => (
  <div hidden={value !== index}>
    {value === index && <Box sx={{ pt: 2 }}>{children}</Box>}
  </div>
);

const formatDate = (dateStr: string) =>
  new Date(dateStr).toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });

const HealthStatusIcon = ({ status }: { status: string }) => {
  if (status === 'Healthy') return <HealthyIcon color="success" />;
  if (status === 'Unhealthy') return <UnhealthyIcon color="error" />;
  return <DegradedIcon color="warning" />;
};

const EVENT_CODES = [
  '', 'Login', 'Logout', 'Register', 'DeleteAccount',
  'CreatePost', 'EditPost', 'EditCatalog', 'DeleteCatalog', 'DeletePost',
  'CreateCalalog', 'Maintenance',
];

export default function SystemMonitoring() {
  const [tab, setTab] = useState(0);
  const [auditFilters, setAuditFilters] = useState<AuditLogFilterParameters>({ pageNumber: 1, pageSize: 50 });
  const [webhookPage, setWebhookPage] = useState(1);
  const [snackbar, setSnackbar] = useState<{ open: boolean; message: string; severity: 'success' | 'error' }>({
    open: false,
    message: '',
    severity: 'success',
  });
  const [localSettings, setLocalSettings] = useState<PlatformSettings | null>(null);

  const queryClient = useQueryClient();

  // System health
  const {
    data: health,
    isLoading: healthLoading,
    error: healthError,
    refetch: refetchHealth,
  } = useQuery({
    queryKey: ['systemHealth'],
    queryFn: () => adminApi.getSystemHealth(),
    refetchInterval: 30_000, // auto-refresh every 30 s
    retry: 1,
  });

  // Audit logs
  const {
    data: auditLogs,
    isLoading: auditLoading,
    error: auditError,
  } = useQuery({
    queryKey: ['auditLogs', auditFilters],
    queryFn: () => adminApi.getAuditLogs(auditFilters),
    enabled: tab === 1,
    retry: 1,
  });

  // Webhooks
  const {
    data: webhooks,
    isLoading: webhooksLoading,
    error: webhooksError,
  } = useQuery({
    queryKey: ['adminWebhooks', webhookPage],
    queryFn: () => adminApi.getWebhooks(webhookPage),
    enabled: tab === 2,
    retry: 1,
  });

  // Platform settings
  const {
    data: settings,
    isLoading: settingsLoading,
    error: settingsError,
  } = useQuery({
    queryKey: ['platformSettings'],
    queryFn: () => adminApi.getPlatformSettings(),
    enabled: tab === 3,
    retry: 1,
  });

  // Sync fetched settings into local edit state once
  useEffect(() => {
    if (settings && !localSettings) setLocalSettings(settings);
  }, [settings]);

  const updateSettingsMutation = useMutation({
    mutationFn: (s: PlatformSettings) => adminApi.updatePlatformSettings(s),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['platformSettings'] });
      setSnackbar({ open: true, message: 'Settings saved successfully.', severity: 'success' });
    },
    onError: () => {
      setSnackbar({ open: true, message: 'Failed to save settings.', severity: 'error' });
    },
  });

  const displaySettings = localSettings ?? settings;

  const handleSettingToggle = (key: keyof PlatformSettings) => {
    if (!displaySettings) return;
    setLocalSettings({ ...displaySettings, [key]: !(displaySettings[key] as boolean) });
  };

  const handleSettingNumber = (key: keyof PlatformSettings, value: number) => {
    if (!displaySettings) return;
    setLocalSettings({ ...displaySettings, [key]: value });
  };

  const handleSaveSettings = () => {
    if (displaySettings) updateSettingsMutation.mutate(displaySettings);
  };

  return (
    <Box>
      <Typography variant="h5" fontWeight="bold" mb={0.5}>
        System Monitoring
      </Typography>
      <Typography variant="body2" color="text.secondary" mb={3}>
        Platform health, audit logs, webhooks and configuration
      </Typography>

      {/* Health overview cards */}
      {healthLoading && (
        <Box display="flex" justifyContent="center" py={3}>
          <CircularProgress />
        </Box>
      )}
      {healthError && (
        <Alert severity="info" sx={{ mb: 2 }}>
          Health status is currently unavailable.
        </Alert>
      )}
      {health && (
        <Box mb={3}>
          <Box display="flex" alignItems="center" gap={1} mb={1.5}>
            <HealthStatusIcon status={health.overallStatus} />
            <Typography variant="h6">
              Overall Status:{' '}
              <Typography
                component="span"
                color={health.overallStatus === 'Healthy' ? 'success.main' : 'warning.main'}
                fontWeight="bold"
              >
                {health.overallStatus}
              </Typography>
            </Typography>
            <Button size="small" startIcon={<RefreshIcon />} onClick={() => refetchHealth()}>
              Refresh
            </Button>
            <Typography variant="caption" color="text.secondary">
              Last checked: {formatDate(health.checkedAt)}
            </Typography>
          </Box>
          <Grid container spacing={2}>
            {health.services.map((svc) => (
              <Grid item xs={12} sm={6} md={4} key={svc.serviceName}>
                <Paper sx={{ p: 2 }}>
                  <Box display="flex" alignItems="center" gap={1} mb={0.5}>
                    <HealthStatusIcon status={svc.status} />
                    <Typography fontWeight="medium">{svc.serviceName}</Typography>
                    <Chip
                      label={svc.status}
                      size="small"
                      color={svc.status === 'Healthy' ? 'success' : 'error'}
                      sx={{ ml: 'auto' }}
                    />
                  </Box>
                  <Typography variant="body2" color="text.secondary">
                    {svc.message}
                  </Typography>
                  {svc.responseTimeMs >= 0 && (
                    <Typography variant="caption" color="text.secondary">
                      Response: {svc.responseTimeMs} ms
                    </Typography>
                  )}
                </Paper>
              </Grid>
            ))}
          </Grid>
        </Box>
      )}

      {/* Tabs */}
      <Paper>
        <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tab label="Health" />
          <Tab label="Audit Logs" />
          <Tab label="Webhooks" />
          <Tab label="Settings" />
        </Tabs>

        {/* Health tab — detailed table */}
        <TabPanel value={tab} index={0}>
          <Box px={2} pb={2}>
            {health && (
              <TableContainer>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>Service</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Message</TableCell>
                      <TableCell>Response (ms)</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {health.services.map((svc) => (
                      <TableRow key={svc.serviceName} hover>
                        <TableCell>{svc.serviceName}</TableCell>
                        <TableCell>
                          <Chip
                            label={svc.status}
                            size="small"
                            color={svc.status === 'Healthy' ? 'success' : 'error'}
                          />
                        </TableCell>
                        <TableCell>{svc.message}</TableCell>
                        <TableCell>{svc.responseTimeMs >= 0 ? svc.responseTimeMs : 'N/A'}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </Box>
        </TabPanel>

        {/* Audit Logs tab */}
        <TabPanel value={tab} index={1}>
          <Box px={2} pb={2}>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} mb={2}>
              <TextField
                label="User ID"
                size="small"
                value={auditFilters.userId ?? ''}
                onChange={(e) =>
                  setAuditFilters({ ...auditFilters, userId: e.target.value || undefined, pageNumber: 1 })
                }
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <SearchIcon fontSize="small" />
                    </InputAdornment>
                  ),
                }}
              />
              <TextField
                select
                label="Event Code"
                size="small"
                value={auditFilters.eventCode ?? ''}
                onChange={(e) =>
                  setAuditFilters({ ...auditFilters, eventCode: e.target.value || undefined, pageNumber: 1 })
                }
                sx={{ minWidth: 160 }}
              >
                {EVENT_CODES.map((c) => (
                  <MenuItem key={c} value={c}>
                    {c || 'All Events'}
                  </MenuItem>
                ))}
              </TextField>
              <TextField
                label="From"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                value={auditFilters.startDate ?? ''}
                onChange={(e) =>
                  setAuditFilters({ ...auditFilters, startDate: e.target.value || undefined, pageNumber: 1 })
                }
              />
              <TextField
                label="To"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                value={auditFilters.endDate ?? ''}
                onChange={(e) =>
                  setAuditFilters({ ...auditFilters, endDate: e.target.value || undefined, pageNumber: 1 })
                }
              />
            </Stack>
            {auditLoading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}
            {auditError && <Alert severity="error">Failed to load audit logs.</Alert>}
            {auditLogs && (
              <>
                <TableContainer>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>Event</TableCell>
                        <TableCell>User</TableCell>
                        <TableCell>Reference</TableCell>
                        <TableCell>Timestamp</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {auditLogs.items.map((entry: AdminAuditLogItem) => (
                        <TableRow key={entry.id} hover>
                          <TableCell>
                            <Chip label={entry.eventCode} size="small" variant="outlined" />
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2">{entry.userEmail || entry.userId}</Typography>
                          </TableCell>
                          <TableCell>
                            <Typography variant="caption" fontFamily="monospace">
                              {entry.referenceId || '—'}
                            </Typography>
                          </TableCell>
                          <TableCell>{formatDate(entry.created)}</TableCell>
                        </TableRow>
                      ))}
                      {auditLogs.items.length === 0 && (
                        <TableRow>
                          <TableCell colSpan={4} align="center">
                            No audit log entries found.
                          </TableCell>
                        </TableRow>
                      )}
                    </TableBody>
                  </Table>
                </TableContainer>
                <Box display="flex" alignItems="center" justifyContent="space-between" mt={2}>
                  <Typography variant="caption" color="text.secondary">
                    {auditLogs.totalCount.toLocaleString()} total entries
                  </Typography>
                  <Pagination
                    count={Math.ceil(auditLogs.totalCount / (auditLogs.pageSize || 50))}
                    page={auditFilters.pageNumber ?? 1}
                    onChange={(_, p) => setAuditFilters({ ...auditFilters, pageNumber: p })}
                  />
                </Box>
              </>
            )}
          </Box>
        </TabPanel>

        {/* Webhooks tab */}
        <TabPanel value={tab} index={2}>
          <Box px={2} pb={2}>
            <Box display="flex" alignItems="center" gap={1} mb={2}>
              <WebhookIcon color="action" />
              <Typography variant="subtitle1">All Registered Webhooks</Typography>
            </Box>
            {webhooksLoading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}
            {webhooksError && <Alert severity="error">Failed to load webhooks.</Alert>}
            {webhooks && (
              <>
                <TableContainer>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>URL</TableCell>
                        <TableCell>Owner</TableCell>
                        <TableCell>Active</TableCell>
                        <TableCell>Success</TableCell>
                        <TableCell>Failures</TableCell>
                        <TableCell>Last Triggered</TableCell>
                        <TableCell>Created</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {webhooks.items.map((wh: AdminWebhookItem) => (
                        <TableRow key={wh.id} hover>
                          <TableCell>
                            <Typography
                              variant="body2"
                              sx={{ maxWidth: 240, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}
                              title={wh.url}
                            >
                              {wh.url}
                            </Typography>
                          </TableCell>
                          <TableCell>{wh.userEmail || wh.userId.slice(0, 8)}</TableCell>
                          <TableCell>
                            <Chip
                              label={wh.isActive ? 'Active' : 'Inactive'}
                              size="small"
                              color={wh.isActive ? 'success' : 'default'}
                            />
                          </TableCell>
                          <TableCell>{wh.successCount}</TableCell>
                          <TableCell>{wh.failureCount}</TableCell>
                          <TableCell>{wh.lastTriggeredAt ? formatDate(wh.lastTriggeredAt) : '—'}</TableCell>
                          <TableCell>{formatDate(wh.createdAt)}</TableCell>
                        </TableRow>
                      ))}
                      {webhooks.items.length === 0 && (
                        <TableRow>
                          <TableCell colSpan={7} align="center">
                            No webhooks registered.
                          </TableCell>
                        </TableRow>
                      )}
                    </TableBody>
                  </Table>
                </TableContainer>
                <Box display="flex" justifyContent="center" mt={2}>
                  <Pagination
                    count={Math.ceil(webhooks.totalCount / (webhooks.pageSize || 20))}
                    page={webhookPage}
                    onChange={(_, p) => setWebhookPage(p)}
                  />
                </Box>
              </>
            )}
          </Box>
        </TabPanel>

        {/* Settings tab */}
        <TabPanel value={tab} index={3}>
          <Box px={2} pb={2}>
            <Box display="flex" alignItems="center" gap={1} mb={2}>
              <SettingsIcon color="action" />
              <Typography variant="subtitle1">Platform Settings &amp; Feature Toggles</Typography>
            </Box>
            {settingsLoading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}
            {settingsError && <Alert severity="error">Failed to load platform settings.</Alert>}
            {displaySettings && (
              <>
                <Typography variant="subtitle2" color="text.secondary" mb={1}>
                  Feature Toggles
                </Typography>
                <Paper variant="outlined" sx={{ p: 2, mb: 2 }}>
                  <Grid container spacing={1}>
                    {(
                      [
                        { key: 'maintenanceMode', label: 'Maintenance Mode', description: 'Show maintenance page to all users' },
                        { key: 'registrationEnabled', label: 'User Registration', description: 'Allow new users to register' },
                        { key: 'guideApplicationsEnabled', label: 'Guide Applications', description: 'Allow guide applications' },
                        { key: 'tourBookingEnabled', label: 'Tour Booking', description: 'Allow tour booking' },
                        { key: 'paymentsEnabled', label: 'Payments', description: 'Enable payment processing' },
                        { key: 'emailNotificationsEnabled', label: 'Email Notifications', description: 'Send email notifications' },
                      ] as { key: keyof PlatformSettings; label: string; description: string }[]
                    ).map(({ key, label, description }) => (
                      <Grid item xs={12} sm={6} key={key}>
                        <FormControlLabel
                          control={
                            <Switch
                              checked={displaySettings[key] as boolean}
                              onChange={() => handleSettingToggle(key)}
                              color={key === 'maintenanceMode' ? 'error' : 'success'}
                            />
                          }
                          label={
                            <Box>
                              <Typography variant="body2" fontWeight="medium">
                                {label}
                              </Typography>
                              <Typography variant="caption" color="text.secondary">
                                {description}
                              </Typography>
                            </Box>
                          }
                        />
                      </Grid>
                    ))}
                  </Grid>
                </Paper>

                <Divider sx={{ my: 2 }} />

                <Typography variant="subtitle2" color="text.secondary" mb={1}>
                  Numeric Settings
                </Typography>
                <Paper variant="outlined" sx={{ p: 2, mb: 2 }}>
                  <Grid container spacing={2}>
                    <Grid item xs={12} sm={4}>
                      <TextField
                        label="Platform Fee (%)"
                        type="number"
                        size="small"
                        fullWidth
                        value={displaySettings.platformFeePercentage}
                        inputProps={{ min: 0, max: 100, step: 0.5 }}
                        onChange={(e) => handleSettingNumber('platformFeePercentage', parseFloat(e.target.value) || 0)}
                      />
                    </Grid>
                    <Grid item xs={12} sm={4}>
                      <TextField
                        label="Max Images per Post"
                        type="number"
                        size="small"
                        fullWidth
                        value={displaySettings.maxImagesPerPost}
                        inputProps={{ min: 1, max: 50 }}
                        onChange={(e) => handleSettingNumber('maxImagesPerPost', parseInt(e.target.value, 10) || 1)}
                      />
                    </Grid>
                    <Grid item xs={12} sm={4}>
                      <TextField
                        label="Min Booking Days in Advance"
                        type="number"
                        size="small"
                        fullWidth
                        value={displaySettings.minBookingDaysAdvance}
                        inputProps={{ min: 0, max: 365 }}
                        onChange={(e) =>
                          handleSettingNumber('minBookingDaysAdvance', parseInt(e.target.value, 10) || 0)
                        }
                      />
                    </Grid>
                  </Grid>
                </Paper>

                <Box display="flex" justifyContent="flex-end">
                  <Button
                    variant="contained"
                    startIcon={<SaveIcon />}
                    onClick={handleSaveSettings}
                    disabled={updateSettingsMutation.isPending}
                  >
                    {updateSettingsMutation.isPending ? 'Saving…' : 'Save Settings'}
                  </Button>
                </Box>
              </>
            )}
          </Box>
        </TabPanel>
      </Paper>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert severity={snackbar.severity} onClose={() => setSnackbar({ ...snackbar, open: false })}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
