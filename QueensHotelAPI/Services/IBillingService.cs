using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Interface for Billing service operations
    /// </summary>
    public interface IBillingService
    {
        /// <summary>
        /// Inserts a new billing record with validation and business logic
        /// </summary>
        /// <param name="dto">Billing information to insert</param>
        /// <returns>Result of the billing insertion operation</returns>
        Task<InsertBillingResponseDto> InsertBillingAsync(InsertBillingRequestDto dto);

        /// <summary>
        /// Gets comprehensive billing information with business logic validation
        /// </summary>
        /// <param name="billingId">Billing ID to retrieve information for</param>
        /// <returns>Comprehensive billing information including reservation and customer details</returns>
        Task<GetBillingInfoResponseDto?> GetBillingInfoAsync(int billingId);

        /// <summary>
        /// Gets a list of billing information with business logic validation
        /// </summary>
        /// <param name="billingId">Billing ID to retrieve information for</param>
        /// <returns>A list of billing information including reservation and customer details</returns>
        Task<List<GetBillingInfoResponseDto>> GetBillingInfoListAsync(int billingId);
    }
}