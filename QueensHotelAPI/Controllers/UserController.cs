using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserLoginResponseDto>> Login([FromBody] UserLoginRequestDto dto)
        {
            var result = await _userService.UserLoginAsync(dto);
            if (result == null || result.LoginStatus != "SUCCESS")
                return Unauthorized(result ?? new UserLoginResponseDto { LoginStatus = "FAILED", Message = "Invalid login." });

            return Ok(result);
        }
    }
}