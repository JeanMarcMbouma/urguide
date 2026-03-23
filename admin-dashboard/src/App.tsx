import { Routes, Route, Navigate } from 'react-router-dom';
import { CircularProgress, Box } from '@mui/material';
import { useAuth } from './hooks/useAuth';
import AdminLayout from './components/Layout/AdminLayout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import UserList from './pages/UserList';
import UserDetail from './pages/UserDetail';
import ActivityLog from './pages/ActivityLog';
import GuideVerification from './pages/GuideVerification';
import GuideVerificationDetail from './pages/GuideVerificationDetail';
import TourModeration from './pages/TourModeration';
import TourModerationDetail from './pages/TourModerationDetail';
import FinancialDashboard from './pages/FinancialDashboard';
import SystemMonitoring from './pages/SystemMonitoring';
import TranslationManagement from './pages/TranslationManagement';

// Protected route wrapper
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
            <AdminLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="dashboard" element={<Dashboard />} />
        <Route path="users" element={<UserList />} />
        <Route path="users/:userId" element={<UserDetail />} />
        <Route path="users/:userId/activity" element={<ActivityLog />} />
        <Route path="guides/verification" element={<GuideVerification />} />
        <Route path="guides/:userId/verification" element={<GuideVerificationDetail />} />
        <Route path="tours/moderation" element={<TourModeration />} />
        <Route path="tours/:postId/moderation" element={<TourModerationDetail />} />
        <Route path="financial" element={<FinancialDashboard />} />
        <Route path="system" element={<SystemMonitoring />} />
        <Route path="translations" element={<TranslationManagement />} />
      </Route>

      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}

export default App;
