﻿using JorgeCred.Application.Dtos.Request;
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

        /// Método: POST
        /// Rota:  /api/User/Register        
        /// Parametros URI: Vazio
        /// Parametros JSON:
        ///     "email": "string"
        ///     "password": "string"
        ///     "passwordConfirmation": "string"
        ///     "rolename": "string"
        /// Essa rota é usada para fazer o registro de novos usuários. Recebe um payload JSON com as informações
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

        /// Método: POST
        /// Rota:   /api/User/CreateAccount/{userId}    
        /// Parametros URI: "userId" 
        /// Parametros JSON: Vazio
        /// Essa rota é usada para fazer o registro de novos usuários. Recebe um payload JSON com as informações
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
        /// Rota:  /api/User/Login        
        /// Parametros URI: Vazio
        /// Parametros JSON:
        ///     "username": "string"
        ///     "password": "string"
        /// Essa rota é usada para lidar com o login de usuários. Recebe um payload JSON com as credenciais do usuário.
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

        /// ESSA É UMA FUNÇÃO APENAS PARA ADMINS
        /// Método: GET
        /// Rota:  /api/User/ListUsers       
        /// Parametros URI: Vazio
        /// Parametros JSON: Vazio
        /// Essa rota é usada para listar todos os usuários com contas registradas, não inclui informações sensíveis.
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

        /// Método: GET
        /// Rota:  /api/User/GetUser        
        /// Parametros URI: Vazio
        /// Parametros JSON: Vazio
        /// Essa rota retorna as credenciais do usuário logado no momento.
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
        
        /// Método: POST
        /// Rota:  /api/User/UpdateUserPassword     
        /// Parametros URI: Vazio
        /// Parametros JSON:
        ///     "newPassword": "string"
        /// Essa rota é usada para atualizar a senha do usuário, recebe a nova senha através de um payload JSON.
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