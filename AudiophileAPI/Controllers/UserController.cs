using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DataAccess.EF.Repositories;
using AudiophileAPI.DataAccess.EF.Services;
using AudiophileAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AudiophileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        
        //Create a DTO for users


        private readonly UsersRepository _usersRepository;
        private readonly PasswordService _passwordService;

        public UserController(UsersRepository usersRepository, PasswordService passwordService)
        {
            _usersRepository = usersRepository;
            _passwordService = passwordService;
        }



        // GET: api/Users

        [HttpGet("all-users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var user = await _usersRepository.GetAllUsers();
                return Ok(user);
            }
            catch (Exception ex) {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while fetching users.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        //GET:api/Users/5
        [HttpGet("find-by-id/{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _usersRepository.GetUserById(id);

                if (user == null)
                {
                    return NotFound(new
                    {
                        Message = $"User with ID {id} was not found."
                    });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Problem(
                     detail: ex.Message,
                     title: "An error occurred while fetching user.",
                     statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        //POST: api/Users
        [HttpPost("create-user")]
        public async Task<ActionResult> CreateUser([FromBody] UsersDTO user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                {
                    return BadRequest(new
                    {
                        Message = "Email and password are required."
                    });
                }

                var newUser = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PasswordHash = _passwordService.HashPassword(user.Password),
                    Role = user.Role
                };

                var createdUser = await _usersRepository.CreateUser(newUser);

                return CreatedAtAction(nameof(GetUser), new { id = createdUser.UsersId }, createdUser);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while creating the user.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }


        // PUT: api/Users/5
        [HttpPut("update-user/{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                // Check if route ID matches body ID (if applicable)
                if (user.UsersId != 0 && user.UsersId != id)
                {
                    return BadRequest("User ID in the body does not match URL.");
                }

                // Optional: Basic validation
                if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    return BadRequest("Email and password cannot be empty.");
                }

                // Hash the password before updating
                user.PasswordHash = _passwordService.HashPassword(user.PasswordHash);

                var updated = await _usersRepository.UpdateUser(id, user.FirstName, user.LastName, user.Email, user.PasswordHash, user.Role);

                if (updated == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while updating the user.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("delete-user/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _usersRepository.GetUserById(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                await _usersRepository.DeleteUser(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while deleting the user.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

    }
}
