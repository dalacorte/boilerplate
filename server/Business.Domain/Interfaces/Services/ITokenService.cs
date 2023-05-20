using Business.Domain.Model;

namespace Business.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        Task<User> GenerateJWT(User u);
    }
}
