using QueensHotelAPI.DTOs;
using QueensHotelAPI.Models;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Service interface for Customer business logic
    /// Author: dilshan-jolanka
    /// Create date: 2025-05-29 12:39:46
    /// Description: Business layer interface for customer operations
    /// </summary>
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDetailsResult>> GetCustomerDataAsync(
            string? nic = null,
            string? passportId = null,
            string? emailAddress = null,
            string? phoneNo = null,
            string? firstName = null,
            string? lName = null);
        
        Task<CustomerDetailsResult?> GetCustomerDataByIdAsync(int customerId);
        
        Task<int?> InsertCustomerAsync(CreateCustomerDto dto);
        Task<UpdateCustomerResponseDto?> UpdateCustomerAsync(UpdateCustomerRequestDto dto);
        Task<CustomerLoginResponseDto> CustomerLoginAsync(CustomerLoginRequestDto dto);
    }
}