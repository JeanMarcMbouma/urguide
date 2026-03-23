import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Container,
  Typography,
  TextField,
  Button,
  Grid,
  Card,
  CardContent,
  CardMedia,
  CardActions,
  Rating,
  Chip,
  Paper,
  InputAdornment,
  CircularProgress,
  Alert,
} from '@mui/material';
import {
  Search as SearchIcon,
  TravelExplore,
  EventAvailable,
  Explore,
} from '@mui/icons-material';
import { getPopularTours, getRecommendations } from '../services/touristApi';
import type { TourPreview } from '../types/tourist.types';

const Home = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [popularTours, setPopularTours] = useState<TourPreview[]>([]);
  const [recommendations, setRecommendations] = useState<TourPreview[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [popular, recommended] = await Promise.all([
          getPopularTours(6).catch(() => []),
          getRecommendations().catch(() => []),
        ]);
        setPopularTours(popular);
        setRecommendations(recommended);
      } catch {
        setError('Failed to load content. Please try again later.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, []);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      navigate(`/search?q=${encodeURIComponent(searchQuery.trim())}`);
    }
  };

  const steps = [
    { icon: <TravelExplore sx={{ fontSize: 48 }} />, title: 'Search', description: 'Find the perfect guide for your destination' },
    { icon: <EventAvailable sx={{ fontSize: 48 }} />, title: 'Book', description: 'Create a tour request and receive bids from guides' },
    { icon: <Explore sx={{ fontSize: 48 }} />, title: 'Explore', description: 'Enjoy your personalized tour experience' },
  ];

  const TourCard = ({ tour }: { tour: TourPreview }) => (
    <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <CardMedia
        component="div"
        sx={{ height: 160, bgcolor: 'grey.200', display: 'flex', alignItems: 'center', justifyContent: 'center' }}
      >
        <Explore sx={{ fontSize: 48, color: 'grey.400' }} />
      </CardMedia>
      <CardContent sx={{ flexGrow: 1 }}>
        <Typography gutterBottom variant="h6" component="div" noWrap>
          {tour.title || 'Tour Experience'}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1, height: 40, overflow: 'hidden' }}>
          {tour.description || 'Discover an amazing tour experience'}
        </Typography>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
          <Rating value={tour.rating} precision={0.5} size="small" readOnly />
          <Typography variant="body2" color="text.secondary" sx={{ ml: 1 }}>
            {tour.rating?.toFixed(1) || '0.0'}
          </Typography>
        </Box>
        <Chip label={`${tour.currency || '$'} ${tour.price || 0}`} color="primary" size="small" />
      </CardContent>
      <CardActions>
        <Button size="small" onClick={() => navigate(`/guides/${tour.id}`)}>View Details</Button>
      </CardActions>
    </Card>
  );

  return (
    <Box>
      {/* Hero Section */}
      <Box
        sx={{
          bgcolor: 'primary.main',
          color: 'white',
          py: 8,
          textAlign: 'center',
        }}
      >
        <Container maxWidth="md">
          <Typography variant="h3" component="h1" gutterBottom fontWeight="bold">
            Find Your Perfect Tour Guide
          </Typography>
          <Typography variant="h6" sx={{ mb: 4, opacity: 0.9 }}>
            Discover local experts who bring destinations to life
          </Typography>
          <Paper
            component="form"
            onSubmit={handleSearch}
            sx={{ p: 1, display: 'flex', alignItems: 'center', maxWidth: 600, mx: 'auto' }}
          >
            <TextField
              fullWidth
              placeholder="Search destinations, guides, or experiences..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              variant="standard"
              slotProps={{
                input: {
                  disableUnderline: true,
                  startAdornment: (
                    <InputAdornment position="start">
                      <SearchIcon color="action" />
                    </InputAdornment>
                  ),
                },
              }}
              sx={{ ml: 1 }}
            />
            <Button type="submit" variant="contained" sx={{ ml: 1, px: 3 }}>
              Search
            </Button>
          </Paper>
        </Container>
      </Box>

      <Container maxWidth="lg" sx={{ py: 6 }}>
        {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        {/* How It Works */}
        <Typography variant="h4" align="center" gutterBottom sx={{ mb: 4 }}>
          How It Works
        </Typography>
        <Grid container spacing={4} sx={{ mb: 8 }}>
          {steps.map((step, index) => (
            <Grid key={index} size={{ xs: 12, md: 4 }}>
              <Paper sx={{ p: 4, textAlign: 'center', height: '100%' }} elevation={2}>
                <Box sx={{ color: 'primary.main', mb: 2 }}>{step.icon}</Box>
                <Typography variant="h6" gutterBottom>{step.title}</Typography>
                <Typography variant="body2" color="text.secondary">{step.description}</Typography>
              </Paper>
            </Grid>
          ))}
        </Grid>

        {/* Popular Tours */}
        <Typography variant="h4" gutterBottom sx={{ mb: 3 }}>
          Popular Tours
        </Typography>
        {isLoading ? (
          <Box display="flex" justifyContent="center" py={4}>
            <CircularProgress />
          </Box>
        ) : (
          <Grid container spacing={3} sx={{ mb: 6 }}>
            {popularTours.length > 0 ? (
              popularTours.map((tour) => (
                <Grid key={tour.id} size={{ xs: 12, sm: 6, md: 4 }}>
                  <TourCard tour={tour} />
                </Grid>
              ))
            ) : (
              <Grid size={{ xs: 12 }}>
                <Paper sx={{ p: 4, textAlign: 'center' }}>
                  <Typography color="text.secondary">
                    No tours available yet. Check back soon!
                  </Typography>
                </Paper>
              </Grid>
            )}
          </Grid>
        )}

        {/* Recommendations */}
        {recommendations.length > 0 && (
          <>
            <Typography variant="h4" gutterBottom sx={{ mb: 3 }}>
              Recommended For You
            </Typography>
            <Grid container spacing={3}>
              {recommendations.map((tour) => (
                <Grid key={tour.id} size={{ xs: 12, sm: 6, md: 4 }}>
                  <TourCard tour={tour} />
                </Grid>
              ))}
            </Grid>
          </>
        )}
      </Container>
    </Box>
  );
};

export default Home;
