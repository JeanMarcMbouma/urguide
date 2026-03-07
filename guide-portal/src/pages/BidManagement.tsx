import { useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Button,
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
  IconButton,
  Alert,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
} from '@mui/icons-material';
import { guideApi } from '../services/guideApi';
import type { Bid, CreateBidRequest, UpdateBidRequest } from '../types/guide.types';
import ConfirmDialog from '../components/shared/ConfirmDialog';

const SUPPORTED_CURRENCIES = ['USD', 'EUR', 'GBP', 'CAD', 'AUD'];

const statusColors: Record<string, 'default' | 'warning' | 'success' | 'error' | 'info'> = {
  pending: 'warning',
  accepted: 'success',
  rejected: 'error',
  withdrawn: 'default',
  active: 'info',
};

const SAMPLE_BIDS: Bid[] = [
  {
    id: 'b1',
    postId: '1',
    guideId: 'g1',
    amount: 280,
    currency: 'USD',
    message: 'I have 8 years of experience guiding tours in Rome and can provide an exceptional cultural experience.',
    status: 'pending',
    createdAt: '2024-02-21T10:00:00Z',
    updatedAt: '2024-02-21T10:00:00Z',
  },
  {
    id: 'b2',
    postId: '2',
    guideId: 'g1',
    amount: 450,
    currency: 'USD',
    message: 'Passionate about Tuscan food and wine. I know the best local wineries and hidden trattorias.',
    status: 'accepted',
    createdAt: '2024-02-22T09:00:00Z',
    updatedAt: '2024-02-22T14:00:00Z',
  },
];

const emptyCreate: CreateBidRequest = { postId: '', amount: 0, currency: 'USD', message: '' };

const BidManagement = () => {
  const [searchParams] = useSearchParams();
  const prefillRequestId = searchParams.get('requestId') ?? '';

  const [bids, setBids] = useState<Bid[]>(SAMPLE_BIDS);
  const [createOpen, setCreateOpen] = useState(!!prefillRequestId);
  const [editBid, setEditBid] = useState<Bid | null>(null);
  const [withdrawId, setWithdrawId] = useState<string | null>(null);
  const [createForm, setCreateForm] = useState<CreateBidRequest>({
    ...emptyCreate,
    postId: prefillRequestId,
  });
  const [editForm, setEditForm] = useState<Omit<UpdateBidRequest, 'bidId'>>({ amount: 0, message: '' });
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 4000);
  };

  const handleCreate = async () => {
    try {
      const created = await guideApi.createBid(createForm);
      setBids((prev) => [created, ...prev]);
      setCreateOpen(false);
      setCreateForm(emptyCreate);
      showAlert('success', 'Bid placed successfully.');
    } catch {
      showAlert('error', 'Failed to place bid.');
    }
  };

  const handleEdit = async () => {
    if (!editBid) return;
    try {
      const updated = await guideApi.updateBid({ bidId: editBid.id, ...editForm });
      setBids((prev) => prev.map((b) => (b.id === updated.id ? updated : b)));
      setEditBid(null);
      showAlert('success', 'Bid updated.');
    } catch {
      showAlert('error', 'Failed to update bid.');
    }
  };

  const handleWithdraw = async () => {
    if (!withdrawId) return;
    try {
      await guideApi.withdrawBid(withdrawId);
      setBids((prev) => prev.map((b) => (b.id === withdrawId ? { ...b, status: 'withdrawn' } : b)));
      setWithdrawId(null);
      showAlert('success', 'Bid withdrawn.');
    } catch {
      showAlert('error', 'Failed to withdraw bid.');
    }
  };

  const openEdit = (bid: Bid) => {
    setEditBid(bid);
    setEditForm({ amount: bid.amount, message: bid.message });
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Bid Management</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setCreateOpen(true)}>
          Create Bid
        </Button>
      </Box>

      {alert && (
        <Alert severity={alert.type} sx={{ mb: 2 }}>
          {alert.message}
        </Alert>
      )}

      <TableContainer component={Paper} elevation={2}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Tour Request</TableCell>
              <TableCell>Amount</TableCell>
              <TableCell>Message</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Created</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {bids.map((bid) => (
              <TableRow key={bid.id}>
                <TableCell>{bid.postId}</TableCell>
                <TableCell>
                  {bid.currency} {bid.amount}
                </TableCell>
                <TableCell sx={{ maxWidth: 200 }}>
                  <Typography variant="body2" noWrap>
                    {bid.message}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Chip
                    label={bid.status}
                    color={statusColors[bid.status] ?? 'default'}
                    size="small"
                  />
                </TableCell>
                <TableCell>{new Date(bid.createdAt).toLocaleDateString()}</TableCell>
                <TableCell align="right">
                  {bid.status === 'pending' && (
                    <>
                      <IconButton size="small" onClick={() => openEdit(bid)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => setWithdrawId(bid.id)}
                      >
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </>
                  )}
                </TableCell>
              </TableRow>
            ))}
            {bids.length === 0 && (
              <TableRow>
                <TableCell colSpan={6} align="center">
                  <Typography color="text.secondary">No bids yet.</Typography>
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Create Bid Dialog */}
      <Dialog open={createOpen} onClose={() => setCreateOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create New Bid</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Tour Request ID"
            value={createForm.postId}
            onChange={(e) => setCreateForm((f) => ({ ...f, postId: e.target.value }))}
            sx={{ mt: 1, mb: 2 }}
          />
          <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
            <TextField
              fullWidth
              label="Amount"
              type="number"
              value={createForm.amount}
              onChange={(e) => setCreateForm((f) => ({ ...f, amount: Number(e.target.value) }))}
              inputProps={{ min: 0 }}
            />
            <FormControl sx={{ minWidth: 120 }}>
              <InputLabel>Currency</InputLabel>
              <Select
                value={createForm.currency}
                label="Currency"
                onChange={(e) => setCreateForm((f) => ({ ...f, currency: e.target.value }))}
              >
                {SUPPORTED_CURRENCIES.map((c) => (
                  <MenuItem key={c} value={c}>{c}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>
          <TextField
            fullWidth
            label="Message to Tourist"
            multiline
            rows={4}
            value={createForm.message}
            onChange={(e) => setCreateForm((f) => ({ ...f, message: e.target.value }))}
            placeholder="Introduce yourself and explain why you're a great fit for this tour..."
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCreateOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleCreate}
            disabled={!createForm.postId || !createForm.amount || !createForm.message}
          >
            Submit Bid
          </Button>
        </DialogActions>
      </Dialog>

      {/* Edit Bid Dialog */}
      <Dialog open={!!editBid} onClose={() => setEditBid(null)} maxWidth="sm" fullWidth>
        <DialogTitle>Edit Bid</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Amount"
            type="number"
            value={editForm.amount}
            onChange={(e) => setEditForm((f) => ({ ...f, amount: Number(e.target.value) }))}
            inputProps={{ min: 0 }}
            sx={{ mt: 1, mb: 2 }}
          />
          <TextField
            fullWidth
            label="Message"
            multiline
            rows={4}
            value={editForm.message}
            onChange={(e) => setEditForm((f) => ({ ...f, message: e.target.value }))}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditBid(null)}>Cancel</Button>
          <Button variant="contained" onClick={handleEdit}>
            Save Changes
          </Button>
        </DialogActions>
      </Dialog>

      {/* Withdraw Confirm Dialog */}
      <ConfirmDialog
        open={!!withdrawId}
        title="Withdraw Bid"
        message="Are you sure you want to withdraw this bid? This action cannot be undone."
        confirmText="Withdraw"
        severity="error"
        onConfirm={handleWithdraw}
        onCancel={() => setWithdrawId(null)}
      />
    </Box>
  );
};

export default BidManagement;
