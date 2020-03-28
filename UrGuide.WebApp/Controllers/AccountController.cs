using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {

        public AccountController(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction)
        {
            SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            Interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
        }

        public SignInManager<ApplicationUser> SignInManager { get; }
        public IIdentityServerInteractionService Interaction { get; }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody]LoginModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var context = await Interaction.GetAuthorizationContextAsync(returnUrl);
                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.Persist, true);
                if(result.RequiresTwoFactor)
                {
                   
                }
                else if (!result.Succeeded || result.IsLockedOut)
                {
                    return BadRequest();
                }
                var user = await SignInManager.UserManager.FindByNameAsync(model.UserName);

                await HttpContext.SignInAsync(user.Id, user.UserName);
                if(Url.IsLocalUrl(returnUrl) || Interaction.IsValidReturnUrl(returnUrl))
                    return LocalRedirect(returnUrl);
                return Ok(returnUrl);
            }
            return BadRequest();
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody]RegistrationModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var userManager = SignInManager.UserManager;
                var user = await userManager.FindByNameAsync(model.UserName);
                if(user != null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }

                var result = await userManager.CreateAsync(new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.UserName,
                    EmailConfirmed = true
                }, model.Password);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                return BadRequest(result.Errors);
            }
            return BadRequest();
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Signout(string returnUrl = null)
        {
            await HttpContext.SignOutAsync();
            if (Interaction.IsValidReturnUrl(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return Ok();
        }
    }
}
