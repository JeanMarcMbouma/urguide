import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import {
  Box,
  Paper,
  Typography,
  Button,
  Alert,
  CircularProgress,
  Chip,
} from '@mui/material';
import { ArrowBack as BackIcon } from '@mui/icons-material';
import { DataGrid, GridColDef, GridPaginationModel } from '@mui/x-data-grid';
import { adminApi } from '../services/adminApi';
import type { UserActivityModel } from '../types/admin.types';

const ActivityLog = () => {
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({
    page: 0,
    pageSize: 50,
  });

  // Fetch user activity
  const { data, isLoading, error } = useQuery({
    queryKey: ['userActivity', userId, paginationModel.page + 1, paginationModel.pageSize],
    queryFn: () =>
      adminApi.getUserActivity(userId!, {
        pageNumber: paginationModel.page + 1,
        pageSize: paginationModel.pageSize,
      }),
    enabled: !!userId,
  });

  const columns: GridColDef<UserActivityModel>[] = [
    {
      field: 'timestamp',
      headerName: 'Timestamp',
      width: 200,
      valueFormatter: (value) => new Date(value).toLocaleString(),
    },
    {
      field: 'actionType',
      headerName: 'Action Type',
      width: 150,
      renderCell: (params) => (
        <Chip label={params.value} size="small" variant="outlined" />
      ),
    },
    {
      field: 'description',
      headerName: 'Description',
      flex: 1,
      minWidth: 300,
    },
    {
      field: 'ipAddress',
      headerName: 'IP Address',
      width: 150,
    },
  ];

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" mt={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box>
        <Alert severity="error">Failed to load activity log</Alert>
        <Button
          startIcon={<BackIcon />}
          onClick={() => navigate(`/users/${userId}`)}
          sx={{ mt: 2 }}
        >
          Back to User Details
        </Button>
      </Box>
    );
  }

  return (
    <Box>
      <Button
        startIcon={<BackIcon />}
        onClick={() => navigate(`/users/${userId}`)}
        sx={{ mb: 2 }}
      >
        Back to User Details
      </Button>

      <Typography variant="h4" gutterBottom>
        User Activity Log
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Showing all activity for this user
      </Typography>

      <Paper sx={{ height: 600, width: '100%' }}>
        <DataGrid
          rows={data?.items || []}
          columns={columns}
          rowCount={data?.totalCount || 0}
          loading={isLoading}
          pageSizeOptions={[25, 50, 100]}
          paginationModel={paginationModel}
          paginationMode="server"
          onPaginationModelChange={setPaginationModel}
          disableRowSelectionOnClick
          getRowId={(row) => `${row.userId}-${row.timestamp}-${row.actionType}`}
        />
      </Paper>
    </Box>
  );
};

export default ActivityLog;
