import { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Paper,
  Box,
  CircularProgress,
  Alert,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  IconButton,
  Chip,
  Divider,
  Pagination,
} from '@mui/material';
import {
  Notifications as NotificationsIcon,
  MarkEmailRead,
  Gavel,
  Payment,
  Star,
  Message as MessageIcon,
  Info,
} from '@mui/icons-material';
import { getNotifications, markNotificationRead } from '../services/touristApi';
import type { NotificationItem } from '../types/tourist.types';

const getNotificationIcon = (type: string) => {
  switch (type?.toLowerCase()) {
    case 'bid': return <Gavel color="primary" />;
    case 'payment': return <Payment color="success" />;
    case 'review': return <Star color="warning" />;
    case 'message': return <MessageIcon color="info" />;
    default: return <Info color="action" />;
  }
};

const Notifications = () => {
  const [notifications, setNotifications] = useState<NotificationItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchNotifications = async () => {
      setIsLoading(true);
      try {
        const data = await getNotifications(page, 20);
        setNotifications(data.items || []);
        setTotalCount(data.totalCount || 0);
      } catch {
        setError('Failed to load notifications.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchNotifications();
  }, [page]);

  const handleMarkRead = async (id: number) => {
    try {
      await markNotificationRead(id);
      setNotifications((prev) =>
        prev.map((n) => (n.id === id ? { ...n, isRead: true } : n))
      );
    } catch {
      // Silent fail for mark as read
    }
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
        <NotificationsIcon sx={{ mr: 1 }} />
        <Typography variant="h4">Notifications</Typography>
        {notifications.filter((n) => !n.isRead).length > 0 && (
          <Chip
            label={`${notifications.filter((n) => !n.isRead).length} unread`}
            color="error"
            size="small"
            sx={{ ml: 2 }}
          />
        )}
      </Box>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {isLoading ? (
        <Box display="flex" justifyContent="center" py={6}>
          <CircularProgress />
        </Box>
      ) : notifications.length === 0 ? (
        <Paper sx={{ p: 6, textAlign: 'center' }}>
          <NotificationsIcon sx={{ fontSize: 48, color: 'grey.400', mb: 2 }} />
          <Typography color="text.secondary">No notifications yet.</Typography>
        </Paper>
      ) : (
        <>
          <Paper>
            <List>
              {notifications.map((notification, index) => (
                <Box key={notification.id}>
                  {index > 0 && <Divider />}
                  <ListItem
                    sx={{
                      bgcolor: notification.isRead ? 'transparent' : 'action.hover',
                    }}
                    secondaryAction={
                      !notification.isRead ? (
                        <IconButton
                          edge="end"
                          onClick={() => handleMarkRead(notification.id)}
                          title="Mark as read"
                        >
                          <MarkEmailRead />
                        </IconButton>
                      ) : undefined
                    }
                  >
                    <ListItemIcon>
                      {getNotificationIcon(notification.type)}
                    </ListItemIcon>
                    <ListItemText
                      primary={notification.title}
                      secondary={
                        <>
                          {notification.message}
                          <br />
                          <Typography variant="caption" color="text.secondary">
                            {new Date(notification.createdAt).toLocaleString()}
                          </Typography>
                        </>
                      }
                    />
                  </ListItem>
                </Box>
              ))}
            </List>
          </Paper>
          {totalCount > 20 && (
            <Box display="flex" justifyContent="center" sx={{ mt: 3 }}>
              <Pagination
                count={Math.ceil(totalCount / 20)}
                page={page}
                onChange={(_e, p) => setPage(p)}
                color="primary"
              />
            </Box>
          )}
        </>
      )}
    </Container>
  );
};

export default Notifications;
