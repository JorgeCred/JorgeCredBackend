using JorgeCred.Application.Dtos.Request;
using JorgeCred.Data.Context;
using JorgeCred.Domain;
using JorgeCred.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;

namespace JorgeCred.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IdentityService _identityService { get; set; }
        private readonly JorgeContext Context;
        private readonly DbSet<Account> Account;
        private readonly UserManager<ApplicationUser> UserManager;

        public AccountController(IdentityService identityService, JorgeContext context, UserManager<ApplicationUser> userManager)
        {
            _identityService = identityService;
            Context = context;
            Account = Context.Set<Account>();
            UserManager = userManager;
        }

        /// Método: POST
        /// Rota: api/Account/CreateAccount/{userId}
        /// Parametros URI: "userId"
        /// Parametros JSON: Nenhum
        /// Cria uma nova conta com o userId fornecido. Inicializa a conta com um saldo de 100, um número de cartão único, validade de 2 anos e um código de segurança único.
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

        /// Método: POST
        /// Rota: api/Account/AssociateTokenWithUser
        /// Parametros URI: Nenhum
        /// Parametros JSON:
        ///     "endpoint": "string"
        ///     "keys": {
        ///         "auth": "string"
        ///         "expirationTime": "string"
        ///         "p256dh": "string"
        ///     }
        /// Associa um token de notificação push ao usuário autenticado. O token é recebido no corpo da requisição como um objeto JSON. Este token é usado para enviar notificações push para o usuário.
        [HttpPost("AssociateTokenWithUser")]
        public async Task<IActionResult> AssociateTokenWithUser([FromBody] PushNotificationRequest pushNotification) {
            var userRef = await UserManager
                .Users
                .FirstOrDefaultAsync(x => x.Id == _identityService.GetUserIdFromToken(Request));

            userRef.PushNotificationAddress = JsonConvert.SerializeObject(pushNotification);

            await UserManager.UpdateAsync(userRef);

            return Ok();
        }

    }
}
