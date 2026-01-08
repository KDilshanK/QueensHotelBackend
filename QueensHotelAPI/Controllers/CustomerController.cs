using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.Services;
using QueensHotelAPI.Models;
using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Controllers
{
    /// <summary>
    /// Controller for Customer management operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-05-29 12:39:46
    /// Description: API endpoints for customer data retrieval using GetCustomerData stored procedure
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Get customer data using GetCustomerData stored procedure
        /// Author: dilshan-jolanka
        /// Create date: 2025-05-29 12:39:46
        /// </summary>
        /// <param name="nic">National Identity Card number</param>
        /// <param name="passportId">Passport ID</param>
        /// <param name="emailAddress">Email address</param>
        /// <param name="phoneNo">Phone number</param>
        /// <param name="firstName">First name (supports partial matching)</param>
        /// <param name="lName">Last name (supports partial matching)</param>
        /// <returns>Customer data matching the search criteria</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerDetailsResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CustomerDetailsResult>>> GetCustomerData(
            [FromQuery] string? nic = null,
            [FromQuery] string? passportId = null,
            [FromQuery] string? emailAddress = null,
            [FromQuery] string? phoneNo = null,
            [FromQuery] string? firstName = null,
            [FromQuery] string? lName = null)
        {
            try
            {
                _logger.LogInformation("GetCustomerData API called at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var customers = await _customerService.GetCustomerDataAsync(
                    nic, passportId, emailAddress, phoneNo, firstName, lName);

                _logger.LogInformation("GetCustomerData API completed successfully - {Count} customers found at {Timestamp} by user: {User}",
                    customers.Count(), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCustomerData API failed at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get customer data by ID using GetCustomerData stored procedure
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-27 00:30:00
        /// </summary>
        /// <param name="id">Customer ID to retrieve</param>
        /// <returns>Customer data for the specified ID</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerDetailsResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDetailsResult>> GetCustomerDataById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest($"Invalid customer ID: {id}. Customer ID must be greater than 0.");
                }

                _logger.LogInformation("GetCustomerDataById API called for ID: {CustomerId} at {Timestamp} by user: {User}",
                    id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var customer = await _customerService.GetCustomerDataByIdAsync(id);

                if (customer == null)
                {
                    _logger.LogInformation("GetCustomerDataById API - Customer not found for ID: {CustomerId} at {Timestamp} by user: {User}",
                        id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                    return NotFound($"Customer with ID {id} not found.");
                }

                _logger.LogInformation("GetCustomerDataById API completed successfully for ID: {CustomerId} at {Timestamp} by user: {User}",
                    id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCustomerDataById API failed for ID: {CustomerId} at {Timestamp} by user: {User}",
                    id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> InsertCustomer([FromBody] CreateCustomerDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Password is now optional, so no extra check

                var newCustomerId = await _customerService.InsertCustomerAsync(dto);

                if (newCustomerId.HasValue && newCustomerId.Value > 0)
                    return CreatedAtAction(nameof(GetCustomerDataById), new { id = newCustomerId.Value }, new { CustomerId = newCustomerId.Value });

                return BadRequest("Customer insertion failed or customer already exists.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Customer insertion failed: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UpdateCustomerResponseDto>> UpdateCustomer(int id, [FromBody] UpdateCustomerRequestDto dto)
        {
            if (id != dto.Id) return BadRequest("Customer ID mismatch.");

            var result = await _customerService.UpdateCustomerAsync(dto);
            if (result == null)
                return StatusCode(500, "Unexpected error.");
            if (result.Status == "FAILED")
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Customer login using Customer_LoginCheck stored procedure
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-26 12:40:00
        /// </summary>
        /// <param name="dto">Login credentials (UserName can be email, phone, NIC, or passport)</param>
        /// <returns>Login result with success status, message, and customer ID</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(CustomerLoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerLoginResponseDto>> CustomerLogin([FromBody] CustomerLoginRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
                    return BadRequest("UserName and Password are required.");

                _logger.LogInformation("CustomerLogin API called at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var result = await _customerService.CustomerLoginAsync(dto);

                _logger.LogInformation("CustomerLogin API completed - Success: {Success}, Message: {Message} at {Timestamp} by user: {User}",
                    result.Success, result.Message, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    // Return 401 Unauthorized for login failures
                    return Unauthorized(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CustomerLogin API failed at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}