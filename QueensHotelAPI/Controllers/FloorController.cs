using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FloorController : ControllerBase
{
    private readonly IFloorService _service;

    public FloorController(IFloorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FloorDto>>> GetAllFloors()
    {
        var floors = await _service.GetAllFloorsAsync();
        return Ok(floors);
    }
}