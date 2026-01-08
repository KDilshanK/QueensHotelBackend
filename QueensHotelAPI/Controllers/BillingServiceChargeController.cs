using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    /// <summary>
    /// Controller for Billing Service Charge operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-30
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BillingServiceChargeController : ControllerBase
    {
        private readonly IInsertBillingServiceChargeService _service;
        private readonly ILogger<BillingServiceChargeController> _logger;

        public BillingServiceChargeController(IInsertBillingServiceChargeService service, ILogger<BillingServiceChargeController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Inserts a billing service charge using the InsertBillingServiceCharge stored procedure
        /// </summary>
        /// <param name="request">Billing service charge information including billing ID and service charge ID</param>
        /// <returns>Result of the billing service charge insertion</returns>
        /// <response code="200">Billing service charge inserted successfully</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> InsertBillingServiceCharge([FromBody] InsertBillingServiceChargeRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("Queens Hotel API: Billing service charge insertion request body is null at {Timestamp}",
                        DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    return BadRequest(new
                    {
                        success = false,
                        message = "Request body cannot be null",
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    });
                }

                if (!ModelState.IsValid)
                {
                    var validationErrors = new List<string>();
                    
                    foreach (var modelError in ModelState)
                    {
                        foreach (var error in modelError.Value.Errors)
                        {
                            validationErrors.Add($"{modelError.Key}: {error.ErrorMessage}");
                        }
                    }

                    _logger.LogWarning("Queens Hotel API: Validation failed for billing service charge insertion. Errors: {Errors}",
                        string.Join("; ", validationErrors));

                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = validationErrors,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    });
                }

                _logger.LogInformation("Queens Hotel API: Billing service charge insertion request received for Billing ID: {BillingId}, Service Charge ID: {ServiceChargeId} at {Timestamp}",
                    request.BillingId, request.ServiceChargeId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var result = await _service.InsertBillingServiceChargeAsync(request.BillingId, request.ServiceChargeId);

                if (result.Result == 1)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully inserted billing service charge for Billing ID: {BillingId}, Service Charge ID: {ServiceChargeId} at {Timestamp}",
                        request.BillingId, request.ServiceChargeId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        data = new
                        {
                            billingId = request.BillingId,
                            serviceChargeId = request.ServiceChargeId,
                            createdBy = "admin@gmail.com", // As per your SP
                            createdDateTime = DateTime.Now
                        },
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    });
                }
                else
                {
                    _logger.LogWarning("Queens Hotel API: Billing service charge insertion failed for Billing ID: {BillingId}, Service Charge ID: {ServiceChargeId} at {Timestamp}. Reason: {Message}",
                        request.BillingId, request.ServiceChargeId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), result.Message);

                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message ?? "Failed to insert billing service charge",
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during billing service charge insertion at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while processing your billing service charge insertion request",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow,
                    processedBy = "dilshan-jolanka"
                });
            }
        }

        /// <summary>
        /// Gets billing service charges using the GetBillingServiceCharges stored procedure
        /// </summary>
        /// <param name="billingId">Billing ID to retrieve service charges for</param>
        /// <returns>List of billing service charges with detailed information</returns>
        /// <response code="200">Billing service charges retrieved successfully</response>
        /// <response code="400">If the billing ID is invalid</response>
        /// <response code="404">If no billing service charges are found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{billingId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetBillingServiceCharges(int billingId)
        {
            try
            {
                if (billingId <= 0)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid billing ID provided: {BillingId} at {Timestamp}",
                        billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid billing ID provided",
                        error = $"Billing ID must be greater than 0. Provided ID: {billingId}",
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    });
                }

                _logger.LogInformation("Queens Hotel API: GetBillingServiceCharges request received for Billing ID: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var results = await _service.GetBillingServiceChargesAsync(billingId);

                if (results == null || !results.Any())
                {
                    _logger.LogInformation("Queens Hotel API: No billing service charges found for Billing ID: {BillingId} at {Timestamp}",
                        billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    return NotFound(new
                    {
                        success = false,
                        message = $"No billing service charges found for Billing ID: {billingId}",
                        data = new List<object>(),
                        count = 0,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    });
                }

                _logger.LogInformation("Queens Hotel API: Successfully retrieved {Count} billing service charges for Billing ID: {BillingId} at {Timestamp}",
                    results.Count, billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return Ok(new
                {
                    success = true,
                    message = $"Retrieved {results.Count} billing service charges for Billing ID: {billingId}",
                    data = results,
                    count = results.Count,
                    timestamp = DateTime.UtcNow,
                    processedBy = "dilshan-jolanka"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred while retrieving billing service charges for Billing ID: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving billing service charges",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow,
                    processedBy = "dilshan-jolanka"
                });
            }
        }

        /// <summary>
        /// Test billing service charge connectivity and shows expected request format
        /// </summary>
        [HttpGet("test-connection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult TestBillingServiceChargeConnection()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Billing service charge connection test called");

                return Ok(new
                {
                    success = true,
                    message = "Billing service charge API is ready",
                    connectionStatus = "Available",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    endpoints = new
                    {
                        insertBillingServiceCharge = "POST /api/billingservicecharge",
                        getBillingServiceCharges = "GET /api/billingservicecharge/{billingId}",
                        testConnection = "GET /api/billingservicecharge/test-connection",
                        availableBillingIds = "GET /api/billingservicecharge/available-billing-ids"
                    },
                    samplePostRequest = new
                    {
                        billingId = 123,
                        serviceChargeId = 456
                    },
                    sampleGetRequest = "GET /api/billingservicecharge/123",
                    note = "Make sure to use exact property names (case-sensitive). Both billingId and serviceChargeId must be valid integers. CreatedBy is automatically set to 'admin@gmail.com' in the stored procedure."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Billing service charge test failed");
                
                return Ok(new
                {
                    success = false,
                    message = "Billing service charge test failed",
                    connectionStatus = "Failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        /// <summary>
        /// Gets available billing IDs to help with testing and validation
        /// </summary>
        [HttpGet("available-billing-ids")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAvailableBillingIds()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Getting available billing IDs for validation");

                return Ok(new
                {
                    success = true,
                    message = "To get available billing IDs, use the billing API",
                    endpoints = new
                    {
                        getAllBillingRecords = "/api/billing/0",
                        getSpecificBilling = "/api/billing/{id}"
                    },
                    troubleshooting = new
                    {
                        foreignKeyError = "The billing ID you're using doesn't exist in the Billing table",
                        solution = "First create a billing record using POST /api/billing, then use that billing ID",
                        checkExisting = "Use GET /api/billing/0 to see all existing billing records"
                    },
                    timestamp = DateTime.UtcNow,
                    processedBy = "dilshan-jolanka"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error in available billing IDs endpoint");
                
                return Ok(new
                {
                    success = false,
                    message = "Error getting billing ID information",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}