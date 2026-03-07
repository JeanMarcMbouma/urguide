import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Grid,
  Paper,
  Typography,
  Button,
  CircularProgress,
  Alert,
} from '@mui/material';
import {
  AttachMoney as AttachMoneyIcon,
  Inbox as InboxIcon,
  Star as StarIcon,
  RateReview as ReviewIcon,
  Person as PersonIcon,
  Explore as ExploreIcon,
  Event as EventIcon,
  BarChart as BarChartIcon,
} from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import type { GuideDashboard } from '../types/guide.types';

const Dashboard = () => {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const [dashboard, setDashboard] = useState<GuideDashboard | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    guideApi
      .getDashboard()
      .then(setDashboard)
      .catch(() => setError(t('dashboard.error')))
      .finally(() => setLoading(false));
  }, [t]);

  const quickActions = [
    { label: t('dashboard.editProfile'), path: '/profile', icon: <PersonIcon /> },
    { label: t('dashboard.viewTourRequests'), path: '/tours', icon: <ExploreIcon /> },
    { label: t('dashboard.manageAvailability'), path: '/availability', icon: <EventIcon /> },
    { label: t('dashboard.viewAnalytics'), path: '/analytics', icon: <BarChartIcon /> },
  ];

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
        <CircularProgress />
        <Typography sx={{ ml: 2 }}>{t('dashboard.loading')}</Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('dashboard.title')}
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        {t('dashboard.subtitle')}
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      <Grid container spacing={3}>
        <Grid item xs={12} sm={6} md={3}>
          <Paper elevation={2} sx={{ p: 3, display: 'flex', flexDirection: 'column', height: 140 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
              <Box>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {t('dashboard.availableBalance')}
                </Typography>
                <Typography variant="h4" fontWeight="bold">
                  {dashboard ? `$${dashboard.availableBalance.toFixed(2)}` : '—'}
                </Typography>
              </Box>
              <Box sx={{ color: '#00796b' }}>
                <AttachMoneyIcon sx={{ fontSize: 40 }} />
              </Box>
            </Box>
          </Paper>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Paper elevation={2} sx={{ p: 3, display: 'flex', flexDirection: 'column', height: 140 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
              <Box>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {t('dashboard.pendingRequests')}
                </Typography>
                <Typography variant="h4" fontWeight="bold">
                  {dashboard ? dashboard.openTourRequests : '—'}
                </Typography>
              </Box>
              <Box sx={{ color: '#1976d2' }}>
                <InboxIcon sx={{ fontSize: 40 }} />
              </Box>
            </Box>
          </Paper>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Paper elevation={2} sx={{ p: 3, display: 'flex', flexDirection: 'column', height: 140 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
              <Box>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {t('dashboard.averageRating')}
                </Typography>
                <Typography variant="h4" fontWeight="bold">
                  {dashboard ? dashboard.averageRating.toFixed(1) : '—'}
                </Typography>
              </Box>
              <Box sx={{ color: '#f57c00' }}>
                <StarIcon sx={{ fontSize: 40 }} />
              </Box>
            </Box>
          </Paper>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Paper elevation={2} sx={{ p: 3, display: 'flex', flexDirection: 'column', height: 140 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
              <Box>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {t('dashboard.reviewCount')}
                </Typography>
                <Typography variant="h4" fontWeight="bold">
                  {dashboard ? dashboard.reviewCount : '—'}
                </Typography>
              </Box>
              <Box sx={{ color: '#388e3c' }}>
                <ReviewIcon sx={{ fontSize: 40 }} />
              </Box>
            </Box>
          </Paper>
        </Grid>
      </Grid>

      <Grid container spacing={3} sx={{ mt: 1 }}>
        <Grid item xs={12} md={6}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              {t('dashboard.quickActions')}
            </Typography>
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mt: 1 }}>
              {quickActions.map((action) => (
                <Button
                  key={action.label}
                  variant="outlined"
                  startIcon={action.icon}
                  onClick={() => navigate(action.path)}
                  size="small"
                >
                  {action.label}
                </Button>
              ))}
            </Box>
          </Paper>
        </Grid>
        <Grid item xs={12} md={6}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              {t('dashboard.recentActivity')}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {t('dashboard.activityComingSoon')}
            </Typography>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Dashboard;
