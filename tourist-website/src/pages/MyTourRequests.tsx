import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Button,
  Chip,
  Box,
  CircularProgress,
  Alert,
  Pagination,
  IconButton,
  Tooltip,
} from '@mui/material';
import {
  Visibility,
  Cancel as CancelIcon,
  Add as AddIcon,
} from '@mui/icons-material';
import { getMyTourRequests, cancelTourRequest } from '../services/touristApi';
import ConfirmDialog from '../components/shared/ConfirmDialog';
import type { TourRequest } from '../types/tourist.types';

const getStatusColor = (status: string): 'default' | 'primary' | 'success' | 'warning' | 'error' => {
  switch (status?.toLowerCase()) {
    case 'open': return 'primary';
    case 'active': return 'success';
    case 'completed': return 'success';
    case 'cancelled': return 'error';
    case 'pending': return 'warning';
    default: return 'default';
  }
};

const MyTourRequests = () => {
  const navigate = useNavigate();
  const [requests, setRequests] = useState<TourRequest[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [cancelTarget, setCancelTarget] = useState<number | null>(null);

  const fetchRequests = async (p: number) => {
    setIsLoading(true);
    try {
      const data = await getMyTourRequests(p, 10);
      setRequests(data.items || []);
      setTotalCount(data.totalCount || 0);
    } catch {
      setError('Failed to load tour requests.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchRequests(page);
  }, [page]);

  const handleCancel = async () => {
    if (cancelTarget === null) return;
    try {
      await cancelTourRequest(cancelTarget);
      setCancelTarget(null);
      fetchRequests(page);
    } catch {
      setError('Failed to cancel tour request.');
      setCancelTarget(null);
    }
  };

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">My Tour Requests</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => navigate('/tours/create')}
        >
          New Request
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {isLoading ? (
        <Box display="flex" justifyContent="center" py={6}>
          <CircularProgress />
        </Box>
      ) : requests.length === 0 ? (
        <Paper sx={{ p: 6, textAlign: 'center' }}>
          <Typography variant="h6" color="text.secondary" gutterBottom>
            No tour requests yet
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            Create your first tour request and let guides bid to be your guide.
          </Typography>
          <Button variant="contained" onClick={() => navigate('/tours/create')}>
            Create Tour Request
          </Button>
        </Paper>
      ) : (
        <>
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Title</TableCell>
                  <TableCell>Dates</TableCell>
                  <TableCell>Budget</TableCell>
                  <TableCell>Bids</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {requests.map((request) => (
                  <TableRow key={request.id} hover>
                    <TableCell>
                      <Typography variant="subtitle2">{request.title}</Typography>
                      <Typography variant="caption" color="text.secondary">
                        {request.regionName}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      {new Date(request.startDate).toLocaleDateString()} - {new Date(request.endDate).toLocaleDateString()}
                    </TableCell>
                    <TableCell>
                      {request.currency} {request.budgetMin} - {request.budgetMax}
                    </TableCell>
                    <TableCell>
                      <Chip label={request.bidCount} size="small" color={request.bidCount > 0 ? 'primary' : 'default'} />
                    </TableCell>
                    <TableCell>
                      <Chip label={request.status} size="small" color={getStatusColor(request.status)} />
                    </TableCell>
                    <TableCell align="right">
                      <Tooltip title="View Details">
                        <IconButton onClick={() => navigate(`/tours/${request.id}`)}>
                          <Visibility />
                        </IconButton>
                      </Tooltip>
                      {request.status?.toLowerCase() === 'open' && (
                        <Tooltip title="Cancel">
                          <IconButton onClick={() => setCancelTarget(request.id)} color="error">
                            <CancelIcon />
                          </IconButton>
                        </Tooltip>
                      )}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
          {totalCount > 10 && (
            <Box display="flex" justifyContent="center" sx={{ mt: 3 }}>
              <Pagination
                count={Math.ceil(totalCount / 10)}
                page={page}
                onChange={(_e, p) => setPage(p)}
                color="primary"
              />
            </Box>
          )}
        </>
      )}
      <ConfirmDialog
        open={cancelTarget !== null}
        title="Cancel Tour Request"
        message="Are you sure you want to cancel this tour request?"
        confirmText="Cancel Request"
        onConfirm={handleCancel}
        onCancel={() => setCancelTarget(null)}
        severity="error"
      />
    </Container>
  );
};

export default MyTourRequests;
