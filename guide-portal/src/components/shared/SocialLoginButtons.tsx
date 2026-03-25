import React from 'react';
import { Button, Stack, Divider, Typography, Box } from '@mui/material';
import GoogleIcon from '@mui/icons-material/Google';
import AppleIcon from '@mui/icons-material/Apple';
import MicrosoftIcon from '@mui/icons-material/Window';
import { socialAuthService } from '../../services/socialAuthService';

interface SocialLoginButtonsProps {
  /** URL to redirect to after successful login */
  returnUrl?: string;
  /** Mode: 'login' for sign-in, 'link' for account linking */
  mode?: 'login' | 'link';
  /** Optional label override for the divider text */
  dividerText?: string;
}

const SocialLoginButtons: React.FC<SocialLoginButtonsProps> = ({
  returnUrl,
  mode = 'login',
  dividerText,
}) => {
  const handleSocialAuth = (provider: string) => {
    if (mode === 'link') {
      socialAuthService.initiateLink(provider, returnUrl);
    } else {
      socialAuthService.initiateLogin(provider, returnUrl);
    }
  };

  const actionText = mode === 'link' ? 'Link' : 'Sign in with';
  const label = dividerText ?? (mode === 'link' ? 'Link a social account' : 'Or sign in with');

  return (
    <Box sx={{ width: '100%', mt: 2 }}>
      <Divider sx={{ my: 2 }}>
        <Typography variant="body2" color="text.secondary">
          {label}
        </Typography>
      </Divider>
      <Stack spacing={1.5}>
        <Button
          variant="outlined"
          fullWidth
          startIcon={<GoogleIcon />}
          onClick={() => handleSocialAuth('Google')}
          sx={{
            textTransform: 'none',
            borderColor: '#4285F4',
            color: '#4285F4',
            '&:hover': { borderColor: '#357ae8', backgroundColor: 'rgba(66,133,244,0.04)' },
          }}
        >
          {actionText} Google
        </Button>
        <Button
          variant="outlined"
          fullWidth
          startIcon={<AppleIcon />}
          onClick={() => handleSocialAuth('Apple')}
          sx={{
            textTransform: 'none',
            borderColor: '#000',
            color: '#000',
            '&:hover': { borderColor: '#333', backgroundColor: 'rgba(0,0,0,0.04)' },
          }}
        >
          {actionText} Apple
        </Button>
        <Button
          variant="outlined"
          fullWidth
          startIcon={<MicrosoftIcon />}
          onClick={() => handleSocialAuth('Microsoft')}
          sx={{
            textTransform: 'none',
            borderColor: '#00a4ef',
            color: '#00a4ef',
            '&:hover': { borderColor: '#0078d7', backgroundColor: 'rgba(0,164,239,0.04)' },
          }}
        >
          {actionText} Microsoft
        </Button>
      </Stack>
    </Box>
  );
};

export default SocialLoginButtons;
