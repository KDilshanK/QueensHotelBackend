using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    /// <summary>
    /// Controller for Customer Invoice operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-26 22:30:00
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomerInvoiceController : ControllerBase
    {
        private readonly ICustomerInvoiceService _customerInvoiceService;
        private readonly ILogger<CustomerInvoiceController> _logger;

        public CustomerInvoiceController(ICustomerInvoiceService customerInvoiceService, ILogger<CustomerInvoiceController> logger)
        {
            _customerInvoiceService = customerInvoiceService;
            _logger = logger;
        }

        /// <summary>
        /// Generates a comprehensive customer invoice using the GenerateCustomerInvoice stored procedure
        /// </summary>
        /// <param name="billingId">The billing ID to generate invoice for</param>
        /// <returns>Complete invoice with header, customer info, charges, payments and statistics</returns>
        /// <response code="200">Returns the complete invoice information</response>
        /// <response code="400">If the billing ID is invalid</response>
        /// <response code="404">If the billing record is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{billingId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GenerateCustomerInvoice(int billingId)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Invoice generation request received for BillingId: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                if (billingId <= 0)
                {
                    var badRequestResponse = new
                    {
                        success = false,
                        message = "Invalid billing ID provided. Billing ID must be greater than 0.",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                var result = await _customerInvoiceService.GenerateCustomerInvoiceAsync(billingId);

                // Check for various error conditions based on invoice number
                if (result.InvoiceHeader.InvoiceNumber == "ERROR")
                {
                    var notFoundResponse = new
                    {
                        success = false,
                        message = $"Billing record with ID {billingId} not found or is invalid",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return NotFound(notFoundResponse);
                }

                if (result.InvoiceHeader.InvoiceNumber == "CONNECTION_ERROR")
                {
                    var connectionErrorResponse = new
                    {
                        success = false,
                        message = "Database connection issue occurred while generating invoice. Please check your network connection and try again.",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return StatusCode(503, connectionErrorResponse);
                }

                if (result.InvoiceHeader.InvoiceNumber == "RETRY_ERROR")
                {
                    var retryErrorResponse = new
                    {
                        success = false,
                        message = "The database service is temporarily unavailable. Please try again in a few minutes.",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return StatusCode(503, retryErrorResponse);
                }

                if (result.InvoiceHeader.InvoiceNumber == "SYSTEM_ERROR" || string.IsNullOrEmpty(result.InvoiceHeader.InvoiceNumber))
                {
                    var systemErrorResponse = new
                    {
                        success = false,
                        message = "An error occurred while generating the invoice. Please contact support if the problem persists.",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return StatusCode(500, systemErrorResponse);
                }

                _logger.LogInformation("Queens Hotel API: Successfully generated invoice {InvoiceNumber} for BillingId: {BillingId} at {Timestamp}",
                    result.InvoiceHeader.InvoiceNumber, billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var successResponse = new
                {
                    success = true,
                    message = $"Invoice {result.InvoiceHeader.InvoiceNumber} generated successfully",
                    data = result,
                    timestamp = DateTime.UtcNow,
                    apiVersion = "1.0.0",
                    processedBy = "dilshan-jolanka"
                };

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during invoice generation at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var response = new
                {
                    success = false,
                    message = "An error occurred while processing your invoice generation request",
                    data = (object?)null,
                    timestamp = DateTime.UtcNow,
                    processedBy = "dilshan-jolanka"
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Generates customer invoice using POST method with request body
        /// </summary>
        /// <param name="request">Request containing billing ID</param>
        /// <returns>Complete invoice information</returns>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GenerateCustomerInvoicePost([FromBody] GenerateCustomerInvoiceRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    var badRequestResponse = new
                    {
                        success = false,
                        message = "Request body cannot be null",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow,
                        processedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                // Delegate to the GET method logic
                return await GenerateCustomerInvoice(request.BillingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during POST invoice generation at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var response = new
                {
                    success = false,
                    message = "An error occurred while processing your invoice generation request",
                    data = (object?)null,
                    timestamp = DateTime.UtcNow,
                    processedBy = "dilshan-jolanka"
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Test customer invoice service connectivity
        /// </summary>
        [HttpGet("test-connection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> TestCustomerInvoiceConnection()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Customer invoice service connection test called");

                return Ok(new
                {
                    success = true,
                    message = "Customer invoice service is ready",
                    connectionStatus = "Available",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    endpoints = new
                    {
                        generateByGet = "/api/customerinvoice/{billingId}",
                        generateByPost = "/api/customerinvoice/generate"
                    },
                    note = "To test actual database connectivity, use the main health check endpoints"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Customer invoice service test failed");
                
                return Ok(new
                {
                    success = false,
                    message = "Customer invoice service test failed",
                    connectionStatus = "Failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }
    }
}