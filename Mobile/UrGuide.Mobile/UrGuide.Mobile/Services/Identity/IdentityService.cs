using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace UrGuide.Mobile.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private OidcClient _client;
        private readonly IPreferenceService _preference;

        public IdentityService(IPreferenceService preference)
        {
            _preference = preference ?? throw new ArgumentNullException(nameof(preference));
        }
        public async Task SignInAsync()
        {
            CreateClient();

            var u = await _client.LoginAsync(new LoginRequest());
            if (!u.IsError)
            {
                _preference.AuthToken = u.AccessToken;
                _preference.FullName = u.User.Claims.LastOrDefault(c => c.Type == IdentityModel.JwtClaimTypes.Name)?.Value;
                _preference.UserId = u.User.Claims.FirstOrDefault(c => c.Type == IdentityModel.JwtClaimTypes.Subject)?.Value;
                _preference.Role = u.User.Claims.FirstOrDefault(c => c.Type == IdentityModel.JwtClaimTypes.Role)?.Value;
                _preference.Image = $"{GlobalSetting.DefaultEndpoint}/{u.User.Claims.FirstOrDefault(c => c.Type == IdentityModel.JwtClaimTypes.Picture)?.Value}";
            }
        }

        private void CreateClient()
        {
            var clientOptions = new OidcClientOptions
            {
                Authority = GlobalSetting.Instance.BaseIdentityEndpoint,
                ClientId = GlobalSetting.Instance.ClientId,
                ClientSecret = GlobalSetting.Instance.ClientSecret,
                Scope = "openid profile offline_access",
                Browser = new SystemBrowser(),
                RedirectUri = GlobalSetting.Instance.Callback,
                PostLogoutRedirectUri = GlobalSetting.Instance.Callback,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                Policy = new Policy
                {
                    Discovery = new DiscoveryPolicy
                    {
                        RequireHttps = false,
                        ValidateIssuerName = false
                    }
                }
            };

            _client ??= new OidcClient(clientOptions);
        }

        public Task LogoutAsync()
        {
            _preference.AuthToken = string.Empty;
            _preference.FullName = string.Empty;
            _preference.UserId = string.Empty;
            _preference.Role = string.Empty;
            _preference.Image = string.Empty;
            return Task.CompletedTask;
        }

        public async Task GetUserInfo()
        {
            CreateClient();
            if (string.IsNullOrEmpty(_preference.AuthToken))
                return;
            var result = await _client.GetUserInfoAsync(_preference.AuthToken);
            if (result.IsError)
            {
                await LogoutAsync();
            }
        }

        class SystemBrowser : IBrowser
        {
            public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
            {
                var authResult = await Xamarin.Essentials.WebAuthenticator.AuthenticateAsync(new Uri(options.StartUrl), new Uri(options.EndUrl));

                return new BrowserResult
                {
                    Response = ParseAuthenticatorResult(authResult)
                };
            }

            string ParseAuthenticatorResult(WebAuthenticatorResult result)
            {
                string code = result?.Properties["code"];
                string scope = result?.Properties["scope"];
                string state = result?.Properties["state"];
                string sessionState = result?.Properties["session_state"];
                return $"{GlobalSetting.Instance.Callback}#code={code}&scope={scope}&state={state}&session_state={sessionState}";
            }
        }
    }
}
