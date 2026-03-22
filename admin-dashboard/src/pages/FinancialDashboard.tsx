import { useState } from 'react';
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
  Button,
  Pagination,
  Stack,
} from '@mui/material';
import {
  AttachMoney as MoneyIcon,
  AccountBalance as FeeIcon,
  SwapHoriz as PayoutIcon,
  Replay as RefundIcon,
  Download as DownloadIcon,
  TrendingUp as TrendingUpIcon,
} from '@mui/icons-material';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from 'recharts';
import { useQuery } from '@tanstack/react-query';
import { adminApi } from '../services/adminApi';
import type {
  AdminTransactionItem,
  AdminPayoutItem,
  AdminRefundItem,
  FinancialFilterParameters,
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

const STATUS_COLORS: Record<string, 'default' | 'primary' | 'success' | 'error' | 'warning' | 'info'> = {
  Succeeded: 'success',
  Paid: 'success',
  Pending: 'warning',
  Processing: 'info',
  Failed: 'error',
  Cancelled: 'default',
  Refunded: 'info',
  PartiallyRefunded: 'warning',
};

const getStatusColor = (status: string) => STATUS_COLORS[status] ?? 'default';

const formatCurrency = (amount: number, currency = 'USD') =>
  new Intl.NumberFormat('en-US', { style: 'currency', currency }).format(amount);

const formatDate = (dateStr: string) =>
  new Date(dateStr).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });

const MetricCard = ({
  title,
  value,
  icon,
  color,
}: {
  title: string;
  value: string;
  icon: React.ReactNode;
  color: string;
}) => (
  <Paper sx={{ p: 2 }}>
    <Box display="flex" alignItems="center" gap={1} mb={1}>
      <Box sx={{ color }}>{icon}</Box>
      <Typography variant="body2" color="text.secondary">
        {title}
      </Typography>
    </Box>
    <Typography variant="h5" fontWeight="bold">
      {value}
    </Typography>
  </Paper>
);

const PAYMENT_STATUSES = ['', 'Pending', 'Processing', 'Succeeded', 'Failed', 'Cancelled', 'Refunded'];
const PAYOUT_STATUSES = ['', 'Pending', 'Processing', 'Paid', 'Failed', 'Cancelled'];
const REFUND_STATUSES = ['', 'Pending', 'Processing', 'Succeeded', 'Failed', 'Cancelled'];

export default function FinancialDashboard() {
  const [tab, setTab] = useState(0);
  const [transactionFilters, setTransactionFilters] = useState<FinancialFilterParameters>({ pageNumber: 1 });
  const [payoutFilters, setPayoutFilters] = useState<FinancialFilterParameters>({ pageNumber: 1 });
  const [refundFilters, setRefundFilters] = useState<FinancialFilterParameters>({ pageNumber: 1 });
  const [exportLoading, setExportLoading] = useState(false);

  // Revenue metrics
  const {
    data: revenue,
    isLoading: revenueLoading,
    error: revenueError,
  } = useQuery({
    queryKey: ['revenueMetrics'],
    queryFn: () => adminApi.getRevenueMetrics(),
    retry: 1,
  });

  // Transactions
  const {
    data: transactions,
    isLoading: txLoading,
    error: txError,
  } = useQuery({
    queryKey: ['adminTransactions', transactionFilters],
    queryFn: () => adminApi.getTransactions(transactionFilters),
    enabled: tab === 1,
    retry: 1,
  });

  // Payouts
  const {
    data: payouts,
    isLoading: payoutsLoading,
    error: payoutsError,
  } = useQuery({
    queryKey: ['adminPayouts', payoutFilters],
    queryFn: () => adminApi.getPayouts(payoutFilters),
    enabled: tab === 2,
    retry: 1,
  });

  // Refunds
  const {
    data: refunds,
    isLoading: refundsLoading,
    error: refundsError,
  } = useQuery({
    queryKey: ['adminRefunds', refundFilters],
    queryFn: () => adminApi.getRefunds(refundFilters),
    enabled: tab === 3,
    retry: 1,
  });

  const handleExport = async (format: 'csv' | 'json') => {
    setExportLoading(true);
    try {
      const blob = await adminApi.exportAnalyticsData(format);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `financial-report-${new Date().toISOString().slice(0, 10)}.${format}`;
      a.click();
      // Defer revoking so the browser has time to initiate the download
      setTimeout(() => window.URL.revokeObjectURL(url), 100);
    } catch {
      // Export unavailable — handled gracefully
    } finally {
      setExportLoading(false);
    }
  };

  const trendData = revenue?.trendData?.map((d) => ({
    date: formatDate(d.date),
    Revenue: Number(d.amount.toFixed(2)),
    'Platform Fees': Number(d.platformFees.toFixed(2)),
  })) ?? [];

  return (
    <Box>
      <Box display="flex" alignItems="center" justifyContent="space-between" mb={3}>
        <Box>
          <Typography variant="h5" fontWeight="bold">
            Financial Monitoring
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Revenue analytics, transactions, payouts and refunds
          </Typography>
        </Box>
        <Stack direction="row" spacing={1}>
          <Button
            variant="outlined"
            startIcon={<DownloadIcon />}
            disabled={exportLoading}
            onClick={() => handleExport('csv')}
          >
            Export CSV
          </Button>
          <Button
            variant="outlined"
            startIcon={<DownloadIcon />}
            disabled={exportLoading}
            onClick={() => handleExport('json')}
          >
            Export JSON
          </Button>
        </Stack>
      </Box>

      {/* Revenue metrics cards */}
      {revenueLoading && (
        <Box display="flex" justifyContent="center" py={4}>
          <CircularProgress />
        </Box>
      )}
      {revenueError && (
        <Alert severity="info" sx={{ mb: 2 }}>
          Revenue metrics are unavailable at this time.
        </Alert>
      )}
      {revenue && (
        <>
          <Grid container spacing={2} mb={3}>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <MetricCard
                title="Total Revenue"
                value={formatCurrency(revenue.totalRevenue)}
                icon={<MoneyIcon />}
                color="success.main"
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <MetricCard
                title="Platform Fees"
                value={formatCurrency(revenue.platformFees)}
                icon={<FeeIcon />}
                color="primary.main"
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <MetricCard
                title="Guide Payouts"
                value={formatCurrency(revenue.guidePayout)}
                icon={<PayoutIcon />}
                color="info.main"
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <MetricCard
                title="Refunded Amount"
                value={formatCurrency(revenue.refundedAmount)}
                icon={<RefundIcon />}
                color="warning.main"
              />
            </Grid>
          </Grid>

          {/* Revenue trend chart */}
          {trendData.length > 0 && (
            <Paper sx={{ p: 2, mb: 3 }}>
              <Box display="flex" alignItems="center" gap={1} mb={2}>
                <TrendingUpIcon color="primary" />
                <Typography variant="h6">Revenue Trend</Typography>
              </Box>
              <ResponsiveContainer width="100%" height={260}>
                <LineChart data={trendData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="date" tick={{ fontSize: 11 }} />
                  <YAxis tickFormatter={(v) => `$${v}`} width={70} />
                  <Tooltip
                    formatter={(value) => {
                      const numericValue = typeof value === 'number' ? value : Number(value);
                      const safeValue = Number.isFinite(numericValue) ? numericValue : 0;
                      return formatCurrency(safeValue);
                    }}
                  />
                  <Legend />
                  <Line type="monotone" dataKey="Revenue" stroke="#1976d2" strokeWidth={2} dot={false} />
                  <Line type="monotone" dataKey="Platform Fees" stroke="#2e7d32" strokeWidth={2} dot={false} />
                </LineChart>
              </ResponsiveContainer>
            </Paper>
          )}

          {/* Additional metrics */}
          <Grid container spacing={2} mb={3}>
            <Grid size={{ xs: 12, sm: 4 }}>
              <Paper sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="body2" color="text.secondary">
                  Net Revenue
                </Typography>
                <Typography variant="h6" fontWeight="bold" color="success.main">
                  {formatCurrency(revenue.netRevenue)}
                </Typography>
              </Paper>
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <Paper sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="body2" color="text.secondary">
                  Transactions
                </Typography>
                <Typography variant="h6" fontWeight="bold">
                  {revenue.transactionCount.toLocaleString()}
                </Typography>
              </Paper>
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <Paper sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="body2" color="text.secondary">
                  Avg. Transaction Value
                </Typography>
                <Typography variant="h6" fontWeight="bold">
                  {formatCurrency(revenue.averageTransactionValue)}
                </Typography>
              </Paper>
            </Grid>
          </Grid>
        </>
      )}

      {/* Tabs for detailed tables */}
      <Paper>
        <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tab label="Overview" />
          <Tab label="Transactions" />
          <Tab label="Payouts" />
          <Tab label="Refunds" />
        </Tabs>

        {/* Transactions tab */}
        <TabPanel value={tab} index={1}>
          <Box px={2} pb={2}>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} mb={2}>
              <TextField
                select
                label="Status"
                size="small"
                value={transactionFilters.status ?? ''}
                onChange={(e) =>
                  setTransactionFilters({ ...transactionFilters, status: e.target.value || undefined, pageNumber: 1 })
                }
                sx={{ minWidth: 140 }}
              >
                {PAYMENT_STATUSES.map((s) => (
                  <MenuItem key={s} value={s}>
                    {s || 'All Statuses'}
                  </MenuItem>
                ))}
              </TextField>
              <TextField
                label="From"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                value={transactionFilters.startDate ?? ''}
                onChange={(e) =>
                  setTransactionFilters({ ...transactionFilters, startDate: e.target.value || undefined, pageNumber: 1 })
                }
              />
              <TextField
                label="To"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                value={transactionFilters.endDate ?? ''}
                onChange={(e) =>
                  setTransactionFilters({ ...transactionFilters, endDate: e.target.value || undefined, pageNumber: 1 })
                }
              />
            </Stack>
            {txLoading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}
            {txError && <Alert severity="error">Failed to load transactions.</Alert>}
            {transactions && (
              <>
                <TableContainer>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>Payment ID</TableCell>
                        <TableCell>User</TableCell>
                        <TableCell>Amount</TableCell>
                        <TableCell>Platform Fee</TableCell>
                        <TableCell>Method</TableCell>
                        <TableCell>Status</TableCell>
                        <TableCell>Date</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {transactions.items.map((tx: AdminTransactionItem) => (
                        <TableRow key={tx.paymentId} hover>
                          <TableCell>
                            <Typography variant="caption" fontFamily="monospace">
                              {tx.paymentId.slice(0, 8)}…
                            </Typography>
                          </TableCell>
                          <TableCell>{tx.userEmail || tx.userId.slice(0, 8)}</TableCell>
                          <TableCell>{formatCurrency(tx.amount, tx.currencyCode)}</TableCell>
                          <TableCell>{formatCurrency(tx.platformFeeAmount, tx.currencyCode)}</TableCell>
                          <TableCell>{tx.paymentMethod}</TableCell>
                          <TableCell>
                            <Chip label={tx.status} size="small" color={getStatusColor(tx.status)} />
                          </TableCell>
                          <TableCell>{formatDate(tx.createdAt)}</TableCell>
                        </TableRow>
                      ))}
                      {transactions.items.length === 0 && (
                        <TableRow>
                          <TableCell colSpan={7} align="center">
                            No transactions found.
                          </TableCell>
                        </TableRow>
                      )}
                    </TableBody>
                  </Table>
                </TableContainer>
                <Box display="flex" justifyContent="center" mt={2}>
                  <Pagination
                    count={Math.ceil(transactions.totalCount / (transactions.pageSize || 20))}
                    page={transactionFilters.pageNumber ?? 1}
                    onChange={(_, p) => setTransactionFilters({ ...transactionFilters, pageNumber: p })}
                  />
                </Box>
              </>
            )}
          </Box>
        </TabPanel>

        {/* Payouts tab */}
        <TabPanel value={tab} index={2}>
          <Box px={2} pb={2}>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} mb={2}>
              <TextField
                select
                label="Status"
                size="small"
                value={payoutFilters.status ?? ''}
                onChange={(e) =>
                  setPayoutFilters({ ...payoutFilters, status: e.target.value || undefined, pageNumber: 1 })
                }
                sx={{ minWidth: 140 }}
              >
                {PAYOUT_STATUSES.map((s) => (
                  <MenuItem key={s} value={s}>
                    {s || 'All Statuses'}
                  </MenuItem>
                ))}
              </TextField>
              <TextField
                label="From"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                value={payoutFilters.startDate ?? ''}
                onChange={(e) =>
                  setPayoutFilters({ ...payoutFilters, startDate: e.target.value || undefined, pageNumber: 1 })
                }
              />
              <TextField
                label="To"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                value={payoutFilters.endDate ?? ''}
                onChange={(e) =>
                  setPayoutFilters({ ...payoutFilters, endDate: e.target.value || undefined, pageNumber: 1 })
                }
              />
            </Stack>
            {payoutsLoading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}
            {payoutsError && <Alert severity="error">Failed to load payouts.</Alert>}
            {payouts && (
              <>
                <TableContainer>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>Payout ID</TableCell>
                        <TableCell>Guide</TableCell>
                        <TableCell>Amount</TableCell>
                        <TableCell>Status</TableCell>
                        <TableCell>Requested</TableCell>
                        <TableCell>Processed</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {payouts.items.map((p: AdminPayoutItem) => (
                        <TableRow key={p.payoutId} hover>
                          <TableCell>
                            <Typography variant="caption" fontFamily="monospace">
                              {p.payoutId.slice(0, 8)}…
                            </Typography>
                          </TableCell>
                          <TableCell>{p.guideName || p.guideId.slice(0, 8)}</TableCell>
                          <TableCell>{formatCurrency(p.amount, p.currencyCode)}</TableCell>
                          <TableCell>
                            <Chip label={p.status} size="small" color={getStatusColor(p.status)} />
                          </TableCell>
                          <TableCell>{formatDate(p.requestedAt)}</TableCell>
                          <TableCell>{p.processedAt ? formatDate(p.processedAt) : '—'}</TableCell>
                        </TableRow>
                      ))}
                      {payouts.items.length === 0 && (
                        <TableRow>
                          <TableCell colSpan={6} align="center">
                            No payouts found.
                          </TableCell>
                        </TableRow>
                      )}
                    </TableBody>
                  </Table>
                </TableContainer>
                <Box display="flex" justifyContent="center" mt={2}>
                  <Pagination
                    count={Math.ceil(payouts.totalCount / (payouts.pageSize || 20))}
                    page={payoutFilters.pageNumber ?? 1}
                    onChange={(_, p) => setPayoutFilters({ ...payoutFilters, pageNumber: p })}
                  />
                </Box>
              </>
            )}
          </Box>
        </TabPanel>

        {/* Refunds tab */}
        <TabPanel value={tab} index={3}>
          <Box px={2} pb={2}>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} mb={2}>
              <TextField
                select
                label="Status"
                size="small"
                value={refundFilters.status ?? ''}
                onChange={(e) =>
                  setRefundFilters({ ...refundFilters, status: e.target.value || undefined, pageNumber: 1 })
                }
                sx={{ minWidth: 140 }}
              >
                {REFUND_STATUSES.map((s) => (
                  <MenuItem key={s} value={s}>
                    {s || 'All Statuses'}
                  </MenuItem>
                ))}
              </TextField>
              <TextField
                label="From"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                value={refundFilters.startDate ?? ''}
                onChange={(e) =>
                  setRefundFilters({ ...refundFilters, startDate: e.target.value || undefined, pageNumber: 1 })
                }
              />
              <TextField
                label="To"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                value={refundFilters.endDate ?? ''}
                onChange={(e) =>
                  setRefundFilters({ ...refundFilters, endDate: e.target.value || undefined, pageNumber: 1 })
                }
              />
            </Stack>
            {refundsLoading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}
            {refundsError && <Alert severity="error">Failed to load refunds.</Alert>}
            {refunds && (
              <>
                <TableContainer>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>Refund ID</TableCell>
                        <TableCell>Payment ID</TableCell>
                        <TableCell>Amount</TableCell>
                        <TableCell>Reason</TableCell>
                        <TableCell>Status</TableCell>
                        <TableCell>Requested</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {refunds.items.map((r: AdminRefundItem) => (
                        <TableRow key={r.refundId} hover>
                          <TableCell>
                            <Typography variant="caption" fontFamily="monospace">
                              {r.refundId.slice(0, 8)}…
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Typography variant="caption" fontFamily="monospace">
                              {r.paymentId.slice(0, 8)}…
                            </Typography>
                          </TableCell>
                          <TableCell>{formatCurrency(r.amount, r.currencyCode)}</TableCell>
                          <TableCell>{r.reason}</TableCell>
                          <TableCell>
                            <Chip label={r.status} size="small" color={getStatusColor(r.status)} />
                          </TableCell>
                          <TableCell>{formatDate(r.requestedAt)}</TableCell>
                        </TableRow>
                      ))}
                      {refunds.items.length === 0 && (
                        <TableRow>
                          <TableCell colSpan={6} align="center">
                            No refunds found.
                          </TableCell>
                        </TableRow>
                      )}
                    </TableBody>
                  </Table>
                </TableContainer>
                <Box display="flex" justifyContent="center" mt={2}>
                  <Pagination
                    count={Math.ceil(refunds.totalCount / (refunds.pageSize || 20))}
                    page={refundFilters.pageNumber ?? 1}
                    onChange={(_, p) => setRefundFilters({ ...refundFilters, pageNumber: p })}
                  />
                </Box>
              </>
            )}
          </Box>
        </TabPanel>
      </Paper>
    </Box>
  );
}
