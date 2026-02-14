using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Biblioteca_WEB_API_REST_ASP.Models;

namespace TesteApiWeb.Services
{
    public class TokenService
{
        private readonly IConfiguration _configuration;
        private readonly UserManager<Usuario> _userManager;

        public TokenService(IConfiguration config, UserManager<Usuario> userManager)
        {
            _configuration = config;
            _userManager = userManager;
        }

        public async Task<string> GerarToken(Usuario usuario)   
        {
            var _jwtSecret = _configuration["Jwt:Secret"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(usuario);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id),
                new Claim(ClaimTypes.Name, usuario.UserName ?? ""),
                new Claim(ClaimTypes.Email, usuario.Email ?? "")
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60")),
                signingCredentials: creds
            );
            Console.WriteLine("token " + token);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
