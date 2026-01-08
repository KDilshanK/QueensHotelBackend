using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckinController : ControllerBase
    {
        private readonly ICheckinService _checkinService;

        public CheckinController(ICheckinService checkinService)
        {
            _checkinService = checkinService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InsertCheckinResponseDto>> InsertCheckin([FromBody] InsertCheckinRequestDto dto)
        {
            var result = await _checkinService.InsertCheckinAsync(dto);
            if (result.Success)
                return CreatedAtAction(nameof(InsertCheckin), result);
            else
                return BadRequest(result.Message);
        }
    }
}