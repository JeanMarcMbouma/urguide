import { useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  Stepper,
  Step,
  StepLabel,
  Button,
  Chip,
  Alert,
  Grid,
  Divider,
  LinearProgress,
} from '@mui/material';
import {
  CloudUpload as UploadIcon,
  CheckCircle as CheckIcon,
  HourglassEmpty as PendingIcon,
  Cancel as RejectedIcon,
} from '@mui/icons-material';
import { guideApi } from '../services/guideApi';

const DOCUMENT_TYPES = [
  {
    type: 'government_id',
    label: 'Government ID',
    description: 'Passport, national ID, or driver\'s license',
  },
  {
    type: 'proof_of_address',
    label: 'Proof of Address',
    description: 'Utility bill or bank statement (not older than 3 months)',
  },
  {
    type: 'credentials',
    label: 'Credentials / Insurance',
    description: 'Professional certifications, guide license, or insurance documents',
  },
];

const verificationSteps = ['Submit ID', 'Submit Proof of Address', 'Under Review'];

const statusConfig: Record<string, { label: string; color: 'success' | 'warning' | 'error' | 'default'; icon: React.ReactNode }> = {
  verified: { label: 'Verified', color: 'success', icon: <CheckIcon /> },
  pending: { label: 'Under Review', color: 'warning', icon: <PendingIcon /> },
  rejected: { label: 'Rejected', color: 'error', icon: <RejectedIcon /> },
  not_submitted: { label: 'Not Submitted', color: 'default', icon: <PendingIcon /> },
};

const Verification = () => {
  const [uploadingType, setUploadingType] = useState<string | null>(null);
  const [uploadedDocs, setUploadedDocs] = useState<Record<string, string>>({});
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const overallStatus = 'not_submitted';

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 4000);
  };

  const handleFileUpload = async (documentType: string, file: File) => {
    setUploadingType(documentType);
    const reader = new FileReader();
    reader.onload = async () => {
      const base64 = (reader.result as string).split(',')[1];
      try {
        await guideApi.submitVerificationDocument('me', {
          documentType,
          fileBase64: base64,
          fileName: file.name,
        });
        setUploadedDocs((prev) => ({ ...prev, [documentType]: file.name }));
        showAlert('success', `${documentType.replace(/_/g, ' ')} uploaded successfully.`);
      } catch {
        showAlert('error', 'Failed to upload document. Please try again.');
      } finally {
        setUploadingType(null);
      }
    };
    reader.readAsDataURL(file);
  };

  const status = statusConfig[overallStatus] ?? statusConfig['not_submitted'];
  const activeStep = Object.keys(uploadedDocs).length >= 2 ? 2 : Object.keys(uploadedDocs).length;

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Identity Verification (KYC)
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Complete your identity verification to unlock full guide features and receive payouts.
      </Typography>

      {alert && (
        <Alert severity={alert.type} sx={{ mb: 2 }}>
          {alert.message}
        </Alert>
      )}

      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
              <Typography variant="h6">Verification Status</Typography>
              <Chip
                icon={status.icon as React.ReactElement}
                label={status.label}
                color={status.color}
                variant="outlined"
              />
            </Box>
            <Stepper activeStep={activeStep} sx={{ mb: 3 }}>
              {verificationSteps.map((label) => (
                <Step key={label}>
                  <StepLabel>{label}</StepLabel>
                </Step>
              ))}
            </Stepper>
          </Paper>

          {DOCUMENT_TYPES.map((doc) => (
            <Paper key={doc.type} elevation={2} sx={{ p: 3, mb: 2 }}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
                <Box>
                  <Typography variant="h6">{doc.label}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {doc.description}
                  </Typography>
                </Box>
                {uploadedDocs[doc.type] && (
                  <Chip label="Uploaded" color="success" size="small" icon={<CheckIcon />} />
                )}
              </Box>

              {uploadingType === doc.type && <LinearProgress sx={{ mb: 1 }} />}

              {uploadedDocs[doc.type] ? (
                <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                  File: {uploadedDocs[doc.type]}
                </Typography>
              ) : (
                <Button
                  variant="outlined"
                  startIcon={<UploadIcon />}
                  component="label"
                  disabled={uploadingType !== null}
                  sx={{ mt: 1 }}
                >
                  Upload {doc.label}
                  <input
                    type="file"
                    hidden
                    accept=".pdf,.jpg,.jpeg,.png"
                    onChange={(e) => {
                      const file = e.target.files?.[0];
                      if (file) handleFileUpload(doc.type, file);
                    }}
                  />
                </Button>
              )}
            </Paper>
          ))}
        </Grid>

        <Grid item xs={12} md={4}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Requirements
            </Typography>
            <Divider sx={{ mb: 2 }} />
            <Typography variant="body2" gutterBottom>
              <strong>Accepted formats:</strong> PDF, JPG, PNG
            </Typography>
            <Typography variant="body2" gutterBottom>
              <strong>Maximum file size:</strong> 10 MB
            </Typography>
            <Typography variant="body2" gutterBottom>
              <strong>Review time:</strong> 2–5 business days
            </Typography>
            <Divider sx={{ my: 2 }} />
            <Typography variant="body2" color="text.secondary">
              Documents must be clear, legible, and unexpired. All personal information must match
              your profile details exactly.
            </Typography>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Verification;
