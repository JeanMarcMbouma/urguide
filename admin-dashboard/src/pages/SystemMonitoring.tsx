import { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
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
  '', 'Login', 'Logout', 'FailedLogin', 'PasswordChanged', 'PasswordReset',
  'TwoFactorEnabled', 'TwoFactorDisabled',
  'Register', 'DeleteAccount', 'ProfileUpdated', 'EmailChanged', 'RolesUpdated',
  'CreatePost', 'EditPost', 'EditCatalog', 'DeleteCatalog', 'DeletePost', 'CreateCalalog',
  'AccountFrozen', 'AccountUnfrozen', 'AccountSuspended', 'AccountActivated', 'AccountDeleted',
  'GuideVerificationApproved', 'GuideVerificationRejected', 'TourApproved', 'TourRejected',
  'PaymentProcessed', 'RefundIssued', 'PayoutProcessed',
  'SettingsUpdated', 'Maintenance',
];

const AUDIT_CATEGORIES = [
  '', 'Authentication', 'Account', 'Content', 'AccountManagement',
  'Moderation', 'Financial', 'Settings', 'System',
];

const AUDIT_SEVERITIES = ['', 'Info', 'Warning', 'Critical'];

export default function SystemMonitoring() {
  const { t } = useTranslation();
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
  }, [settings, localSettings]);

  const updateSettingsMutation = useMutation({
    mutationFn: (s: PlatformSettings) => adminApi.updatePlatformSettings(s),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['platformSettings'] });
      setSnackbar({ open: true, message: t('system.settingsSaved'), severity: 'success' });
    },
    onError: () => {
      setSnackbar({ open: true, message: t('system.settingsSaveError'), severity: 'error' });
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
        {t('system.title')}
      </Typography>
      <Typography variant="body2" color="text.secondary" mb={3}>
        {t('system.subtitle')}
      </Typography>

      {/* Health overview cards */}
      {healthLoading && (
        <Box display="flex" justifyContent="center" py={3}>
          <CircularProgress />
        </Box>
      )}
      {healthError && (
        <Alert severity="info" sx={{ mb: 2 }}>
          {t('system.healthUnavailable')}
        </Alert>
      )}
      {health && (
        <Box mb={3}>
          <Box display="flex" alignItems="center" gap={1} mb={1.5}>
            <HealthStatusIcon status={health.overallStatus} />
            <Typography variant="h6">
              {t('system.overallStatus')}:{' '}
              <Typography
                component="span"
                color={health.overallStatus === 'Healthy' ? 'success.main' : 'warning.main'}
                fontWeight="bold"
              >
                {health.overallStatus}
              </Typography>
            </Typography>
            <Button size="small" startIcon={<RefreshIcon />} onClick={() => refetchHealth()}>
              {t('common.refresh')}
            </Button>
            <Typography variant="caption" color="text.secondary">
              {t('system.lastChecked')}: {formatDate(health.checkedAt)}
            </Typography>
          </Box>
          <Grid container spacing={2}>
            {health.services.map((svc) => (
              <Grid size={{ xs: 12, sm: 6, md: 4 }} key={svc.serviceName}>
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
                      {t('system.response')}: {svc.responseTimeMs} ms
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
          <Tab label={t('system.tabHealth')} />
          <Tab label={t('system.tabAuditLogs')} />
          <Tab label={t('system.tabWebhooks')} />
          <Tab label={t('system.tabSettings')} />
        </Tabs>

        {/* Health tab — detailed table */}
        <TabPanel value={tab} index={0}>
          <Box px={2} pb={2}>
            {health && (
              <TableContainer>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>{t('system.service')}</TableCell>
                      <TableCell>{t('system.status')}</TableCell>
                      <TableCell>{t('system.message')}</TableCell>
                      <TableCell>{t('system.responseMs')}</TableCell>
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
                        <TableCell>{svc.responseTimeMs >= 0 ? svc.responseTimeMs : t('common.na')}</TableCell>
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
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} mb={2} flexWrap="wrap">
              <TextField
                label={t('system.userId')}
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
                label={t('system.eventCode')}
                size="small"
                value={auditFilters.eventCode ?? ''}
                onChange={(e) =>
                  setAuditFilters({ ...auditFilters, eventCode: e.target.value || undefined, pageNumber: 1 })
                }
                sx={{ minWidth: 160 }}
              >
                {EVENT_CODES.map((c) => (
                  <MenuItem key={c} value={c}>
                    {c || t('system.allEvents')}
                  </MenuItem>
                ))}
              </TextField>
              <TextField
                select
                label={t('system.category')}
                size="small"
                value={auditFilters.category ?? ''}
                onChange={(e) =>
                  setAuditFilters({ ...auditFilters, category: e.target.value || undefined, pageNumber: 1 })
                }
                sx={{ minWidth: 160 }}
              >
                {AUDIT_CATEGORIES.map((c) => (
                  <MenuItem key={c} value={c}>
                    {c || t('system.allCategories')}
                  </MenuItem>
                ))}
              </TextField>
              <TextField
                select
                label={t('system.severity')}
                size="small"
                value={auditFilters.severity ?? ''}
                onChange={(e) =>
                  setAuditFilters({ ...auditFilters, severity: e.target.value || undefined, pageNumber: 1 })
                }
                sx={{ minWidth: 120 }}
              >
                {AUDIT_SEVERITIES.map((s) => (
                  <MenuItem key={s} value={s}>
                    {s || t('system.all')}
                  </MenuItem>
                ))}
              </TextField>
              <TextField
                label={t('system.from')}
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                value={auditFilters.startDate ?? ''}
                onChange={(e) =>
                  setAuditFilters({ ...auditFilters, startDate: e.target.value || undefined, pageNumber: 1 })
                }
              />
              <TextField
                label={t('system.to')}
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
            {auditError && <Alert severity="error">{t('system.auditLoadError')}</Alert>}
            {auditLogs && (
              <>
                <TableContainer>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>{t('system.event')}</TableCell>
                        <TableCell>{t('system.category')}</TableCell>
                        <TableCell>{t('system.severity')}</TableCell>
                        <TableCell>{t('system.user')}</TableCell>
                        <TableCell>{t('system.details')}</TableCell>
                        <TableCell>{t('system.ipAddress')}</TableCell>
                        <TableCell>{t('system.timestamp')}</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {auditLogs.items.map((entry: AdminAuditLogItem) => (
                        <TableRow key={entry.id} hover>
                          <TableCell>
                            <Chip label={entry.eventCode} size="small" variant="outlined" />
                          </TableCell>
                          <TableCell>
                            <Typography variant="caption">{entry.category || '—'}</Typography>
                          </TableCell>
                          <TableCell>
                            {entry.severity && (
                              <Chip
                                label={entry.severity}
                                size="small"
                                color={
                                  entry.severity === 'Critical' ? 'error' :
                                  entry.severity === 'Warning' ? 'warning' : 'default'
                                }
                              />
                            )}
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2">{entry.userEmail || entry.userId}</Typography>
                          </TableCell>
                          <TableCell>
                            <Typography variant="caption" sx={{ maxWidth: 250, display: 'block', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                              {entry.details || entry.referenceId || '—'}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Typography variant="caption" fontFamily="monospace">
                              {entry.ipAddress || '—'}
                            </Typography>
                          </TableCell>
                          <TableCell>{formatDate(entry.created)}</TableCell>
                        </TableRow>
                      ))}
                      {auditLogs.items.length === 0 && (
                        <TableRow>
                          <TableCell colSpan={7} align="center">
                            {t('system.noAuditEntries')}
                          </TableCell>
                        </TableRow>
                      )}
                    </TableBody>
                  </Table>
                </TableContainer>
                <Box display="flex" alignItems="center" justifyContent="space-between" mt={2}>
                  <Typography variant="caption" color="text.secondary">
                    {t('system.totalEntries', { count: auditLogs.totalCount.toLocaleString() })}
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
              <Typography variant="subtitle1">{t('system.webhooksTitle')}</Typography>
            </Box>
            {webhooksLoading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}
            {webhooksError && <Alert severity="error">{t('system.webhookLoadError')}</Alert>}
            {webhooks && (
              <>
                <TableContainer>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>{t('system.url')}</TableCell>
                        <TableCell>{t('system.owner')}</TableCell>
                        <TableCell>{t('system.active')}</TableCell>
                        <TableCell>{t('system.success')}</TableCell>
                        <TableCell>{t('system.failures')}</TableCell>
                        <TableCell>{t('system.lastTriggered')}</TableCell>
                        <TableCell>{t('system.created')}</TableCell>
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
                              label={wh.isActive ? t('system.active') : t('system.inactive')}
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
                            {t('system.noWebhooks')}
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
              <Typography variant="subtitle1">{t('system.settingsTitle')}</Typography>
            </Box>
            {settingsLoading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}
            {settingsError && <Alert severity="error">{t('system.settingsLoadError')}</Alert>}
            {displaySettings && (
              <>
                <Typography variant="subtitle2" color="text.secondary" mb={1}>
                  {t('system.featureToggles')}
                </Typography>
                <Paper variant="outlined" sx={{ p: 2, mb: 2 }}>
                  <Grid container spacing={1}>
                    {(
                      [
                        { key: 'maintenanceMode', label: t('system.maintenanceMode'), description: t('system.maintenanceModeDesc') },
                        { key: 'registrationEnabled', label: t('system.userRegistration'), description: t('system.userRegistrationDesc') },
                        { key: 'guideApplicationsEnabled', label: t('system.guideApplications'), description: t('system.guideApplicationsDesc') },
                        { key: 'tourBookingEnabled', label: t('system.tourBooking'), description: t('system.tourBookingDesc') },
                        { key: 'paymentsEnabled', label: t('system.payments'), description: t('system.paymentsDesc') },
                        { key: 'emailNotificationsEnabled', label: t('system.emailNotifications'), description: t('system.emailNotificationsDesc') },
                      ] as { key: keyof PlatformSettings; label: string; description: string }[]
                    ).map(({ key, label, description }) => (
                      <Grid size={{ xs: 12, sm: 6 }} key={key}>
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
                  {t('system.numericSettings')}
                </Typography>
                <Paper variant="outlined" sx={{ p: 2, mb: 2 }}>
                  <Grid container spacing={2}>
                    <Grid size={{ xs: 12, sm: 4 }}>
                      <TextField
                        label={t('system.platformFee')}
                        type="number"
                        size="small"
                        fullWidth
                        value={displaySettings.platformFeePercentage}
                        inputProps={{ min: 0, max: 100, step: 0.5 }}
                        onChange={(e) => handleSettingNumber('platformFeePercentage', parseFloat(e.target.value) || 0)}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 4 }}>
                      <TextField
                        label={t('system.maxImagesPerPost')}
                        type="number"
                        size="small"
                        fullWidth
                        value={displaySettings.maxImagesPerPost}
                        inputProps={{ min: 1, max: 50 }}
                        onChange={(e) => handleSettingNumber('maxImagesPerPost', parseInt(e.target.value, 10) || 1)}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 4 }}>
                      <TextField
                        label={t('system.minBookingDays')}
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
                    {updateSettingsMutation.isPending ? t('system.saving') : t('system.saveSettings')}
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
