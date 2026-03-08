import { useState, useEffect, useRef } from 'react';
import {
  Box,
  Paper,
  Typography,
  List,
  ListItemButton,
  ListItemAvatar,
  ListItemText,
  Avatar,
  TextField,
  Button,
  Badge,
  Divider,
  CircularProgress,
  Alert,
  IconButton,
} from '@mui/material';
import {
  Send as SendIcon,
  ArrowBack as ArrowBackIcon,
} from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import { useAuth } from '../hooks/useAuth';
import type { Conversation, Message } from '../types/guide.types';

const Messages = () => {
  const { t } = useTranslation();
  const { user } = useAuth();
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [selectedConversation, setSelectedConversation] = useState<Conversation | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);
  const [newMessage, setNewMessage] = useState('');
  const [loadingConvs, setLoadingConvs] = useState(true);
  const [loadingMsgs, setLoadingMsgs] = useState(false);
  const [convError, setConvError] = useState('');
  const [msgError, setMsgError] = useState('');
  const [sendError, setSendError] = useState('');
  const [isMobileConvVisible, setIsMobileConvVisible] = useState(true);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    guideApi
      .getConversations()
      .then((r) => setConversations(r.items ?? []))
      .catch(() => setConvError(t('messages.loadError')))
      .finally(() => setLoadingConvs(false));
  }, [t]);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const handleSelectConversation = async (conv: Conversation) => {
    setSelectedConversation(conv);
    setIsMobileConvVisible(false);
    setMessages([]);
    setLoadingMsgs(true);
    setMsgError('');
    try {
      const result = await guideApi.getMessages(conv.id);
      setMessages(result.items ?? []);
      // Mark as read
      await guideApi.markConversationRead(conv.id).catch(() => {});
      setConversations((prev) =>
        prev.map((c) => c.id === conv.id ? { ...c, unreadCount: 0 } : c)
      );
    } catch {
      setMsgError(t('messages.messagesLoadError'));
    } finally {
      setLoadingMsgs(false);
    }
  };

  const handleSend = async () => {
    if (!newMessage.trim() || !selectedConversation) return;
    setSendError('');
    const content = newMessage.trim();
    setNewMessage('');
    try {
      const sent = await guideApi.sendMessage({ conversationId: selectedConversation.id, content });
      setMessages((prev) => [...prev, sent]);
    } catch {
      setSendError(t('messages.sendError'));
      setNewMessage(content); // restore on error
    }
  };

  const convName = (c: Conversation) => c.participantName ?? c.touristName ?? '';

  return (
    <Box>
      <Typography variant="h4" gutterBottom>{t('messages.title')}</Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>{t('messages.subtitle')}</Typography>

      <Paper elevation={2} sx={{ display: 'flex', height: 600, overflow: 'hidden' }}>
        {/* Conversations panel */}
        <Box
          sx={{
            width: { xs: '100%', sm: 280 },
            borderRight: { sm: '1px solid' },
            borderColor: 'divider',
            display: {
              xs: isMobileConvVisible ? 'block' : 'none',
              sm: 'block',
            },
            overflowY: 'auto',
          }}
        >
          <Box sx={{ p: 2, borderBottom: '1px solid', borderColor: 'divider' }}>
            <Typography variant="h6">{t('messages.conversations')}</Typography>
          </Box>
          {loadingConvs ? (
            <Box sx={{ display: 'flex', justifyContent: 'center', pt: 4 }}>
              <CircularProgress size={24} />
            </Box>
          ) : convError ? (
            <Alert severity="error" sx={{ m: 1 }}>{convError}</Alert>
          ) : (
            <List disablePadding>
              {conversations.map((conv) => (
                <Box key={conv.id}>
                  <ListItemButton
                    selected={selectedConversation?.id === conv.id}
                    onClick={() => handleSelectConversation(conv)}
                  >
                    <ListItemAvatar>
                      <Badge badgeContent={conv.unreadCount} color="primary">
                        <Avatar>{convName(conv)[0] ?? '?'}</Avatar>
                      </Badge>
                    </ListItemAvatar>
                    <ListItemText
                      primary={convName(conv)}
                      secondary={conv.lastMessage}
                      secondaryTypographyProps={{ noWrap: true }}
                    />
                  </ListItemButton>
                  <Divider />
                </Box>
              ))}
            </List>
          )}
        </Box>

        {/* Message panel */}
        <Box
          sx={{
            flexGrow: 1,
            display: {
              xs: isMobileConvVisible ? 'none' : 'flex',
              sm: 'flex',
            },
            flexDirection: 'column',
          }}
        >
          {!selectedConversation ? (
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
              <Typography color="text.secondary">{t('messages.selectConversation')}</Typography>
            </Box>
          ) : (
            <>
              <Box sx={{ p: 2, borderBottom: '1px solid', borderColor: 'divider', display: 'flex', alignItems: 'center', gap: 1 }}>
                <IconButton sx={{ display: { sm: 'none' } }} onClick={() => setIsMobileConvVisible(true)}>
                  <ArrowBackIcon />
                </IconButton>
                <Avatar>{convName(selectedConversation)[0] ?? '?'}</Avatar>
                <Typography variant="h6">{convName(selectedConversation)}</Typography>
              </Box>

              <Box sx={{ flexGrow: 1, overflowY: 'auto', p: 2 }}>
                {loadingMsgs ? (
                  <Box sx={{ display: 'flex', justifyContent: 'center', pt: 4 }}>
                    <CircularProgress size={24} />
                  </Box>
                ) : msgError ? (
                  <Alert severity="error">{msgError}</Alert>
                ) : (
                  messages.map((msg) => {
                    const isMe = msg.senderId === user?.id;
                    return (
                      <Box
                        key={msg.id}
                        sx={{
                          display: 'flex',
                          justifyContent: isMe ? 'flex-end' : 'flex-start',
                          mb: 1,
                        }}
                      >
                        <Box
                          sx={{
                            maxWidth: '70%',
                            bgcolor: isMe ? 'primary.main' : 'grey.200',
                            color: isMe ? 'white' : 'text.primary',
                            px: 2,
                            py: 1,
                            borderRadius: 2,
                          }}
                        >
                          <Typography variant="body2">{msg.content}</Typography>
                          <Typography variant="caption" sx={{ opacity: 0.7, display: 'block', textAlign: 'right' }}>
                            {new Date(msg.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                          </Typography>
                        </Box>
                      </Box>
                    );
                  })
                )}
                <div ref={messagesEndRef} />
              </Box>

              {sendError && <Alert severity="error" sx={{ mx: 2 }}>{sendError}</Alert>}
              <Box sx={{ p: 2, borderTop: '1px solid', borderColor: 'divider', display: 'flex', gap: 1 }}>
                <TextField
                  fullWidth
                  size="small"
                  placeholder={t('messages.typeMessage')}
                  value={newMessage}
                  onChange={(e) => setNewMessage(e.target.value)}
                  onKeyDown={(e) => { if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); handleSend(); } }}
                />
                <Button variant="contained" onClick={handleSend} disabled={!newMessage.trim()}>
                  <SendIcon />
                </Button>
              </Box>
            </>
          )}
        </Box>
      </Paper>
    </Box>
  );
};

export default Messages;
