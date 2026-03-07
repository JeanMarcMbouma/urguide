import { useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Avatar,
  Badge,
  TextField,
  IconButton,
  Divider,
  useTheme,
  useMediaQuery,
  Button,
} from '@mui/material';
import {
  Send as SendIcon,
  ArrowBack as BackIcon,
} from '@mui/icons-material';
import type { Conversation, Message } from '../types/guide.types';
import { guideApi } from '../services/guideApi';

const SAMPLE_CONVERSATIONS: Conversation[] = [
  {
    id: 'c1',
    touristId: 't1',
    touristName: 'Alice Johnson',
    touristAvatar: '',
    lastMessage: 'That sounds great! Can we confirm the meeting point?',
    lastMessageAt: '2024-02-21T15:30:00Z',
    unreadCount: 2,
  },
  {
    id: 'c2',
    touristId: 't2',
    touristName: 'Bob Martinez',
    touristAvatar: '',
    lastMessage: 'Thank you for the wonderful tour!',
    lastMessageAt: '2024-02-20T10:00:00Z',
    unreadCount: 0,
  },
  {
    id: 'c3',
    touristId: 't3',
    touristName: 'Carol Smith',
    touristAvatar: '',
    lastMessage: 'What should we bring for the hike?',
    lastMessageAt: '2024-02-19T08:00:00Z',
    unreadCount: 1,
  },
];

const SAMPLE_MESSAGES: Record<string, Message[]> = {
  c1: [
    { id: 'm1', conversationId: 'c1', senderId: 't1', senderName: 'Alice Johnson', content: 'Hi! I\'m excited about our upcoming tour.', sentAt: '2024-02-21T14:00:00Z', isRead: true },
    { id: 'm2', conversationId: 'c1', senderId: 'g1', senderName: 'You', content: 'Hi Alice! I\'m looking forward to it too. We\'ll meet at the Colosseum main entrance.', sentAt: '2024-02-21T14:30:00Z', isRead: true },
    { id: 'm3', conversationId: 'c1', senderId: 't1', senderName: 'Alice Johnson', content: 'That sounds great! Can we confirm the meeting point?', sentAt: '2024-02-21T15:30:00Z', isRead: false },
  ],
  c2: [
    { id: 'm4', conversationId: 'c2', senderId: 't2', senderName: 'Bob Martinez', content: 'Thank you for the wonderful tour!', sentAt: '2024-02-20T10:00:00Z', isRead: true },
    { id: 'm5', conversationId: 'c2', senderId: 'g1', senderName: 'You', content: 'It was my pleasure, Bob! Hope to see you again sometime.', sentAt: '2024-02-20T10:30:00Z', isRead: true },
  ],
};

const MY_GUIDE_ID = 'g1';

const Messages = () => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const [selectedConvo, setSelectedConvo] = useState<Conversation | null>(null);
  const [messages, setMessages] = useState<Record<string, Message[]>>(SAMPLE_MESSAGES);
  const [newMessage, setNewMessage] = useState('');
  const [showList, setShowList] = useState(true);

  const handleSelectConvo = (convo: Conversation) => {
    setSelectedConvo(convo);
    if (isMobile) setShowList(false);
    // Mark as read
    guideApi.markConversationRead(convo.id).catch(() => {});
  };

  const handleSend = async () => {
    if (!newMessage.trim() || !selectedConvo) return;
    const msg: Message = {
      id: `m${Date.now()}`,
      conversationId: selectedConvo.id,
      senderId: MY_GUIDE_ID,
      senderName: 'You',
      content: newMessage,
      sentAt: new Date().toISOString(),
      isRead: false,
    };
    setMessages((prev) => ({
      ...prev,
      [selectedConvo.id]: [...(prev[selectedConvo.id] ?? []), msg],
    }));
    setNewMessage('');
    try {
      await guideApi.sendMessage({ conversationId: selectedConvo.id, content: msg.content });
    } catch {
      // optimistic update already applied; silently fail for demo
    }
  };

  const conversationMessages = selectedConvo ? (messages[selectedConvo.id] ?? []) : [];

  const ConversationList = (
    <Box sx={{ width: { xs: '100%', md: 300 }, flexShrink: 0, borderRight: { md: '1px solid' }, borderColor: 'divider' }}>
      <Box sx={{ p: 2, bgcolor: 'primary.main', color: 'white' }}>
        <Typography variant="h6">Conversations</Typography>
      </Box>
      <List disablePadding>
        {SAMPLE_CONVERSATIONS.map((convo, index) => (
          <Box key={convo.id}>
            <ListItem
              component="div"
              onClick={() => handleSelectConvo(convo)}
              sx={{
                cursor: 'pointer',
                bgcolor: selectedConvo?.id === convo.id ? 'action.selected' : 'transparent',
                '&:hover': { bgcolor: 'action.hover' },
              }}
            >
              <ListItemAvatar>
                <Badge badgeContent={convo.unreadCount} color="error">
                  <Avatar src={convo.touristAvatar}>{convo.touristName[0]}</Avatar>
                </Badge>
              </ListItemAvatar>
              <ListItemText
                primary={convo.touristName}
                secondary={convo.lastMessage}
                primaryTypographyProps={{ fontWeight: convo.unreadCount > 0 ? 'bold' : 'normal' }}
                secondaryTypographyProps={{ noWrap: true, maxWidth: 180 }}
              />
            </ListItem>
            {index < SAMPLE_CONVERSATIONS.length - 1 && <Divider />}
          </Box>
        ))}
      </List>
    </Box>
  );

  const MessageThread = (
    <Box sx={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>
      {selectedConvo ? (
        <>
          <Box sx={{ p: 2, bgcolor: 'grey.100', display: 'flex', alignItems: 'center', gap: 1, borderBottom: '1px solid', borderColor: 'divider' }}>
            {isMobile && (
              <IconButton size="small" onClick={() => setShowList(true)}>
                <BackIcon />
              </IconButton>
            )}
            <Avatar src={selectedConvo.touristAvatar}>{selectedConvo.touristName[0]}</Avatar>
            <Typography variant="h6">{selectedConvo.touristName}</Typography>
          </Box>

          <Box sx={{ flex: 1, p: 2, overflowY: 'auto', display: 'flex', flexDirection: 'column', gap: 1 }}>
            {conversationMessages.map((msg) => {
              const isGuide = msg.senderId === MY_GUIDE_ID;
              return (
                <Box
                  key={msg.id}
                  sx={{
                    alignSelf: isGuide ? 'flex-end' : 'flex-start',
                    maxWidth: '70%',
                  }}
                >
                  <Paper
                    elevation={1}
                    sx={{
                      p: 1.5,
                      bgcolor: isGuide ? 'primary.main' : 'grey.100',
                      color: isGuide ? 'white' : 'inherit',
                      borderRadius: 2,
                    }}
                  >
                    <Typography variant="body2">{msg.content}</Typography>
                    <Typography
                      variant="caption"
                      sx={{ opacity: 0.75, display: 'block', textAlign: 'right', mt: 0.5 }}
                    >
                      {new Date(msg.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                    </Typography>
                  </Paper>
                </Box>
              );
            })}
          </Box>

          <Box sx={{ p: 2, borderTop: '1px solid', borderColor: 'divider', display: 'flex', gap: 1 }}>
            <TextField
              fullWidth
              size="small"
              placeholder="Type a message..."
              value={newMessage}
              onChange={(e) => setNewMessage(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter' && !e.shiftKey) {
                  e.preventDefault();
                  handleSend();
                }
              }}
            />
            <IconButton
              color="primary"
              onClick={handleSend}
              disabled={!newMessage.trim()}
            >
              <SendIcon />
            </IconButton>
          </Box>
        </>
      ) : (
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', flex: 1 }}>
          <Typography color="text.secondary">Select a conversation to start messaging</Typography>
        </Box>
      )}
    </Box>
  );

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Messages
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Communicate with your clients.
      </Typography>

      <Paper elevation={2} sx={{ display: 'flex', height: 560, overflow: 'hidden' }}>
        {isMobile ? (
          showList ? ConversationList : MessageThread
        ) : (
          <>
            {ConversationList}
            {MessageThread}
          </>
        )}
      </Paper>

      {isMobile && !showList && (
        <Button startIcon={<BackIcon />} sx={{ mt: 1 }} onClick={() => setShowList(true)}>
          Back to conversations
        </Button>
      )}
    </Box>
  );
};

export default Messages;
