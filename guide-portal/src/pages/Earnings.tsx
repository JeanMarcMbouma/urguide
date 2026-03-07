import { useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Pagination,
} from '@mui/material';
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';
import type { AnalyticsPeriod } from '../types/guide.types';

const earningsTrendData = [
  { date: 'Jan', amount: 320, tourCount: 3 },
  { date: 'Feb', amount: 480, tourCount: 4 },
  { date: 'Mar', amount: 650, tourCount: 6 },
  { date: 'Apr', amount: 420, tourCount: 4 },
  { date: 'May', amount: 780, tourCount: 7 },
  { date: 'Jun', amount: 540, tourCount: 5 },
];

const monthlyData = [
  { month: 'January', tours: 3, earnings: 320, refunds: 0 },
  { month: 'February', tours: 4, earnings: 480, refunds: 50 },
  { month: 'March', tours: 6, earnings: 650, refunds: 0 },
  { month: 'April', tours: 4, earnings: 420, refunds: 80 },
  { month: 'May', tours: 7, earnings: 780, refunds: 0 },
  { month: 'June', tours: 5, earnings: 540, refunds: 30 },
];

const transactionData = [
  { id: 't1', type: 'earning', description: 'Cultural Tour – Rome', amount: 280, currency: 'USD', status: 'completed', date: '2024-02-15' },
  { id: 't2', type: 'earning', description: 'Food Tour – Tuscany', amount: 450, currency: 'USD', status: 'completed', date: '2024-02-22' },
  { id: 't3', type: 'refund', description: 'Refund – Cancelled Tour', amount: -80, currency: 'USD', status: 'processed', date: '2024-03-01' },
  { id: 't4', type: 'payout', description: 'Payout to Bank Account', amount: -500, currency: 'USD', status: 'completed', date: '2024-03-05' },
];

const txStatusColors: Record<string, 'success' | 'warning' | 'error' | 'default'> = {
  completed: 'success',
  processed: 'success',
  pending: 'warning',
  failed: 'error',
};

const summaryCards = [
  { title: 'Total Earnings', value: '$4,280', color: '#00796b' },
  { title: 'This Month', value: '$540', color: '#1976d2' },
  { title: 'Available Balance', value: '$1,230', color: '#388e3c' },
  { title: 'Pending', value: '$180', color: '#f57c00' },
];

const Earnings = () => {
  const [period, setPeriod] = useState<AnalyticsPeriod>('month');
  const [page, setPage] = useState(1);

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Earnings Dashboard
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Track your earnings, transactions, and financial performance.
      </Typography>

      {/* Summary Cards */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        {summaryCards.map((card) => (
          <Grid item xs={12} sm={6} md={3} key={card.title}>
            <Paper elevation={2} sx={{ p: 3 }}>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                {card.title}
              </Typography>
              <Typography variant="h4" fontWeight="bold" sx={{ color: card.color }}>
                {card.value}
              </Typography>
            </Paper>
          </Grid>
        ))}
      </Grid>

      {/* Earnings Trend Chart */}
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6">Earnings Trend</Typography>
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
        <ResponsiveContainer width="100%" height={260}>
          <AreaChart data={earningsTrendData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="date" />
            <YAxis />
            <Tooltip formatter={(v) => `$${v}`} />
            <Area
              type="monotone"
              dataKey="amount"
              stroke="#00796b"
              fill="#b2dfdb"
              name="Earnings ($)"
            />
          </AreaChart>
        </ResponsiveContainer>
      </Paper>

      {/* Monthly Breakdown */}
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Monthly Breakdown
        </Typography>
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Month</TableCell>
                <TableCell align="right">Tours</TableCell>
                <TableCell align="right">Earnings</TableCell>
                <TableCell align="right">Refunds</TableCell>
                <TableCell align="right">Net</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {monthlyData.map((row) => (
                <TableRow key={row.month}>
                  <TableCell>{row.month}</TableCell>
                  <TableCell align="right">{row.tours}</TableCell>
                  <TableCell align="right">${row.earnings}</TableCell>
                  <TableCell align="right" sx={{ color: 'error.main' }}>
                    {row.refunds > 0 ? `-$${row.refunds}` : '—'}
                  </TableCell>
                  <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                    ${row.earnings - row.refunds}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {/* Transaction History */}
      <Paper elevation={2} sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Transaction History
        </Typography>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Description</TableCell>
                <TableCell>Type</TableCell>
                <TableCell align="right">Amount</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Date</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {transactionData.map((tx) => (
                <TableRow key={tx.id}>
                  <TableCell>{tx.description}</TableCell>
                  <TableCell>
                    <Chip label={tx.type} size="small" variant="outlined" />
                  </TableCell>
                  <TableCell
                    align="right"
                    sx={{ color: tx.amount < 0 ? 'error.main' : 'success.main', fontWeight: 'bold' }}
                  >
                    {tx.amount < 0 ? `-$${Math.abs(tx.amount)}` : `+$${tx.amount}`}
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={tx.status}
                      color={txStatusColors[tx.status] ?? 'default'}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>{new Date(tx.date).toLocaleDateString()}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
          <Pagination count={5} page={page} onChange={(_, v) => setPage(v)} color="primary" />
        </Box>
      </Paper>
    </Box>
  );
};

export default Earnings;
