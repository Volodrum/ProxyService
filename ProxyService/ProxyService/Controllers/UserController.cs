using MicroServices1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProxyService.Interfaces;
using Serilog;
using System.Text.Json;

namespace MicroServices1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            Log.Information("Received request for user with ID {Id}", id);
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                Log.Warning("User with ID {Id} not found", id);
                return NotFound($"User with id {id} not found.");
            }
            Log.Information("Returning user with ID {Id}", id);

            return Ok(user);
        }
    }
}
