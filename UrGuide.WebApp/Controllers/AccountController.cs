using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {

        public AccountController(IUserService userService, IAuthService authService)
        {
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
            AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IUserService UserService { get; }
        public IAuthService AuthService { get; }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] Model.Users.LoginModel model, CancellationToken cancellationToken, string returnUrl = null)
        {
            var result = await UserService.LoginAsync(model, cancellationToken);
            if (result.HasError)
            {
                return BadRequest(ErrorEnvelop.Create(result.Errors));
            }
            await HttpContext.SignInAsync(result.Data.Id, result.Data.UserName);
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
        [HttpGet("logout")]
        public async Task<IActionResult> Signout(string returnUrl = null)
        {
            await HttpContext.SignOutAsync();
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return Ok();
        }
    }
}
