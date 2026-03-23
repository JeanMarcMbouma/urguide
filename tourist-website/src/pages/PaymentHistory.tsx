import { useState, useEffect } from 'react';
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
  Chip,
  Box,
  CircularProgress,
  Alert,
  Pagination,
} from '@mui/material';
import { getTransactionHistory } from '../services/touristApi';
import type { TransactionItem } from '../types/tourist.types';

const PaymentHistory = () => {
  const [transactions, setTransactions] = useState<TransactionItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchTransactions = async () => {
      setIsLoading(true);
      try {
        const data = await getTransactionHistory(page, 15);
        setTransactions(data.transactions || []);
        setTotalCount(data.totalCount || 0);
      } catch {
        setError('Failed to load payment history.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchTransactions();
  }, [page]);

  const getStatusColor = (status: string): 'success' | 'warning' | 'error' | 'default' => {
    switch (status?.toLowerCase()) {
      case 'completed': case 'paid': return 'success';
      case 'pending': return 'warning';
      case 'failed': case 'cancelled': return 'error';
      default: return 'default';
    }
  };

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>
        Payment History
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {isLoading ? (
        <Box display="flex" justifyContent="center" py={6}>
          <CircularProgress />
        </Box>
      ) : transactions.length === 0 ? (
        <Paper sx={{ p: 6, textAlign: 'center' }}>
          <Typography color="text.secondary">No transactions found.</Typography>
        </Paper>
      ) : (
        <>
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Date</TableCell>
                  <TableCell>Description</TableCell>
                  <TableCell>Tour</TableCell>
                  <TableCell>Guide</TableCell>
                  <TableCell align="right">Amount</TableCell>
                  <TableCell>Status</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {transactions.map((tx) => (
                  <TableRow key={tx.id} hover>
                    <TableCell>{new Date(tx.createdAt).toLocaleDateString()}</TableCell>
                    <TableCell>{tx.description}</TableCell>
                    <TableCell>{tx.tourTitle || '-'}</TableCell>
                    <TableCell>{tx.guideName || '-'}</TableCell>
                    <TableCell align="right">
                      <Typography
                        fontWeight="bold"
                        color={tx.type === 'refund' ? 'success.main' : 'text.primary'}
                      >
                        {tx.type === 'refund' ? '+' : ''}{tx.currency} {tx.amount}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Chip label={tx.status} size="small" color={getStatusColor(tx.status)} />
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
          {totalCount > 15 && (
            <Box display="flex" justifyContent="center" sx={{ mt: 3 }}>
              <Pagination
                count={Math.ceil(totalCount / 15)}
                page={page}
                onChange={(_e, p) => setPage(p)}
                color="primary"
              />
            </Box>
          )}
        </>
      )}
    </Container>
  );
};

export default PaymentHistory;
