using Business.Application.UOW;
using Business.Domain.Fakers;
using Business.Domain.Interfaces.Application;
using Business.Domain.Model;
using Business.Domain.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Business.Api.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkCategory("ControllerBenchmark")]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class UserControllerBenchmark
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IValidator<User>> _validatorMock;
        private Mock<IUserApplication<User, UnitOfWork>> _userApplicationMock;
        private Mock<IMemoryCache> _cacheMock;
        private Mock<ILogger<UserController>> _loggerMock;
        private UserController _controller;

        private UserFaker _userFaker;

        private Guid _userId;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<User>>();
            _userApplicationMock = new Mock<IUserApplication<User, UnitOfWork>>();
            _cacheMock = new Mock<IMemoryCache>();
            _loggerMock = new Mock<ILogger<UserController>>();

            _userFaker = new UserFaker();

            //_controller = new UserController(
            //    mapper: _mapperMock.Object,
            //    validator: _validatorMock.Object,
            //    userApplication: _userApplicationMock.Object,
            //    cache: _cacheMock.Object,
            //    logger: _loggerMock.Object
            //);

            _userId = Guid.NewGuid();
            _userApplicationMock.Setup(a => a.GetById(_userId)).ReturnsAsync(GetMockUser());
        }

        [Benchmark]
        public async Task<IActionResult> GetBenchmark()
        {
            IActionResult result = await _controller.Get();

            return result;
        }

        [Benchmark]
        public async Task<IActionResult> GetByIdBenchmark()
        {
            IActionResult result = await _controller.GetById(_userId);

            return result;
        }

        [Benchmark]
        public async Task<IActionResult> PutBenchmark()
        {
            IActionResult result = await _controller.Put(_userId, GetMockUserDTO());

            return result;
        }

        [Benchmark]
        public async Task<IActionResult> DeleteBenchmark()
        {
            IActionResult result = await _controller.Delete(_userId);

            return result;
        }

        private User GetMockUser()
            => _userFaker.Generate();

        private UserDTO GetMockUserDTO()
            => _userFaker.GenerateDTO();

        private ICollection<User> GetMockUsers()
            => _userFaker.GenerateBetween(1, 10);

        private ICollection<UserDTO> GetMockUsersDTO()
            => _userFaker.GenerateBetweenDTO(1, 10);
    }
}
