using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MorixChatService.Models;
using MorixChatService.DTO;

namespace MorixChatService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IConfiguration configuration;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            var user = new User
            {
                UserName = registerModel.Username,
                Name = registerModel.Name,
                LastName = registerModel.LastName,
                UserType = UserType.User
            };

            var result = await userManager.CreateAsync(user, registerModel.Password ?? String.Empty);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var result = await signInManager?.PasswordSignInAsync(loginModel.Username, loginModel.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await userManager?.FindByNameAsync(loginModel.Username);
                return Ok(user.UserType);
            }
            return Unauthorized();
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }
    }    
}
