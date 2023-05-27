using AutoMapper;
using Business.Api.Controllers;
using Business.Application.UOW;
using Business.Domain.Interfaces.Application;
using Business.Domain.Model;
using Business.Domain.Model.DTO;
using Business.Domain.ViewModels;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Api.Controllers
{
    ///<Summary>
    /// Auth Controller :)
    ///</Summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("{language:regex(^[[a-z]]{{2}}(?:-[[A-Z]]{{2}})?$)}/api/v{version:apiVersion}/auth")]
    public class AuthController : BaseController<User, UnitOfWork, UserViewModel, AuthController>
    {
        private readonly IUserApplication<User, UnitOfWork> _userApplication;
        private readonly ITokenApplication _tokenApplication;
        private readonly IdentityConfig _identity;

        ///<Summary>
        /// Constructor
        ///</Summary>
        public AuthController(IMapper mapper,
                              IValidator<User> validator,
                              IUserApplication<User, UnitOfWork> userApplication,
                              ITokenApplication tokenApplication,
                              IOptions<IdentityConfig> identity,
                              IMemoryCache cache,
                              IStringLocalizer localizer,
                              ILogger<AuthController> logger) : base(mapper, validator, userApplication, cache, localizer, logger)
        {
            _userApplication = userApplication;
            _tokenApplication = tokenApplication;
            _identity = identity.Value;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="dto">The user data.</param>
        /// <returns>The registered user's JWT token if registration is successful.</returns>
        /// <response code="200">Returns the registered user's JWT token.</response>
        /// <response code="400">If the provided user data is invalid.</response>
        /// <response code="409">If a user with the same email already exists.</response>
        [HttpPost("registration")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Registration([FromBody] UserDTO dto)
        {
            User user = _mapper.Map<User>(dto);
            ValidationResult resultModel = await _validator.ValidateAsync(user);

            if (!resultModel.IsValid) return BadRequest(Results.ValidationProblem(resultModel.ToDictionary()));

            if (!await _userApplication.VerifyIfUserExistsByEmail(dto.Email))
            {
                await _userApplication.Insert(user);

                IdentityConfig config = new IdentityConfig();
                config.Secret = _identity.Secret;
                config.Expires = _identity.Expires;
                config.ValidIssuer = _identity.ValidIssuer;
                config.ValidAudience = _identity.ValidAudience;

                return Ok(await _tokenApplication.GenerateJWT(user, config));
            }

            return Conflict(_localizer["UserAlreadyExists"].Value);
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="dto">The user login data.</param>
        /// <returns>The JWT token if authentication is successful.</returns>
        /// <response code="200">Returns the JWT token if authentication is successful.</response>
        /// <response code="400">If the provided email or password is invalid.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            User user = await _userApplication.GetUserByEmailPassword(dto.Email, dto.Password);

            if (user is null) return BadRequest(_localizer["UserOrPasswordInvalid"].Value);

            IdentityConfig config = new IdentityConfig();
            config.Secret = _identity.Secret;
            config.Expires = _identity.Expires;
            config.ValidIssuer = _identity.ValidIssuer;
            config.ValidAudience = _identity.ValidAudience;

            return Ok(await _tokenApplication.GenerateJWT(user, config));
        }

        /// <summary>
        /// Logs out a user by deleting their access tokens.
        /// </summary>
        /// <param name="dto">The user logout data.</param>
        /// <returns>No content if logout is successful.</returns>
        /// <response code="200">No content if logout is successful.</response>
        /// <response code="400">If the provided access token is invalid.</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout([FromBody] UserLogoutDTO dto)
        {
            ClaimsPrincipal user = HttpContext.User;

            if (user is not null)
            {
                User u = await _userApplication.GetUserByAccessToken(dto.AccessToken);
                await _userApplication.DeleteTokens(u.Id);

                return Ok();
            }

            return BadRequest(_localizer["InvalidAccessToken"].Value);
        }

        /// <summary>
        /// Refreshes the access token using a refresh token.
        /// </summary>
        /// <param name="dto">The user refresh data.</param>
        /// <returns>The new access token.</returns>
        /// <response code="200">The new access token.</response>
        /// <response code="400">If the provided refresh token is invalid or empty.</response>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] UserRefreshtDTO dto)
        {
            if (string.IsNullOrEmpty(dto.RefreshToken)) return BadRequest(_localizer["InvalidRefreshToken"].Value);

            User user = await _userApplication.GetUserByRefreshToken(dto.RefreshToken);

            if (user is null) return BadRequest(_localizer["InvalidRefreshToken"].Value);

            IdentityConfig config = new IdentityConfig();
            config.Secret = _identity.Secret;
            config.Expires = _identity.Expires;
            config.ValidIssuer = _identity.ValidIssuer;
            config.ValidAudience = _identity.ValidAudience;

            return Ok(await _tokenApplication.GenerateJWT(user, config));
        }
    }
}
