using Business.Domain.Interfaces.UOW;
using Business.Domain.Model;

namespace Business.Domain.Interfaces.Application
{
    public interface IUserApplication<TEntity, TContext> : IBaseApplication<User, TContext>
        where TContext : IUnitOfWork
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUserById(Guid id);
        Task<User> GetUserByUsernamePassword(string u, string p);
        Task<User> GetUserByEmailPassword(string e, string p);
        Task<User> GetUserByUsername(string u);
        Task<User> GetUserByRefreshToken(string refreshToken);
        Task<User> GetUserByAccessToken(string accessToken);
        Task<bool> VerifyIfUserExistsByEmail(string e);
        Task PostUser(User u);
        Task SaveTokens(User u, string accessToken, string refreshToken);
        Task<User> PutUser(Guid id, User u);
        Task DeleteUser(Guid id);
        Task DeleteTokens(Guid id);
    }
}
