
using E_Commerce.Core.IServices;
using E_Commerce.DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.DataAccess.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken(AppUser user)
        {
            var claims = new List<Claim>()
            {
                 new Claim(ClaimTypes.Name,user.UserName),
                 new Claim(ClaimTypes.Email,user.Email),
            };


            var TokenHandler = new JwtSecurityTokenHandler();

            var Descriptor = new SecurityTokenDescriptor()
            {
                Expires = DateTime.Now.AddMinutes(30),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
            };

            var token = TokenHandler.CreateToken(Descriptor);

            return TokenHandler.WriteToken(token);
        }
    }
}
