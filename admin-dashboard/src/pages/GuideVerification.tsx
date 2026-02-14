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

const GuideVerification = () => {
  const navigate = useNavigate();
  const [guides, setGuides] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    fetchPendingGuides();
  }, [page]);

  const fetchPendingGuides = async () => {
    try {
      setLoading(true);
      const data = await adminService.getPendingGuides(page);
      setGuides(data.items || []);
      setTotalPages(Math.ceil((data.totalCount || 0) / 10));
      setError(null);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch pending guides');
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status: number) => {
    switch (status) {
      case 0: return 'warning'; // Pending
      case 1: return 'info'; // UnderReview
      case 2: return 'success'; // Approved
      case 3: return 'error'; // Rejected
      default: return 'default';
    }
  };

  const getStatusLabel = (status: number) => {
    switch (status)  {
      case 0: return 'Pending';
      case 1: return 'Under Review';
      case 2: return 'Approved';
      case 3: return 'Rejected';
      default: return 'Unknown';
    }
  };

  const handleViewDetails = (userId: string) => {
    navigate(`/guides/${userId}/verification`);
  };

  if (loading && guides.length === 0) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Guide Verification Queue
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
              <TableCell>Name</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Location</TableCell>
              <TableCell>Registered</TableCell>
              <TableCell>Tours</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {guides.map((guide) => (
              <TableRow key={guide.userId}>
                <TableCell>{guide.fullName}</TableCell>
                <TableCell>{guide.email}</TableCell>
                <TableCell>{guide.city}, {guide.country}</TableCell>
                <TableCell>
                  {new Date(guide.registeredAt).toLocaleDateString()}
                </TableCell>
                <TableCell>{guide.tourCount}</TableCell>
                <TableCell>
                  <Chip 
                    label={getStatusLabel(guide.status)} 
                    color={getStatusColor(guide.status)}
                    size="small"
                  />
                </TableCell>
                <TableCell>
                  <Button
                    variant="outlined"
                    size="small"
                    onClick={() => handleViewDetails(guide.userId)}
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

export default GuideVerification;
