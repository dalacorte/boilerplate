using Business.Domain.Model;

namespace Business.Domain.Interfaces.Application
{
    public interface ITokenApplication
    {
        Task<User> GenerateJWT(User u, IdentityConfig c);
    }
}
