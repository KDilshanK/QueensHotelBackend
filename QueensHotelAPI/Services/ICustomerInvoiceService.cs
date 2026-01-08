using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Interface for Customer Invoice service operations
    /// </summary>
    public interface ICustomerInvoiceService
    {
        /// <summary>
        /// Generates customer invoice with validation and business logic
        /// </summary>
        /// <param name="billingId">The billing ID to generate invoice for</param>
        /// <returns>Complete invoice information or error details</returns>
        Task<GenerateCustomerInvoiceResponseDto> GenerateCustomerInvoiceAsync(int billingId);
    }
}