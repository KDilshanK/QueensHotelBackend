using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        /// <summary>
        /// Get room/suite details using GetRoomDetails_Data stored procedure
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<object>>> GetRoomDetailsData(
            [FromQuery] string? type = "all",
            [FromQuery] int roomId = 0,
            [FromQuery] int roomNumber = 0,
            [FromQuery] int floorId = 0,
            [FromQuery] int roomTypeId = 0,
            [FromQuery] string accommodation = "Room")
        {
            try
            {
                var data = await _roomService.GetRoomDetailsDataAsync(type, roomId, roomNumber, floorId, roomTypeId, accommodation);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRoomDetailsData API failed");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InsertRoomResponseDto>> InsertRoom([FromBody] InsertRoomRequestDto dto)
        {
            var result = await _roomService.InsertRoomAsync(dto);
            if (result.Success)
                return CreatedAtAction(nameof(InsertRoom), result);
            else
                return BadRequest(result.Message);
        }
    }
}