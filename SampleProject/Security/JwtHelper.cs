using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
namespace SampleProject.Security
{
    public static class JwtHelper
    {
        private static string secret =
            ConfigurationManager.AppSettings["JwtSecret"];

        
        public static string GenerateToken(int userId, string email, string role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, email),
        new Claim(ClaimTypes.Role, role)
    };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        
        public static ClaimsPrincipal ValidateToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(secret);

            var handler = new JwtSecurityTokenHandler();

            var principal = handler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                },
                out SecurityToken validatedToken);

            return principal;
        }
    }
}