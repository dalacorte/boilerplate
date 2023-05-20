using Business.Application.UOW;
using Business.Domain.Fakers;
using Business.Domain.Interfaces.Application;
using Business.Domain.Model;
using Business.Domain.Model.DTO;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Business.Api.Test
{
    public class UserControllerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<User>> _validatorMock;
        private readonly Mock<IUserApplication<User, UnitOfWork>> _userApplicationMock;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly Mock<ILogger<UserController>> _loggerMock;
        private readonly UserController _controller;

        private readonly UserFaker _userFaker;

        public UserControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<User>>();
            _userApplicationMock = new Mock<IUserApplication<User, UnitOfWork>>();
            _cacheMock = new Mock<IMemoryCache>();
            _loggerMock = new Mock<ILogger<UserController>>();

            _userFaker = new UserFaker();

            _controller = new UserController(
                mapper: _mapperMock.Object,
                validator: _validatorMock.Object,
                userApplication: _userApplicationMock.Object,
                cache: _cacheMock.Object,
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
            _userApplicationMock.Setup(a => a.GetAll()).ReturnsAsync(users);

            // Act
            IActionResult result = await _controller.Get();

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            IEnumerable<User> returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.True(returnedUsers.Count() > 0);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsUser()
        {
            // Arrange
            User user = _userFaker.Generate();
            _userApplicationMock.Setup(a => a.GetById(user.Id)).ReturnsAsync(user);

            // Act
            IActionResult result = await _controller.GetById(user.Id);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            User returnedUser = Assert.IsType<User>(okResult.Value);
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
            User user = new User();
            CancellationToken cancellation = new CancellationToken();
            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            IValidator<User> valid = (IValidator<User>)_validatorMock.Setup(v => v.ValidateAsync(user, cancellation)).ReturnsAsync(new ValidationResult());
            _userApplicationMock.Setup(a => a.PutUser(userId, user)).ReturnsAsync(user);

            // Act
            IActionResult result = await _controller.Put(userId, dto);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            User returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(user.Id, returnedUser.Id);
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
