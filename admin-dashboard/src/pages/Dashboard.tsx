import { Box, Grid, Paper, Typography } from '@mui/material';
import {
  People as PeopleIcon,
  PersonAdd as PersonAddIcon,
  Security as SecurityIcon,
  TrendingUp as TrendingIcon,
} from '@mui/icons-material';

const Dashboard = () => {
  // Placeholder for statistics - will be fetched from API
  const stats = [
    {
      title: 'Total Users',
      value: '1,234',
      icon: <PeopleIcon sx={{ fontSize: 40 }} />,
      color: '#1976d2',
    },
    {
      title: 'New This Month',
      value: '87',
      icon: <PersonAddIcon sx={{ fontSize: 40 }} />,
      color: '#388e3c',
    },
    {
      title: 'Active Guides',
      value: '156',
      icon: <SecurityIcon sx={{ fontSize: 40 }} />,
      color: '#f57c00',
    },
    {
      title: 'Growth Rate',
      value: '+12%',
      icon: <TrendingIcon sx={{ fontSize: 40 }} />,
      color: '#7b1fa2',
    },
  ];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Overview of the UrGuide platform
      </Typography>

      <Grid container spacing={3}>
        {stats.map((stat) => (
          <Grid item xs={12} sm={6} md={3} key={stat.title}>
            <Paper
              elevation={2}
              sx={{
                p: 3,
                display: 'flex',
                flexDirection: 'column',
                height: 140,
              }}
            >
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

      <Box sx={{ mt: 4 }}>
        <Paper elevation={2} sx={{ p: 3 }}>
          <Typography variant="h6" gutterBottom>
            Recent Activity
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Activity feed coming soon...
          </Typography>
        </Paper>
      </Box>
    </Box>
  );
};

export default Dashboard;
