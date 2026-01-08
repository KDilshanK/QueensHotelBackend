using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
    /// <summary>
    /// Interface for Customer Invoice repository operations
    /// </summary>
    public interface ICustomerInvoiceRepository
    {
        /// <summary>
        /// Generates customer invoice using the GenerateCustomerInvoice stored procedure
        /// </summary>
        /// <param name="billingId">The billing ID to generate invoice for</param>
        /// <returns>Complete invoice information with all details</returns>
        Task<GenerateCustomerInvoiceResponseDto> GenerateCustomerInvoiceAsync(int billingId);
    }
}