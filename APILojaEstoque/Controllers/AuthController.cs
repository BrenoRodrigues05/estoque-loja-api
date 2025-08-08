using APILojaEstoque.DTOs;
using APILojaEstoque.Models;
using APILojaEstoque.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username!);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password!))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _tokenService.GenerateAccessToken(authClaims, _configuration);

                var refreshToken = _tokenService.GenerateRefreshToken();

                int.TryParse(_configuration["Jwt:RefreshTokenValidityInMinutes"], out int
                    refreshTokenValidityInMinutes);

                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);
                user.RefreshToken = refreshToken;

                await _userManager.UpdateAsync(user);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    refreshToken = refreshToken
                });
            }

            return Unauthorized(new { message = "Usuário ou senha inválidos." });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username!);
            if (userExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Usuário já existe!" });

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
            };

            var result = await _userManager.CreateAsync(user, model.Password!);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = 
                    "Erro ao criar usuário." });
            }

            return Ok(new { message = "Usuário criado com sucesso!" });
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            if (tokenModel == null)
                return BadRequest(new { message = "Token inválido." });

            var accessToken = tokenModel.AccessToken;
            var refreshToken = tokenModel.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);
            if (principal == null)
                return BadRequest(new { message = "Token inválido." });

            var username = principal.Identity?.Name;
            if (username == null)
                return BadRequest(new { message = "Token inválido." });

            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest(new { message = "Token inválido." });
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            });
        }

        [Authorize]
        [HttpPost("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado." });

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Refresh token revogado com sucesso." });
        }

        [HttpPost("create-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromQuery] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest(new { message = "Nome da role não pode ser vazio." });

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("Role {RoleName} criada com sucesso.", roleName);
                    return Created("", new { message = $"Role {roleName} criada com sucesso." });
                }
                else
                {
                    _logger.LogError("Erro ao criar role {RoleName}: {Errors}",
                        roleName, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = 
                        $"Erro ao criar role {roleName}." });
                }
            }

            return BadRequest(new { message = $"Role {roleName} já existe." });
        }

        [HttpPost("add-user-to-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserToRole([FromQuery] string email, [FromQuery] string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest(new { message = $"Usuário {email} não encontrado." });

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                _logger.LogInformation("Usuário {Email} adicionado à role {RoleName} com sucesso.", email, roleName);
                return Ok(new { message = $"Usuário {email} adicionado à role {roleName} com sucesso." });
            }
            else
            {
                _logger.LogError("Erro ao adicionar usuário {Email} à role {RoleName}: {Errors}",
                    email, roleName, string.Join(", ", result.Errors.Select(e => e.Description)));

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = 
                    $"Erro ao adicionar usuário {email} à role {roleName}." });
            }
        }
    }
}
