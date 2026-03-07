import { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  CircularProgress,
  Alert,
} from '@mui/material';

import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import { useAuth } from '../hooks/useAuth';
import type { AnalyticsPeriod, GuideDashboard } from '../types/guide.types';

interface MetricCard {
  titleKey: string;
  descKey: string;
  value: string;
  color: string;
}

const Analytics = () => {
  const { t } = useTranslation();
  const { user } = useAuth();
  const [period, setPeriod] = useState<AnalyticsPeriod>('month');
  const [dashboard, setDashboard] = useState<GuideDashboard | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!user?.id) return;
    setLoading(true);
    setError('');
    Promise.all([
      guideApi.getDashboard(),
      guideApi.getPerformanceMetrics(user.id, period),
    ])
      .then(([dash]) => setDashboard(dash))
      .catch(() => setError(t('analytics.loadError')))
      .finally(() => setLoading(false));
  }, [user?.id, period, t]);

  const metrics: MetricCard[] = [
    { titleKey: 'analytics.averageRating', descKey: 'analytics.completionRateDesc', value: dashboard ? `${dashboard.averageRating.toFixed(1)} ★` : '—', color: '#f57c00' },
    { titleKey: 'analytics.reviewCount', descKey: 'analytics.repeatClientRateDesc', value: dashboard ? String(dashboard.reviewCount) : '—', color: '#1976d2' },
    { titleKey: 'analytics.pendingRequests', descKey: 'analytics.responseRateDesc', value: dashboard ? String(dashboard.openTourRequests) : '—', color: '#00796b' },
    { titleKey: 'earnings.availableBalance', descKey: 'analytics.cancellationRateDesc', value: dashboard ? `$${dashboard.availableBalance.toFixed(2)}` : '—', color: '#388e3c' },
  ];

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
        <CircularProgress />
        <Typography sx={{ ml: 2 }}>{t('analytics.loading')}</Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Box>
          <Typography variant="h4">{t('analytics.title')}</Typography>
          <Typography variant="body1" color="text.secondary">{t('analytics.subtitle')}</Typography>
        </Box>
        <FormControl size="small" sx={{ minWidth: 120 }}>
          <InputLabel>{t('analytics.period')}</InputLabel>
          <Select
            value={period}
            label={t('analytics.period')}
            onChange={(e) => setPeriod(e.target.value as AnalyticsPeriod)}
          >
            <MenuItem value="week">{t('analytics.week')}</MenuItem>
            <MenuItem value="month">{t('analytics.month')}</MenuItem>
            <MenuItem value="year">{t('analytics.year')}</MenuItem>
          </Select>
        </FormControl>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Grid container spacing={3} sx={{ mb: 3 }}>
        {metrics.map((m) => (
          <Grid item xs={12} sm={6} md={3} key={m.titleKey}>
            <Paper elevation={2} sx={{ p: 3 }}>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                {t(m.titleKey as Parameters<typeof t>[0])}
              </Typography>
              <Typography variant="h4" fontWeight="bold" sx={{ color: m.color }}>
                {m.value}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                {t(m.descKey as Parameters<typeof t>[0])}
              </Typography>
            </Paper>
          </Grid>
        ))}
      </Grid>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>{t('analytics.responseTimeTrend')}</Typography>
            <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 6 }}>
              {t('analytics.loading')}…
            </Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} md={6}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>{t('analytics.ratingDistribution')}</Typography>
            <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 6 }}>
              {t('analytics.loading')}…
            </Typography>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Analytics;
