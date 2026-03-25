import { useState } from 'react';
import {
  Snackbar,
  Alert,
  Button,
  Stack,
  IconButton,
  Typography,
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import SystemUpdateAltIcon from '@mui/icons-material/SystemUpdateAlt';
import GetAppIcon from '@mui/icons-material/GetApp';
import WifiOffIcon from '@mui/icons-material/WifiOff';
import { useTranslation } from 'react-i18next';
import { usePWA } from '../../hooks/usePWA';

export default function PWAPrompt() {
  const { t } = useTranslation();
  const { needRefresh, offlineReady, updateServiceWorker, installPromptAvailable, installApp } =
    usePWA();
  const [offlineDismissed, setOfflineDismissed] = useState(false);
  const [installDismissed, setInstallDismissed] = useState(false);

  return (
    <>
      {/* Offline ready notification */}
      <Snackbar
        open={offlineReady && !offlineDismissed}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
        autoHideDuration={5000}
        onClose={() => setOfflineDismissed(true)}
      >
        <Alert
          severity="success"
          icon={<WifiOffIcon />}
          action={
            <IconButton size="small" color="inherit" onClick={() => setOfflineDismissed(true)}>
              <CloseIcon fontSize="small" />
            </IconButton>
          }
        >
          {t('pwa.offlineReady')}
        </Alert>
      </Snackbar>

      {/* Update available notification */}
      <Snackbar
        open={needRefresh}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert
          severity="info"
          icon={<SystemUpdateAltIcon />}
          action={
            <Stack direction="row" spacing={1}>
              <Button
                size="small"
                color="inherit"
                variant="outlined"
                onClick={() => updateServiceWorker(true)}
              >
                {t('pwa.reload')}
              </Button>
              <IconButton size="small" color="inherit" onClick={() => updateServiceWorker(false)}>
                <CloseIcon fontSize="small" />
              </IconButton>
            </Stack>
          }
        >
          {t('pwa.newVersion')}
        </Alert>
      </Snackbar>

      {/* Install prompt */}
      <Snackbar
        open={installPromptAvailable && !installDismissed}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert
          severity="info"
          icon={<GetAppIcon />}
          action={
            <Stack direction="row" spacing={1} alignItems="center">
              <Button size="small" color="inherit" variant="outlined" onClick={installApp}>
                {t('pwa.install')}
              </Button>
              <IconButton size="small" color="inherit" onClick={() => setInstallDismissed(true)}>
                <CloseIcon fontSize="small" />
              </IconButton>
            </Stack>
          }
        >
          <Typography variant="body2">{t('pwa.installPrompt', { appName: 'Guide Portal' })}</Typography>
        </Alert>
      </Snackbar>
    </>
  );
}
