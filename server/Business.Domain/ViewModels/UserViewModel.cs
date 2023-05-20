using Business.Domain.Helpers;
using Business.Domain.Model;

namespace Business.Domain.ViewModels
{
    public class UserViewModel : IViewModel<User>
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public User Model()
        {
            return new User();
        }
    }
}
