import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  CircularProgress,
  Alert,
  Chip,
} from '@mui/material';
import { Language as LanguageIcon } from '@mui/icons-material';
import { useQuery } from '@tanstack/react-query';
import axios from 'axios';

interface LanguageInfo {
  code: string;
  name: string;
  nativeName: string;
}

interface TranslationData {
  language: string;
  culture: string;
  translations: Record<string, string | null>;
}

const LANGUAGES_ENDPOINT = '/api/localization/languages';
const TRANSLATIONS_ENDPOINT = (lang: string) => `/api/localization/${lang}`;

const TranslationManagement = () => {
  const { t } = useTranslation();
  const [selectedLanguage, setSelectedLanguage] = useState('en');

  const { data: languages, isLoading: langsLoading } = useQuery<LanguageInfo[]>({
    queryKey: ['supported-languages'],
    queryFn: async () => {
      const res = await axios.get<LanguageInfo[]>(LANGUAGES_ENDPOINT);
      return res.data;
    },
  });

  const { data: translationData, isLoading: transLoading, isError } = useQuery<TranslationData>({
    queryKey: ['translations', selectedLanguage],
    queryFn: async () => {
      const token = localStorage.getItem('adminToken');
      const res = await axios.get<TranslationData>(TRANSLATIONS_ENDPOINT(selectedLanguage), {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      });
      return res.data;
    },
    enabled: !!selectedLanguage,
  });

  const isLoading = langsLoading || transLoading;

  return (
    <Box>
      <Box sx={{ mb: 3 }}>
        <Typography variant="h5" fontWeight={600} gutterBottom>
          {t('translations.title')}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {t('translations.subtitle')}
        </Typography>
      </Box>

      <Box sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 2 }}>
        <LanguageIcon color="action" />
        <FormControl size="small" sx={{ minWidth: 200 }}>
          <InputLabel>{t('translations.selectLanguage')}</InputLabel>
          <Select
            value={selectedLanguage}
            label={t('translations.selectLanguage')}
            onChange={(e) => setSelectedLanguage(e.target.value)}
          >
            {(languages ?? []).map((lang) => (
              <MenuItem key={lang.code} value={lang.code}>
                {lang.nativeName} ({lang.name})
              </MenuItem>
            ))}
          </Select>
        </FormControl>
        {translationData && (
          <Chip
            label={translationData.culture}
            size="small"
            color="primary"
            variant="outlined"
          />
        )}
      </Box>

      {isLoading && (
        <Box display="flex" justifyContent="center" py={4}>
          <CircularProgress />
        </Box>
      )}

      {isError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {t('translations.loadError')}
        </Alert>
      )}

      {!isLoading && !isError && translationData && (
        <TableContainer component={Paper} elevation={1}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 600, width: '35%' }}>
                  {t('translations.key')}
                </TableCell>
                <TableCell sx={{ fontWeight: 600 }}>
                  {t('translations.value')}
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {Object.entries(translationData.translations).length === 0 ? (
                <TableRow>
                  <TableCell colSpan={2} align="center">
                    {t('translations.noTranslations')}
                  </TableCell>
                </TableRow>
              ) : (
                Object.entries(translationData.translations).map(([key, value]) => (
                  <TableRow key={key} hover>
                    <TableCell>
                      <Typography variant="body2" fontFamily="monospace" fontSize={12}>
                        {key}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" color={value ? 'text.primary' : 'text.disabled'}>
                        {value ?? '—'}
                      </Typography>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
};

export default TranslationManagement;
