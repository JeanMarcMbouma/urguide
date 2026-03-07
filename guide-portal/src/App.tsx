import { Routes, Route, Navigate } from 'react-router-dom';
import { CircularProgress, Box } from '@mui/material';
import { useAuth } from './hooks/useAuth';
import GuideLayout from './components/Layout/GuideLayout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Profile from './pages/Profile';
import Gallery from './pages/Gallery';
import Verification from './pages/Verification';
import TourRequests from './pages/TourRequests';
import BidManagement from './pages/BidManagement';
import Availability from './pages/Availability';
import Earnings from './pages/Earnings';
import Payouts from './pages/Payouts';
import Reviews from './pages/Reviews';
import Messages from './pages/Messages';
import Analytics from './pages/Analytics';

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />

      <Route
        path="/"
        element={
          <ProtectedRoute>
            <GuideLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="dashboard" element={<Dashboard />} />
        <Route path="profile" element={<Profile />} />
        <Route path="gallery" element={<Gallery />} />
        <Route path="verification" element={<Verification />} />
        <Route path="tours" element={<TourRequests />} />
        <Route path="bids" element={<BidManagement />} />
        <Route path="availability" element={<Availability />} />
        <Route path="earnings" element={<Earnings />} />
        <Route path="payouts" element={<Payouts />} />
        <Route path="reviews" element={<Reviews />} />
        <Route path="messages" element={<Messages />} />
        <Route path="analytics" element={<Analytics />} />
      </Route>

      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}

export default App;
