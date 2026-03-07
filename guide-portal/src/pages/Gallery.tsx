import { useState } from 'react';
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
} from '@mui/material';
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  AddPhotoAlternate as AddPhotoIcon,
} from '@mui/icons-material';
import { guideApi } from '../services/guideApi';
import type { Gallery as GalleryType, GalleryItem } from '../types/guide.types';
import ConfirmDialog from '../components/shared/ConfirmDialog';

const Gallery = () => {
  const [galleries, setGalleries] = useState<GalleryType[]>([]);
  const [selectedGallery, setSelectedGallery] = useState<GalleryType | null>(null);
  const [createOpen, setCreateOpen] = useState(false);
  const [newGalleryName, setNewGalleryName] = useState('');
  const [newGalleryDesc, setNewGalleryDesc] = useState('');
  const [confirmDelete, setConfirmDelete] = useState<{ type: 'gallery' | 'image'; id: string; catalogId?: string } | null>(null);
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  const showAlert = (type: 'success' | 'error', message: string) => {
    setAlert({ type, message });
    setTimeout(() => setAlert(null), 4000);
  };

  const handleCreateGallery = async () => {
    if (!newGalleryName.trim()) return;
    try {
      const created = await guideApi.createGallery({
        name: newGalleryName,
        description: newGalleryDesc,
      });
      setGalleries((prev) => [...prev, created]);
      setCreateOpen(false);
      setNewGalleryName('');
      setNewGalleryDesc('');
      showAlert('success', 'Gallery created successfully.');
    } catch {
      showAlert('error', 'Failed to create gallery.');
    }
  };

  const handleDeleteGallery = async (catalogId: string) => {
    try {
      await guideApi.deleteGallery(catalogId);
      setGalleries((prev) => prev.filter((g) => g.id !== catalogId));
      if (selectedGallery?.id === catalogId) setSelectedGallery(null);
      showAlert('success', 'Gallery deleted.');
    } catch {
      showAlert('error', 'Failed to delete gallery.');
    }
  };

  const handleUploadImage = async (catalogId: string, file: File) => {
    const reader = new FileReader();
    reader.onload = async () => {
      const base64 = (reader.result as string).split(',')[1];
      try {
        const item = await guideApi.addImageToGallery(catalogId, {
          fileBase64: base64,
          fileName: file.name,
        });
        setGalleries((prev) =>
          prev.map((g) =>
            g.id === catalogId ? { ...g, images: [...g.images, item] } : g
          )
        );
        if (selectedGallery?.id === catalogId) {
          setSelectedGallery((prev) =>
            prev ? { ...prev, images: [...prev.images, item] } : prev
          );
        }
        showAlert('success', 'Image uploaded successfully.');
      } catch {
        showAlert('error', 'Failed to upload image.');
      }
    };
    reader.readAsDataURL(file);
  };

  const handleRemoveImage = async (catalogId: string, imageId: string) => {
    try {
      await guideApi.removeImageFromGallery(catalogId, imageId);
      const updater = (g: GalleryType) =>
        g.id === catalogId
          ? { ...g, images: g.images.filter((img: GalleryItem) => img.id !== imageId) }
          : g;
      setGalleries((prev) => prev.map(updater));
      if (selectedGallery?.id === catalogId) {
        setSelectedGallery((prev) => (prev ? updater(prev) : prev));
      }
      showAlert('success', 'Image removed.');
    } catch {
      showAlert('error', 'Failed to remove image.');
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Photo Gallery</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setCreateOpen(true)}>
          New Gallery
        </Button>
      </Box>

      {alert && (
        <Alert severity={alert.type} sx={{ mb: 2 }}>
          {alert.message}
        </Alert>
      )}

      {galleries.length === 0 ? (
        <Paper elevation={2} sx={{ p: 4, textAlign: 'center' }}>
          <Typography variant="body1" color="text.secondary">
            No galleries yet. Create your first gallery to showcase your tours.
          </Typography>
          <Button
            variant="outlined"
            startIcon={<AddIcon />}
            sx={{ mt: 2 }}
            onClick={() => setCreateOpen(true)}
          >
            Create Gallery
          </Button>
        </Paper>
      ) : (
        <Grid container spacing={3}>
          {galleries.map((gallery) => (
            <Grid item xs={12} sm={6} md={4} key={gallery.id}>
              <Paper
                elevation={2}
                sx={{ p: 2, cursor: 'pointer', border: selectedGallery?.id === gallery.id ? '2px solid' : 'none', borderColor: 'primary.main' }}
                onClick={() => setSelectedGallery(gallery)}
              >
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="h6" noWrap>
                    {gallery.name}
                  </Typography>
                  <IconButton
                    size="small"
                    color="error"
                    onClick={(e) => {
                      e.stopPropagation();
                      setConfirmDelete({ type: 'gallery', id: gallery.id });
                    }}
                  >
                    <DeleteIcon />
                  </IconButton>
                </Box>
                <Typography variant="body2" color="text.secondary" noWrap>
                  {gallery.description}
                </Typography>
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
            <Button
              variant="outlined"
              startIcon={<AddPhotoIcon />}
              component="label"
            >
              Upload Image
              <input
                type="file"
                hidden
                accept="image/*"
                onChange={(e) => {
                  const file = e.target.files?.[0];
                  if (file) handleUploadImage(selectedGallery.id, file);
                }}
              />
            </Button>
          </Box>
          {selectedGallery.images.length === 0 ? (
            <Typography variant="body2" color="text.secondary">
              No images yet. Upload photos to this gallery.
            </Typography>
          ) : (
            <ImageList cols={3} rowHeight={164}>
              {selectedGallery.images.map((img) => (
                <ImageListItem key={img.id}>
                  <img src={img.thumbnailUrl || img.imageUrl} alt={img.title} loading="lazy" />
                  <ImageListItemBar
                    title={img.title || img.description}
                    actionIcon={
                      <IconButton
                        size="small"
                        sx={{ color: 'white' }}
                        onClick={() =>
                          setConfirmDelete({
                            type: 'image',
                            id: img.id,
                            catalogId: selectedGallery.id,
                          })
                        }
                      >
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

      {/* Create Gallery Dialog */}
      <Dialog open={createOpen} onClose={() => setCreateOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create New Gallery</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            fullWidth
            label="Gallery Name"
            value={newGalleryName}
            onChange={(e) => setNewGalleryName(e.target.value)}
            sx={{ mt: 1, mb: 2 }}
          />
          <TextField
            fullWidth
            label="Description"
            multiline
            rows={3}
            value={newGalleryDesc}
            onChange={(e) => setNewGalleryDesc(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCreateOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreateGallery} disabled={!newGalleryName.trim()}>
            Create
          </Button>
        </DialogActions>
      </Dialog>

      {/* Confirm Delete Dialog */}
      <ConfirmDialog
        open={!!confirmDelete}
        title={confirmDelete?.type === 'gallery' ? 'Delete Gallery' : 'Remove Image'}
        message={
          confirmDelete?.type === 'gallery'
            ? 'Are you sure you want to delete this gallery and all its images?'
            : 'Are you sure you want to remove this image?'
        }
        confirmText="Delete"
        severity="error"
        onConfirm={() => {
          if (!confirmDelete) return;
          if (confirmDelete.type === 'gallery') {
            handleDeleteGallery(confirmDelete.id);
          } else if (confirmDelete.catalogId) {
            handleRemoveImage(confirmDelete.catalogId, confirmDelete.id);
          }
          setConfirmDelete(null);
        }}
        onCancel={() => setConfirmDelete(null)}
      />
    </Box>
  );
};

export default Gallery;
