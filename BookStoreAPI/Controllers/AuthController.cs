using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // 1. Validate user (for now, hardcoded)
            var user = ValidateUser(request.Username, request.Password);
            if (user == null)
                return Unauthorized("Invalid username or password");

            // 2. Create JWT token
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        private UserModel? ValidateUser(string username, string password)
        {
            // In real apps → check DB. For now, hardcoded users.
            if (username == "User1" && password == "admin123")
                return new UserModel { Username = "User1", Role = "Admin" };

            if (username == "User2" && password == "mod123")
                return new UserModel { Username = "User2", Role = "Moderator" };

            if (username == "User3" && password == "readonly123")
                return new UserModel { Username = "User3", Role = "ReadOnly" };

            return null;
        }

        private string GenerateJwtToken(UserModel user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // Simple DTOs
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserModel
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
