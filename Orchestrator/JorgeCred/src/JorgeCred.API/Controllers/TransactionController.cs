using JorgeCred.Application.Dtos.Response;
using JorgeCred.Data.Context;
using JorgeCred.Domain;
using JorgeCred.Identity.Data;
using JorgeCred.Identity.Services;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        private readonly IdentityService IdentityService;
        private readonly IBus _bus;

        public TransactionController(JorgeContext context, UserManager<ApplicationUser> userManager, IdentityDataContext dbContext, IdentityService identityService, IBus bus)
        {
            DbContext = dbContext;
            Transactions = DbContext.Set<Transaction>();
            UserManager = userManager;
            IdentityService = identityService;
            _bus = bus;
        }

        [HttpGet("ListMyTransactions")]
        public async Task<IActionResult> ListTransactions()
        {
            var userId = IdentityService.GetUserIdFromToken(Request);

            var query = await Transactions
                .Where(x => x.OriginAccount.ApplicationUserId == userId)
                .Include(x => x.TargetAccount)
                    .ThenInclude(x=>x.ApplicationUser)
                .Include(x => x.OriginAccount)
                    .ThenInclude(x=>x.ApplicationUser)
                .ToListAsync();

            foreach (var transaction in query)
            {
                transaction.TargetAccount.Transactions = null;
                transaction.OriginAccount.Transactions = null;
            }

            return Ok(query.Select(x=>new TransactionsDTO{
                UserName = x.TargetAccount.ApplicationUser.UserName,
                Value = x.Value
            }));
        }

        [HttpGet("ListReceivedTransactions")]
        public async Task<IActionResult> ListReceivedTransactions()
        {
            var userId = IdentityService.GetUserIdFromToken(Request);

            var query = await Transactions
                .Where(x => x.TargetAccount.ApplicationUserId == userId)
                .Include(x => x.TargetAccount)
                    .ThenInclude(x=>x.ApplicationUser)
                .Include(x => x.OriginAccount)
                    .ThenInclude(x=>x.ApplicationUser)
                .ToListAsync();

            foreach (var transaction in query)
            {
                transaction.TargetAccount.Transactions = null;
                transaction.OriginAccount.Transactions = null;
            }

            return Ok(query.Select(x=>new TransactionsDTO{
                UserName = x.TargetAccount.ApplicationUser.UserName,
                Value = x.Value
            }));
        }

        [HttpPost("Transact")]
        public async Task<IActionResult> Transact([FromBody] TransactionDTO transactiondto)
        {
            var sourceUser = UserManager.Users.Include(x => x.Account).FirstOrDefault(x => x.Id == IdentityService.GetUserIdFromToken(Request));
            var targetUser = UserManager.Users.Include(x => x.Account).FirstOrDefault(x => x.Id == transactiondto.TargetUserId);

            if (targetUser == null || sourceUser == null)
                return BadRequest($"O usuario de id: {transactiondto.TargetUserId} NÃO EXISTE");

            if (sourceUser.Account.Balance < transactiondto.Value)
                return BadRequest("Você não tem saldo");

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

            var msg = new TransactionMessageNotification
            {
                Title = "Transação Efetuada",
                Message = $"Transferência de R${transactiondto.Value:F2}",
                TargetUserBrowserCredentials = targetUser.PushNotificationAddress,
                SourceUserBrowserCredentials = sourceUser.PushNotificationAddress
            };

            var sendEndpoint = await _bus.GetSendEndpoint(new Uri("queue:TransactionQueue"));
            await sendEndpoint.Send(msg);

            return Ok();
        }
    }

    public class TransactionDTO
    {
        public string TargetUserId { get; set; }
        public decimal Value { get; set; }
    }

    public class TransactionMessageNotification
    {
        public string Title { get; set; }
        public string TargetUserBrowserCredentials { get; set; }
        public string SourceUserBrowserCredentials { get; set; }
        public string Message { get; set; }
    }
}
