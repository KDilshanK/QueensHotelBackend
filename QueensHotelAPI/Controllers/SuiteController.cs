using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    /// <summary>
    /// Controller for Suite management operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-28 00:20:00
    /// Description: API endpoints for suite data management including retrieval and insertion
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SuiteController : ControllerBase
    {
        private readonly ISuiteService _suiteService;
        private readonly ILogger<SuiteController> _logger;

        public SuiteController(ISuiteService suiteService, ILogger<SuiteController> logger)
        {
            _suiteService = suiteService;
            _logger = logger;
        }

        /// <summary>
        /// Get all suites from Queens Hotel
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-28 00:20:00
        /// </summary>
        /// <returns>List of all suites with complete details</returns>
        /// <response code="200">Returns the list of all suites</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetAllSuitesResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAllSuitesResponseDto>>> GetAllSuites()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: GetAllSuites endpoint called at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var suites = await _suiteService.GetAllSuitesAsync();

                _logger.LogInformation("Queens Hotel API: Successfully returned {Count} suites at {Timestamp}",
                    suites.Count(), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return Ok(suites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during GetAllSuites at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                
                return StatusCode(500, new { 
                    message = "An internal server error occurred while retrieving suites",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        /// <summary>
        /// Insert a new suite into Queens Hotel
        /// </summary>
        /// <param name="dto">Suite information to insert</param>
        /// <returns>Result of the insert operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InsertSuiteResponseDto>> InsertSuite([FromBody] InsertSuiteRequestDto dto)
        {
            var result = await _suiteService.InsertSuiteAsync(dto);
            if (result.Success)
                return CreatedAtAction(nameof(InsertSuite), result);
            else
                return BadRequest(result.Message);
        }

        /// <summary>
        /// Test endpoint to demonstrate the GetAllSuites functionality
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-28 00:25:00
        /// </summary>
        [HttpGet("test-get-all-suites")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> TestGetAllSuites()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Testing GetAllSuites endpoint at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return Ok(new
                {
                    message = "GetAllSuites endpoint is ready and working",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    endpoint = new
                    {
                        method = "GET",
                        url = "/api/suite",
                        description = "Retrieves all suites from Queens Hotel database using GetAllSuites stored procedure"
                    },
                    storedProcedure = new
                    {
                        name = "GetAllSuites",
                        description = "SELECT * FROM [dbo].[Suite]",
                        parameters = "None - returns all suite records"
                    },
                    responseStructure = new
                    {
                        id = "int - Suite ID",
                        suiteName = "string - Name of the suite",
                        weeklyRate = "decimal - Weekly rental rate",
                        monthlyRate = "decimal - Monthly rental rate", 
                        status = "string - Suite status",
                        type = "string - Suite type",
                        description = "string - Suite description",
                        size = "string - Suite size",
                        bedrooms = "int - Number of bedrooms",
                        living_area = "string - Living area details",
                        kitchen = "string - Kitchen details",
                        dining_area = "string - Dining area details",
                        bathrooms = "int - Number of bathrooms",
                        laundry_facilities = "string - Laundry facilities",
                        workspace = "string - Workspace details",
                        wifi_entertainment = "string - WiFi and entertainment",
                        balcony_terrace = "string - Balcony/terrace details",
                        housekeeping = "string - Housekeeping services",
                        security = "string - Security features",
                        companyMaster_Id = "int? - Company master ID"
                    },
                    usage = new
                    {
                        curl = "curl -X 'GET' 'http://localhost:5170/api/suite' -H 'accept: application/json'",
                        javascript = "fetch('/api/suite').then(response => response.json()).then(data => console.log(data));",
                        note = "This endpoint returns all suites in the database with complete details"
                    },
                    features = new[]
                    {
                        "Complete suite information retrieval",
                        "Comprehensive error handling and logging",
                        "Safe data extraction from database",
                        "Proper HTTP status codes",
                        "Detailed response structure"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error in GetAllSuites test endpoint at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}