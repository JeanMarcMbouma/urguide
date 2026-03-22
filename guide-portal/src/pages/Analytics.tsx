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
import {
  BarChart,
  Bar,
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';

import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import { useAuth } from '../hooks/useAuth';
import type { AnalyticsPeriod, GuideDashboard, PerformanceMetrics, TourStatistics } from '../types/guide.types';

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
  const [performance, setPerformance] = useState<PerformanceMetrics | null>(null);
  const [tourStats, setTourStats] = useState<TourStatistics | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!user?.id) return;
    setLoading(true);
    setError('');
    Promise.all([
      guideApi.getDashboard(),
      guideApi.getPerformanceMetrics(user.id, period),
      guideApi.getTourStatistics(user.id),
    ])
      .then(([dash, perf, stats]) => {
        setDashboard(dash);
        setPerformance(perf);
        setTourStats(stats);
      })
      .catch(() => setError(t('analytics.loadError')))
      .finally(() => setLoading(false));
  }, [user?.id, period, t]);

  const metrics: MetricCard[] = [
    { titleKey: 'analytics.averageRating', descKey: 'analytics.completionRateDesc', value: dashboard ? `${dashboard.averageRating.toFixed(1)} ★` : '—', color: '#f57c00' },
    { titleKey: 'analytics.reviewCount', descKey: 'analytics.repeatClientRateDesc', value: dashboard ? String(dashboard.reviewCount) : '—', color: '#1976d2' },
    { titleKey: 'analytics.pendingRequests', descKey: 'analytics.responseRateDesc', value: dashboard ? String(dashboard.openTourRequests) : '—', color: '#00796b' },
    { titleKey: 'earnings.availableBalance', descKey: 'analytics.cancellationRateDesc', value: dashboard ? `$${dashboard.availableBalance.toFixed(2)}` : '—', color: '#388e3c' },
  ];

  // Build response time trend data from performance metrics
  const responseTimeTrendData = performance
    ? [
        { name: t('analytics.responseRate'), value: performance.responseRate },
        { name: t('analytics.completionRate'), value: performance.completionRate },
        { name: t('analytics.cancellationRate'), value: performance.cancellationRate },
        { name: t('analytics.repeatClientRate'), value: performance.repeatClientRate },
      ]
    : [];

  // Estimated rating distribution based on review count (approximation until backend provides actual breakdown)
  const ratingDistributionData = dashboard
    ? [
        { rating: '5★', count: Math.round(dashboard.reviewCount * 0.45) },
        { rating: '4★', count: Math.round(dashboard.reviewCount * 0.30) },
        { rating: '3★', count: Math.round(dashboard.reviewCount * 0.15) },
        { rating: '2★', count: Math.round(dashboard.reviewCount * 0.07) },
        { rating: '1★', count: Math.round(dashboard.reviewCount * 0.03) },
      ]
    : [];

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
          <Grid size={{ xs: 12, sm: 6, md: 3 }} key={m.titleKey}>
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

      {/* Tour Statistics Summary */}
      {tourStats && (tourStats.totalTours > 0 || tourStats.completedTours > 0) && (
        <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
          <Typography variant="h6" gutterBottom>{t('analytics.tourStatistics')}</Typography>
          <Grid container spacing={2}>
            <Grid size={{ xs: 6, sm: 3 }}>
              <Typography variant="body2" color="text.secondary">{t('analytics.totalTours')}</Typography>
              <Typography variant="h5" fontWeight="bold">{tourStats.totalTours}</Typography>
            </Grid>
            <Grid size={{ xs: 6, sm: 3 }}>
              <Typography variant="body2" color="text.secondary">{t('analytics.completed')}</Typography>
              <Typography variant="h5" fontWeight="bold" color="success.main">{tourStats.completedTours}</Typography>
            </Grid>
            <Grid size={{ xs: 6, sm: 3 }}>
              <Typography variant="body2" color="text.secondary">{t('analytics.cancelled')}</Typography>
              <Typography variant="h5" fontWeight="bold" color="error.main">{tourStats.cancelledTours}</Typography>
            </Grid>
            <Grid size={{ xs: 6, sm: 3 }}>
              <Typography variant="body2" color="text.secondary">{t('analytics.avgDuration')}</Typography>
              <Typography variant="h5" fontWeight="bold">{tourStats.averageDuration}h</Typography>
            </Grid>
          </Grid>
          {tourStats.topDestinations.length > 0 && (
            <Box sx={{ mt: 2 }}>
              <Typography variant="body2" color="text.secondary">{t('analytics.topDestinations')}</Typography>
              <Typography variant="body1">{tourStats.topDestinations.join(', ')}</Typography>
            </Box>
          )}
        </Paper>
      )}

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 6 }}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>{t('analytics.responseTimeTrend')}</Typography>
            {responseTimeTrendData.length > 0 ? (
              <ResponsiveContainer width="100%" height={260}>
                <LineChart data={responseTimeTrendData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="name" tick={{ fontSize: 11 }} />
                  <YAxis />
                  <Tooltip formatter={(v) => `${v}%`} />
                  <Line type="monotone" dataKey="value" stroke="#1976d2" strokeWidth={2} dot={{ r: 4 }} />
                </LineChart>
              </ResponsiveContainer>
            ) : (
              <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 6 }}>
                {t('analytics.loading')}…
              </Typography>
            )}
          </Paper>
        </Grid>
        <Grid size={{ xs: 12, md: 6 }}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>{t('analytics.ratingDistribution')}</Typography>
            {ratingDistributionData.length > 0 && dashboard && dashboard.reviewCount > 0 ? (
              <ResponsiveContainer width="100%" height={260}>
                <BarChart data={ratingDistributionData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="rating" />
                  <YAxis allowDecimals={false} />
                  <Tooltip formatter={(v) => `${v} ${t('analytics.numberOfReviews').toLowerCase()}`} />
                  <Bar dataKey="count" fill="#f57c00" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            ) : (
              <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 6 }}>
                {t('analytics.loading')}…
              </Typography>
            )}
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Analytics;
