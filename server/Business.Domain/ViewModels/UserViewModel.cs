using Business.Domain.Helpers;
using Business.Domain.Model;

namespace Business.Domain.ViewModels
{
    public class UserViewModel : IViewModel<User>
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Language { get; set; }

        public UserViewModel(Guid id, string username, string password, string email, string name, string? profilePicture, string? language)
        {
            Id = id;
            Username = username;
            Password = password;
            Email = email;
            Name = name;
            ProfilePicture = profilePicture;
            Language = language;
        }

        public User Model()
        {
            return new User();
        }
    }
}
