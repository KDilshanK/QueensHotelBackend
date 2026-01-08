using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    /// <summary>
    /// Controller for Billing operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-27 14:50:00
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billingService;
        private readonly ILogger<BillingController> _logger;

        public BillingController(IBillingService billingService, ILogger<BillingController> logger)
        {
            _billingService = billingService;
            _logger = logger;
        }

        /// <summary>
        /// Inserts a new billing record using the InsertBilling stored procedure
        /// </summary>
        /// <param name="request">Billing information including reservation ID, amounts, and payment details</param>
        /// <returns>Result of the billing insertion with success status and billing ID</returns>
        /// <response code="201">Billing record created successfully</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> InsertBilling([FromBody] InsertBillingRequestDto? request)
        {
            try
            {
                // Check if request is null first
                if (request == null)
                {
                    _logger.LogWarning("Queens Hotel API: Billing insertion request body is null at {Timestamp}",
                        DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    var nullRequestResponse = new
                    {
                        success = false,
                        message = "Request body cannot be null",
                        errors = new[] { "The request body is required and cannot be null." },
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(nullRequestResponse);
                }

                _logger.LogInformation("Queens Hotel API: Billing insertion request received for Reservation ID: {ReservationId} at {Timestamp} by user: {User}",
                    request.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), request.CreatedBy);

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

                    _logger.LogWarning("Queens Hotel API: Validation failed for billing insertion. Errors: {Errors}",
                        string.Join("; ", validationErrors));

                    var badRequestResponse = new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = validationErrors,
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                var result = await _billingService.InsertBillingAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully processed billing insertion for Reservation ID: {ReservationId}, Billing ID: {BillingId} at {Timestamp}",
                        request.ReservationId, result.BillingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    var successResponse = new
                    {
                        success = true,
                        message = result.Message,
                        data = new
                        {
                            billingId = result.BillingId,
                            reservationId = request.ReservationId,
                            totalAmount = request.TotalAmount,
                            paymentStatus = request.PaymentStatus,
                            isNoShowCharge = request.IsNoShowCharge,
                            createdDateTime = result.CreatedDateTime,
                            createdBy = request.CreatedBy
                        },
                        timestamp = DateTime.UtcNow,
                        apiVersion = "1.0.0",
                        processedBy = "dilshan-jolanka"
                    };

                    return CreatedAtAction(nameof(InsertBilling), new { billingId = result.BillingId }, successResponse);
                }
                else
                {
                    _logger.LogWarning("Queens Hotel API: Billing insertion failed for Reservation ID: {ReservationId} at {Timestamp} by user: {User}. Reason: {Message}",
                        request.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), request.CreatedBy, result.Message);

                    var errorResponse = new
                    {
                        success = false,
                        message = result.Message,
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };

                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during billing insertion at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var response = new
                {
                    success = false,
                    message = "An error occurred while processing your billing insertion request",
                    error = ex.Message,
                    data = (object?)null,
                    timestamp = DateTime.UtcNow,
                    processedBy = "dilshan-jolanka"
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Test billing service connectivity and shows expected request format
        /// </summary>
        [HttpGet("test-connection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> TestBillingConnection()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Billing service connection test called");

                return Ok(new
                {
                    success = true,
                    message = "Billing service is ready",
                    connectionStatus = "Available",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    endpoints = new
                    {
                        insertBilling = "/api/billing",
                        getBillingInfo = "/api/billing/{id}"
                    },
                    supportedPaymentMethods = new
                    {
                        note = "Payment method IDs 1-10 are supported",
                        examples = new[]
                        {
                            "1 = Cash",
                            "2 = Credit Card",
                            "3 = Debit Card",
                            "4 = Bank Transfer",
                            "5 = Digital Wallet"
                        }
                    },
                    sampleRequest = new
                    {
                        billingDate = "2025-08-27",
                        totalAmount = 1500.50,
                        isNoShowCharge = false,
                        paymentStatus = "Pending",
                        paymentMethod = 2,
                        createdBy = "john.doe",
                        reservationId = 123
                    },
                    note = "Make sure to use exact property names (case-sensitive). For boolean values, use true/false (lowercase)."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Billing service test failed");
                
                return Ok(new
                {
                    success = false,
                    message = "Billing service test failed",
                    connectionStatus = "Failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        /// <summary>
        /// Gets comprehensive billing information using the GetBillingInfo stored procedure
        /// </summary>
        /// <param name="id">Billing ID to retrieve information for</param>
        /// <returns>Comprehensive billing information including reservation and customer details</returns>
        /// <response code="200">Billing information retrieved successfully</response>
        /// <response code="400">If the billing ID is invalid</response>
        /// <response code="404">If the billing record is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetBillingInfo(int id)
        {
            try
            {
                if (id < 0)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid billing ID provided: {BillingId} at {Timestamp}",
                        id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    var badRequestResponse = new
                    {
                        success = false,
                        message = "Invalid billing ID provided",
                        error = $"Billing ID must be greater than or equal to 0. Provided ID: {id}",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                _logger.LogInformation("Queens Hotel API: GetBillingInfo request received for Billing ID: {BillingId} at {Timestamp}",
                    id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                if (id == 0)
                {
                    var results = await _billingService.GetBillingInfoListAsync(0);
                    return Ok(new
                    {
                        success = true,
                        message = $"Retrieved {results.Count} billing records.",
                        count = results.Count,
                        data = results,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    });
                }
                else
                {
                    var result = await _billingService.GetBillingInfoAsync(id);
                    if (result == null)
                    {
                        _logger.LogInformation("Queens Hotel API: No billing record found for Billing ID: {BillingId} at {Timestamp}",
                            id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                        var notFoundResponse = new
                        {
                            success = false,
                            message = $"No billing record found for Billing ID: {id}",
                            data = (object?)null,
                            timestamp = DateTime.UtcNow,
                            processedBy = "dilshan-jolanka"
                        };
                        return NotFound(notFoundResponse);
                    }
                    return Ok(new
                    {
                        success = true,
                        message = $"Billing record found for Billing ID: {id}",
                        data = result,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred while retrieving billing info for Billing ID: {BillingId} at {Timestamp}",
                    id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return StatusCode(500, new
                {
                    success = false,
                    message = $"An error occurred while retrieving billing info for Billing ID: {id}",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow,
                    processedBy = "dilshan-jolanka"
                });
            }
        }
    }
}