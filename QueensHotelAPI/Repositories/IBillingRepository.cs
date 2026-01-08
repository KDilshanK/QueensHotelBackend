using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
    /// <summary>
    /// Interface for Billing repository operations
    /// </summary>
    public interface IBillingRepository
    {
        /// <summary>
        /// Inserts a new billing record using the InsertBilling stored procedure
        /// </summary>
        /// <param name="dto">Billing information to insert</param>
        /// <returns>Result of the billing insertion operation</returns>
        Task<InsertBillingResponseDto> InsertBillingAsync(InsertBillingRequestDto dto);

        /// <summary>
        /// Gets comprehensive billing information using the GetBillingInfo stored procedure
        /// </summary>
        /// <param name="billingId">Billing ID to retrieve information for</param>
        /// <returns>Comprehensive billing information including reservation and customer details</returns>
        Task<GetBillingInfoResponseDto?> GetBillingInfoAsync(int billingId);

        /// <summary>
        /// Gets a list of billing information using the GetBillingInfo stored procedure
        /// </summary>
        /// <param name="billingId">Billing ID to retrieve information for</param>
        /// <returns>List of billing information including reservation and customer details</returns>
        Task<List<GetBillingInfoResponseDto>> GetBillingInfoListAsync(int billingId);
    }
}