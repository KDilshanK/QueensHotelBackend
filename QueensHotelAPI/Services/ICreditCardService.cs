using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Interface for Credit Card service operations
    /// </summary>
    public interface ICreditCardService
    {
        /// <summary>
        /// Inserts a new credit card with validation and business logic
        /// </summary>
        /// <param name="dto">Credit card information to insert</param>
        /// <returns>Result of the insertion operation</returns>
        Task<InsertCreditCardResponseDto> InsertCreditCardAsync(InsertCreditCardRequestDto dto);
    }
}