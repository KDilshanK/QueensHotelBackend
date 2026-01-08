using QueensHotelAPI.DTOs;
using QueensHotelAPI.Models;

namespace QueensHotelAPI.Repositories
{
    public interface ICustomerRepository
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