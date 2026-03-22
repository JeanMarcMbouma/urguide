import { useState, useEffect } from 'react';
import {
  Typography,
  Button,
  CircularProgress,
  Alert,
  Box,
  Grid,
  Card,
  CardContent,
  Chip,
  List,
  ListItem,
  ListItemText,
  Divider,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControlLabel,
  Checkbox
} from '@mui/material';
import { useParams, useNavigate } from 'react-router-dom';
import { adminService } from '../services/adminApi';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import CancelIcon from '@mui/icons-material/Cancel';

const TourModerationDetail = () => {
  const { postId } = useParams();
  const navigate = useNavigate();
  const [tour, setTour] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [decisionType, setDecisionType] = useState(''); // 'approve' or 'reject'
  const [reason, setReason] = useState('');
  const [adminNotes, setAdminNotes] = useState('');
  const [notifyGuide, setNotifyGuide] = useState(true);
  const [processing, setProcessing] = useState(false);

  useEffect(() => {
    fetchTourDetail();
  }, [postId]);

  const fetchTourDetail = async () => {
    if (!postId) return;
    try {
      setLoading(true);
      const data = await adminService.getTourModerationDetail(postId);
      setTour(data);
      setError(null);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch tour details');
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (type: string) => {
    setDecisionType(type);
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setReason('');
    setAdminNotes('');
    setNotifyGuide(true);
  };

  const handleSubmitDecision = async () => {
    try {
      setProcessing(true);
      await adminService.processTourModeration({
        postId,
        approve: decisionType === 'approve',
        reason,
        adminNotes,
        notifyGuide
      });
      handleCloseDialog();
      navigate('/tours/moderation');
    } catch (err: any) {
      setError(err.message || 'Failed to process tour moderation');
    } finally {
      setProcessing(false);
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return <Alert severity="error">{error}</Alert>;
  }

  if (!tour) {
    return <Alert severity="info">Tour not found</Alert>;
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">
          Tour Moderation: {tour.title}
        </Typography>
        <Box>
          <Button
            variant="contained"
            color="success"
            startIcon={<CheckCircleIcon />}
            onClick={() => handleOpenDialog('approve')}
            sx={{ mr: 1 }}
          >
            Approve
          </Button>
          <Button
            variant="contained"
            color="error"
            startIcon={<CancelIcon />}
            onClick={() => handleOpenDialog('reject')}
          >
            Reject
          </Button>
        </Box>
      </Box>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 8 }}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Tour Information
              </Typography>
              <List>
                <ListItem>
                  <ListItemText primary="Guide" secondary={`${tour.guideName} (${tour.guideEmail})`} />
                </ListItem>
                <ListItem>
                  <ListItemText primary="Location" secondary={tour.location} />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Start Date" 
                    secondary={tour.startDate ? new Date(tour.startDate).toLocaleDateString() : 'N/A'} 
                  />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="End Date" 
                    secondary={tour.endDate ? new Date(tour.endDate).toLocaleDateString() : 'N/A'} 
                  />
                </ListItem>
                <ListItem>
                  <ListItemText primary="Cost" secondary={`$${tour.cost.toFixed(2)}`} />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Created" 
                    secondary={new Date(tour.createdAt).toLocaleDateString()} 
                  />
                </ListItem>
              </List>

              <Divider sx={{ my: 2 }} />

              <Typography variant="h6" gutterBottom>
                Description
              </Typography>
              <Typography variant="body1" paragraph>
                {tour.description || 'No description provided'}
              </Typography>

              {tour.tags && tour.tags.length > 0 && (
                <>
                  <Typography variant="h6" gutterBottom>
                    Tags
                  </Typography>
                  <Box mb={2}>
                    {tour.tags.map((tag: string, index: number) => (
                      <Chip key={index} label={tag} sx={{ mr: 1, mb: 1 }} />
                    ))}
                  </Box>
                </>
              )}

              {tour.itinerary && tour.itinerary.length > 0 && (
                <>
                  <Typography variant="h6" gutterBottom>
                    Itinerary
                  </Typography>
                  <List>
                    {tour.itinerary.map((item: string, index: number) => (
                      <ListItem key={index}>
                        <ListItemText primary={`Day ${index + 1}`} secondary={item} />
                      </ListItem>
                    ))}
                  </List>
                </>
              )}

              <Typography variant="h6" gutterBottom>
                Statistics
              </Typography>
              <List>
                <ListItem>
                  <ListItemText primary="Bid Count" secondary={tour.bidCount} />
                </ListItem>
                <ListItem>
                  <ListItemText primary="Reservations" secondary={tour.reservationCount} />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Reports" 
                    secondary={
                      tour.reportCount > 0 ? (
                        <Chip label={tour.reportCount} color="error" size="small" />
                      ) : (
                        '0'
                      )
                    } 
                  />
                </ListItem>
              </List>
            </CardContent>
          </Card>
        </Grid>

        <Grid size={{ xs: 12, md: 4 }}>
          {tour.violations && tour.violations.length > 0 && (
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom color="error">
                  Content Violations
                </Typography>
                <List>
                  {tour.violations.map((violation: any, index: number) => (
                    <ListItem key={index}>
                      <ListItemText 
                        primary={violation.violationType}
                        secondary={
                          <>
                            <Typography variant="body2">{violation.description}</Typography>
                            <Typography variant="caption">
                              Reported by: {violation.reportedBy} on {new Date(violation.reportedAt).toLocaleDateString()}
                            </Typography>
                          </>
                        }
                      />
                    </ListItem>
                  ))}
                </List>
              </CardContent>
            </Card>
          )}

          {tour.images && tour.images.length > 0 && (
            <Card sx={{ mt: 2 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Images
                </Typography>
                <Grid container spacing={1}>
                  {tour.images.map((image: string, index: number) => (
                    <Grid size={{ xs: 6 }} key={index}>
                      <Box 
                        component="img" 
                        src={image} 
                        alt={`Tour ${index + 1}`} 
                        sx={{ width: '100%', height: 100, objectFit: 'cover', borderRadius: 1 }} 
                      />
                    </Grid>
                  ))}
                </Grid>
              </CardContent>
            </Card>
          )}
        </Grid>
      </Grid>

      <Dialog open={dialogOpen} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>
          {decisionType === 'approve' ? 'Approve Tour' : 'Reject Tour'}
        </DialogTitle>
        <DialogContent>
          <TextField
            label="Reason (will be sent to guide)"
            multiline
            rows={3}
            fullWidth
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            margin="normal"
          />
          <TextField
            label="Admin Notes (Internal)"
            multiline
            rows={3}
            fullWidth
            value={adminNotes}
            onChange={(e) => setAdminNotes(e.target.value)}
            margin="normal"
          />
          <FormControlLabel
            control={
              <Checkbox 
                checked={notifyGuide} 
                onChange={(e) => setNotifyGuide(e.target.checked)}
              />
            }
            label="Notify guide via email"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog} disabled={processing}>
            Cancel
          </Button>
          <Button 
            onClick={handleSubmitDecision} 
            variant="contained"
            color={decisionType === 'approve' ? 'success' : 'error'}
            disabled={processing}
          >
            {processing ? <CircularProgress size={24} /> : 'Confirm'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default TourModerationDetail;
