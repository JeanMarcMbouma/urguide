import { Routes, Route, Navigate } from 'react-router-dom';
import { CircularProgress, Box } from '@mui/material';
import { useAuth } from './hooks/useAuth';
import TouristLayout from './components/Layout/TouristLayout';
import Login from './pages/Login';
import Home from './pages/Home';
import GuideSearch from './pages/GuideSearch';
import GuideProfile from './pages/GuideProfile';
import CreateTourRequest from './pages/CreateTourRequest';
import MyTourRequests from './pages/MyTourRequests';
import TourRequestDetail from './pages/TourRequestDetail';
import Bookings from './pages/Bookings';
import Payment from './pages/Payment';
import PaymentHistory from './pages/PaymentHistory';
import Profile from './pages/Profile';
import Settings from './pages/Settings';
import WriteReview from './pages/WriteReview';
import Reviews from './pages/Reviews';
import Notifications from './pages/Notifications';
import Messages from './pages/Messages';

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
            <TouristLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<Navigate to="home" replace />} />
        <Route path="home" element={<Home />} />
        <Route path="search" element={<GuideSearch />} />
        <Route path="guides/:postId" element={<GuideProfile />} />
        <Route path="tours/create" element={<CreateTourRequest />} />
        <Route path="tours/my" element={<MyTourRequests />} />
        <Route path="tours/:tourRequestId" element={<TourRequestDetail />} />
        <Route path="bookings" element={<Bookings />} />
        <Route path="payment/:bookingId" element={<Payment />} />
        <Route path="payment/history" element={<PaymentHistory />} />
        <Route path="profile" element={<Profile />} />
        <Route path="settings" element={<Settings />} />
        <Route path="reviews/write/:postId" element={<WriteReview />} />
        <Route path="reviews" element={<Reviews />} />
        <Route path="notifications" element={<Notifications />} />
        <Route path="messages" element={<Messages />} />
      </Route>

      <Route path="*" element={<Navigate to="/home" replace />} />
    </Routes>
  );
}

export default App;
