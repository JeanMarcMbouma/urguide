import { useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Alert,
  Divider,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  IconButton,
} from '@mui/material';
import {
  Add as AddIcon,
  AccountBalanceWallet as WalletIcon,
  Delete as DeleteIcon,
  Star as StarIcon,
} from '@mui/icons-material';
import type { PayoutItem, PaymentMethod } from '../types/guide.types';

const statusColors: Record<string, 'success' | 'warning' | 'error' | 'default' | 'info'> = {
  completed: 'success',
  pending: 'warning',
  processing: 'info',
  failed: 'error',
};

const SAMPLE_PAYOUTS: PayoutItem[] = [
  {
    payoutId: 'p1',
    guideId: 'g1',
    amount: 500,
    currencyCode: 'USD',
    status: 'completed',
    requestedAt: '2024-03-05T10:00:00Z',
    processedAt: '2024-03-07T10:00:00Z',
    paymentMethod: 'bank_transfer',
  },
  {
    payoutId: 'p2',
    guideId: 'g1',
    amount: 350,
    currencyCode: 'USD',
    status: 'pending',
    requestedAt: '2024-03-20T10:00:00Z',
    paymentMethod: 'paypal',
  },
];

const SAMPLE_METHODS: PaymentMethod[] = [
  { id: 'm1', type: 'bank_transfer', details: 'Bank **** 4321', isDefault: true, createdAt: '2024-01-01' },
  { id: 'm2', type: 'paypal', details: 'guide@example.com', isDefault: false, createdAt: '2024-02-01' },
];

const Payouts = () => {
  const [payouts] = useState<PayoutItem[]>(SAMPLE_PAYOUTS);
  const [methods, setMethods] = useState<PaymentMethod[]>(SAMPLE_METHODS);
  const [requestOpen, setRequestOpen] = useState(false);
  const [amount, setAmount] = useState('');
  const [selectedMethod, setSelectedMethod] = useState('m1');
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const availableBalance = 1230;

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 4000);
  };

  const handleRequestPayout = async () => {
    const amountNum = Number(amount);
    if (!amountNum || amountNum <= 0 || amountNum > availableBalance) {
      showAlert('error', 'Please enter a valid amount within your available balance.');
      return;
    }
    try {
      // In production, call guideApi.createPayout(...)
      setRequestOpen(false);
      setAmount('');
      showAlert('success', `Payout of $${amount} requested successfully.`);
    } catch {
      showAlert('error', 'Failed to request payout.');
    }
  };

  const handleRemoveMethod = (id: string) => {
    setMethods((prev) => prev.filter((m) => m.id !== id));
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Payouts
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Manage your payout requests and payment methods.
      </Typography>

      {alert && (
        <Alert severity={alert.type} sx={{ mb: 2 }}>
          {alert.message}
        </Alert>
      )}

      <Grid container spacing={3}>
        {/* Available Balance Card */}
        <Grid item xs={12} md={4}>
          <Paper elevation={2} sx={{ p: 3, textAlign: 'center' }}>
            <WalletIcon sx={{ fontSize: 48, color: 'primary.main', mb: 1 }} />
            <Typography variant="body2" color="text.secondary">
              Available Balance
            </Typography>
            <Typography variant="h3" fontWeight="bold" color="primary.main">
              ${availableBalance.toLocaleString()}
            </Typography>
            <Button
              variant="contained"
              sx={{ mt: 2 }}
              onClick={() => setRequestOpen(true)}
              fullWidth
            >
              Request Payout
            </Button>
          </Paper>
        </Grid>

        {/* Payment Methods */}
        <Grid item xs={12} md={8}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
              <Typography variant="h6">Payment Methods</Typography>
              <Button startIcon={<AddIcon />} size="small">
                Add Method
              </Button>
            </Box>
            <Divider sx={{ mb: 1 }} />
            <List dense>
              {methods.map((method) => (
                <ListItem key={method.id}>
                  <ListItemText
                    primary={
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        {method.details}
                        {method.isDefault && (
                          <Chip
                            icon={<StarIcon />}
                            label="Default"
                            size="small"
                            color="primary"
                            variant="outlined"
                          />
                        )}
                      </Box>
                    }
                    secondary={method.type.replace(/_/g, ' ')}
                  />
                  <ListItemSecondaryAction>
                    <IconButton
                      edge="end"
                      size="small"
                      color="error"
                      onClick={() => handleRemoveMethod(method.id)}
                      disabled={method.isDefault}
                    >
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </ListItemSecondaryAction>
                </ListItem>
              ))}
            </List>
          </Paper>
        </Grid>

        {/* Payout History */}
        <Grid item xs={12}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Payout History
            </Typography>
            <TableContainer>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Date Requested</TableCell>
                    <TableCell align="right">Amount</TableCell>
                    <TableCell>Payment Method</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell>Processed At</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {payouts.map((payout) => (
                    <TableRow key={payout.payoutId}>
                      <TableCell>{new Date(payout.requestedAt).toLocaleDateString()}</TableCell>
                      <TableCell align="right">
                        {payout.currencyCode} {payout.amount.toLocaleString()}
                      </TableCell>
                      <TableCell>{payout.paymentMethod.replace(/_/g, ' ')}</TableCell>
                      <TableCell>
                        <Chip
                          label={payout.status}
                          color={statusColors[payout.status] ?? 'default'}
                          size="small"
                        />
                      </TableCell>
                      <TableCell>
                        {payout.processedAt
                          ? new Date(payout.processedAt).toLocaleDateString()
                          : '—'}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Grid>
      </Grid>

      {/* Request Payout Dialog */}
      <Dialog open={requestOpen} onClose={() => setRequestOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Request Payout</DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Available balance: <strong>${availableBalance.toLocaleString()}</strong>
          </Typography>
          <TextField
            fullWidth
            label="Amount (USD)"
            type="number"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            inputProps={{ min: 1, max: availableBalance }}
            sx={{ mb: 2 }}
          />
          <FormControl fullWidth>
            <InputLabel>Payment Method</InputLabel>
            <Select
              value={selectedMethod}
              label="Payment Method"
              onChange={(e) => setSelectedMethod(e.target.value)}
            >
              {methods.map((m) => (
                <MenuItem key={m.id} value={m.id}>
                  {m.details} ({m.type.replace(/_/g, ' ')})
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRequestOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleRequestPayout} disabled={!amount}>
            Request Payout
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Payouts;
