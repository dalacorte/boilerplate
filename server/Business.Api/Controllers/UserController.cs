using AutoMapper;
using Business.Api.Controllers;
using Business.Application.UOW;
using Business.Domain.Interfaces.Application;
using Business.Domain.Model;
using Business.Domain.Model.DTO;
using Business.Domain.ViewModels;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Controllers
{
    ///<Summary>
    /// User Controller :)
    ///</Summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
    public class UserController : BaseController<User, UnitOfWork, UserViewModel, UserController>
    {
        private readonly IUserApplication<User, UnitOfWork> _userApplication;

        ///<Summary>
        /// Constructor
        ///</Summary>
        public UserController(IMapper mapper,
                              IValidator<User> validator,
                              IUserApplication<User, UnitOfWork> userApplication,
                              IMemoryCache cache,
                              ILogger<UserController> logger) : base(mapper, validator, userApplication, cache, logger)
        {
            _userApplication = userApplication;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        /// <response code="200">Users retrieved successfully.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserViewModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            IEnumerable<User> users = await _application.GetAll();
            IEnumerable<UserViewModel> usersViewModel = _mapper.Map<IEnumerable<UserViewModel>>(users);

            return Ok(usersViewModel);
        }

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user with the specified ID.</returns>
        /// <response code="200">Returns the user with the specified ID.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            User user = await _application.GetById(id);
            UserViewModel userViewModel = _mapper.Map<UserViewModel>(user);

            return userViewModel is null ? NotFound() : Ok(userViewModel);
        }

        /// <summary>
        /// Updates a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="dto">The data to update the user with.</param>
        /// <returns>The updated user.</returns>
        /// <response code="200">Returns the updated user.</response>
        /// <response code="400">Bad request if the provided data is invalid.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">User not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UserDTO dto)
        {
            User user = _mapper.Map<User>(dto);
            ValidationResult result = await _validator.ValidateAsync(user);

            if (!result.IsValid) return BadRequest(Results.ValidationProblem(result.ToDictionary()));

            try
            {
                User u = await _userApplication.PutUser(id, user);

                _cache.GetOrCreate(u.Id, item =>
                {
                    item.Value = u.Username;
                    return item;
                });

                return Ok(new { id = u.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: {HttpContext.Request.Path} -> {ex}");
                return NotFound();
            }
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">User deleted successfully.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _application.DeleteById(id);

            return NoContent();
        }
    }
}
