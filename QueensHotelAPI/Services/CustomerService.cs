using QueensHotelAPI.DTOs;
using QueensHotelAPI.Models;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Service implementation for Customer business logic
    /// Author: dilshan-jolanka
    /// Create date: 2025-05-29 12:39:46
    /// Description: Business layer implementation for customer operations
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerDetailsResult>> GetCustomerDataAsync(
            string? nic = null,
            string? passportId = null,
            string? emailAddress = null,
            string? phoneNo = null,
            string? firstName = null,
            string? lName = null)
        {
            try
            {
                _logger.LogInformation("Customer service GetCustomerData called at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var customers = await _customerRepository.GetCustomerDataAsync(
                    nic, passportId, emailAddress, phoneNo, firstName, lName);

                _logger.LogInformation("Customer service GetCustomerData completed - {Count} customers found at {Timestamp} by user: {User}",
                    customers.Count(), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer service GetCustomerData failed at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                throw;
            }
        }

        public async Task<CustomerDetailsResult?> GetCustomerDataByIdAsync(int customerId)
        {
            try
            {
                _logger.LogInformation("Customer service GetCustomerDataById called for CustomerId: {CustomerId} at {Timestamp} by user: {User}",
                    customerId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var customer = await _customerRepository.GetCustomerDataByIdAsync(customerId);

                if (customer != null)
                {
                    _logger.LogInformation("Customer service GetCustomerDataById completed - Customer found for ID: {CustomerId} at {Timestamp} by user: {User}",
                        customerId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                }
                else
                {
                    _logger.LogInformation("Customer service GetCustomerDataById completed - No customer found for ID: {CustomerId} at {Timestamp} by user: {User}",
                        customerId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                }

                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer service GetCustomerDataById failed for CustomerId: {CustomerId} at {Timestamp} by user: {User}",
                    customerId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                throw;
            }
        }

        public async Task<int?> InsertCustomerAsync(CreateCustomerDto dto)
        {
            return await _customerRepository.InsertCustomerAsync(dto);
        }

        public async Task<UpdateCustomerResponseDto?> UpdateCustomerAsync(UpdateCustomerRequestDto dto)
        {
            return await _customerRepository.UpdateCustomerAsync(dto);
        }

        public async Task<CustomerLoginResponseDto> CustomerLoginAsync(CustomerLoginRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Customer service CustomerLogin called at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var result = await _customerRepository.CustomerLoginAsync(dto);

                _logger.LogInformation("Customer service CustomerLogin completed - Success: {Success} at {Timestamp} by user: {User}",
                    result.Success, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer service CustomerLogin failed at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                throw;
            }
        }
    }
}