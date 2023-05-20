using AutoMapper;
using Business.Application.UOW;
using Business.Domain.Interfaces.Application;
using Business.Domain.Model;
using Business.Domain.Model.DTO;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IValidator<User> _validator;
        private readonly IUserApplication<User, UnitOfWork> _userApplication;
        private readonly ITokenApplication _tokenApplication;
        private readonly IMemoryCache _cache;

        public AuthController(IMapper mapper,
                              IValidator<User> validator,
                              IUserApplication<User, UnitOfWork> userApplication,
                              ITokenApplication tokenApplication,
                              IMemoryCache cache)
        {
            _mapper = mapper;
            _validator = validator;
            _userApplication = userApplication;
            _tokenApplication = tokenApplication;
            _cache = cache;
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] UserDTO dto)
        {
            User user = _mapper.Map<User>(dto);
            ValidationResult resultModel = await _validator.ValidateAsync(user);

            if (!resultModel.IsValid) return BadRequest(Results.ValidationProblem(resultModel.ToDictionary()));

            if (!await _userApplication.VerifyIfUserExistsByEmail(dto.Email))
            {
                await _userApplication.Insert(user);

                _cache.GetOrCreate(user.Id, item =>
                {
                    item.Value = user.Username;
                    return item;
                });

                return Ok(await _tokenApplication.GenerateJWT(user));
            }

            return Conflict("User already exists");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            User user = await _userApplication.GetUserByEmailPassword(dto.Email, dto.Password);

            if (user is null) return BadRequest("User or password invalid");

            return Ok(await _tokenApplication.GenerateJWT(user));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] UserLogoutDTO dto)
        {
            ClaimsPrincipal user = HttpContext.User;

            if (user is not null)
            {
                User u = await _userApplication.GetUserByAccessToken(dto.AccessToken);
                await _userApplication.DeleteTokens(u.Id);

                return Ok();
            }

            return BadRequest("Invalid access token");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] UserRefreshtDTO dto)
        {
            if (string.IsNullOrEmpty(dto.RefreshToken)) return BadRequest("Invalid refresh token");

            User user = await _userApplication.GetUserByRefreshToken(dto.RefreshToken);

            if (user is null) return BadRequest("Invalid refresh token");

            return Ok(await _tokenApplication.GenerateJWT(user));
        }
    }
}
