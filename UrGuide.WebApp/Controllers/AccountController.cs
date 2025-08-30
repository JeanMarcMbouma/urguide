using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    [Route("[controller]")]
    public class AccountController : Controller
    {

        public AccountController(IUserService userService, IAuthService authService, IIdentityServerInteractionService interactionService)
        {
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
            AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            InteractionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
        }

        public IUserService UserService { get; }
        public IAuthService AuthService { get; }
        public IIdentityServerInteractionService InteractionService { get; }

        [HttpGet("/login")]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken cancellationToken, string returnUrl = null)
        {
            var result = await UserService.LoginAsync(model, cancellationToken);
            if (result.HasError)
            {
                return BadRequest(ErrorEnvelop.Create(result.Errors));
            }
            
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, result.Data.Id),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, result.Data.UserName)
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, "login");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            
            await HttpContext.SignInAsync(principal);
            var context = InteractionService.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                return Redirect(returnUrl);
            }
            return Ok(returnUrl);
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody]CreateUserModel model,
            CancellationToken cancellationToken,
            string returnUrl = null)
        {
            var result = await UserService.RegisterUserAsync(model, cancellationToken);
            return !result.HasError ? Ok(returnUrl) : (IActionResult)BadRequest(ErrorEnvelop.Create(result.Errors));
        }

        [HttpPost("/newguide")]
        public async Task<IActionResult> NewGuide([FromBody]CreateGuideModel model,
            CancellationToken cancellationToken,
            string returnUrl = null)
        {
            var result = await UserService.RegisterGuideAsync(model, cancellationToken);
            return !result.HasError ? Ok(returnUrl) : (IActionResult)BadRequest(ErrorEnvelop.Create(result.Errors));
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]EmailConfirmationModel emailConfirmation, CancellationToken cancellationToken)
        {
            var result = await AuthService.ConfirmEmailAsync(emailConfirmation, cancellationToken);
            if(!result.HasError)
                return Redirect("/email-confirmed");
            return Forbid();
        }

        [HttpGet("forgetpassword")]
        public async Task<IActionResult> ForgetPassword([FromQuery]PasswordResetRequestModel model, 
            CancellationToken cancellationToken) {
            await AuthService.RequestPasswordResetAsync(model, cancellationToken);
            return Ok();
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassord([FromBody]ResetPasswordModel model,
            CancellationToken cancellationToken) {
            var result = await AuthService.ResetPasswordAsync(model, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok();
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model, 
            CancellationToken cancellationToken)
        {
            var result = await AuthService.ChangePasswordAsync(model, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok();
        }

        [Authorize]
        [HttpGet("/getdetails")]
        [ProducesDefaultResponseType(typeof(User))]
        public async Task<IActionResult> GetDetails(CancellationToken cancellationToken)
        {
            var result = await UserService.GetDetailsAsync(cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [Authorize]
        [HttpPost("/updateguide")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> UpdateGuide([FromBody]UpdateGuideModel model, CancellationToken cancellationToken)
        {
            var result = await UserService.UpdateGuideAsync(model, cancellationToken);

            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [Authorize]
        [HttpPost("/updateuser")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model, CancellationToken cancellationToken)
        {
            var result = await UserService.UpdateUserAsync(model, cancellationToken);

            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Signout(string returnUrl = null)
        {
            await AuthService.SignOutAsync();
            await HttpContext.SignOutAsync();
            var logoutId = Request.Query["logoutId"].ToString();

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else if (!string.IsNullOrEmpty(logoutId))
            {
                var context = await InteractionService.GetLogoutContextAsync(logoutId);
                returnUrl = context.PostLogoutRedirectUri;
                return Redirect(returnUrl);
            }
            return Ok();
        }

        [Authorize]
        [HttpGet("delete")]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, string returnUrl = null)
        {
            var r = await UserService.DeleteUserAccountAsync(cancellationToken);
            if (!r.HasError)
                await HttpContext.SignOutAsync();
            else
                return BadRequest(ErrorEnvelop.Create(r.Errors));
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return Ok();
        }
    }
}
