

using DataAccessLayer.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
// Add the necessary NuGet package reference to resolve the error.
// Ensure the following package is installed in your project:
// Package: System.IdentityModel.Tokens.Jwt

// You can install it using the NuGet Package Manager or the following command in the Package Manager Console:
// Install-Package System.IdentityModel.Tokens.Jwt

// No changes are needed in the code itself as the error is due to a missing assembly reference.
using System.Security.Claims;
using System.Text;

namespace Server_Apis.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.UserId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
