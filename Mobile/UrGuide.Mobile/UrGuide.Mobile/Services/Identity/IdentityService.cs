using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace UrGuide.Mobile.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        public Task<LoginResult> SignInAsync()
        {
            var clientOptions = new OidcClientOptions {
                Authority = GlobalSetting.Instance.BaseIdentityEndpoint,
                ClientId = GlobalSetting.Instance.ClientId,
                ClientSecret = GlobalSetting.Instance.ClientSecret,
                Scope = "openid profile UrGuide.WebAppAPI offline_access",
                Browser = new SystemBrowser(),
                RedirectUri = GlobalSetting.Instance.Callback,
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

            var client = new OidcClient(clientOptions);
            return client.LoginAsync(new LoginRequest());
        }


        class SystemBrowser : IBrowser
        {
            public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
            {
                var authResult = await Xamarin.Essentials.WebAuthenticator.AuthenticateAsync(new Uri(options.StartUrl), new Uri(GlobalSetting.Instance.Callback));

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
