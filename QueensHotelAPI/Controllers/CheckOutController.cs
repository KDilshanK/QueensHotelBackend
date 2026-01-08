using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CheckOutController : ControllerBase
{
    private readonly ICheckOutService _service;

    public CheckOutController(ICheckOutService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<CheckOutResultDto>> InsertCheckOut([FromBody] InsertCheckOutRequestDto dto)
    {
        try
        {
            var result = await _service.InsertCheckOutAsync(dto.CheckInId, dto.UserId);
            if (result == null)
                return StatusCode(500, "Could not process checkout due to an unexpected internal error.");

            if (result.CheckOutID == 0)
                return BadRequest(result.Result ?? "Could not process checkout.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log ex.ToString() here!
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("details")]
    public async Task<ActionResult<List<CheckOutDetailsDto>>> GetCheckOutDetails(
    [FromQuery] int checkOutId = 0,
    [FromQuery] int reservationId = 0,
    [FromQuery] int roomId = 0,
    [FromQuery] string? customerNIC = null,
    [FromQuery] string? customerEmail = null,
    [FromQuery] string? customerPhone = null,
    [FromQuery] DateTime? checkInDate = null)
    {
        var result = await _service.GetCheckOutDetailsAsync(
            checkOutId,
            reservationId,
            roomId,
            customerNIC,
            customerEmail,
            customerPhone,
            checkInDate
        );
        return Ok(result);
    }
}

public class InsertCheckOutRequestDto
{
    public int CheckInId { get; set; }
    public int UserId { get; set; }
}