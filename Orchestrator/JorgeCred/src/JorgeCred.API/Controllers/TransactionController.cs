using JorgeCred.Data.Context;
using JorgeCred.Domain;
using JorgeCred.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JorgeCred.API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly DbSet<Transaction> Transactions;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IdentityDataContext DbContext;
        public TransactionController(JorgeContext context, UserManager<ApplicationUser> userManager, IdentityDataContext dbContext)
        {
            DbContext = dbContext;
            Transactions = DbContext.Set<Transaction>();
            UserManager = userManager;
        }

        [HttpGet("ListTransactions")]
        public async Task<IActionResult> ListTransactions()
        {
            var query = await Transactions
                .Include(x => x.TargetAccount)
                .Include(x => x.OriginAccount)
                .ToListAsync();

            foreach (var transaction in query)
            {
                transaction.TargetAccount.Transactions = null;
                transaction.OriginAccount.Transactions = null;
            }

            return Ok(query);
        }

        [HttpPost("Transact")]
        public async Task<IActionResult> Transact([FromBody] TransactionDTO transactiondto)
        {
            var sourceUser = UserManager.Users.Include(x => x.Account).FirstOrDefault(x => x.Id == transactiondto.TargetSourceId);
            var targetUser = UserManager.Users.Include(x => x.Account).FirstOrDefault(x => x.Id == transactiondto.TargetUserId);

            if (sourceUser.Account.Balance < transactiondto.Value)
            {
                throw new Exception("Account without balance to make a transaction");
            }

            var transaction = new Transaction
            {
                Value = transactiondto.Value,
                Date = DateTime.UtcNow,
                Valid = true,
                ValidationDate = DateTime.UtcNow,
            };

            transaction.TargetAccount = targetUser.Account;
            transaction.OriginAccount = sourceUser.Account;

            sourceUser.Account.Balance = sourceUser.Account.Balance - transactiondto.Value;
            targetUser.Account.Balance = targetUser.Account.Balance + transactiondto.Value;

            await Transactions.AddAsync(transaction);
            await DbContext.SaveChangesAsync();

            return Ok();
        }
    }

    public class TransactionDTO
    {
        public string TargetUserId { get; set; }
        public string TargetSourceId { get; set; }
        public decimal Value { get; set; }
    }
}
