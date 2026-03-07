import { useNavigate } from 'react-router-dom';
import { Box, Grid, Paper, Typography, Button } from '@mui/material';
import {
  AttachMoney as AttachMoneyIcon,
  Inbox as InboxIcon,
  Star as StarIcon,
  CheckCircle as CheckCircleIcon,
  Person as PersonIcon,
  Explore as ExploreIcon,
  Event as EventIcon,
  BarChart as BarChartIcon,
} from '@mui/icons-material';

const Dashboard = () => {
  const navigate = useNavigate();

  const stats = [
    {
      title: 'Total Earnings',
      value: '$4,280',
      icon: <AttachMoneyIcon sx={{ fontSize: 40 }} />,
      color: '#00796b',
    },
    {
      title: 'Pending Requests',
      value: '8',
      icon: <InboxIcon sx={{ fontSize: 40 }} />,
      color: '#1976d2',
    },
    {
      title: 'Average Rating',
      value: '4.7',
      icon: <StarIcon sx={{ fontSize: 40 }} />,
      color: '#f57c00',
    },
    {
      title: 'Completed Tours',
      value: '42',
      icon: <CheckCircleIcon sx={{ fontSize: 40 }} />,
      color: '#388e3c',
    },
  ];

  const quickActions = [
    { label: 'Edit Profile', path: '/profile', icon: <PersonIcon /> },
    { label: 'View Tour Requests', path: '/tours', icon: <ExploreIcon /> },
    { label: 'Manage Availability', path: '/availability', icon: <EventIcon /> },
    { label: 'View Analytics', path: '/analytics', icon: <BarChartIcon /> },
  ];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Welcome back! Here's an overview of your guide activity.
      </Typography>

      <Grid container spacing={3}>
        {stats.map((stat) => (
          <Grid item xs={12} sm={6} md={3} key={stat.title}>
            <Paper elevation={2} sx={{ p: 3, display: 'flex', flexDirection: 'column', height: 140 }}>
              <Box
                sx={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'flex-start',
                  mb: 2,
                }}
              >
                <Box>
                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    {stat.title}
                  </Typography>
                  <Typography variant="h4" fontWeight="bold">
                    {stat.value}
                  </Typography>
                </Box>
                <Box sx={{ color: stat.color }}>{stat.icon}</Box>
              </Box>
            </Paper>
          </Grid>
        ))}
      </Grid>

      <Grid container spacing={3} sx={{ mt: 1 }}>
        <Grid item xs={12} md={6}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Quick Actions
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
              Recent Activity
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Activity feed coming soon...
            </Typography>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Dashboard;
