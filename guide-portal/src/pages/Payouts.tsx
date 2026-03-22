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
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  CircularProgress,
  Alert,
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import { useAuth } from '../hooks/useAuth';
import type { PayoutItem } from '../types/guide.types';

const statusColors: Record<string, 'default' | 'warning' | 'success' | 'error'> = {
  pending: 'warning',
  completed: 'success',
  processed: 'success',
  failed: 'error',
};

const Payouts = () => {
  const { t } = useTranslation();
  const { user } = useAuth();
  const [payouts, setPayouts] = useState<PayoutItem[]>([]);
  const [balance, setBalance] = useState<number>(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [requestOpen, setRequestOpen] = useState(false);
  const [amountStr, setAmountStr] = useState('');
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 4000);
  };

  useEffect(() => {
    if (!user?.id) return;
    const load = async () => {
      setLoading(true);
      setError('');
      try {
        const [bal, payoutResp] = await Promise.all([
          guideApi.getAvailableBalance(user.id),
          guideApi.getPayouts(user.id),
        ]);
        setBalance(bal);
        setPayouts(payoutResp.payouts ?? []);
      } catch {
        setError(t('payouts.loadError'));
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [user?.id, t]);

  const handleRequestPayout = async () => {
    if (!user?.id) return;
    const amount = parseFloat(amountStr);
    if (isNaN(amount) || amount <= 0 || amount > balance) {
      showAlert('error', t('payouts.invalidAmount'));
      return;
    }
    try {
      const created = await guideApi.createPayout({ guideId: user.id, amount, currencyCode: 'USD' });
      setPayouts((prev) => [created, ...prev]);
      setBalance((prev) => prev - amount);
      setRequestOpen(false);
      setAmountStr('');
      showAlert('success', t('payouts.requestSuccess'));
    } catch {
      showAlert('error', t('payouts.requestError'));
    }
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
        <CircularProgress />
        <Typography sx={{ ml: 2 }}>{t('payouts.loading')}</Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('payouts.title')}
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        {t('payouts.subtitle')}
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {alert && <Alert severity={alert.type} sx={{ mb: 2 }}>{alert.message}</Alert>}

      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid size={{ xs: 12, sm: 6, md: 4 }}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="body2" color="text.secondary" gutterBottom>
              {t('payouts.availableBalance')}
            </Typography>
            <Typography variant="h4" fontWeight="bold" sx={{ color: '#00796b' }}>
              ${balance.toFixed(2)}
            </Typography>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => setRequestOpen(true)}
              sx={{ mt: 2 }}
              disabled={balance <= 0}
            >
              {t('payouts.requestPayout')}
            </Button>
          </Paper>
        </Grid>
      </Grid>

      <Paper elevation={2} sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          {t('payouts.payoutHistory')}
        </Typography>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>{t('payouts.dateRequested')}</TableCell>
                <TableCell align="right">{t('payouts.amount')}</TableCell>
                <TableCell>{t('payouts.status')}</TableCell>
                <TableCell>{t('payouts.processedAt')}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {payouts.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={4} align="center">
                    <Typography color="text.secondary">{t('payouts.noPayouts')}</Typography>
                  </TableCell>
                </TableRow>
              ) : (
                payouts.map((p) => (
                  <TableRow key={p.payoutId}>
                    <TableCell>{new Date(p.requestedAt).toLocaleDateString()}</TableCell>
                    <TableCell align="right">
                      {p.currencyCode} {p.amount.toFixed(2)}
                    </TableCell>
                    <TableCell>
                      <Chip label={p.status} color={statusColors[p.status] ?? 'default'} size="small" />
                    </TableCell>
                    <TableCell>
                      {p.processedAt ? new Date(p.processedAt).toLocaleDateString() : '—'}
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      <Dialog open={requestOpen} onClose={() => setRequestOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>{t('payouts.requestPayoutTitle')}</DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            {t('payouts.availableBalance')}: ${balance.toFixed(2)}
          </Typography>
          <TextField
            fullWidth
            label={t('payouts.amountLabel')}
            type="number"
            value={amountStr}
            onChange={(e) => setAmountStr(e.target.value)}
            inputProps={{ min: 1, max: balance, step: 0.01 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRequestOpen(false)}>{t('payouts.cancel')}</Button>
          <Button variant="contained" onClick={handleRequestPayout}>
            {t('payouts.submit')}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Payouts;
