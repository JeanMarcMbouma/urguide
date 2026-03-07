import { useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Chip,
  Alert,
  IconButton,
  Divider,
} from '@mui/material';
import {
  ChevronLeft as PrevIcon,
  ChevronRight as NextIcon,
  Block as BlockIcon,
} from '@mui/icons-material';
import { guideApi } from '../services/guideApi';

// Zero-indexed to match JavaScript's Date.getDay() convention (0 = Sunday).
const DAYS_OF_WEEK = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
const MONTH_NAMES = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December',
];

type DayStatus = 'available' | 'blocked' | 'booked';

const Availability = () => {
  const today = new Date();
  const [currentYear, setCurrentYear] = useState(today.getFullYear());
  const [currentMonth, setCurrentMonth] = useState(today.getMonth());
  const [blockOpen, setBlockOpen] = useState(false);
  const [recurringOpen, setRecurringOpen] = useState(false);
  const [blockStart, setBlockStart] = useState('');
  const [blockEnd, setBlockEnd] = useState('');
  const [blockReason, setBlockReason] = useState('');
  const [recurringType, setRecurringType] = useState<'weekly' | 'monthly'>('weekly');
  const [recurringDay, setRecurringDay] = useState(0);
  const [recurringEndDate, setRecurringEndDate] = useState('');
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  // Sample blocked dates for demo
  const [blockedDates] = useState<Set<string>>(new Set(['2024-03-10', '2024-03-11', '2024-03-12']));

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 4000);
  };

  const navigateMonth = (direction: number) => {
    const newDate = new Date(currentYear, currentMonth + direction, 1);
    setCurrentYear(newDate.getFullYear());
    setCurrentMonth(newDate.getMonth());
  };

  const getDaysInMonth = (year: number, month: number) => new Date(year, month + 1, 0).getDate();
  const getFirstDayOfMonth = (year: number, month: number) => new Date(year, month, 1).getDay();

  const getDayStatus = (dateStr: string): DayStatus => {
    if (blockedDates.has(dateStr)) return 'blocked';
    return 'available';
  };

  const getDayColor = (status: DayStatus) => {
    switch (status) {
      case 'blocked': return '#ef5350';
      case 'booked': return '#1976d2';
      default: return '#43a047';
    }
  };

  const handleBlockDates = async () => {
    if (!blockStart || !blockEnd) return;
    try {
      await guideApi.blockDates('me', { startDate: blockStart, endDate: blockEnd, reason: blockReason });
      setBlockOpen(false);
      setBlockStart('');
      setBlockEnd('');
      setBlockReason('');
      showAlert('success', 'Dates blocked successfully.');
    } catch {
      showAlert('error', 'Failed to block dates.');
    }
  };

  const handleSetRecurring = async () => {
    try {
      await guideApi.setRecurringPattern('me', {
        type: recurringType,
        ...(recurringType === 'weekly' ? { dayOfWeek: recurringDay } : { dayOfMonth: recurringDay }),
        endDate: recurringEndDate || undefined,
      });
      setRecurringOpen(false);
      showAlert('success', 'Recurring pattern set successfully.');
    } catch {
      showAlert('error', 'Failed to set recurring pattern.');
    }
  };

  const daysInMonth = getDaysInMonth(currentYear, currentMonth);
  const firstDay = getFirstDayOfMonth(currentYear, currentMonth);

  const calendarCells: (null | number)[] = [
    ...Array(firstDay).fill(null),
    ...Array.from({ length: daysInMonth }, (_, i) => i + 1),
  ];

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Availability Calendar</Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button variant="outlined" startIcon={<BlockIcon />} onClick={() => setBlockOpen(true)}>
            Block Dates
          </Button>
          <Button variant="outlined" onClick={() => setRecurringOpen(true)}>
            Recurring Pattern
          </Button>
        </Box>
      </Box>

      {alert && (
        <Alert severity={alert.type} sx={{ mb: 2 }}>
          {alert.message}
        </Alert>
      )}

      <Grid container spacing={3}>
        <Grid item xs={12} md={9}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
              <IconButton onClick={() => navigateMonth(-1)}>
                <PrevIcon />
              </IconButton>
              <Typography variant="h6">
                {MONTH_NAMES[currentMonth]} {currentYear}
              </Typography>
              <IconButton onClick={() => navigateMonth(1)}>
                <NextIcon />
              </IconButton>
            </Box>

            <Grid container columns={7}>
              {DAYS_OF_WEEK.map((day) => (
                <Grid item xs={1} key={day}>
                  <Box sx={{ textAlign: 'center', py: 1 }}>
                    <Typography variant="caption" fontWeight="bold" color="text.secondary">
                      {day}
                    </Typography>
                  </Box>
                </Grid>
              ))}
              {calendarCells.map((day, idx) => {
                if (!day) {
                  return <Grid item xs={1} key={`empty-${idx}`} />;
                }
                const dateStr = `${currentYear}-${String(currentMonth + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
                const status = getDayStatus(dateStr);
                const isToday =
                  day === today.getDate() &&
                  currentMonth === today.getMonth() &&
                  currentYear === today.getFullYear();
                return (
                  <Grid item xs={1} key={day}>
                    <Box
                      sx={{
                        m: 0.5,
                        p: 1,
                        borderRadius: 1,
                        textAlign: 'center',
                        bgcolor: getDayColor(status),
                        color: 'white',
                        cursor: 'pointer',
                        border: isToday ? '2px solid #000' : 'none',
                        opacity: 0.85,
                        '&:hover': { opacity: 1 },
                      }}
                    >
                      <Typography variant="body2">{day}</Typography>
                    </Box>
                  </Grid>
                );
              })}
            </Grid>
          </Paper>
        </Grid>

        <Grid item xs={12} md={3}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Legend
            </Typography>
            <Divider sx={{ mb: 2 }} />
            {[
              { label: 'Available', color: '#43a047' },
              { label: 'Blocked', color: '#ef5350' },
              { label: 'Booked', color: '#1976d2' },
            ].map((item) => (
              <Box key={item.label} sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                <Box sx={{ width: 16, height: 16, borderRadius: 0.5, bgcolor: item.color }} />
                <Typography variant="body2">{item.label}</Typography>
              </Box>
            ))}
          </Paper>
        </Grid>
      </Grid>

      {/* Block Dates Dialog */}
      <Dialog open={blockOpen} onClose={() => setBlockOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Block Dates</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Start Date"
            type="date"
            value={blockStart}
            onChange={(e) => setBlockStart(e.target.value)}
            InputLabelProps={{ shrink: true }}
            sx={{ mt: 1, mb: 2 }}
          />
          <TextField
            fullWidth
            label="End Date"
            type="date"
            value={blockEnd}
            onChange={(e) => setBlockEnd(e.target.value)}
            InputLabelProps={{ shrink: true }}
            sx={{ mb: 2 }}
          />
          <TextField
            fullWidth
            label="Reason (optional)"
            value={blockReason}
            onChange={(e) => setBlockReason(e.target.value)}
            placeholder="e.g., Personal holiday, medical appointment..."
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setBlockOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleBlockDates} disabled={!blockStart || !blockEnd}>
            Block Dates
          </Button>
        </DialogActions>
      </Dialog>

      {/* Recurring Pattern Dialog */}
      <Dialog open={recurringOpen} onClose={() => setRecurringOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Set Recurring Availability Pattern</DialogTitle>
        <DialogContent>
          <FormControl fullWidth sx={{ mt: 1, mb: 2 }}>
            <InputLabel>Pattern Type</InputLabel>
            <Select
              value={recurringType}
              label="Pattern Type"
              onChange={(e) => setRecurringType(e.target.value as 'weekly' | 'monthly')}
            >
              <MenuItem value="weekly">Weekly</MenuItem>
              <MenuItem value="monthly">Monthly</MenuItem>
            </Select>
          </FormControl>

          {recurringType === 'weekly' ? (
            <FormControl fullWidth sx={{ mb: 2 }}>
              <InputLabel>Day of Week</InputLabel>
              <Select
                value={recurringDay}
                label="Day of Week"
                onChange={(e) => setRecurringDay(Number(e.target.value))}
              >
                {DAYS_OF_WEEK.map((d, i) => (
                  <MenuItem key={d} value={i}>{d}</MenuItem>
                ))}
              </Select>
            </FormControl>
          ) : (
            <TextField
              fullWidth
              label="Day of Month"
              type="number"
              value={recurringDay}
              onChange={(e) => setRecurringDay(Number(e.target.value))}
              inputProps={{ min: 1, max: 31 }}
              sx={{ mb: 2 }}
            />
          )}

          <TextField
            fullWidth
            label="End Date (optional)"
            type="date"
            value={recurringEndDate}
            onChange={(e) => setRecurringEndDate(e.target.value)}
            InputLabelProps={{ shrink: true }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRecurringOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSetRecurring}>
            Save Pattern
          </Button>
        </DialogActions>
      </Dialog>

      {/* Active patterns preview */}
      <Paper elevation={2} sx={{ p: 3, mt: 3 }}>
        <Typography variant="h6" gutterBottom>
          Active Patterns
        </Typography>
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
          <Chip label="Blocked: Mar 10–12, 2024" onDelete={() => {}} color="error" variant="outlined" />
        </Box>
      </Paper>
    </Box>
  );
};

export default Availability;
