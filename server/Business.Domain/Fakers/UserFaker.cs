using Bogus;
using Business.Domain.Model;
using Business.Domain.Model.DTO;

namespace Business.Domain.Fakers
{
    public class UserFaker
    {
        public readonly Faker<User> _faker;
        public readonly Faker<UserDTO> _fakerDTO;

        public UserFaker()
        {
            _faker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Password, f => f.Random.String())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.ProfilePicture, f => f.Internet.Avatar())
                .RuleFor(u => u.Role, f => f.Random.String())
                .RuleFor(u => u.AccessToken, f => f.Random.String())
                .RuleFor(u => u.RefreshToken, f => f.Random.String())
                .RuleFor(u => u.IsActive, f => f.Random.Bool())
                .RuleFor(u => u.Language, f => f.Internet.Locale)
                .FinishWith((f, u) => u.UpdateCreatedDate());

            _fakerDTO = new Faker<UserDTO>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Password, f => f.Random.String())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.ProfilePicture, f => f.Internet.Avatar())
                .RuleFor(u => u.Role, f => f.Random.String())
                .RuleFor(u => u.AccessToken, f => f.Random.String())
                .RuleFor(u => u.RefreshToken, f => f.Random.String())
                .RuleFor(u => u.IsActive, f => f.Random.Bool())
                .RuleFor(u => u.Language, f => f.Internet.Locale)
                .RuleFor(u => u.CreatedDate, f => f.Date.Past());
        }

        public User Generate()
            => _faker.Generate();

        public ICollection<User> GenerateBetween(int min, int max)
            => _faker.GenerateBetween<User>(min, max);

        public UserDTO GenerateDTO()
            => _fakerDTO.Generate();

        public ICollection<UserDTO> GenerateBetweenDTO(int min, int max)
            => _fakerDTO.GenerateBetween<UserDTO>(min, max);
    }
}
