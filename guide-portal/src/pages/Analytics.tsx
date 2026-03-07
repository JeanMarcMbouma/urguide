import { useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Chip,
} from '@mui/material';
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from 'recharts';
import type { AnalyticsPeriod } from '../types/guide.types';

const responseTimeTrend = [
  { date: 'Jan', avgHours: 3.2 },
  { date: 'Feb', avgHours: 2.8 },
  { date: 'Mar', avgHours: 1.9 },
  { date: 'Apr', avgHours: 2.4 },
  { date: 'May', avgHours: 1.5 },
  { date: 'Jun', avgHours: 1.2 },
];

const ratingDistributionData = [
  { rating: '1★', count: 0 },
  { rating: '2★', count: 1 },
  { rating: '3★', count: 3 },
  { rating: '4★', count: 10 },
  { rating: '5★', count: 28 },
];

const performanceMetrics = [
  { label: 'Response Rate', value: '97%', description: 'Percentage of requests responded to' },
  { label: 'Avg Response Time', value: '1.2h', description: 'Average time to first response' },
  { label: 'Completion Rate', value: '94%', description: 'Tours completed as scheduled' },
  { label: 'Cancellation Rate', value: '4%', description: 'Tours cancelled by guide' },
  { label: 'Repeat Client Rate', value: '22%', description: 'Clients who booked more than once' },
];

const TOP_DESTINATIONS = ['Rome, Italy', 'Tuscany, Italy', 'Florence, Italy', 'Amalfi Coast'];

const Analytics = () => {
  const [period, setPeriod] = useState<AnalyticsPeriod>('month');

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            Analytics
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Performance metrics and insights about your guide activity.
          </Typography>
        </Box>
        <FormControl size="small" sx={{ minWidth: 120 }}>
          <InputLabel>Period</InputLabel>
          <Select
            value={period}
            label="Period"
            onChange={(e) => setPeriod(e.target.value as AnalyticsPeriod)}
          >
            <MenuItem value="week">Week</MenuItem>
            <MenuItem value="month">Month</MenuItem>
            <MenuItem value="year">Year</MenuItem>
          </Select>
        </FormControl>
      </Box>

      {/* Performance Metrics Cards */}
      <Grid container spacing={2} sx={{ mb: 3 }}>
        {performanceMetrics.map((metric) => (
          <Grid item xs={12} sm={6} md={4} lg={2.4} key={metric.label}>
            <Paper elevation={2} sx={{ p: 2, textAlign: 'center' }}>
              <Typography variant="h4" fontWeight="bold" color="primary.main">
                {metric.value}
              </Typography>
              <Typography variant="body2" fontWeight="bold" gutterBottom>
                {metric.label}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                {metric.description}
              </Typography>
            </Paper>
          </Grid>
        ))}
      </Grid>

      <Grid container spacing={3}>
        {/* Tour Statistics */}
        <Grid item xs={12} md={5}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Tour Statistics
            </Typography>
            <Grid container spacing={2} sx={{ mb: 2 }}>
              {[
                { label: 'Total Tours', value: 42 },
                { label: 'Completed', value: 40 },
                { label: 'Cancelled', value: 2 },
                { label: 'Avg Duration', value: '6.5h' },
              ].map((stat) => (
                <Grid item xs={6} key={stat.label}>
                  <Box sx={{ p: 1.5, bgcolor: 'grey.50', borderRadius: 1, textAlign: 'center' }}>
                    <Typography variant="h5" fontWeight="bold">
                      {stat.value}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      {stat.label}
                    </Typography>
                  </Box>
                </Grid>
              ))}
            </Grid>
            <Typography variant="body2" fontWeight="bold" gutterBottom>
              Top Destinations
            </Typography>
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
              {TOP_DESTINATIONS.map((dest) => (
                <Chip key={dest} label={dest} size="small" variant="outlined" color="primary" />
              ))}
            </Box>
          </Paper>
        </Grid>

        {/* Response Time Trend */}
        <Grid item xs={12} md={7}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Average Response Time Trend (hours)
            </Typography>
            <ResponsiveContainer width="100%" height={220}>
              <LineChart data={responseTimeTrend}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis unit="h" />
                <Tooltip formatter={(v) => `${v}h`} />
                <Line
                  type="monotone"
                  dataKey="avgHours"
                  stroke="#00796b"
                  strokeWidth={2}
                  dot={{ r: 4 }}
                  name="Avg Response Time"
                />
              </LineChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        {/* Client Feedback Distribution */}
        <Grid item xs={12}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Client Feedback – Rating Distribution
            </Typography>
            <ResponsiveContainer width="100%" height={220}>
              <BarChart data={ratingDistributionData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="rating" />
                <YAxis allowDecimals={false} />
                <Tooltip />
                <Legend />
                <Bar dataKey="count" fill="#00796b" name="Number of Reviews" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Analytics;
