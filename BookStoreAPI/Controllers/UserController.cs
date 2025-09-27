using Books_Core.Interface;
using Books_Core.Models;
using Books_Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // Registration (anonymous)
        [HttpPost("register")]
        [AllowAnonymous]
        public ActionResult Register([FromBody] RegisterRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "username and password required" });

            try
            {
                var user = _userService.Register(req.Username, req.Email, req.Password);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { user.Id, user.UserName, user.Email, user.IsVerified, user.Roles });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Admin only: list unverified users
        [HttpGet("unverified")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<object>> GetUnverified()
        {
            var users = _userService.GetUnverifiedUsers()
                .Select(u => new { u.Id, u.UserName, u.Email, u.IsVerified, u.Roles });
            return Ok(users);
        }

        // Admin only: set verified
        [HttpPut("{id:length(24)}/verify")]
        [Authorize(Roles = "Admin")]
        public ActionResult VerifyUser(string id)
        {
            try
            {
                _userService.VerifyUser(id);
                return Ok(new { message = "User verified." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "User not found." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Admin only: change roles (accepts multiple roles)
        [HttpPut("{id:length(24)}/roles")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateRoles(string id, [FromBody] UpdateRolesRequest req)
        {
            if (req == null || req.Roles == null || req.Roles.Count == 0)
                return BadRequest(new { message = "At least one role is required." });

            try
            {
                _userService.UpdateRoles(id, req.Roles);
                return Ok(new { message = "Roles updated." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "User not found." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Admin only: get all users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<object>> GetAll()
        {
            var users = _userService.GetAllUsers()
                                   .Select(u => new { u.Id, u.UserName, u.Email, u.IsVerified, u.Roles });
            return Ok(users);
        }

        // Helper endpoint used by the CreatedAtAction above
        [HttpGet("{id:length(24)}")]
        [Authorize(Roles = "Admin,Moderator,ReadOnly")]
        public ActionResult<object> GetById(string id)
        {
            var user = _userService.GetAllUsers().FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound(new { message = "User not found." });
            return Ok(new { user.Id, user.UserName, user.Email, user.IsVerified, user.Roles });
        }
    }

    // DTOs
    public class RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = null!;
    }

    public class UpdateRolesRequest
    {
        public List<string> Roles { get; set; } = new List<string>();
    }
}
