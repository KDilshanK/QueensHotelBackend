using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace QueensHotelAPI.Controllers
{
    /// <summary>
    /// Base controller with CORS enabled for all derived controllers
    /// This ensures CORS is applied to all API controllers in the project
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-27 16:20:00
    /// </summary>
    [EnableCors("QueensHotelApiPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        // Base controller implementation can be extended here if needed
        // All controllers that inherit from this will automatically have CORS enabled
    }
}