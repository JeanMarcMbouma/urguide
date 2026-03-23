import { useState, useEffect, useCallback, useRef } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import {
  Box,
  Container,
  Typography,
  TextField,
  Button,
  Grid,
  Card,
  CardContent,
  CardActions,
  Rating,
  Chip,
  Paper,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Slider,
  Pagination,
  CircularProgress,
  Alert,
  Collapse,
  IconButton,
  Avatar,
} from '@mui/material';
import {
  Search as SearchIcon,
  FilterList as FilterIcon,
  ExpandMore,
  ExpandLess,
  VerifiedUser,
} from '@mui/icons-material';
import { searchGuides, getSearchSuggestions } from '../services/touristApi';
import type { SearchFilters, GuideListItem, SearchResult } from '../types/tourist.types';

const GuideSearch = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();
  const [query, setQuery] = useState(searchParams.get('q') || '');
  const [filters, setFilters] = useState<SearchFilters>({
    query: searchParams.get('q') || '',
    page: 1,
    pageSize: 12,
    sortBy: 'rating',
    sortOrder: 'desc',
  });
  const [results, setResults] = useState<SearchResult<GuideListItem> | null>(null);
  const [suggestions, setSuggestions] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [showFilters, setShowFilters] = useState(false);
  const [priceRange, setPriceRange] = useState<number[]>([0, 500]);
  const [minRating, setMinRating] = useState<number>(0);

  const doSearch = useCallback(async (searchFilters: SearchFilters) => {
    setIsLoading(true);
    setError('');
    try {
      const data = await searchGuides(searchFilters);
      setResults(data);
    } catch {
      setError('Search failed. Please try again.');
    } finally {
      setIsLoading(false);
    }
  }, []);

  const initialLoadDone = useRef(false);
  useEffect(() => {
    if (initialLoadDone.current) return;
    initialLoadDone.current = true;
    const q = searchParams.get('q');
    if (q) {
      const searchFilters: SearchFilters = {
        query: q,
        page: 1,
        pageSize: 12,
        sortBy: 'rating',
        sortOrder: 'desc',
      };
      setQuery(q);
      setFilters(searchFilters);
      doSearch(searchFilters);
    }
  }, [searchParams, doSearch]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const newFilters = {
      ...filters,
      query: query,
      minPrice: priceRange[0] || undefined,
      maxPrice: priceRange[1] < 500 ? priceRange[1] : undefined,
      minRating: minRating || undefined,
      page: 1,
    };
    setFilters(newFilters);
    setSearchParams({ q: query });
    doSearch(newFilters);
  };

  const handlePageChange = (_event: React.ChangeEvent<unknown>, page: number) => {
    const newFilters = { ...filters, page };
    setFilters(newFilters);
    doSearch(newFilters);
  };

  const handleQueryChange = async (value: string) => {
    setQuery(value);
    if (value.length >= 2) {
      try {
        const s = await getSearchSuggestions(value);
        setSuggestions(s);
      } catch {
        setSuggestions([]);
      }
    } else {
      setSuggestions([]);
    }
  };

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>
        Find Tour Guides
      </Typography>

      {/* Search Bar */}
      <Paper component="form" onSubmit={handleSearch} sx={{ p: 2, mb: 3 }}>
        <Box sx={{ display: 'flex', gap: 2, alignItems: 'center' }}>
          <TextField
            fullWidth
            placeholder="Search by destination, specialty, or guide name..."
            value={query}
            onChange={(e) => handleQueryChange(e.target.value)}
            slotProps={{
              input: {
                startAdornment: <SearchIcon sx={{ mr: 1, color: 'action.active' }} />,
              },
            }}
          />
          <Button type="submit" variant="contained" sx={{ px: 4, height: 56 }}>
            Search
          </Button>
          <IconButton onClick={() => setShowFilters(!showFilters)}>
            <FilterIcon />
            {showFilters ? <ExpandLess /> : <ExpandMore />}
          </IconButton>
        </Box>

        {/* Autocomplete Suggestions */}
        {suggestions.length > 0 && (
          <Box sx={{ mt: 1, display: 'flex', gap: 1, flexWrap: 'wrap' }}>
            {suggestions.map((s, i) => (
              <Chip
                key={i}
                label={s}
                onClick={() => { setQuery(s); setSuggestions([]); }}
                size="small"
                variant="outlined"
              />
            ))}
          </Box>
        )}

        {/* Advanced Filters */}
        <Collapse in={showFilters}>
          <Box sx={{ mt: 3 }}>
            <Grid container spacing={3}>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <TextField
                  fullWidth
                  label="Location"
                  value={filters.location || ''}
                  onChange={(e) => setFilters({ ...filters, location: e.target.value })}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <Typography gutterBottom>Price Range ($/hr)</Typography>
                <Slider
                  value={priceRange}
                  onChange={(_e, val) => setPriceRange(val as number[])}
                  valueLabelDisplay="auto"
                  min={0}
                  max={500}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <Typography gutterBottom>Minimum Rating</Typography>
                <Rating
                  value={minRating}
                  onChange={(_e, val) => setMinRating(val || 0)}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormControl fullWidth>
                  <InputLabel>Sort By</InputLabel>
                  <Select
                    value={filters.sortBy || 'rating'}
                    label="Sort By"
                    onChange={(e) => setFilters({ ...filters, sortBy: e.target.value as SearchFilters['sortBy'] })}
                  >
                    <MenuItem value="rating">Highest Rated</MenuItem>
                    <MenuItem value="price">Price</MenuItem>
                    <MenuItem value="reviews">Most Reviewed</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
            </Grid>
          </Box>
        </Collapse>
      </Paper>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {/* Results */}
      {isLoading ? (
        <Box display="flex" justifyContent="center" py={6}>
          <CircularProgress />
        </Box>
      ) : results ? (
        <>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            {results.totalCount} guides found
          </Typography>
          <Grid container spacing={3}>
            {results.items.map((guide) => (
              <Grid key={guide.id} size={{ xs: 12, sm: 6, md: 4 }}>
                <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                  <CardContent sx={{ flexGrow: 1 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                      <Avatar sx={{ width: 56, height: 56, mr: 2, bgcolor: 'primary.main' }}>
                        {guide.firstName?.[0]}{guide.lastName?.[0]}
                      </Avatar>
                      <Box>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                          <Typography variant="h6">
                            {guide.firstName} {guide.lastName}
                          </Typography>
                          {guide.verified && <VerifiedUser color="primary" fontSize="small" />}
                        </Box>
                        <Typography variant="body2" color="text.secondary">
                          {guide.location}
                        </Typography>
                      </Box>
                    </Box>
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                      <Rating value={guide.rating} precision={0.5} size="small" readOnly />
                      <Typography variant="body2" sx={{ ml: 1 }}>
                        ({guide.reviewCount} reviews)
                      </Typography>
                    </Box>
                    <Box sx={{ mb: 1 }}>
                      {guide.languages?.map((lang) => (
                        <Chip key={lang} label={lang} size="small" sx={{ mr: 0.5, mb: 0.5 }} />
                      ))}
                    </Box>
                    <Typography variant="subtitle2" color="primary">
                      ${guide.pricePerHour}/hr
                    </Typography>
                  </CardContent>
                  <CardActions>
                    <Button size="small" onClick={() => navigate(`/guides/${guide.id}`)}>
                      View Profile
                    </Button>
                  </CardActions>
                </Card>
              </Grid>
            ))}
          </Grid>

          {results.totalPages > 1 && (
            <Box display="flex" justifyContent="center" sx={{ mt: 4 }}>
              <Pagination
                count={results.totalPages}
                page={filters.page || 1}
                onChange={handlePageChange}
                color="primary"
              />
            </Box>
          )}
        </>
      ) : (
        <Paper sx={{ p: 6, textAlign: 'center' }}>
          <SearchIcon sx={{ fontSize: 64, color: 'grey.400', mb: 2 }} />
          <Typography variant="h6" color="text.secondary">
            Search for guides by destination, language, or specialty
          </Typography>
        </Paper>
      )}
    </Container>
  );
};

export default GuideSearch;
