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

            var userRef = await UserManager.Users.FirstOrDefaultAsync(x=>x.UserName == registerUserRequest.Email);

            var newAccount = new Account
            {
                Balance = 100,
                CardNumber = Guid.NewGuid(),
                CardValidity = DateTime.UtcNow.AddYears(2),
                CardSecurityStamp = Guid.NewGuid().ToString(),
                Transactions = new List<Transaction> { },
                ApplicationUser = userRef!
            };

            userRef.Account = newAccount;
            await UserManager.UpdateAsync(userRef);


            if (result)
                return Ok("Usuario criado com sucesso!");

            return BadRequest("Algo deu errado....");
        }

        [HttpPost("CreateAccount/{userId}")]
        public async Task<IActionResult> CreateAccount(string userId)
        {
            var userRef = await UserManager.FindByIdAsync(userId);

            var newAccount = new Account
            {
                Balance = 100,
                CardNumber = Guid.NewGuid(),
                CardValidity = DateTime.UtcNow.AddYears(2),
                CardSecurityStamp = Guid.NewGuid().ToString(),
                Transactions = new List<Transaction> { },
                ApplicationUser = userRef!
            };

            userRef.Account = newAccount;

            await UserManager.UpdateAsync(userRef);

            return Ok();
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

        [Authorize(Roles = "Admin")]
        [HttpGet("ListUsers")]
        public async Task<IActionResult> ListUsers()
        {
            var users = await UserManager
                .Users
                .Include(x => x.Account)
                .ToListAsync();

            foreach (var user in users.Where(x => x.Account != null))
            {
                user.Account.ApplicationUser = null;
            }

            return Ok(users);
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
          var user = await UserManager
                .Users
                .Include(x => x.Account)
                    .ThenInclude(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.Id == _identityService.GetUserIdFromToken(Request));


          if (user.Account != null)
                user.Account.ApplicationUser = null;

         return Ok(user);
        }

        [HttpPost("UpdateUserPassword")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] changePasswordDTO newPassword) 
        {
            var user = await _identityService.UpdatePassword(Request, newPassword.NewPassword);
            return Ok(user);
        }
    }

    public class changePasswordDTO {
        public string NewPassword { get; set; }
    }
}