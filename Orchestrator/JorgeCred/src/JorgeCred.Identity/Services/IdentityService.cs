using JorgeCred.Application.Dtos.Request;
using JorgeCred.Domain;
using JorgeCred.Identity.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JorgeCred.Identity.Services
{
    public class IdentityService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtOptions _jwtOptions;

        public IdentityService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _roleManager = roleManager;
        }

        public async Task<bool> RegisterUser(RegisterUserRequest credentials)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = credentials.Email,
                Email = credentials.Email,
                EmailConfirmed = true
            };

            await CreateRole("Admin");
            await CreateRole("Jorginho");
            await CreateRole("Ernesto");

            var result = await _userManager.CreateAsync(applicationUser, credentials.Password);

            if (result.Succeeded)
            {
                await _userManager.SetLockoutEnabledAsync(applicationUser, false);
                await _userManager.AddToRoleAsync(applicationUser, credentials.RoleName);
            }

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                throw new Exception(string.Join(", ", errors));
            }

            return true;
        }

        public async Task<string> Login(LoginRequest credentials)
        {
            var result = await _signInManager.PasswordSignInAsync(credentials.Username, credentials.Password, false, true);

            if (!result.Succeeded)
                throw new Exception("Error while credentiating into the application");

            return await GenerateToken(credentials.Username);
        }

        public async Task<string> GenerateToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var tokenClaims = await GetClaims(user);

            var expirationDate = DateTime.UtcNow.AddDays(_jwtOptions.Expiration);

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: tokenClaims,
                notBefore: DateTime.Now,
                expires: expirationDate,
                signingCredentials: _jwtOptions.SigningCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }

        public async Task<IList<Claim>> GetClaims(ApplicationUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public async Task<List<ApplicationUser>> ListUsers()
        {
            var query = await _userManager.Users
                .Include(x => x.Account)
                .ToListAsync();

            return query;
        }

        public async Task CreateRole(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public string GetUserIdFromToken(HttpRequest httpRequest)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _jwtOptions.SigningCredentials.Key;

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Optional: reduz a margem de tolerância para a expiração do token
                };

                string authorizationHeader = httpRequest.Headers["Authorization"];
                var trimmedToken = authorizationHeader.Substring("Bearer ".Length).Trim();

                var principal = tokenHandler.ValidateToken(trimmedToken, tokenValidationParameters, out var validatedToken);

                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    throw new SecurityTokenException("Invalid token");
                }


                var nameIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

                return nameIdClaim.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }
    }

}
