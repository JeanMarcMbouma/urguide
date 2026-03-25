import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Box,
  Paper,
  TextField,
  Button,
  Typography,
  Alert,
  CircularProgress,
} from '@mui/material';
import { LockOutlined as LockIcon } from '@mui/icons-material';
import { useAuth } from '../hooks/useAuth';
import SocialLoginButtons from '../components/shared/SocialLoginButtons';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [twoFactorCode, setTwoFactorCode] = useState('');
  const [requiresTwoFactor, setRequiresTwoFactor] = useState(false);
  const [userId, setUserId] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();
  const { login, verifyTwoFactor } = useAuth();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      const result = await login(email, password);
      
      if (result.requiresTwoFactor && result.userId) {
        setRequiresTwoFactor(true);
        setUserId(result.userId);
      } else {
        navigate('/dashboard');
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Invalid credentials');
    } finally {
      setIsLoading(false);
    }
  };

  const handleTwoFactorVerify = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await verifyTwoFactor(userId, twoFactorCode);
      navigate('/dashboard');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Invalid 2FA code');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Paper elevation={3} sx={{ p: 4, width: '100%' }}>
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
            }}
          >
            <Box
              sx={{
                bgcolor: 'primary.main',
                color: 'white',
                borderRadius: '50%',
                p: 2,
                mb: 2,
              }}
            >
              <LockIcon />
            </Box>
            <Typography component="h1" variant="h5" gutterBottom>
              Admin Login
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              {requiresTwoFactor
                ? 'Enter your 2FA verification code'
                : 'Sign in to UrGuide Admin Dashboard'}
            </Typography>
          </Box>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          {!requiresTwoFactor ? (
            <Box component="form" onSubmit={handleLogin} noValidate>
              <TextField
                margin="normal"
                required
                fullWidth
                id="email"
                label="Email Address"
                name="email"
                autoComplete="email"
                autoFocus
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
              <TextField
                margin="normal"
                required
                fullWidth
                name="password"
                label="Password"
                type="password"
                id="password"
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
                disabled={isLoading || !email || !password}
              >
                {isLoading ? <CircularProgress size={24} /> : 'Sign In'}
              </Button>
              <SocialLoginButtons returnUrl="/dashboard" />
            </Box>
          ) : (
            <Box component="form" onSubmit={handleTwoFactorVerify} noValidate>
              <TextField
                margin="normal"
                required
                fullWidth
                id="twoFactorCode"
                label="2FA Code"
                name="twoFactorCode"
                autoFocus
                value={twoFactorCode}
                onChange={(e) => setTwoFactorCode(e.target.value)}
                inputProps={{ maxLength: 6 }}
              />
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
                disabled={isLoading || twoFactorCode.length !== 6}
              >
                {isLoading ? <CircularProgress size={24} /> : 'Verify'}
              </Button>
              <Button
                fullWidth
                variant="text"
                onClick={() => {
                  setRequiresTwoFactor(false);
                  setTwoFactorCode('');
                  setError('');
                }}
              >
                Back to Login
              </Button>
            </Box>
          )}
        </Paper>
      </Box>
    </Container>
  );
};

export default Login;
