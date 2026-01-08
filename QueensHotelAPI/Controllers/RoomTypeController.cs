using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.Models;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomTypeController : ControllerBase
    {
        private readonly RoomTypeService _service;

        public RoomTypeController(RoomTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomType>>> GetRoomTypes()
        {
            var types = await _service.GetAllRoomTypesAsync();
            return Ok(types);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomType>> GetRoomType(int id)
        {
            var type = await _service.GetRoomTypeByIdAsync(id);
            if (type == null)
                return NotFound();
            return Ok(type);
        }
    }
}