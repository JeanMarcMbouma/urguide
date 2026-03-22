import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Grid,
  Card,
  CardContent,
  CardActions,
  Avatar,
  Chip,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  Pagination,
  CircularProgress,
  Alert,
  InputAdornment,
} from '@mui/material';
import {
  Search as SearchIcon,
  Group as GroupIcon,
  AttachMoney as MoneyIcon,
  CalendarToday as CalendarIcon,
} from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import type { TourRequest } from '../types/guide.types';

const TOUR_REQUEST_STATUS_OPTIONS = ['all', 'pending', 'accepted', 'rejected'];

const statusColors: Record<string, 'default' | 'warning' | 'success' | 'error'> = {
  pending: 'warning',
  accepted: 'success',
  rejected: 'error',
};

const TourRequests = () => {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const [requests, setRequests] = useState<TourRequest[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [detailRequest, setDetailRequest] = useState<TourRequest | null>(null);
  const [page, setPage] = useState(1);

  const load = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const result = await guideApi.getTourRequests({ status: statusFilter, searchTerm }, page);
      setRequests(result.items ?? []);
      setTotalCount(result.totalCount ?? 0);
    } catch {
      setError(t('tourRequests.loadError'));
    } finally {
      setLoading(false);
    }
  }, [statusFilter, searchTerm, page, t]);

  useEffect(() => {
    load();
  }, [load]);

  const displayName = (r: TourRequest) =>
    r.requesterName ?? r.touristName ?? '';

  const destination = (r: TourRequest) =>
    r.regionName ?? r.destination ?? '';

  const budget = (r: TourRequest) =>
    r.maxBudget ?? r.budget ?? 0;

  const groupSize = (r: TourRequest) =>
    r.maxParticipants ?? r.groupSize ?? 0;

  const dateDisplay = (r: TourRequest) => {
    if (r.preferredDate) return r.preferredDate;
    if (r.startDate) return `${r.startDate}${r.endDate ? ` – ${r.endDate}` : ''}`;
    return '—';
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('tourRequests.title')}
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        {t('tourRequests.subtitle')}
      </Typography>

      <Paper elevation={2} sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid size={{ xs: 12, sm: 7 }}>
            <TextField
              fullWidth
              placeholder={t('tourRequests.search')}
              value={searchTerm}
              onChange={(e) => { setSearchTerm(e.target.value); setPage(1); }}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon sx={{ color: 'text.secondary' }} />
                  </InputAdornment>
                ),
              }}
              size="small"
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 5 }}>
            <FormControl fullWidth size="small">
              <InputLabel>{t('tourRequests.status')}</InputLabel>
              <Select
                value={statusFilter}
                label={t('tourRequests.status')}
                onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}
              >
                {TOUR_REQUEST_STATUS_OPTIONS.map((s) => (
                  <MenuItem key={s} value={s}>
                    {t(`tourRequests.${s}` as const) ?? s.charAt(0).toUpperCase() + s.slice(1)}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
        </Grid>
      </Paper>

      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <CircularProgress />
          <Typography sx={{ ml: 2 }}>{t('tourRequests.loading')}</Typography>
        </Box>
      ) : error ? (
        <Alert severity="error">{error}</Alert>
      ) : (
        <>
          <Grid container spacing={2}>
            {requests.map((request) => (
              <Grid size={{ xs: 12, md: 6 }} key={request.id}>
                <Card elevation={2}>
                  <CardContent>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Avatar src={request.touristAvatar}>{displayName(request)[0] ?? '?'}</Avatar>
                        <Typography variant="subtitle1" fontWeight="bold">
                          {displayName(request)}
                        </Typography>
                      </Box>
                      <Chip
                        label={request.status}
                        color={statusColors[request.status] ?? 'default'}
                        size="small"
                      />
                    </Box>
                    <Typography variant="h6" gutterBottom>
                      {request.title}
                    </Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                      {request.description.substring(0, 120)}{request.description.length > 120 ? '...' : ''}
                    </Typography>
                    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, mt: 1 }}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <CalendarIcon fontSize="small" color="action" />
                        <Typography variant="body2">{dateDisplay(request)}</Typography>
                      </Box>
                      {groupSize(request) > 0 && (
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                          <GroupIcon fontSize="small" color="action" />
                          <Typography variant="body2">{groupSize(request)} {t('tourRequests.people')}</Typography>
                        </Box>
                      )}
                      {budget(request) > 0 && (
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                          <MoneyIcon fontSize="small" color="action" />
                          <Typography variant="body2">{t('tourRequests.budget')}: ${budget(request)}</Typography>
                        </Box>
                      )}
                      {destination(request) && (
                        <Typography variant="body2" color="text.secondary">
                          📍 {destination(request)}
                        </Typography>
                      )}
                    </Box>
                  </CardContent>
                  <CardActions>
                    <Button size="small" onClick={() => setDetailRequest(request)}>
                      {t('tourRequests.viewDetails')}
                    </Button>
                    {request.status === 'pending' && (
                      <Button
                        size="small"
                        variant="contained"
                        onClick={() => navigate(`/bids?requestId=${request.id}`)}
                      >
                        {t('tourRequests.placeBid')}
                      </Button>
                    )}
                  </CardActions>
                </Card>
              </Grid>
            ))}
            {requests.length === 0 && (
              <Grid size={{ xs: 12 }}>
                <Paper elevation={1} sx={{ p: 4, textAlign: 'center' }}>
                  <Typography color="text.secondary">{t('tourRequests.noRequests')}</Typography>
                </Paper>
              </Grid>
            )}
          </Grid>

          <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
            <Pagination
              count={Math.ceil(totalCount / 10) || 1}
              page={page}
              onChange={(_, v) => setPage(v)}
              color="primary"
            />
          </Box>
        </>
      )}

      {/* Detail Dialog */}
      <Dialog open={!!detailRequest} onClose={() => setDetailRequest(null)} maxWidth="sm" fullWidth>
        {detailRequest && (
          <>
            <DialogTitle>{detailRequest.title}</DialogTitle>
            <DialogContent>
              <DialogContentText component="div">
                <Typography variant="body1" gutterBottom>
                  {detailRequest.description}
                </Typography>
                <Box sx={{ mt: 2, display: 'flex', flexDirection: 'column', gap: 1 }}>
                  {destination(detailRequest) && (
                    <Typography variant="body2">
                      <strong>{t('tourRequests.destination')}:</strong> {destination(detailRequest)}
                    </Typography>
                  )}
                  <Typography variant="body2">
                    <strong>{t('tourRequests.dates')}:</strong> {dateDisplay(detailRequest)}
                  </Typography>
                  {groupSize(detailRequest) > 0 && (
                    <Typography variant="body2">
                      <strong>{t('tourRequests.groupSize')}:</strong> {groupSize(detailRequest)} {t('tourRequests.people')}
                    </Typography>
                  )}
                  {budget(detailRequest) > 0 && (
                    <Typography variant="body2">
                      <strong>{t('tourRequests.budget')}:</strong> ${budget(detailRequest)}
                    </Typography>
                  )}
                  {displayName(detailRequest) && (
                    <Typography variant="body2">
                      <strong>{t('tourRequests.tourist')}:</strong> {displayName(detailRequest)}
                    </Typography>
                  )}
                </Box>
              </DialogContentText>
            </DialogContent>
            <Box sx={{ p: 2, display: 'flex', justifyContent: 'flex-end', gap: 1 }}>
              <Button onClick={() => setDetailRequest(null)}>{t('tourRequests.close')}</Button>
              {detailRequest.status === 'pending' && (
                <Button
                  variant="contained"
                  onClick={() => {
                    navigate(`/bids?requestId=${detailRequest.id}`);
                    setDetailRequest(null);
                  }}
                >
                  {t('tourRequests.placeBid')}
                </Button>
              )}
            </Box>
          </>
        )}
      </Dialog>
    </Box>
  );
};

export default TourRequests;
