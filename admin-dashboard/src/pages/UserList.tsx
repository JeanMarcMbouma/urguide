import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Box,
  Paper,
  TextField,
  Button,
  IconButton,
  Chip,
  Typography,
  Alert,
  Snackbar,
} from '@mui/material';
import {
  Visibility as ViewIcon,
  Block as BlockIcon,
  CheckCircle as ActivateIcon,
  Delete as DeleteIcon,
} from '@mui/icons-material';
import { DataGrid, GridColDef, GridPaginationModel } from '@mui/x-data-grid';
import { adminApi } from '../services/adminApi';
import ConfirmDialog from '../components/shared/ConfirmDialog';
import type { AdminUserInfo } from '../types/admin.types';

const UserList = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [searchTerm, setSearchTerm] = useState('');
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({
    page: 0,
    pageSize: 20,
  });
  const [confirmDialog, setConfirmDialog] = useState<{
    open: boolean;
    action: 'suspend' | 'activate' | 'delete' | null;
    userId: string;
    userName: string;
  }>({
    open: false,
    action: null,
    userId: '',
    userName: '',
  });
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  // Fetch users query
  const { data, isLoading, error } = useQuery({
    queryKey: ['users', paginationModel.page + 1, paginationModel.pageSize, searchTerm],
    queryFn: () =>
      adminApi.getUsers({
        pageNumber: paginationModel.page + 1,
        pageSize: paginationModel.pageSize,
        term: searchTerm || undefined,
      }),
  });

  // Mutations
  const suspendMutation = useMutation({
    mutationFn: (userId: string) => adminApi.suspendUser(userId, 30),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      setSnackbar({ open: true, message: 'User suspended successfully', severity: 'success' });
    },
    onError: () => {
      setSnackbar({ open: true, message: 'Failed to suspend user', severity: 'error' });
    },
  });

  const activateMutation = useMutation({
    mutationFn: (userId: string) => adminApi.activateUser(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      setSnackbar({ open: true, message: 'User activated successfully', severity: 'success' });
    },
    onError: () => {
      setSnackbar({ open: true, message: 'Failed to activate user', severity: 'error' });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (userId: string) => adminApi.deleteUser(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      setSnackbar({ open: true, message: 'User deleted successfully', severity: 'success' });
    },
    onError: () => {
      setSnackbar({ open: true, message: 'Failed to delete user', severity: 'error' });
    },
  });

  const handleSearch = () => {
    setPaginationModel({ ...paginationModel, page: 0 });
  };

  const handleConfirm = () => {
    const { action, userId } = confirmDialog;
    if (action === 'suspend') suspendMutation.mutate(userId);
    if (action === 'activate') activateMutation.mutate(userId);
    if (action === 'delete') deleteMutation.mutate(userId);
    setConfirmDialog({ open: false, action: null, userId: '', userName: '' });
  };

  const columns: GridColDef<AdminUserInfo>[] = [
    {
      field: 'email',
      headerName: 'Email',
      flex: 1,
      minWidth: 200,
    },
    {
      field: 'firstName',
      headerName: 'First Name',
      flex: 1,
      minWidth: 120,
    },
    {
      field: 'lastName',
      headerName: 'Last Name',
      flex: 1,
      minWidth: 120,
    },
    {
      field: 'roles',
      headerName: 'Roles',
      flex: 1,
      minWidth: 150,
      renderCell: (params) => (
        <Box sx={{ display: 'flex', gap: 0.5, flexWrap: 'wrap' }}>
          {params.value.map((role: string) => (
            <Chip key={role} label={role} size="small" />
          ))}
        </Box>
      ),
    },
    {
      field: 'lockoutEnd',
      headerName: 'Status',
      width: 100,
      renderCell: (params) => {
        const isLocked = params.value && new Date(params.value) > new Date();
        return (
          <Chip
            label={isLocked ? 'Locked' : 'Active'}
            color={isLocked ? 'error' : 'success'}
            size="small"
          />
        );
      },
    },
    {
      field: 'actions',
      headerName: 'Actions',
      width: 200,
      sortable: false,
      renderCell: (params) => {
        const isLocked = params.row.lockoutEnd && new Date(params.row.lockoutEnd) > new Date();
        return (
          <Box>
            <IconButton
              size="small"
              onClick={() => navigate(`/users/${params.row.id}`)}
              title="View Details"
            >
              <ViewIcon />
            </IconButton>
            {isLocked ? (
              <IconButton
                size="small"
                color="success"
                onClick={() =>
                  setConfirmDialog({
                    open: true,
                    action: 'activate',
                    userId: params.row.id,
                    userName: params.row.email,
                  })
                }
                title="Activate User"
              >
                <ActivateIcon />
              </IconButton>
            ) : (
              <IconButton
                size="small"
                color="warning"
                onClick={() =>
                  setConfirmDialog({
                    open: true,
                    action: 'suspend',
                    userId: params.row.id,
                    userName: params.row.email,
                  })
                }
                title="Suspend User"
              >
                <BlockIcon />
              </IconButton>
            )}
            <IconButton
              size="small"
              color="error"
              onClick={() =>
                setConfirmDialog({
                  open: true,
                  action: 'delete',
                  userId: params.row.id,
                  userName: params.row.email,
                })
              }
              title="Delete User"
            >
              <DeleteIcon />
            </IconButton>
          </Box>
        );
      },
    },
  ];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        User Management
      </Typography>

      <Paper sx={{ p: 2, mb: 2 }}>
        <Box sx={{ display: 'flex', gap: 2 }}>
          <TextField
            placeholder="Search by email, name..."
            size="small"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
            sx={{ flexGrow: 1 }}
          />
          <Button variant="contained" onClick={handleSearch}>
            Search
          </Button>
        </Box>
      </Paper>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          Failed to load users
        </Alert>
      )}

      <Paper sx={{ height: 600, width: '100%' }}>
        <DataGrid
          rows={data?.items || []}
          columns={columns}
          rowCount={data?.totalCount || 0}
          loading={isLoading}
          pageSizeOptions={[10, 20, 50]}
          paginationModel={paginationModel}
          paginationMode="server"
          onPaginationModelChange={setPaginationModel}
          disableRowSelectionOnClick
        />
      </Paper>

      <ConfirmDialog
        open={confirmDialog.open}
        title={`${confirmDialog.action === 'delete' ? 'Delete' : confirmDialog.action === 'suspend' ? 'Suspend' : 'Activate'} User`}
        message={`Are you sure you want to ${confirmDialog.action} user "${confirmDialog.userName}"?`}
        confirmText={confirmDialog.action === 'delete' ? 'Delete' : 'Confirm'}
        severity={confirmDialog.action === 'delete' ? 'error' : 'warning'}
        onConfirm={handleConfirm}
        onCancel={() => setConfirmDialog({ open: false, action: null, userId: '', userName: '' })}
      />

      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
      >
        <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
};

export default UserList;
