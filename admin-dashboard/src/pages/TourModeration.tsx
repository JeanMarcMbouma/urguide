import { useState, useEffect } from 'react';
import {
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Button,
  Chip,
  CircularProgress,
  Alert,
  Box,
  Pagination
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { adminService } from '../services/adminApi';

const TourModeration = () => {
  const navigate = useNavigate();
  const [tours, setTours] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    fetchPendingTours();
  }, [page]);

  const fetchPendingTours = async () => {
    try {
      setLoading(true);
      const data = await adminService.getPendingTours(page);
      setTours(data.items || []);
      setTotalPages(Math.ceil((data.totalCount || 0) / 10));
      setError(null);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch pending tours');
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status: number) => {
    switch (status) {
      case 0: return 'warning'; // PendingReview
      case 1: return 'info'; // UnderReview
      case 2: return 'success'; // Approved
      case 3: return 'error'; // Rejected
      case 4: return 'error'; // Flagged
      default: return 'default';
    }
  };

  const getStatusLabel = (status: number) => {
    switch (status) {
      case 0: return 'Pending Review';
      case 1: return 'Under Review';
      case 2: return 'Approved';
      case 3: return 'Rejected';
      case 4: return 'Flagged';
      default: return 'Unknown';
    }
  };

  const handleViewDetails = (postId: string) => {
    navigate(`/tours/${postId}/moderation`);
  };

  if (loading && tours.length === 0) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Tour Moderation Queue
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Title</TableCell>
              <TableCell>Guide</TableCell>
              <TableCell>Location</TableCell>
              <TableCell>Created</TableCell>
              <TableCell>Cost</TableCell>
              <TableCell>Reports</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {tours.map((tour) => (
              <TableRow key={tour.postId}>
                <TableCell>{tour.title}</TableCell>
                <TableCell>{tour.guideName}</TableCell>
                <TableCell>{tour.location}</TableCell>
                <TableCell>
                  {new Date(tour.createdAt).toLocaleDateString()}
                </TableCell>
                <TableCell>${tour.cost.toFixed(2)}</TableCell>
                <TableCell>
                  {tour.reportCount > 0 ? (
                    <Chip label={tour.reportCount} color="error" size="small" />
                  ) : (
                    '0'
                  )}
                </TableCell>
                <TableCell>
                  <Chip 
                    label={getStatusLabel(tour.status)} 
                    color={getStatusColor(tour.status)}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  <Button
                    variant="outlined"
                    size="small"
                    onClick={() => handleViewDetails(tour.postId)}
                  >
                    Review
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {totalPages > 1 && (
        <Box display="flex" justifyContent="center" mt={3}>
          <Pagination 
            count={totalPages} 
            page={page} 
            onChange={(_e, value) => setPage(value)}
            color="primary"
          />
        </Box>
      )}
    </Box>
  );
};

export default TourModeration;
