import { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Button,
  Chip,
  Stepper,
  Step,
  StepLabel,
  Alert,
  CircularProgress,
} from '@mui/material';
import {
  CloudUpload as UploadIcon,
  CheckCircle as CheckIcon,
} from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import type { KycVerificationStatus, VerificationDocument } from '../types/guide.types';

const DOCUMENT_TYPES = [
  { key: 'GovernmentId', labelKey: 'verification.governmentId', descKey: 'verification.governmentIdDesc' },
  { key: 'ProofOfAddress', labelKey: 'verification.proofOfAddress', descKey: 'verification.proofOfAddressDesc' },
  { key: 'Credentials', labelKey: 'verification.credentials', descKey: 'verification.credentialsDesc' },
];

const statusColor = (s: string): 'default' | 'warning' | 'success' | 'error' => {
  if (s === 'Verified' || s === 'approved') return 'success';
  if (s === 'Rejected' || s === 'rejected') return 'error';
  if (s === 'Pending' || s === 'pending' || s === 'under_review') return 'warning';
  return 'default';
};

const Verification = () => {
  const { t } = useTranslation();
  const [status, setStatus] = useState<KycVerificationStatus | null>(null);
  const [uploadedDocs, setUploadedDocs] = useState<VerificationDocument[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 5000);
  };

  useEffect(() => {
    guideApi
      .getVerificationStatus()
      .then((s) => {
        setStatus(s);
        setUploadedDocs(s.documents ?? []);
      })
      .catch(() => setError(t('verification.loading')))
      .finally(() => setLoading(false));
  }, [t]);

  const handleFileUpload = async (docType: string, file: File) => {
    const reader = new FileReader();
    reader.onload = async () => {
      const base64 = (reader.result as string).split(',')[1];
      try {
        const doc = await guideApi.submitVerificationDocument({
          documentType: docType,
          fileBase64: base64,
          fileName: file.name,
        });
        setUploadedDocs((prev) => {
          const filtered = prev.filter((d) => d.type !== docType);
          return [...filtered, doc];
        });
        showAlert('success', `${file.name} ${t('verification.uploadSuccess')}`);
      } catch {
        showAlert('error', t('verification.uploadError'));
      }
    };
    reader.readAsDataURL(file);
  };

  const getDocStatus = (docType: string) =>
    uploadedDocs.find((d) => d.type === docType);

  const overallStatus = status?.overallStatus ?? 'NotSubmitted';
  const activeStep = (() => {
    if (overallStatus === 'Verified') return 3;
    if (overallStatus === 'Pending' || overallStatus === 'under_review') return 2;
    if (uploadedDocs.length > 0) return 1;
    return 0;
  })();

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
        <CircularProgress />
        <Typography sx={{ ml: 2 }}>{t('verification.loading')}</Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>{t('verification.title')}</Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>{t('verification.subtitle')}</Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {alert && <Alert severity={alert.type} sx={{ mb: 2 }}>{alert.message}</Alert>}

      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6">{t('verification.verificationStatus')}</Typography>
          <Chip
            label={t(`verification.status${overallStatus}` as Parameters<typeof t>[0]) ?? overallStatus}
            color={statusColor(overallStatus)}
          />
        </Box>
        <Stepper activeStep={activeStep}>
          <Step><StepLabel>{t('verification.stepSubmitId')}</StepLabel></Step>
          <Step><StepLabel>{t('verification.stepProofAddress')}</StepLabel></Step>
          <Step><StepLabel>{t('verification.stepUnderReview')}</StepLabel></Step>
        </Stepper>
      </Paper>

      <Grid container spacing={3}>
        {DOCUMENT_TYPES.map((dt) => {
          const doc = getDocStatus(dt.key);
          return (
            <Grid item xs={12} md={4} key={dt.key}>
              <Paper elevation={2} sx={{ p: 3, height: '100%' }}>
                <Typography variant="h6" gutterBottom>{t(dt.labelKey as Parameters<typeof t>[0])}</Typography>
                <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                  {t(dt.descKey as Parameters<typeof t>[0])}
                </Typography>
                {doc ? (
                  <Box>
                    <Chip
                      icon={<CheckIcon />}
                      label={`${t('verification.uploaded')}: ${doc.fileName}`}
                      color={statusColor(doc.status)}
                      size="small"
                      sx={{ mb: 1 }}
                    />
                  </Box>
                ) : (
                  <Button
                    variant="outlined"
                    component="label"
                    startIcon={<UploadIcon />}
                    fullWidth
                  >
                    {t('verification.upload')}
                    <input
                      type="file"
                      hidden
                      accept="image/*,application/pdf"
                      onChange={(e) => {
                        const file = e.target.files?.[0];
                        if (file) handleFileUpload(dt.key, file);
                        e.target.value = '';
                      }}
                    />
                  </Button>
                )}
              </Paper>
            </Grid>
          );
        })}
      </Grid>

      <Paper elevation={2} sx={{ p: 3, mt: 3 }}>
        <Typography variant="h6" gutterBottom>{t('verification.requirements')}</Typography>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={4}>
            <Typography variant="body2" color="text.secondary">{t('verification.acceptedFormats')}: JPG, PNG, PDF</Typography>
          </Grid>
          <Grid item xs={12} sm={4}>
            <Typography variant="body2" color="text.secondary">{t('verification.maxFileSize')}: 10 MB</Typography>
          </Grid>
          <Grid item xs={12} sm={4}>
            <Typography variant="body2" color="text.secondary">
              {t('verification.reviewTime')}: {t('verification.reviewTimeValue')}
            </Typography>
          </Grid>
        </Grid>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          {t('verification.documentNote')}
        </Typography>
      </Paper>
    </Box>
  );
};

export default Verification;
