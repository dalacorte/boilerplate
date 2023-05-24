using Business.Application.UOW;
using Business.Domain.Interfaces.Application;
using Business.Domain.Interfaces.Services;
using Business.Domain.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUserApplication<User, UnitOfWork> _userApplication;

        public TokenService(IUserApplication<User, UnitOfWork> userApplication)
        {
            _userApplication = userApplication;
        }

        public async Task<User> GenerateJWT(User u, IdentityConfig c)
        {
            User user = await _userApplication.GetUserById(u.Id);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(new Claim("Id", u.Id.ToString()));
            claimsIdentity.AddClaim(new Claim("Username", u.Username));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, u.Email));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, u.Role));

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(c.Secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = c.ValidIssuer,
                Audience = c.ValidAudience,
                Expires = DateTime.UtcNow.AddHours(c.Expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            string accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            string refreshToken = Guid.NewGuid().ToString();

            user.UpdateAccessToken(accessToken);
            user.UpdateRefreshToken(refreshToken);
            await _userApplication.SaveTokens(user, accessToken, refreshToken);

            return user;
        }
    }
}
