using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
    /// <summary>
    /// Interface for Credit Card repository operations
    /// </summary>
    public interface ICreditCardRepository
    {
        /// <summary>
        /// Inserts a new credit card using the InsertCreditCardSecure stored procedure
        /// </summary>
        /// <param name="dto">Credit card information to insert</param>
        /// <returns>Success status of the insertion</returns>
        Task<bool> InsertCreditCardAsync(InsertCreditCardRequestDto dto);
    }
}