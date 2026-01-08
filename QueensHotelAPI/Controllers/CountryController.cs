using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryService countryService, ILogger<CountryController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        /// <summary>
        /// Get list of all countries of residence
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCountryOfResidenceData()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Country endpoint called at {Timestamp}", 
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var result = await _countryService.GetCountryOfResidenceDataAsync();
                
                return Ok(new
                {
                    success = true,
                    message = $"Successfully retrieved {result.Count()} countries",
                    data = result,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("connection"))
            {
                _logger.LogError(ex, "Queens Hotel API: Database connection error in country endpoint");
                
                return StatusCode(503, new
                {
                    success = false,
                    message = "Database connection issue occurred. Please check your network connection and try again.",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred in country endpoint");
                
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving country data",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        /// <summary>
        /// Test country database connectivity
        /// </summary>
        [HttpGet("test-connection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> TestCountryDatabaseConnection()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Country database connection test called");

                // Simple test to verify country service can connect
                var result = await _countryService.GetCountryOfResidenceDataAsync();
                var count = result.Count();
                
                return Ok(new
                {
                    success = true,
                    message = "Country database connection test successful",
                    connectionStatus = "Connected",
                    recordCount = count,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Country database connection test failed");
                
                return Ok(new
                {
                    success = false,
                    message = "Country database connection test failed",
                    connectionStatus = "Failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }
    }
}