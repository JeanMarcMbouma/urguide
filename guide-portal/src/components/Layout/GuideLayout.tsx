import { useState, useEffect } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import {
  Box,
  Drawer,
  AppBar,
  Toolbar,
  List,
  Typography,
  Divider,
  IconButton,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Avatar,
  Menu,
  MenuItem,
  Select,
  FormControl,
} from '@mui/material';
import {
  Menu as MenuIcon,
  Dashboard as DashboardIcon,
  Person as PersonIcon,
  PhotoLibrary as PhotoLibraryIcon,
  VerifiedUser as VerifiedUserIcon,
  Explore as ExploreIcon,
  Gavel as GavelIcon,
  Event as EventIcon,
  AttachMoney as AttachMoneyIcon,
  AccountBalanceWallet as AccountBalanceWalletIcon,
  Star as StarIcon,
  Message as MessageIcon,
  BarChart as BarChartIcon,
  ExitToApp as LogoutIcon,
  AccountCircle,
  Language as LanguageIcon,
} from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import i18n from '../../i18n';
import { useAuth } from '../../hooks/useAuth';

const drawerWidth = 240;

const LANGUAGES: { code: string; label: string }[] = [
  { code: 'en', label: 'English' },
  { code: 'fr', label: 'Français' },
  { code: 'es', label: 'Español' },
  { code: 'de', label: 'Deutsch' },
  { code: 'ar', label: 'العربية' },
];

const GuideLayout = () => {
  const { t } = useTranslation();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [currentLang, setCurrentLang] = useState(i18n.language?.split('-')[0] ?? 'en');
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  // Sync direction for RTL languages
  useEffect(() => {
    document.documentElement.dir = currentLang === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.lang = currentLang;
  }, [currentLang]);

  const handleLanguageChange = (lang: string) => {
    i18n.changeLanguage(lang);
    setCurrentLang(lang);
  };

  const handleDrawerToggle = () => setMobileOpen(!mobileOpen);
  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => setAnchorEl(event.currentTarget);
  const handleMenuClose = () => setAnchorEl(null);
  const handleLogout = async () => { handleMenuClose(); await logout(); };

  const menuItems = [
    { textKey: 'nav.dashboard', icon: <DashboardIcon />, path: '/dashboard' },
    { textKey: 'nav.profile', icon: <PersonIcon />, path: '/profile' },
    { textKey: 'nav.gallery', icon: <PhotoLibraryIcon />, path: '/gallery' },
    { textKey: 'nav.verification', icon: <VerifiedUserIcon />, path: '/verification' },
    { textKey: 'nav.tourRequests', icon: <ExploreIcon />, path: '/tours' },
    { textKey: 'nav.bids', icon: <GavelIcon />, path: '/bids' },
    { textKey: 'nav.availability', icon: <EventIcon />, path: '/availability' },
    { textKey: 'nav.earnings', icon: <AttachMoneyIcon />, path: '/earnings' },
    { textKey: 'nav.payouts', icon: <AccountBalanceWalletIcon />, path: '/payouts' },
    { textKey: 'nav.reviews', icon: <StarIcon />, path: '/reviews' },
    { textKey: 'nav.messages', icon: <MessageIcon />, path: '/messages' },
    { textKey: 'nav.analytics', icon: <BarChartIcon />, path: '/analytics' },
  ];

  const drawer = (
    <div>
      <Toolbar>
        <Typography variant="h6" noWrap component="div">
          {t('layout.portalTitle')}
        </Typography>
      </Toolbar>
      <Divider />
      <List>
        {menuItems.map((item) => (
          <ListItem key={item.textKey} disablePadding>
            <ListItemButton onClick={() => navigate(item.path)}>
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={t(item.textKey as Parameters<typeof t>[0])} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </div>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar
        position="fixed"
        sx={{
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          ml: { sm: `${drawerWidth}px` },
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={handleDrawerToggle}
            sx={{ mr: 2, display: { sm: 'none' } }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            {t('layout.dashboardTitle')}
          </Typography>

          {/* Language Switcher */}
          <Box sx={{ display: 'flex', alignItems: 'center', mr: 1 }}>
            <LanguageIcon sx={{ mr: 0.5, opacity: 0.85 }} fontSize="small" />
            <FormControl size="small" variant="standard">
              <Select
                value={currentLang}
                onChange={(e) => handleLanguageChange(e.target.value)}
                disableUnderline
                sx={{ color: 'inherit', '& .MuiSelect-icon': { color: 'inherit' }, fontSize: 14 }}
                aria-label={t('layout.language')}
              >
                {LANGUAGES.map((l) => (
                  <MenuItem key={l.code} value={l.code}>{l.label}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>

          <IconButton color="inherit" onClick={handleMenuOpen}>
            <Avatar sx={{ width: 32, height: 32 }}>
              {user?.firstName?.[0] || <AccountCircle />}
            </Avatar>
          </IconButton>
          <Menu
            anchorEl={anchorEl}
            open={Boolean(anchorEl)}
            onClose={handleMenuClose}
            anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
          >
            <MenuItem disabled>
              <Typography variant="body2">{user?.email || 'Guide User'}</Typography>
            </MenuItem>
            <Divider />
            <MenuItem onClick={handleLogout}>
              <ListItemIcon>
                <LogoutIcon fontSize="small" />
              </ListItemIcon>
              {t('nav.logout')}
            </MenuItem>
          </Menu>
        </Toolbar>
      </AppBar>

      <Box component="nav" sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}>
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          mt: 8,
        }}
      >
        <Outlet />
      </Box>
    </Box>
  );
};

export default GuideLayout;
