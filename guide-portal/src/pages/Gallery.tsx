import { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  ImageList,
  ImageListItem,
  ImageListItemBar,
  IconButton,
  Alert,
  Grid,
  Chip,
  CircularProgress,
} from '@mui/material';
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  AddPhotoAlternate as AddPhotoIcon,
} from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { guideApi } from '../services/guideApi';
import { useAuth } from '../hooks/useAuth';
import type { Gallery as GalleryType, GalleryItem } from '../types/guide.types';
import ConfirmDialog from '../components/shared/ConfirmDialog';

const Gallery = () => {
  const { t } = useTranslation();
  const { user } = useAuth();
  const [galleries, setGalleries] = useState<GalleryType[]>([]);
  const [selectedGallery, setSelectedGallery] = useState<GalleryType | null>(null);
  const [createOpen, setCreateOpen] = useState(false);
  const [newGalleryName, setNewGalleryName] = useState('');
  const [newGalleryDesc, setNewGalleryDesc] = useState('');
  const [confirmDelete, setConfirmDelete] = useState<{ type: 'gallery' | 'image'; id: string; catalogId?: string } | null>(null);
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState('');

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 4000);
  };

  useEffect(() => {
    if (!user?.id) return;
    guideApi.getGalleries(user.id)
      .then(setGalleries)
      .catch(() => setLoadError(t('gallery.loadError')))
      .finally(() => setLoading(false));
  }, [user?.id, t]);

  const handleCreateGallery = async () => {
    if (!newGalleryName.trim()) return;
    try {
      const created = await guideApi.createGallery({ name: newGalleryName, description: newGalleryDesc });
      setGalleries((prev) => [...prev, created]);
      setCreateOpen(false);
      setNewGalleryName('');
      setNewGalleryDesc('');
      showAlert('success', t('gallery.createSuccess'));
    } catch {
      showAlert('error', t('gallery.createError'));
    }
  };

  const handleDeleteGallery = async (catalogId: string) => {
    try {
      await guideApi.deleteGallery(catalogId);
      setGalleries((prev) => prev.filter((g) => g.id !== catalogId));
      if (selectedGallery?.id === catalogId) setSelectedGallery(null);
      showAlert('success', t('gallery.deleteSuccess'));
    } catch {
      showAlert('error', t('gallery.createError'));
    }
  };

  const handleUploadImage = async (catalogId: string, file: File) => {
    const reader = new FileReader();
    reader.onload = async () => {
      const base64 = (reader.result as string).split(',')[1];
      try {
        const item = await guideApi.addImageToGallery(catalogId, { fileBase64: base64, fileName: file.name });
        const updater = (g: GalleryType) => g.id === catalogId ? { ...g, images: [...g.images, item] } : g;
        setGalleries((prev) => prev.map(updater));
        if (selectedGallery?.id === catalogId) setSelectedGallery((prev) => prev ? updater(prev) : prev);
        showAlert('success', t('gallery.createSuccess'));
      } catch {
        showAlert('error', t('gallery.uploadError'));
      }
    };
    reader.readAsDataURL(file);
  };

  const handleRemoveImage = async (catalogId: string, imageId: string) => {
    try {
      await guideApi.removeImageFromGallery(catalogId, imageId);
      const updater = (g: GalleryType) =>
        g.id === catalogId ? { ...g, images: g.images.filter((img: GalleryItem) => img.id !== imageId) } : g;
      setGalleries((prev) => prev.map(updater));
      if (selectedGallery?.id === catalogId) setSelectedGallery((prev) => prev ? updater(prev) : prev);
      showAlert('success', t('gallery.deleteSuccess'));
    } catch {
      showAlert('error', t('gallery.removeImageError'));
    }
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Box>
          <Typography variant="h4">{t('gallery.title')}</Typography>
          <Typography variant="body1" color="text.secondary">{t('gallery.subtitle')}</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setCreateOpen(true)}>
          {t('gallery.createGallery')}
        </Button>
      </Box>

      {loadError && <Alert severity="error" sx={{ mb: 2 }}>{loadError}</Alert>}
      {alert && <Alert severity={alert.type} sx={{ mb: 2 }}>{alert.message}</Alert>}

      {galleries.length === 0 ? (
        <Paper elevation={2} sx={{ p: 4, textAlign: 'center' }}>
          <Typography variant="body1" color="text.secondary">{t('gallery.noGalleries')}</Typography>
          <Button variant="outlined" startIcon={<AddIcon />} sx={{ mt: 2 }} onClick={() => setCreateOpen(true)}>
            {t('gallery.createGallery')}
          </Button>
        </Paper>
      ) : (
        <Grid container spacing={3}>
          {galleries.map((gallery) => (
            <Grid size={{ xs: 12, sm: 6, md: 4 }} key={gallery.id}>
              <Paper
                elevation={2}
                sx={{ p: 2, cursor: 'pointer', border: selectedGallery?.id === gallery.id ? '2px solid' : 'none', borderColor: 'primary.main' }}
                onClick={() => setSelectedGallery(gallery)}
              >
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="h6" noWrap>{gallery.name}</Typography>
                  <IconButton size="small" color="error" onClick={(e) => { e.stopPropagation(); setConfirmDelete({ type: 'gallery', id: gallery.id }); }}>
                    <DeleteIcon />
                  </IconButton>
                </Box>
                <Typography variant="body2" color="text.secondary" noWrap>{gallery.description}</Typography>
                <Chip label={`${gallery.images.length} images`} size="small" sx={{ mt: 1 }} />
              </Paper>
            </Grid>
          ))}
        </Grid>
      )}

      {selectedGallery && (
        <Paper elevation={2} sx={{ p: 3, mt: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
            <Typography variant="h6">{selectedGallery.name}</Typography>
            <Button variant="outlined" startIcon={<AddPhotoIcon />} component="label">
              {t('gallery.addImage')}
              <input type="file" hidden accept="image/*" onChange={(e) => {
                const file = e.target.files?.[0];
                if (file) handleUploadImage(selectedGallery.id, file);
                e.target.value = '';
              }} />
            </Button>
          </Box>
          {selectedGallery.images.length === 0 ? (
            <Typography variant="body2" color="text.secondary">{t('gallery.noImages')}</Typography>
          ) : (
            <ImageList cols={3} rowHeight={164}>
              {selectedGallery.images.map((img) => (
                <ImageListItem key={img.id}>
                  <img src={img.thumbnailUrl || img.imageUrl} alt={img.title} loading="lazy" />
                  <ImageListItemBar
                    title={img.title || img.description}
                    actionIcon={
                      <IconButton size="small" sx={{ color: 'white' }}
                        onClick={() => setConfirmDelete({ type: 'image', id: img.id, catalogId: selectedGallery.id })}>
                        <DeleteIcon />
                      </IconButton>
                    }
                  />
                </ImageListItem>
              ))}
            </ImageList>
          )}
        </Paper>
      )}

      <Dialog open={createOpen} onClose={() => setCreateOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{t('gallery.createGallery')}</DialogTitle>
        <DialogContent>
          <TextField autoFocus fullWidth label={t('gallery.galleryName')} value={newGalleryName}
            onChange={(e) => setNewGalleryName(e.target.value)} sx={{ mt: 1, mb: 2 }} />
          <TextField fullWidth label={t('gallery.description')} multiline rows={3}
            value={newGalleryDesc} onChange={(e) => setNewGalleryDesc(e.target.value)} />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCreateOpen(false)}>{t('gallery.cancel')}</Button>
          <Button variant="contained" onClick={handleCreateGallery} disabled={!newGalleryName.trim()}>
            {t('gallery.create')}
          </Button>
        </DialogActions>
      </Dialog>

      <ConfirmDialog
        open={!!confirmDelete}
        title={confirmDelete?.type === 'gallery' ? t('gallery.deleteGallery') : t('gallery.addImage')}
        message={t('gallery.confirmDelete')}
        confirmText={t('common.delete')}
        severity="error"
        onConfirm={() => {
          if (!confirmDelete) return;
          if (confirmDelete.type === 'gallery') handleDeleteGallery(confirmDelete.id);
          else if (confirmDelete.catalogId) handleRemoveImage(confirmDelete.catalogId, confirmDelete.id);
          setConfirmDelete(null);
        }}
        onCancel={() => setConfirmDelete(null)}
      />
    </Box>
  );
};

export default Gallery;
