using JorgeCred.Application.Dtos.Request;
using JorgeCred.Domain;
using JorgeCred.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JorgeCred.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IdentityService _identityService { get; set; }
        private readonly UserManager<ApplicationUser> UserManager;
        public UserController(IdentityService identityService, UserManager<ApplicationUser> userManager)
        {
            _identityService = identityService;
            UserManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserRequest registerUserRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _identityService.RegisterUser(registerUserRequest);

            if (result)
                return Ok("Usuario criado com sucesso!");

            return BadRequest("Algo deu errado....");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _identityService.Login(loginRequest);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("ListUsers")]
        public async Task<IActionResult> ListUsers()
        {
            var users = await UserManager
                .Users
                .Include(x => x.Account)
                .ToListAsync();

            foreach (var user in users)
            {
                user.Account.ApplicationUser = null;
            }

            return Ok(users);
        }

        [Authorize]
        [HttpGet("GetUser/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
          var user = await UserManager
                .Users
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == userId);

          user.Account.ApplicationUser = null;

         return Ok(user);
        }
    }
}
