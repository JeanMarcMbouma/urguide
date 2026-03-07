import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Grid,
  Card,
  CardContent,
  CardActions,
  Avatar,
  Chip,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  Pagination,
} from '@mui/material';
import {
  Search as SearchIcon,
  Group as GroupIcon,
  AttachMoney as MoneyIcon,
  CalendarToday as CalendarIcon,
} from '@mui/icons-material';
import type { TourRequest } from '../types/guide.types';

const TOUR_REQUEST_STATUS_OPTIONS = ['all', 'pending', 'accepted', 'rejected'];

const statusColors: Record<string, 'default' | 'warning' | 'success' | 'error'> = {
  pending: 'warning',
  accepted: 'success',
  rejected: 'error',
};

const SAMPLE_REQUESTS: TourRequest[] = [
  {
    id: '1',
    touristId: 't1',
    touristName: 'Alice Johnson',
    touristAvatar: '',
    title: 'Cultural Tour of Old City',
    description: 'Looking for a knowledgeable guide to explore the historic old city district including local markets and heritage sites.',
    destination: 'Rome, Italy',
    startDate: '2024-03-15',
    endDate: '2024-03-17',
    groupSize: 4,
    budget: 350,
    status: 'pending',
    createdAt: '2024-02-20T10:00:00Z',
  },
  {
    id: '2',
    touristId: 't2',
    touristName: 'Bob Martinez',
    touristAvatar: '',
    title: 'Food & Wine Experience',
    description: 'Wine tasting and authentic local cuisine tour for a group of food enthusiasts.',
    destination: 'Tuscany, Italy',
    startDate: '2024-04-01',
    endDate: '2024-04-02',
    groupSize: 6,
    budget: 500,
    status: 'pending',
    createdAt: '2024-02-21T09:00:00Z',
  },
];

const TourRequests = () => {
  const navigate = useNavigate();
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [detailRequest, setDetailRequest] = useState<TourRequest | null>(null);
  const [page, setPage] = useState(1);

  const filtered = SAMPLE_REQUESTS.filter((r) => {
    const matchesSearch =
      !searchTerm ||
      r.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      r.destination.toLowerCase().includes(searchTerm.toLowerCase()) ||
      r.touristName.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || r.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Tour Request Inbox
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Browse and respond to tour requests from tourists.
      </Typography>

      <Paper elevation={2} sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={7}>
            <TextField
              fullWidth
              placeholder="Search by title, destination, or tourist..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              InputProps={{ startAdornment: <SearchIcon sx={{ mr: 1, color: 'text.secondary' }} /> }}
              size="small"
            />
          </Grid>
          <Grid item xs={12} sm={5}>
            <FormControl fullWidth size="small">
              <InputLabel>Status</InputLabel>
              <Select
                value={statusFilter}
                label="Status"
                onChange={(e) => setStatusFilter(e.target.value)}
              >
                {TOUR_REQUEST_STATUS_OPTIONS.map((s) => (
                  <MenuItem key={s} value={s}>
                    {s.charAt(0).toUpperCase() + s.slice(1)}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
        </Grid>
      </Paper>

      <Grid container spacing={2}>
        {filtered.map((request) => (
          <Grid item xs={12} md={6} key={request.id}>
            <Card elevation={2}>
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Avatar src={request.touristAvatar}>{request.touristName[0]}</Avatar>
                    <Typography variant="subtitle1" fontWeight="bold">
                      {request.touristName}
                    </Typography>
                  </Box>
                  <Chip
                    label={request.status}
                    color={statusColors[request.status] ?? 'default'}
                    size="small"
                  />
                </Box>
                <Typography variant="h6" gutterBottom>
                  {request.title}
                </Typography>
                <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                  {request.description.substring(0, 100)}...
                </Typography>
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, mt: 1 }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <CalendarIcon fontSize="small" color="action" />
                    <Typography variant="body2">
                      {request.startDate} – {request.endDate}
                    </Typography>
                  </Box>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <GroupIcon fontSize="small" color="action" />
                    <Typography variant="body2">{request.groupSize} people</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <MoneyIcon fontSize="small" color="action" />
                    <Typography variant="body2">Budget: ${request.budget}</Typography>
                  </Box>
                </Box>
              </CardContent>
              <CardActions>
                <Button size="small" onClick={() => setDetailRequest(request)}>
                  View Details
                </Button>
                {request.status === 'pending' && (
                  <Button
                    size="small"
                    variant="contained"
                    onClick={() => navigate(`/bids?requestId=${request.id}`)}
                  >
                    Place Bid
                  </Button>
                )}
              </CardActions>
            </Card>
          </Grid>
        ))}
        {filtered.length === 0 && (
          <Grid item xs={12}>
            <Paper elevation={1} sx={{ p: 4, textAlign: 'center' }}>
              <Typography color="text.secondary">No tour requests found.</Typography>
            </Paper>
          </Grid>
        )}
      </Grid>

      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
        <Pagination count={3} page={page} onChange={(_, v) => setPage(v)} color="primary" />
      </Box>

      {/* Detail Dialog */}
      <Dialog
        open={!!detailRequest}
        onClose={() => setDetailRequest(null)}
        maxWidth="sm"
        fullWidth
      >
        {detailRequest && (
          <>
            <DialogTitle>{detailRequest.title}</DialogTitle>
            <DialogContent>
              <DialogContentText component="div">
                <Typography variant="body1" gutterBottom>
                  {detailRequest.description}
                </Typography>
                <Box sx={{ mt: 2, display: 'flex', flexDirection: 'column', gap: 1 }}>
                  <Typography variant="body2">
                    <strong>Destination:</strong> {detailRequest.destination}
                  </Typography>
                  <Typography variant="body2">
                    <strong>Dates:</strong> {detailRequest.startDate} – {detailRequest.endDate}
                  </Typography>
                  <Typography variant="body2">
                    <strong>Group Size:</strong> {detailRequest.groupSize} people
                  </Typography>
                  <Typography variant="body2">
                    <strong>Budget:</strong> ${detailRequest.budget}
                  </Typography>
                  <Typography variant="body2">
                    <strong>Tourist:</strong> {detailRequest.touristName}
                  </Typography>
                </Box>
              </DialogContentText>
            </DialogContent>
            <Box sx={{ p: 2, display: 'flex', justifyContent: 'flex-end', gap: 1 }}>
              <Button onClick={() => setDetailRequest(null)}>Close</Button>
              {detailRequest.status === 'pending' && (
                <Button
                  variant="contained"
                  onClick={() => {
                    navigate(`/bids?requestId=${detailRequest.id}`);
                    setDetailRequest(null);
                  }}
                >
                  Place Bid
                </Button>
              )}
            </Box>
          </>
        )}
      </Dialog>
    </Box>
  );
};

export default TourRequests;
