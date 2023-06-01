namespace Business.Domain.Model
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }
        public string Name { get; private set; }
        public string? ProfilePicture { get; private set; }
        public string Role { get; private set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public bool IsActive { get; private set; }
        public string? Language { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public User() { }

        public void UpdateId() => Id = Guid.NewGuid();

        public void UpdateUsername(string username) => Username = username;

        public void UpdatePassword(string password) => Password = password;

        public void UpdateEmail(string email) => Email = email;

        public void UpdateName(string name) => Name = name;

        public void UpdateProfilePicture(string? profilePìcture) => ProfilePicture = profilePìcture;

        public void UpdateRole(string role) => Role = role;

        public void UpdateAccessToken(string accessToken) => AccessToken = accessToken;

        public void UpdateRefreshToken(string refreshToken) => RefreshToken = refreshToken;

        public void UpdateIsActive(bool isActive) => IsActive = isActive;

        public void UpdateLanguage(string? language) => Language = language;

        public void UpdateCreatedDate() => CreatedDate = DateTime.Now;
    }
}
