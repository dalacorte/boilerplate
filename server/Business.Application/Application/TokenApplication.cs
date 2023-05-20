using Business.Domain.Interfaces.Application;
using Business.Domain.Interfaces.Services;
using Business.Domain.Model;

namespace Business.Application.Application
{
    public class TokenApplication : ITokenApplication
    {
        private readonly ITokenService _tokenService;

        public TokenApplication(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<User> GenerateJWT(User u)
            => await _tokenService.GenerateJWT(u);
    }
}
