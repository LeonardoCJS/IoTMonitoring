using IoTMonitoring.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IoTMonitoring.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Tags("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>Autentica um usuário e retorna um token JWT.</summary>
        /// <remarks>
        /// Usuários disponíveis para teste:
        ///
        ///     Admin: username=admin, password=admin123
        ///     User:  username=user,  password=user123
        ///
        /// Use o token retornado no header: **Authorization: Bearer {token}**
        /// </remarks>
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDto), 200)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var users = _configuration.GetSection("Users").Get<List<UserConfig>>();
            var user = users?.FirstOrDefault(u =>
                u.Username == loginDto.Username && u.Password == loginDto.Password);

            if (user == null)
                return Unauthorized(new { message = "Usuário ou senha inválidos." });

            var token = GenerateJwtToken(user);
            return Ok(token);
        }

        private TokenResponseDto GenerateJwtToken(UserConfig user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpirationHours"]!));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new TokenResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiration,
                Username = user.Username,
                Role = user.Role
            };
        }
    }

    internal class UserConfig
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
