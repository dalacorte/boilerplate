using Business.Domain.Interfaces.Mongo;
using Business.Domain.Interfaces.Repositories;
using Business.Domain.Model;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Business.Repository.Repositories
{
    public class UserRepository<TEntity> : BaseRepository<User>, IUserRepository<TEntity>
        where TEntity : class
    {
        public UserRepository(IMongoContext context) : base(context)
        {

        }

        public async Task<User> GetUserByUsernamePassword(string u, string p)
            => await DbSet.Find(doc => doc.Username == u && doc.Password == p).FirstOrDefaultAsync();

        public async Task<User> GetUserByEmailPassword(string e, string p)
            => await DbSet.Find(doc => doc.Email == e && doc.Password == p).FirstOrDefaultAsync();

        public async Task<User> GetUserByUsername(string u)
            => await DbSet.Find(doc => doc.Username == u).FirstOrDefaultAsync();

        public async Task<User> GetUserByRefreshToken(string refreshToken)
            => await DbSet.Find(doc => doc.RefreshToken == refreshToken).FirstOrDefaultAsync();

        public async Task<User> GetUserByAccessToken(string accessToken)
            => await DbSet.Find(doc => doc.AccessToken == accessToken).FirstOrDefaultAsync();

        public async Task<bool> VerifyIfUserExistsByEmail(string e)
            => await DbSet.Find(doc => doc.Email == e).AnyAsync();

        public async Task<User> PutUser(Guid id, User u)
        {
            Expression<Func<User, bool>> filter = x => x.Id.Equals(id);

            User user = await DbSet.Find(filter).FirstOrDefaultAsync();

            if (user is not null)
            {
                user.UpdateUsername(u.Username);
                user.UpdatePassword(u.Password);
                user.UpdateEmail(u.Email);
                user.UpdateName(u.Name);
                user.UpdateProfilePicture(u.ProfilePicture);
                user.UpdateRole(u.Role);
                user.UpdateLanguage(u.Language);

                Update(user);

                return user;
            }
            else
                throw new KeyNotFoundException();
        }
    }
}
