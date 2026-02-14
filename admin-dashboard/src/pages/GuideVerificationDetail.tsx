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
  TextField
} from '@mui/material';
import { useParams, useNavigate } from 'react-router-dom';
import { adminService } from '../services/adminApi';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import CancelIcon from '@mui/icons-material/Cancel';

const GuideVerificationDetail = () => {
  const { userId } = useParams();
  const navigate = useNavigate();
  const [guide, setGuide] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [decisionType, setDecisionType] = useState(''); // 'approve' or 'reject'
  const [reason, setReason] = useState('');
  const [adminNotes, setAdminNotes] = useState('');
  const [processing, setProcessing] = useState(false);

  useEffect(() => {
    fetchGuideDetail();
  }, [userId]);

  const fetchGuideDetail = async () => {
    if (!userId) return;
    try {
      setLoading(true);
      const data = await adminService.getGuideVerificationDetail(userId);
      setGuide(data);
      setError(null);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch guide details');
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
  };

  const handleSubmitDecision = async () => {
    try {
      setProcessing(true);
      await adminService.processGuideVerification({
        userId,
        approve: decisionType === 'approve',
        reason,
        adminNotes
      });
      handleCloseDialog();
      navigate('/guides/verification');
    } catch (err: any) {
      setError(err.message || 'Failed to process guide verification');
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

  if (!guide) {
    return <Alert severity="info">Guide not found</Alert>;
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">
          Guide Verification: {guide.fullName}
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
        <Grid item xs={12} md={8}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Guide Information
              </Typography>
              <List>
                <ListItem>
                  <ListItemText primary="Email" secondary={guide.email} />
                </ListItem>
                <ListItem>
                  <ListItemText primary="Phone" secondary={guide.phoneNumber || 'N/A'} />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Location" 
                    secondary={`${guide.city}, ${guide.country}`} 
                  />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Address" 
                    secondary={guide.address || 'N/A'} 
                  />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Date of Birth" 
                    secondary={guide.dateOfBirth ? new Date(guide.dateOfBirth).toLocaleDateString() : 'N/A'} 
                  />
                </ListItem>
                <ListItem>
                  <ListItemText primary="Gender" secondary={guide.gender || 'N/A'} />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Registered" 
                    secondary={new Date(guide.registeredAt).toLocaleDateString()} 
                  />
                </ListItem>
              </List>

              <Divider sx={{ my: 2 }} />

              <Typography variant="h6" gutterBottom>
                Profile Description
              </Typography>
              <Typography variant="body1" paragraph>
                {guide.description || 'No description provided'}
              </Typography>

              <Typography variant="h6" gutterBottom>
                Statistics
              </Typography>
              <List>
                <ListItem>
                  <ListItemText primary="Total Tours" secondary={guide.tourCount} />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Average Rating" 
                    secondary={`${guide.averageRating.toFixed(1)} / 5.0`} 
                  />
                </ListItem>
                <ListItem>
                  <ListItemText primary="Review Count" secondary={guide.reviewCount} />
                </ListItem>
              </List>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Verification Checklist
              </Typography>
              <List>
                <ListItem>
                  <ListItemText 
                    primary="Profile Complete" 
                    secondary={
                      <Chip 
                        label={guide.checklist.profileComplete ? 'Yes' : 'No'} 
                        color={guide.checklist.profileComplete ? 'success' : 'error'}
                        size="small"
                      />
                    }
                  />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Contact Verified" 
                    secondary={
                      <Chip 
                        label={guide.checklist.contactVerified ? 'Yes' : 'No'} 
                        color={guide.checklist.contactVerified ? 'success' : 'error'}
                        size="small"
                      />
                    }
                  />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Adequate Description" 
                    secondary={
                      <Chip 
                        label={guide.checklist.profileDescriptionAdequate ? 'Yes' : 'No'} 
                        color={guide.checklist.profileDescriptionAdequate ? 'success' : 'error'}
                        size="small"
                      />
                    }
                  />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="ID Document Provided" 
                    secondary={
                      <Chip 
                        label={guide.checklist.identityDocumentProvided ? 'Yes' : 'No'} 
                        color={guide.checklist.identityDocumentProvided ? 'success' : 'error'}
                        size="small"
                      />
                    }
                  />
                </ListItem>
                <ListItem>
                  <ListItemText 
                    primary="Background Check" 
                    secondary={
                      <Chip 
                        label={guide.checklist.backgroundCheckPassed ? 'Passed' : 'Pending'} 
                        color={guide.checklist.backgroundCheckPassed ? 'success' : 'warning'}
                        size="small"
                      />
                    }
                  />
                </ListItem>
              </List>
            </CardContent>
          </Card>

          {guide.profileImage && (
            <Card sx={{ mt: 2 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Profile Image
                </Typography>
                <Box component="img" src={guide.profileImage} alt="Profile" sx={{ width: '100%', borderRadius: 1 }} />
              </CardContent>
            </Card>
          )}
        </Grid>
      </Grid>

      <Dialog open={dialogOpen} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>
          {decisionType === 'approve' ? 'Approve Guide' : 'Reject Guide'}
        </DialogTitle>
        <DialogContent>
          <TextField
            label="Reason"
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

export default GuideVerificationDetail;
