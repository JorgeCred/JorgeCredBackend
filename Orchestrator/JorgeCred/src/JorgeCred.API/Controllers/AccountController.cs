using JorgeCred.Data.Context;
using JorgeCred.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JorgeCred.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JorgeContext Context;
        private readonly DbSet<Account> Account;
        private readonly UserManager<ApplicationUser> UserManager;

        public AccountController(JorgeContext context, UserManager<ApplicationUser> userManager)
        {
            Context = context;
            Account = Context.Set<Account>();
            UserManager = userManager;
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
    }
}
