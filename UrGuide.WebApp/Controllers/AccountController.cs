using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {

        public AccountController(IUserService userService)
        {
           UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public IUserService UserService { get; }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody]LoginCommand model, CancellationToken cancellationToken, string returnUrl = null)
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
        public async Task<IActionResult> Register([FromBody]CreateUserCommand model,
            [FromServices]Services.IEmailService emailService, 
            CancellationToken cancellationToken,
            string returnUrl = null)
        {
            var result = await UserService.RegisterUserAsync(model, cancellationToken);
            if (result.HasError)
            {
                return BadRequest(ErrorEnvelop.Create(result.Errors));
            }

            if (!result.HasError)
            {
                //var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
                //var url = Url.ActionLink(nameof(ConfirmEmail), "Account", new { confirmationToken, email = newUser.Email });
                //// send email
                //var link = $"<a href='{url}'>clicking here</a>";
                //var message = $"Please confirm your account by {HtmlEncoder.Default.Encode(link)}.";
                //await emailService.Send(newUser.Email, message, Services.EmailService.MessageTypes.Confirmation, cancellationToken).ConfigureAwait(false);
                return Ok(returnUrl);
            }
            return BadRequest(ErrorEnvelop.Create(result.Errors));
        }

        [HttpPost("/newguide")]
        public async Task<IActionResult> NewGuide([FromBody]CreateGuideCommand model,
            [FromServices]Services.IEmailService emailService,
            CancellationToken cancellationToken,
            string returnUrl = null)
        {
            var result = await UserService.RegisterGuideAsync(model, cancellationToken);
            if (result.HasError)
            {
                return BadRequest(ErrorEnvelop.Create(result.Errors));
            }

            if (!result.HasError)
            {
                //var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
                //var url = Url.ActionLink(nameof(ConfirmEmail), "Account", new { confirmationToken, email = newUser.Email });
                //// send email
                //var link = $"<a href='{url}'>clicking here</a>";
                //var message = $"Please confirm your account by {HtmlEncoder.Default.Encode(link)}.";
                //await emailService.Send(newUser.Email, message, Services.EmailService.MessageTypes.Confirmation, cancellationToken).ConfigureAwait(false);
                return Ok(returnUrl);
            }
            return BadRequest(ErrorEnvelop.Create(result.Errors));
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string confirmationToken, [FromQuery]string email){
            //var userManager = SignInManager.UserManager;
            //var user = await userManager.FindByEmailAsync(email);
            //var result = await userManager.ConfirmEmailAsync(user, confirmationToken);
            //if(result.Succeeded)
            //    return Redirect("/email-confirmed");
            return Forbid();
        }

        [HttpGet("forgetpassword")]
        public async Task<IActionResult> ForgetPassword([FromQuery]string email, 
            [FromServices]Services.IEmailService emailService,
            CancellationToken cancellationToken) {
            //var userManager = SignInManager.UserManager;
            //var user = await userManager.FindByEmailAsync(email);
            //if(user != null) {
            //    var confirmationToken = await userManager.GeneratePasswordResetTokenAsync(user);
            //    // ideally send to the user email
            //    var url = Url.Link("pforget", new {confirmationToken, email});
            //    var message = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(url)}'>clicking here</a>.";
            //    await emailService.Send(email, message, Services.EmailService.MessageTypes.PasswordReset, cancellationToken).ConfigureAwait(false);
            //    #if DEBUG
            //    return Ok(confirmationToken);
            //    #endif
            //}
            return Ok();
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassord([FromBody]PasswordResetModel model,
            [FromServices]Services.IEmailService emailService,
            CancellationToken cancellationToken) {
            //var userManager = SignInManager.UserManager;
            //var user = await userManager.FindByEmailAsync(model.Email);
            //if(user != null) {
            //    var confirmationResult = await userManager.ResetPasswordAsync(user, model.ConfirmationToken, model.Password);
            //    if (confirmationResult.Succeeded) {
            //        await emailService.Send(user.Email, "Your password was reset successfully.", Services.EmailService.MessageTypes.PasswordReset, cancellationToken).ConfigureAwait(false);
            //        return Ok();
            //    }
            //    return BadRequest(ErrorEnvelop.Create(confirmationResult.Errors));
            //}
            return Forbid();
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model, 
            [FromServices]Services.IEmailService emailService,
            CancellationToken cancellationToken)
        {
            return Ok();
            //var userManager = SignInManager.UserManager;
            //var user = await userManager.FindByEmailAsync(model.Email);
            //if (!User.Identity.IsAuthenticated || user?.UserName != User.Identity.Name)
            //{
            //    if(user != null)
            //        await emailService.Send(user.Email, "An attempt was made to change your password.", 
            //        Services.EmailService.MessageTypes.ChangePassword, cancellationToken).ConfigureAwait(false);
            //    return Forbid();
            //}

            //var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);
            //if(result.Succeeded)
            //{
            //    await emailService.Send(user.Email, "Your password was changed successfully.",
            //           Services.EmailService.MessageTypes.ChangePassword, cancellationToken).ConfigureAwait(false);
            //    return Ok();
            //}

            //return BadRequest(ErrorEnvelop.Create(result.Errors));
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
