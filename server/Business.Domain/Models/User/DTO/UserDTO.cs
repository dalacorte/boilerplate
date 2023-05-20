namespace Business.Domain.Model.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? ProfilePicture { get; set; }
        public string Role { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool? IsActive { get; set; }
        public string? Language { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
