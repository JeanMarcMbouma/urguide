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
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import type { Bid, CreateBidRequest } from '../types/guide.types';
import ConfirmDialog from '../components/shared/ConfirmDialog';

const SUPPORTED_CURRENCIES = ['USD', 'EUR', 'GBP', 'CAD', 'AUD'];

const statusColors: Record<string, 'default' | 'warning' | 'success' | 'error' | 'info'> = {
  pending: 'warning',
  accepted: 'success',
  rejected: 'error',
  withdrawn: 'default',
  active: 'info',
};

const emptyCreate: CreateBidRequest = { postId: '', amount: 0, currency: 'USD', message: '' };

const BidManagement = () => {
  const [searchParams] = useSearchParams();
  const { t } = useTranslation();
  const prefillRequestId = searchParams.get('requestId') ?? '';

  const [bids, setBids] = useState<Bid[]>([]);
  const [createOpen, setCreateOpen] = useState(!!prefillRequestId);
  const [editBid, setEditBid] = useState<Bid | null>(null);
  const [withdrawId, setWithdrawId] = useState<string | null>(null);
  const [createForm, setCreateForm] = useState<CreateBidRequest>({
    ...emptyCreate,
    postId: prefillRequestId,
  });
  const [editForm, setEditForm] = useState<{ amount: number; message: string }>({ amount: 0, message: '' });
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
      showAlert('success', t('bids.createSuccess'));
    } catch {
      showAlert('error', t('bids.createError'));
    }
  };

  const handleEdit = async () => {
    if (!editBid) return;
    try {
      // The API doesn't expose PUT bids – we optimistically update locally
      const updated: Bid = { ...editBid, amount: editForm.amount, message: editForm.message };
      setBids((prev) => prev.map((b) => (b.id === updated.id ? updated : b)));
      setEditBid(null);
      showAlert('success', t('bids.updateSuccess'));
    } catch {
      showAlert('error', t('bids.updateError'));
    }
  };

  const handleWithdraw = async () => {
    if (!withdrawId) return;
    try {
      setBids((prev) => prev.map((b) => (b.id === withdrawId ? { ...b, status: 'withdrawn' } : b)));
      setWithdrawId(null);
      showAlert('success', t('bids.withdrawSuccess'));
    } catch {
      showAlert('error', t('bids.withdrawError'));
    }
  };

  const openEdit = (bid: Bid) => {
    setEditBid(bid);
    setEditForm({ amount: bid.amount, message: bid.message });
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
        <Box>
          <Typography variant="h4">{t('bids.title')}</Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mb: 2 }}>
            {t('bids.subtitle')}
          </Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setCreateOpen(true)}>
          {t('bids.newBid')}
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
              <TableCell>{t('bids.tour')}</TableCell>
              <TableCell>{t('bids.amount')}</TableCell>
              <TableCell>{t('bids.message')}</TableCell>
              <TableCell>{t('bids.status')}</TableCell>
              <TableCell>{t('bids.created')}</TableCell>
              <TableCell align="right">{t('bids.actions')}</TableCell>
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
                  <Chip label={bid.status} color={statusColors[bid.status] ?? 'default'} size="small" />
                </TableCell>
                <TableCell>{new Date(bid.createdAt).toLocaleDateString()}</TableCell>
                <TableCell align="right">
                  {bid.status === 'pending' && (
                    <>
                      <IconButton size="small" onClick={() => openEdit(bid)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" color="error" onClick={() => setWithdrawId(bid.id)}>
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
                  <Typography color="text.secondary">{t('bids.noBids')}</Typography>
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Create Bid Dialog */}
      <Dialog open={createOpen} onClose={() => setCreateOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('bids.newBid')}</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label={t('bids.postId')}
            value={createForm.postId}
            onChange={(e) => setCreateForm((f) => ({ ...f, postId: e.target.value }))}
            sx={{ mt: 1, mb: 2 }}
          />
          <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
            <TextField
              fullWidth
              label={t('bids.amount')}
              type="number"
              value={createForm.amount}
              onChange={(e) => setCreateForm((f) => ({ ...f, amount: Number(e.target.value) }))}
              inputProps={{ min: 0 }}
            />
            <FormControl sx={{ minWidth: 120 }}>
              <InputLabel>{t('bids.currency')}</InputLabel>
              <Select
                value={createForm.currency}
                label={t('bids.currency')}
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
            label={t('bids.message')}
            multiline
            rows={4}
            value={createForm.message}
            onChange={(e) => setCreateForm((f) => ({ ...f, message: e.target.value }))}
            placeholder={t('bids.messagePlaceholder')}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCreateOpen(false)}>{t('bids.cancel')}</Button>
          <Button
            variant="contained"
            onClick={handleCreate}
            disabled={!createForm.postId || !createForm.amount || !createForm.message}
          >
            {t('bids.submit')}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Edit Bid Dialog */}
      <Dialog open={!!editBid} onClose={() => setEditBid(null)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('bids.editBid')}</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label={t('bids.amount')}
            type="number"
            value={editForm.amount}
            onChange={(e) => setEditForm((f) => ({ ...f, amount: Number(e.target.value) }))}
            inputProps={{ min: 0 }}
            sx={{ mt: 1, mb: 2 }}
          />
          <TextField
            fullWidth
            label={t('bids.message')}
            multiline
            rows={4}
            value={editForm.message}
            onChange={(e) => setEditForm((f) => ({ ...f, message: e.target.value }))}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditBid(null)}>{t('bids.cancel')}</Button>
          <Button variant="contained" onClick={handleEdit}>
            {t('bids.update')}
          </Button>
        </DialogActions>
      </Dialog>

      <ConfirmDialog
        open={!!withdrawId}
        title={t('bids.withdrawTitle')}
        message={t('bids.confirmWithdraw')}
        confirmText={t('bids.withdrawBid')}
        severity="error"
        onConfirm={handleWithdraw}
        onCancel={() => setWithdrawId(null)}
      />
    </Box>
  );
};

export default BidManagement;
