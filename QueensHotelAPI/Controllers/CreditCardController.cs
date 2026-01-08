using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    /// <summary>
    /// Controller for Credit Card management operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CreditCardController : ControllerBase
    {
        private readonly ICreditCardService _creditCardService;
        private readonly ILogger<CreditCardController> _logger;

        public CreditCardController(ICreditCardService creditCardService, ILogger<CreditCardController> logger)
        {
            _creditCardService = creditCardService;
            _logger = logger;
        }

        /// <summary>
        /// Inserts a new credit card securely using encrypted storage
        /// </summary>
        /// <param name="request">Credit card information to be stored securely</param>
        /// <returns>Result of the credit card insertion operation</returns>
        /// <response code="200">Returns the insertion result</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost("insert")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> InsertCreditCard([FromBody] InsertCreditCardRequestDto request)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing credit card insertion request for Customer_Id: {CustomerId} at {Timestamp}",
                    request?.Customer_Id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                if (request == null)
                {
                    var badRequestResponse = new
                    {
                        success = false,
                        message = "Credit card request cannot be null",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                if (request.Customer_Id <= 0)
                {
                    var badRequestResponse = new
                    {
                        success = false,
                        message = "Invalid customer ID provided",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                if (string.IsNullOrWhiteSpace(request.CardHolderName))
                {
                    var badRequestResponse = new
                    {
                        success = false,
                        message = "Card holder name is required",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                if (string.IsNullOrWhiteSpace(request.CardNumber))
                {
                    var badRequestResponse = new
                    {
                        success = false,
                        message = "Card number is required",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                if (string.IsNullOrWhiteSpace(request.CVV))
                {
                    var badRequestResponse = new
                    {
                        success = false,
                        message = "CVV is required",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                if (request.ExpiryMonth < 1 || request.ExpiryMonth > 12)
                {
                    var badRequestResponse = new
                    {
                        success = false,
                        message = "Expiry month must be between 1 and 12",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                if (request.ExpiryYear < DateTime.Now.Year)
                {
                    var badRequestResponse = new
                    {
                        success = false,
                        message = "Expiry year cannot be in the past",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                var result = await _creditCardService.InsertCreditCardAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully inserted credit card for Customer_Id: {CustomerId} at {Timestamp}",
                        request.Customer_Id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    var successResponse = new
                    {
                        success = true,
                        message = "Credit card added successfully",
                        data = result,
                        timestamp = DateTime.UtcNow,
                        apiVersion = "1.0.0",
                        processedBy = "dilshan-jolanka"
                    };

                    return Ok(successResponse);
                }
                else
                {
                    var errorResponse = new
                    {
                        success = false,
                        message = result.Message,
                        data = result,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during credit card insertion at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var response = new
                {
                    success = false,
                    message = "An error occurred while processing your credit card insertion request",
                    data = (object?)null,
                    timestamp = DateTime.UtcNow,
                    processedBy = "dilshan-jolanka"
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Test credit card database connectivity
        /// </summary>
        [HttpGet("test-connection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> TestCreditCardDatabaseConnection()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Credit card database connection test called");

                return Ok(new
                {
                    success = true,
                    message = "Credit card service is ready",
                    connectionStatus = "Available",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    note = "To test actual database connectivity, use the main health check endpoints"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Credit card service test failed");
                
                return Ok(new
                {
                    success = false,
                    message = "Credit card service test failed",
                    connectionStatus = "Failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }
    }
}