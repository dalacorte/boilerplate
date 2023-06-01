using Business.Application.UOW;
using Business.Domain.Fakers;
using Business.Domain.Interfaces.Application;
using Business.Domain.Model;
using Business.Domain.Model.DTO;
using Business.Domain.ViewModels;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Business.Api.Test
{
    public class UserControllerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<User>> _validatorMock;
        private readonly Mock<IUserApplication<User, UnitOfWork>> _userApplicationMock;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly Mock<IStringLocalizer> _localizerMock;
        private readonly Mock<ILogger<UserController>> _loggerMock;
        private readonly UserController _controller;

        private readonly UserFaker _userFaker;

        public UserControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<User>>();
            _userApplicationMock = new Mock<IUserApplication<User, UnitOfWork>>();
            _cacheMock = new Mock<IMemoryCache>();
            _localizerMock = new Mock<IStringLocalizer>();
            _loggerMock = new Mock<ILogger<UserController>>();

            _userFaker = new UserFaker();

            _controller = new UserController(
                mapper: _mapperMock.Object,
                validator: _validatorMock.Object,
                userApplication: _userApplicationMock.Object,
                cache: _cacheMock.Object,
                localizer: _localizerMock.Object,
                logger: _loggerMock.Object
            );
        }

        [Fact]
        public async Task Get_ReturnsListOfUsers()
        {
            // Arrange
            User user1 = _userFaker.Generate();
            User user2 = _userFaker.Generate();
            List<User> users = new List<User> { user1, user2 };
            IEnumerable<UserViewModel> expectedViewModels = new List<UserViewModel>
            {
                new UserViewModel(user1.Id, user1.Username, user1.Password, user1.Email, user1.Name, user1.ProfilePicture, user1.Language),
                new UserViewModel(user2.Id, user2.Username, user2.Password, user2.Email, user2.Name, user2.ProfilePicture, user2.Language)
            };
            _userApplicationMock.Setup(a => a.GetAll()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserViewModel>>(users)).Returns(expectedViewModels);

            // Act
            IActionResult result = await _controller.Get();

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            IEnumerable<UserViewModel> returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(okResult.Value);
            Assert.True(returnedUsers.Count() > 0);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsUser()
        {
            // Arrange
            User user = _userFaker.Generate();
            _userApplicationMock.Setup(a => a.GetById(user.Id)).ReturnsAsync(user);
            UserViewModel expectedViewModels = new UserViewModel(user.Id, user.Username, user.Password, user.Email, user.Name, user.ProfilePicture, user.Language);
            _mapperMock.Setup(m => m.Map<UserViewModel>(user)).Returns(expectedViewModels);

            // Act
            IActionResult result = await _controller.GetById(user.Id);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            UserViewModel returnedUser = Assert.IsType<UserViewModel>(okResult.Value);
            Assert.Equal(user.Id, returnedUser.Id);
        }
        [Fact]
        public async Task GetById_NonexistentId_ReturnsNotFound()
        {
            // Arrange
            Guid nonExistentId = Guid.NewGuid();
            _userApplicationMock.Setup(a => a.GetById(nonExistentId)).ReturnsAsync(null as User);

            // Act
            IActionResult result = await _controller.GetById(nonExistentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Put_ValidData_ReturnsUser()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserDTO dto = _userFaker.GenerateDTO();
            CancellationToken cancellation = new CancellationToken();
            User user = new User();
            user.UpdateId(Guid.NewGuid());
            user.UpdateName(dto.Name);
            user.UpdatePassword(dto.Password);
            user.UpdateEmail(dto.Email);
            user.UpdateName(dto.Name);
            user.UpdateProfilePicture(dto.ProfilePicture);
            user.UpdateRole(dto.Role);
            user.UpdateAccessToken(dto.AccessToken);
            user.UpdateRefreshToken(dto.RefreshToken);
            user.UpdateIsActive(dto.IsActive.Value);
            user.UpdateLanguage(dto.Language);
            user.UpdateCreatedDate();
            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            var valid = _validatorMock.Setup(v => v.ValidateAsync(user, cancellation)).ReturnsAsync(new ValidationResult());
            _userApplicationMock.Setup(a => a.PutUser(userId, user)).ReturnsAsync(user);

            // Act
            IActionResult result = await _controller.Put(userId, dto);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            dynamic propertyInfo = value.GetType().GetProperty("id");
            Guid returnedId = propertyInfo.GetValue(value, null);
            Assert.Equal(user.Id, returnedId);
        }

        [Fact]
        public async Task Put_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserDTO dto = _userFaker.GenerateDTO();
            User user = new User();
            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("PropertyName", "Validation Message") });
            CancellationToken cancellation = new CancellationToken();
            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            _validatorMock.Setup(v => v.ValidateAsync(user, cancellation)).ReturnsAsync(validationResult);

            // Act
            IActionResult result = await _controller.Put(userId, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Put_NonexistentId_ReturnsNotFound()
        {
            // Arrange
            Guid nonExistentId = Guid.NewGuid();
            UserDTO dto = _userFaker.GenerateDTO();
            CancellationToken cancellation = new CancellationToken();
            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(new User());
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<User>(), cancellation)).ReturnsAsync(new ValidationResult());
            _userApplicationMock.Setup(a => a.PutUser(nonExistentId, It.IsAny<User>())).ThrowsAsync(new Exception());

            // Act
            IActionResult result = await _controller.Put(nonExistentId, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingId_ReturnsNoContent()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            _userApplicationMock.Setup(a => a.DeleteById(userId)).Verifiable();

            // Act
            IActionResult result = await _controller.Delete(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _userApplicationMock.Verify(a => a.DeleteById(userId), Times.Once);
        }
    }
}
