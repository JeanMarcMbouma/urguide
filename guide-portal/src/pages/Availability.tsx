import { useState, useEffect, useCallback } from 'react';
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
  Alert,
  CircularProgress,
  Tooltip,
} from '@mui/material';
import {
  ChevronLeft as ChevronLeftIcon,
  ChevronRight as ChevronRightIcon,
  Block as BlockIcon,
  Repeat as RepeatIcon,
} from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import type { AvailabilitySlot, BlockDatesRequest, RecurringPattern } from '../types/guide.types';

const DAYS_OF_WEEK = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

const Availability = () => {
  const { t } = useTranslation();
  const today = new Date();
  const [currentMonth, setCurrentMonth] = useState(today.getMonth());
  const [currentYear, setCurrentYear] = useState(today.getFullYear());
  const [blockedDates, setBlockedDates] = useState<Set<string>>(new Set());
  const [slots, setSlots] = useState<AvailabilitySlot[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  const [blockOpen, setBlockOpen] = useState(false);
  const [recurringOpen, setRecurringOpen] = useState(false);
  const [blockForm, setBlockForm] = useState<BlockDatesRequest>({ startDate: '', endDate: '', reason: '' });
  const [recurringForm, setRecurringForm] = useState<RecurringPattern & { dayOfWeekStr: string; dayOfMonthStr: string }>({
    type: 'weekly',
    dayOfWeekStr: '0',
    dayOfMonthStr: '1',
    endDate: '',
  });

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 4000);
  };

  const getStartEnd = useCallback(() => {
    const start = `${currentYear}-${String(currentMonth + 1).padStart(2, '0')}-01`;
    const lastDay = new Date(currentYear, currentMonth + 1, 0).getDate();
    const end = `${currentYear}-${String(currentMonth + 1).padStart(2, '0')}-${String(lastDay).padStart(2, '0')}`;
    return { start, end };
  }, [currentYear, currentMonth]);

  const loadAvailability = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const { start, end } = getStartEnd();
      const fetchedSlots = await guideApi.getAvailability(start, end);
      setSlots(fetchedSlots);
      const blocked = new Set(
        fetchedSlots.filter((s) => s.isBlocked).map((s) => s.date.substring(0, 10))
      );
      setBlockedDates(blocked);
    } catch {
      setError(t('availability.loadError'));
    } finally {
      setLoading(false);
    }
  }, [getStartEnd, t]);

  useEffect(() => {
    loadAvailability();
  }, [loadAvailability]);

  const handleBlock = async () => {
    try {
      await guideApi.blockDates(blockForm);
      setBlockOpen(false);
      setBlockForm({ startDate: '', endDate: '', reason: '' });
      await loadAvailability();
      showAlert('success', t('availability.blockSuccess'));
    } catch {
      showAlert('error', t('availability.blockError'));
    }
  };

  const handleUnblock = async (dateStr: string) => {
    try {
      await guideApi.unblockDates(dateStr, dateStr);
      await loadAvailability();
      showAlert('success', t('availability.unblockSuccess'));
    } catch {
      showAlert('error', t('availability.blockError'));
    }
  };

  const handleSetRecurring = async () => {
    const pattern: RecurringPattern = { type: recurringForm.type };
    if (recurringForm.type === 'weekly') {
      pattern.dayOfWeek = parseInt(recurringForm.dayOfWeekStr, 10);
    } else {
      const dom = parseInt(recurringForm.dayOfMonthStr, 10);
      if (isNaN(dom) || dom < 1 || dom > 31) {
        showAlert('error', t('availability.invalidMonthDay'));
        return;
      }
      pattern.dayOfMonth = dom;
    }
    if (recurringForm.endDate) pattern.endDate = recurringForm.endDate;
    try {
      await guideApi.setRecurringPattern(pattern);
      setRecurringOpen(false);
      await loadAvailability();
      showAlert('success', t('availability.recurringSuccess'));
    } catch {
      showAlert('error', t('availability.recurringError'));
    }
  };

  // Build calendar grid
  const firstDay = new Date(currentYear, currentMonth, 1).getDay();
  const daysInMonth = new Date(currentYear, currentMonth + 1, 0).getDate();
  const monthName = new Date(currentYear, currentMonth).toLocaleString('default', { month: 'long', year: 'numeric' });

  const calendarCells: (number | null)[] = [
    ...Array(firstDay).fill(null),
    ...Array.from({ length: daysInMonth }, (_, i) => i + 1),
  ];

  const prevMonth = () => {
    if (currentMonth === 0) { setCurrentYear(y => y - 1); setCurrentMonth(11); }
    else setCurrentMonth(m => m - 1);
  };
  const nextMonth = () => {
    if (currentMonth === 11) { setCurrentYear(y => y + 1); setCurrentMonth(0); }
    else setCurrentMonth(m => m + 1);
  };

  const formatDateKey = (day: number) =>
    `${currentYear}-${String(currentMonth + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2, flexWrap: 'wrap', gap: 1 }}>
        <Typography variant="h4">{t('availability.title')}</Typography>
        <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
          <Button variant="outlined" startIcon={<BlockIcon />} onClick={() => setBlockOpen(true)}>
            {t('availability.blockDates')}
          </Button>
          <Button variant="outlined" startIcon={<RepeatIcon />} onClick={() => setRecurringOpen(true)}>
            {t('availability.setRecurring')}
          </Button>
        </Box>
      </Box>

      {alert && <Alert severity={alert.type} sx={{ mb: 2 }}>{alert.message}</Alert>}
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Paper elevation={2} sx={{ p: 3 }}>
        {/* Month navigation */}
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
          <Button onClick={prevMonth} startIcon={<ChevronLeftIcon />} />
          <Typography variant="h6">{monthName}</Typography>
          <Button onClick={nextMonth} endIcon={<ChevronRightIcon />} />
        </Box>

        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
            <CircularProgress />
            <Typography sx={{ ml: 2 }}>{t('availability.loading')}</Typography>
          </Box>
        ) : (
          <>
            {/* Day headers */}
            <Grid container columns={7} sx={{ mb: 1 }}>
              {['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map((d) => (
                <Grid item key={d} sx={{ textAlign: 'center', fontWeight: 'bold', py: 1 }}>
                  <Typography variant="caption">{d}</Typography>
                </Grid>
              ))}
            </Grid>

            {/* Calendar cells */}
            <Grid container columns={7}>
              {calendarCells.map((day, idx) => {
                if (!day) return <Grid item key={`empty-${idx}`} />;
                const dateKey = formatDateKey(day);
                const slot = slots.find((s) => s.date.substring(0, 10) === dateKey);
                const isBlocked = blockedDates.has(dateKey);
                const isToday =
                  day === today.getDate() &&
                  currentMonth === today.getMonth() &&
                  currentYear === today.getFullYear();
                return (
                  <Grid item key={dateKey}>
                    <Tooltip title={isBlocked ? (slot?.blockReason ?? t('availability.blocked')) : t('availability.available')}>
                      <Box
                        onClick={() => isBlocked && handleUnblock(dateKey)}
                        sx={{
                          m: 0.5,
                          p: 1,
                          minWidth: 36,
                          minHeight: 36,
                          borderRadius: 1,
                          textAlign: 'center',
                          cursor: isBlocked ? 'pointer' : 'default',
                          bgcolor: isBlocked ? 'error.light' : isToday ? 'primary.light' : 'grey.100',
                          color: isBlocked ? 'error.contrastText' : isToday ? 'primary.contrastText' : 'text.primary',
                          '&:hover': isBlocked ? { bgcolor: 'error.main', color: 'white' } : {},
                        }}
                      >
                        <Typography variant="caption">{day}</Typography>
                      </Box>
                    </Tooltip>
                  </Grid>
                );
              })}
            </Grid>

            <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <Box sx={{ width: 16, height: 16, bgcolor: 'grey.100', borderRadius: 0.5 }} />
                <Typography variant="caption">{t('availability.available')}</Typography>
              </Box>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <Box sx={{ width: 16, height: 16, bgcolor: 'error.light', borderRadius: 0.5 }} />
                <Typography variant="caption">{t('availability.blocked')}</Typography>
              </Box>
            </Box>
          </>
        )}
      </Paper>

      {/* Block Dates Dialog */}
      <Dialog open={blockOpen} onClose={() => setBlockOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('availability.blockTitle')}</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', gap: 2, mt: 1, mb: 2 }}>
            <TextField
              fullWidth
              type="date"
              label={t('availability.startDate')}
              value={blockForm.startDate}
              onChange={(e) => setBlockForm((f) => ({ ...f, startDate: e.target.value }))}
              InputLabelProps={{ shrink: true }}
            />
            <TextField
              fullWidth
              type="date"
              label={t('availability.endDate')}
              value={blockForm.endDate}
              onChange={(e) => setBlockForm((f) => ({ ...f, endDate: e.target.value }))}
              InputLabelProps={{ shrink: true }}
            />
          </Box>
          <TextField
            fullWidth
            label={t('availability.reason')}
            value={blockForm.reason}
            onChange={(e) => setBlockForm((f) => ({ ...f, reason: e.target.value }))}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setBlockOpen(false)}>{t('availability.cancel')}</Button>
          <Button
            variant="contained"
            onClick={handleBlock}
            disabled={!blockForm.startDate || !blockForm.endDate}
          >
            {t('availability.submit')}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Recurring Dialog */}
      <Dialog open={recurringOpen} onClose={() => setRecurringOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('availability.recurringTitle')}</DialogTitle>
        <DialogContent>
          <FormControl fullWidth sx={{ mt: 1, mb: 2 }}>
            <InputLabel>{t('availability.pattern')}</InputLabel>
            <Select
              value={recurringForm.type}
              label={t('availability.pattern')}
              onChange={(e) => setRecurringForm((f) => ({ ...f, type: e.target.value as 'weekly' | 'monthly' }))}
            >
              <MenuItem value="weekly">{t('availability.weekly')}</MenuItem>
              <MenuItem value="monthly">{t('availability.monthly')}</MenuItem>
            </Select>
          </FormControl>

          {recurringForm.type === 'weekly' ? (
            <FormControl fullWidth sx={{ mb: 2 }}>
              <InputLabel>{t('availability.dayOfWeek')}</InputLabel>
              <Select
                value={recurringForm.dayOfWeekStr}
                label={t('availability.dayOfWeek')}
                onChange={(e) => setRecurringForm((f) => ({ ...f, dayOfWeekStr: e.target.value }))}
              >
                {DAYS_OF_WEEK.map((d, i) => (
                  <MenuItem key={i} value={String(i)}>{d}</MenuItem>
                ))}
              </Select>
            </FormControl>
          ) : (
            <TextField
              fullWidth
              label={t('availability.dayOfMonth')}
              type="number"
              value={recurringForm.dayOfMonthStr}
              onChange={(e) => setRecurringForm((f) => ({ ...f, dayOfMonthStr: e.target.value }))}
              inputProps={{ min: 1, max: 31 }}
              sx={{ mb: 2 }}
            />
          )}

          <TextField
            fullWidth
            type="date"
            label={t('availability.endDateOptional')}
            value={recurringForm.endDate}
            onChange={(e) => setRecurringForm((f) => ({ ...f, endDate: e.target.value }))}
            InputLabelProps={{ shrink: true }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRecurringOpen(false)}>{t('availability.cancel')}</Button>
          <Button variant="contained" onClick={handleSetRecurring}>
            {t('availability.setPattern')}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Availability;
