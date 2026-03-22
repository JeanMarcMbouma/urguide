import { useState, useEffect } from 'react';
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
  CircularProgress,
  Alert,
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
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import type { TransactionItem, GuideDashboard, AnalyticsPeriod } from '../types/guide.types';

const txStatusColors: Record<string, 'success' | 'warning' | 'error' | 'default'> = {
  completed: 'success',
  processed: 'success',
  pending: 'warning',
  failed: 'error',
};

const Earnings = () => {
  const { t } = useTranslation();
  const [period, setPeriod] = useState<AnalyticsPeriod>('month');
  const [page, setPage] = useState(1);
  const [transactions, setTransactions] = useState<TransactionItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [dashboard, setDashboard] = useState<GuideDashboard | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      setError('');
      try {
        const [dash, txResp] = await Promise.all([
          guideApi.getDashboard(),
          guideApi.getTransactions(page),
        ]);
        setDashboard(dash);
        setTransactions(txResp.transactions ?? []);
        setTotalCount(txResp.totalCount ?? 0);
      } catch {
        setError(t('earnings.loadError'));
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [page, t]);

  // Build a simple trend from transactions grouped by month
  const trendData = (() => {
    const map = new Map<string, number>();
    transactions.forEach((tx) => {
      const date = tx.createdAt ?? tx.date ?? '';
      const month = date.substring(0, 7); // YYYY-MM
      if (month) map.set(month, (map.get(month) ?? 0) + (tx.amount > 0 ? tx.amount : 0));
    });
    return Array.from(map.entries())
      .sort(([a], [b]) => a.localeCompare(b))
      .map(([date, amount]) => ({ date, amount }));
  })();

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
        <CircularProgress />
        <Typography sx={{ ml: 2 }}>{t('earnings.loading')}</Typography>
      </Box>
    );
  }

  const summaryCards = [
    { titleKey: 'earnings.totalEarnings', value: dashboard ? `$${dashboard.availableBalance.toFixed(2)}` : '—', color: '#00796b' },
    { titleKey: 'earnings.availableBalance', value: dashboard ? `$${dashboard.availableBalance.toFixed(2)}` : '—', color: '#388e3c' },
  ];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('earnings.title')}
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        {t('earnings.subtitle')}
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Grid container spacing={3} sx={{ mb: 3 }}>
        {summaryCards.map((card) => (
          <Grid size={{ xs: 12, sm: 6, md: 3 }} key={card.titleKey}>
            <Paper elevation={2} sx={{ p: 3 }}>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                {t(card.titleKey)}
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
          <Typography variant="h6">{t('earnings.earningsTrend')}</Typography>
          <FormControl size="small" sx={{ minWidth: 120 }}>
            <InputLabel>{t('earnings.period')}</InputLabel>
            <Select
              value={period}
              label={t('earnings.period')}
              onChange={(e) => setPeriod(e.target.value as AnalyticsPeriod)}
            >
              <MenuItem value="week">{t('earnings.week')}</MenuItem>
              <MenuItem value="month">{t('earnings.month')}</MenuItem>
              <MenuItem value="year">{t('earnings.year')}</MenuItem>
            </Select>
          </FormControl>
        </Box>
        {trendData.length > 0 ? (
          <ResponsiveContainer width="100%" height={260}>
            <AreaChart data={trendData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="date" />
              <YAxis />
              <Tooltip formatter={(v) => `$${v}`} />
              <Area type="monotone" dataKey="amount" stroke="#00796b" fill="#b2dfdb" name={t('earnings.earnings')} />
            </AreaChart>
          </ResponsiveContainer>
        ) : (
          <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 4 }}>
            {t('earnings.noTransactions')}
          </Typography>
        )}
      </Paper>

      {/* Transaction History */}
      <Paper elevation={2} sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          {t('earnings.transactionHistory')}
        </Typography>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>{t('earnings.description')}</TableCell>
                <TableCell>{t('earnings.type')}</TableCell>
                <TableCell align="right">{t('earnings.amount')}</TableCell>
                <TableCell>{t('earnings.status')}</TableCell>
                <TableCell>{t('earnings.date')}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {transactions.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={5} align="center">
                    <Typography color="text.secondary">{t('earnings.noTransactions')}</Typography>
                  </TableCell>
                </TableRow>
              ) : (
                transactions.map((tx, idx) => {
                  const txId = tx.transactionId ?? tx.id ?? String(idx);
                  const date = tx.createdAt ?? tx.date ?? '';
                  const currency = tx.currencyCode ?? tx.currency ?? 'USD';
                  return (
                    <TableRow key={txId}>
                      <TableCell>{tx.description}</TableCell>
                      <TableCell>
                        <Chip label={tx.type} size="small" variant="outlined" />
                      </TableCell>
                      <TableCell
                        align="right"
                        sx={{ color: tx.amount < 0 ? 'error.main' : 'success.main', fontWeight: 'bold' }}
                      >
                        {tx.amount < 0 ? `-${currency} ${Math.abs(tx.amount)}` : `+${currency} ${tx.amount}`}
                      </TableCell>
                      <TableCell>
                        <Chip label={tx.status} color={txStatusColors[tx.status] ?? 'default'} size="small" />
                      </TableCell>
                      <TableCell>{date ? new Date(date).toLocaleDateString() : '—'}</TableCell>
                    </TableRow>
                  );
                })
              )}
            </TableBody>
          </Table>
        </TableContainer>
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
          <Pagination
            count={Math.ceil(totalCount / 10) || 1}
            page={page}
            onChange={(_, v) => setPage(v)}
            color="primary"
          />
        </Box>
      </Paper>
    </Box>
  );
};

export default Earnings;
