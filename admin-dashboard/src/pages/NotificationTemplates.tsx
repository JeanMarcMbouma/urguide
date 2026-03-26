import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Box,
  Paper,
  Button,
  TextField,
  Typography,
  Alert,
  Snackbar,
  Chip,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Switch,
  FormControlLabel,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Tooltip,
  Divider,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Preview as PreviewIcon,
  FilterList as FilterIcon,
} from '@mui/icons-material';
import { adminApi } from '../services/adminApi';
import type {
  NotificationTemplateDto,
  CreateNotificationTemplateRequest,
  UpdateNotificationTemplateRequest,
} from '../types/admin.types';

const CATEGORIES = [
  'tour_updates',
  'booking_alerts',
  'chat_messages',
  'promotional',
  'system_alerts',
];

const LANGUAGES = [
  { code: 'en', label: 'English' },
  { code: 'fr', label: 'Français' },
  { code: 'es', label: 'Español' },
  { code: 'de', label: 'Deutsch' },
  { code: 'ar', label: 'العربية' },
];

type FormMode = 'create' | 'edit' | null;

interface TemplateFormState {
  name: string;
  category: string;
  language: string;
  titleTemplate: string;
  bodyTemplate: string;
  imageUrl: string;
  actionUrl: string;
  isActive: boolean;
  variantGroup: string;
}

const emptyForm = (): TemplateFormState => ({
  name: '',
  category: 'tour_updates',
  language: 'en',
  titleTemplate: '',
  bodyTemplate: '',
  imageUrl: '',
  actionUrl: '',
  isActive: true,
  variantGroup: '',
});

const NotificationTemplates = () => {
  const queryClient = useQueryClient();

  // Filters
  const [filterCategory, setFilterCategory] = useState('');
  const [filterLanguage, setFilterLanguage] = useState('');

  // Form dialog
  const [formMode, setFormMode] = useState<FormMode>(null);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<TemplateFormState>(emptyForm());

  // Preview dialog
  const [previewOpen, setPreviewOpen] = useState(false);
  const [previewTemplateId, setPreviewTemplateId] = useState<string | null>(null);
  const [previewVarsText, setPreviewVarsText] = useState('{}');
  const [previewResult, setPreviewResult] = useState<{ title: string; body: string } | null>(null);

  // Delete confirmation
  const [deleteId, setDeleteId] = useState<string | null>(null);

  // Snackbar
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  const showSnack = (message: string, severity: 'success' | 'error' = 'success') =>
    setSnackbar({ open: true, message, severity });

  // ── Queries ──────────────────────────────────────────────────────────────────

  const { data: templates = [], isLoading, error } = useQuery({
    queryKey: ['notification-templates', filterCategory, filterLanguage],
    queryFn: () =>
      adminApi.getNotificationTemplates(
        filterCategory || undefined,
        filterLanguage || undefined,
      ),
  });

  // ── Mutations ────────────────────────────────────────────────────────────────

  const createMutation = useMutation({
    mutationFn: (req: CreateNotificationTemplateRequest) =>
      adminApi.createNotificationTemplate(req),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['notification-templates'] });
      setFormMode(null);
      showSnack('Template created successfully.');
    },
    onError: () => showSnack('Failed to create template.', 'error'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, req }: { id: string; req: UpdateNotificationTemplateRequest }) =>
      adminApi.updateNotificationTemplate(id, req),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['notification-templates'] });
      setFormMode(null);
      showSnack('Template updated successfully (new version created).');
    },
    onError: () => showSnack('Failed to update template.', 'error'),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => adminApi.deleteNotificationTemplate(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['notification-templates'] });
      setDeleteId(null);
      showSnack('Template deactivated.');
    },
    onError: () => showSnack('Failed to deactivate template.', 'error'),
  });

  // ── Handlers ─────────────────────────────────────────────────────────────────

  const openCreate = () => {
    setForm(emptyForm());
    setEditingId(null);
    setFormMode('create');
  };

  const openEdit = (t: NotificationTemplateDto) => {
    setForm({
      name: t.name,
      category: t.category,
      language: t.language,
      titleTemplate: t.titleTemplate,
      bodyTemplate: t.bodyTemplate,
      imageUrl: t.imageUrl ?? '',
      actionUrl: t.actionUrl ?? '',
      isActive: t.isActive,
      variantGroup: t.variantGroup ?? '',
    });
    setEditingId(t.id);
    setFormMode('edit');
  };

  const handleFormSubmit = () => {
    if (formMode === 'create') {
      createMutation.mutate({
        name: form.name,
        category: form.category,
        language: form.language,
        titleTemplate: form.titleTemplate,
        bodyTemplate: form.bodyTemplate,
        imageUrl: form.imageUrl || undefined,
        actionUrl: form.actionUrl || undefined,
        variantGroup: form.variantGroup || undefined,
      });
    } else if (formMode === 'edit' && editingId) {
      updateMutation.mutate({
        id: editingId,
        req: {
          titleTemplate: form.titleTemplate,
          bodyTemplate: form.bodyTemplate,
          imageUrl: form.imageUrl || undefined,
          actionUrl: form.actionUrl || undefined,
          isActive: form.isActive,
          variantGroup: form.variantGroup || undefined,
        },
      });
    }
  };

  const openPreview = (t: NotificationTemplateDto) => {
    setPreviewTemplateId(t.id);
    setPreviewVarsText('{}');
    setPreviewResult(null);
    setPreviewOpen(true);
  };

  const runPreview = async () => {
    if (!previewTemplateId) return;
    try {
      const vars = JSON.parse(previewVarsText);
      const result = await adminApi.previewNotificationTemplate(previewTemplateId, vars);
      setPreviewResult(result);
    } catch {
      showSnack('Invalid JSON in variables.', 'error');
    }
  };

  // ── Render ───────────────────────────────────────────────────────────────────

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Box>
          <Typography variant="h5" fontWeight={600}>
            Push Notification Templates
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Manage reusable templates with variable substitution, multi-language support, and A/B variants.
          </Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
          New Template
        </Button>
      </Box>

      {/* Filters */}
      <Paper sx={{ p: 2, mb: 2, display: 'flex', gap: 2, alignItems: 'center' }}>
        <FilterIcon color="action" />
        <FormControl size="small" sx={{ minWidth: 160 }}>
          <InputLabel>Category</InputLabel>
          <Select
            value={filterCategory}
            label="Category"
            onChange={(e) => setFilterCategory(e.target.value)}
          >
            <MenuItem value="">All</MenuItem>
            {CATEGORIES.map((c) => (
              <MenuItem key={c} value={c}>{c}</MenuItem>
            ))}
          </Select>
        </FormControl>
        <FormControl size="small" sx={{ minWidth: 140 }}>
          <InputLabel>Language</InputLabel>
          <Select
            value={filterLanguage}
            label="Language"
            onChange={(e) => setFilterLanguage(e.target.value)}
          >
            <MenuItem value="">All</MenuItem>
            {LANGUAGES.map((l) => (
              <MenuItem key={l.code} value={l.code}>{l.label}</MenuItem>
            ))}
          </Select>
        </FormControl>
        {(filterCategory || filterLanguage) && (
          <Button size="small" onClick={() => { setFilterCategory(''); setFilterLanguage(''); }}>
            Clear
          </Button>
        )}
      </Paper>

      {/* Template Table */}
      {isLoading && <Typography>Loading templates...</Typography>}
      {error && <Alert severity="error">Failed to load templates.</Alert>}
      {!isLoading && !error && (
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Category</TableCell>
                <TableCell>Language</TableCell>
                <TableCell align="center">Version</TableCell>
                <TableCell>Title Template</TableCell>
                <TableCell>Variant</TableCell>
                <TableCell align="center">Active</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {templates.length === 0 && (
                <TableRow>
                  <TableCell colSpan={8} align="center">
                    No templates found. Create one to get started.
                  </TableCell>
                </TableRow>
              )}
              {templates.map((t) => (
                <TableRow key={t.id} hover>
                  <TableCell>{t.name}</TableCell>
                  <TableCell>
                    <Chip label={t.category} size="small" variant="outlined" />
                  </TableCell>
                  <TableCell>{t.language.toUpperCase()}</TableCell>
                  <TableCell align="center">v{t.version}</TableCell>
                  <TableCell sx={{ maxWidth: 200, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                    {t.titleTemplate}
                  </TableCell>
                  <TableCell>{t.variantGroup || '—'}</TableCell>
                  <TableCell align="center">
                    <Chip
                      label={t.isActive ? 'Active' : 'Inactive'}
                      size="small"
                      color={t.isActive ? 'success' : 'default'}
                    />
                  </TableCell>
                  <TableCell align="right">
                    <Tooltip title="Preview">
                      <IconButton size="small" onClick={() => openPreview(t)}>
                        <PreviewIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Edit">
                      <IconButton size="small" onClick={() => openEdit(t)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Deactivate">
                      <IconButton size="small" color="error" onClick={() => setDeleteId(t.id)}>
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* Create / Edit Dialog */}
      <Dialog open={formMode !== null} onClose={() => setFormMode(null)} maxWidth="sm" fullWidth>
        <DialogTitle>{formMode === 'create' ? 'New Template' : 'Edit Template'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
          {formMode === 'create' && (
            <>
              <TextField
                label="Name"
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
                helperText="Logical name used to look up the template, e.g. booking_confirmed"
                required
                fullWidth
              />
              <FormControl fullWidth>
                <InputLabel>Category</InputLabel>
                <Select
                  value={form.category}
                  label="Category"
                  onChange={(e) => setForm({ ...form, category: e.target.value })}
                >
                  {CATEGORIES.map((c) => (
                    <MenuItem key={c} value={c}>{c}</MenuItem>
                  ))}
                </Select>
              </FormControl>
              <FormControl fullWidth>
                <InputLabel>Language</InputLabel>
                <Select
                  value={form.language}
                  label="Language"
                  onChange={(e) => setForm({ ...form, language: e.target.value })}
                >
                  {LANGUAGES.map((l) => (
                    <MenuItem key={l.code} value={l.code}>{l.label}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </>
          )}
          <TextField
            label="Title Template"
            value={form.titleTemplate}
            onChange={(e) => setForm({ ...form, titleTemplate: e.target.value })}
            helperText="Use {{variable_name}} for variable substitution"
            required
            fullWidth
          />
          <TextField
            label="Body Template"
            value={form.bodyTemplate}
            onChange={(e) => setForm({ ...form, bodyTemplate: e.target.value })}
            helperText="Use {{variable_name}} for variable substitution"
            required
            multiline
            rows={3}
            fullWidth
          />
          <TextField
            label="Image URL (optional)"
            value={form.imageUrl}
            onChange={(e) => setForm({ ...form, imageUrl: e.target.value })}
            fullWidth
          />
          <TextField
            label="Action URL (optional)"
            value={form.actionUrl}
            onChange={(e) => setForm({ ...form, actionUrl: e.target.value })}
            fullWidth
          />
          <TextField
            label="Variant Group (A/B testing, optional)"
            value={form.variantGroup}
            onChange={(e) => setForm({ ...form, variantGroup: e.target.value })}
            helperText='e.g. "A" or "B"'
            fullWidth
          />
          {formMode === 'edit' && (
            <FormControlLabel
              control={
                <Switch
                  checked={form.isActive}
                  onChange={(e) => setForm({ ...form, isActive: e.target.checked })}
                />
              }
              label="Active"
            />
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setFormMode(null)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleFormSubmit}
            disabled={!form.name || !form.titleTemplate || !form.bodyTemplate}
          >
            {formMode === 'create' ? 'Create' : 'Save (new version)'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Preview Dialog */}
      <Dialog open={previewOpen} onClose={() => setPreviewOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Preview Template</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
          <TextField
            label="Variables (JSON)"
            value={previewVarsText}
            onChange={(e) => setPreviewVarsText(e.target.value)}
            helperText='e.g. {"name": "Alice", "tour_name": "Paris Tour"}'
            multiline
            rows={3}
            fullWidth
          />
          <Button variant="outlined" onClick={runPreview}>
            Render Preview
          </Button>
          {previewResult && (
            <>
              <Divider />
              <Typography variant="subtitle2">Title</Typography>
              <Paper variant="outlined" sx={{ p: 1.5 }}>
                <Typography>{previewResult.title}</Typography>
              </Paper>
              <Typography variant="subtitle2">Body</Typography>
              <Paper variant="outlined" sx={{ p: 1.5 }}>
                <Typography>{previewResult.body}</Typography>
              </Paper>
            </>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPreviewOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteId !== null} onClose={() => setDeleteId(null)}>
        <DialogTitle>Deactivate Template</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to deactivate this template? It will remain in the version history.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteId(null)}>Cancel</Button>
          <Button
            color="error"
            variant="contained"
            onClick={() => deleteId && deleteMutation.mutate(deleteId)}
          >
            Deactivate
          </Button>
        </DialogActions>
      </Dialog>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert severity={snackbar.severity} onClose={() => setSnackbar({ ...snackbar, open: false })}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default NotificationTemplates;
