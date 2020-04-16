export const ApplicationName = 'UrGuide.WebApp';

const configuration = {
  client_id: ApplicationName,
  redirect_uri: 'https://localhost:5001/authentication/login-callback',
  response_type: 'code',
  post_logout_redirect_uri: 'https://localhost:5001/',
  scope: 'openid profile',
  authority: '/',
  silent_redirect_uri: 'https://localhost:5001/authentication/silent_callback',
  automaticSilentRenew: true,
  loadUserInfo: true,
};

export default configuration;