using Business.Application.UOW;
using Business.Domain.Interfaces.Application;
using Business.Domain.Interfaces.Services;
using Business.Domain.Interfaces.UOW;
using Business.Domain.Model;

namespace Business.Application.Application
{
    public class UserApplication<TEntity, TContext> : BaseApplication<User, UnitOfWork>, IUserApplication<TEntity, TContext>
        where TEntity : class
        where TContext : IUnitOfWork
    {
        private readonly IUserService<TEntity> _userService;
        private readonly IUnitOfWork _unitOfWork;

        public UserApplication(IUserService<TEntity> userService,
                               IUnitOfWork unitOfWork) : base(userService, unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<User>> GetUsers()
            => await _userService.GetUsers();

        public async Task<User> GetUserById(Guid id)
            => await _userService.GetUserById(id);

        public async Task<User> GetUserByUsernamePassword(string u, string p)
            => await _userService.GetUserByUsernamePassword(u, p);

        public async Task<User> GetUserByEmailPassword(string e, string p)
            => await _userService.GetUserByEmailPassword(e, p);

        public async Task<User> GetUserByUsername(string u)
            => await _userService.GetUserByUsername(u);

        public async Task<User> GetUserByRefreshToken(string refreshToken)
            => await _userService.GetUserByRefreshToken(refreshToken);

        public async Task<User> GetUserByAccessToken(string accessToken)
            => await _userService.GetUserByAccessToken(accessToken);

        public async Task<bool> VerifyIfUserExistsByEmail(string e)
            => await _userService.VerifyIfUserExistsByEmail(e);

        public async Task PostUser(User u)
        {
            _userService.PostUser(u);
            await _unitOfWork.Commit();
        }

        public async Task SaveTokens(User u, string accessToken, string refreshToken)
        {
            await _userService.SaveTokens(u, accessToken, refreshToken);
            await _unitOfWork.Commit();
        }

        public async Task<User> PutUser(Guid id, User u)
        {
            var user = await _userService.PutUser(id, u);
            await _unitOfWork.Commit();

            return user;
        }

        public async Task DeleteUser(Guid id)
        {
            _userService.DeleteUser(id);
            await _unitOfWork.Commit();
        }

        public async Task DeleteTokens(Guid id)
        {
            await _userService.DeleteTokens(id);
            await _unitOfWork.Commit();
        }
    }
}