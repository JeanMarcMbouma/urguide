import { useState, useEffect, useRef } from 'react';
import {
  Container,
  Typography,
  Paper,
  Grid,
  Box,
  TextField,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Avatar,
  Badge,
  Divider,
  CircularProgress,
  Alert,
  IconButton,
} from '@mui/material';
import { Send as SendIcon } from '@mui/icons-material';
import { getConversations, getMessages, sendMessage, markConversationRead } from '../services/touristApi';
import type { Conversation, Message as MessageType } from '../types/tourist.types';

const Messages = () => {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [selectedConversation, setSelectedConversation] = useState<number | null>(null);
  const [messages, setMessages] = useState<MessageType[]>([]);
  const [newMessage, setNewMessage] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [isSending, setIsSending] = useState(false);
  const [error, setError] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const fetchConversations = async () => {
      try {
        const data = await getConversations();
        setConversations(data.items || []);
      } catch {
        setError('Failed to load conversations.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchConversations();
  }, []);

  useEffect(() => {
    if (!selectedConversation) return;
    const fetchMessages = async () => {
      try {
        const data = await getMessages(selectedConversation);
        setMessages(data.items || []);
        await markConversationRead(selectedConversation);
        setConversations((prev) =>
          prev.map((c) =>
            c.id === selectedConversation ? { ...c, unreadCount: 0 } : c
          )
        );
      } catch {
        setError('Failed to load messages.');
      }
    };
    fetchMessages();
  }, [selectedConversation]);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const handleSend = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newMessage.trim() || !selectedConversation) return;
    setIsSending(true);
    try {
      const msg = await sendMessage({
        conversationId: selectedConversation,
        content: newMessage.trim(),
      });
      setMessages((prev) => [...prev, msg]);
      setNewMessage('');
    } catch {
      setError('Failed to send message.');
    } finally {
      setIsSending(false);
    }
  };

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>Messages</Typography>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {isLoading ? (
        <Box display="flex" justifyContent="center" py={6}>
          <CircularProgress />
        </Box>
      ) : (
        <Paper sx={{ height: '70vh', display: 'flex' }}>
          <Grid container sx={{ height: '100%' }}>
            {/* Conversations List */}
            <Grid size={{ xs: 12, md: 4 }} sx={{ borderRight: 1, borderColor: 'divider', overflow: 'auto' }}>
              <List>
                {conversations.length === 0 ? (
                  <ListItem>
                    <ListItemText
                      primary={<Typography color="text.secondary" align="center">No conversations</Typography>}
                    />
                  </ListItem>
                ) : (
                  conversations.map((conv) => (
                    <Box key={conv.id}>
                      <ListItem
                        component="div"
                        onClick={() => setSelectedConversation(conv.id)}
                        sx={{
                          cursor: 'pointer',
                          bgcolor: selectedConversation === conv.id ? 'action.selected' : 'transparent',
                          '&:hover': { bgcolor: 'action.hover' },
                        }}
                      >
                        <ListItemAvatar>
                          <Badge badgeContent={conv.unreadCount} color="error">
                            <Avatar>{conv.participantName?.[0] || 'U'}</Avatar>
                          </Badge>
                        </ListItemAvatar>
                        <ListItemText
                          primary={conv.participantName}
                          secondary={conv.lastMessage}
                          secondaryTypographyProps={{ noWrap: true }}
                        />
                      </ListItem>
                      <Divider />
                    </Box>
                  ))
                )}
              </List>
            </Grid>

            {/* Messages */}
            <Grid size={{ xs: 12, md: 8 }} sx={{ display: 'flex', flexDirection: 'column' }}>
              {!selectedConversation ? (
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                  <Typography color="text.secondary">Select a conversation to view messages</Typography>
                </Box>
              ) : (
                <>
                  <Box sx={{ flexGrow: 1, overflow: 'auto', p: 2 }}>
                    {messages.map((msg) => (
                      <Box
                        key={msg.id}
                        sx={{
                          display: 'flex',
                          justifyContent: msg.senderId === 'me' ? 'flex-end' : 'flex-start',
                          mb: 1,
                        }}
                      >
                        <Paper
                          sx={{
                            p: 1.5,
                            maxWidth: '70%',
                            bgcolor: msg.senderId === 'me' ? 'primary.main' : 'grey.100',
                            color: msg.senderId === 'me' ? 'white' : 'text.primary',
                          }}
                        >
                          <Typography variant="body2">{msg.content}</Typography>
                          <Typography
                            variant="caption"
                            sx={{ opacity: 0.7, display: 'block', textAlign: 'right', mt: 0.5 }}
                          >
                            {new Date(msg.createdAt).toLocaleTimeString()}
                          </Typography>
                        </Paper>
                      </Box>
                    ))}
                    <div ref={messagesEndRef} />
                  </Box>
                  <Divider />
                  <Box component="form" onSubmit={handleSend} sx={{ p: 2, display: 'flex', gap: 1 }}>
                    <TextField
                      fullWidth
                      placeholder="Type a message..."
                      value={newMessage}
                      onChange={(e) => setNewMessage(e.target.value)}
                      size="small"
                      disabled={isSending}
                    />
                    <IconButton
                      type="submit"
                      color="primary"
                      disabled={!newMessage.trim() || isSending}
                    >
                      {isSending ? <CircularProgress size={24} /> : <SendIcon />}
                    </IconButton>
                  </Box>
                </>
              )}
            </Grid>
          </Grid>
        </Paper>
      )}
    </Container>
  );
};

export default Messages;
