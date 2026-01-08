using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class TravelAgencyController : ControllerBase
{
    private readonly ITravelAgencyService _service;

    public TravelAgencyController(ITravelAgencyService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> InsertTravelAgency([FromBody] TravelAgencyCreateDto dto)
    {
        var result = await _service.InsertTravelAgencyAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TravelAgencyDto>>> GetTravelAgencies(
        [FromQuery] string agencyName = null,
        [FromQuery] string contactPerson = null,
        [FromQuery] string phone = null,
        [FromQuery] string email = null,
        [FromQuery] string webpageUrl = null)
    {
        var result = await _service.GetTravelAgencyDataAsync(agencyName, contactPerson, phone, email, webpageUrl);
        return Ok(result);
    }
}