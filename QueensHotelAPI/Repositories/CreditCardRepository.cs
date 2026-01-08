using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using System.Data;

namespace QueensHotelAPI.Repositories
{
    /// <summary>
    /// Repository implementation for Credit Card data operations
    /// </summary>
    public class CreditCardRepository : ICreditCardRepository
    {
        private readonly QueensHotelDbContext _context;
        private readonly ILogger<CreditCardRepository> _logger;

        public CreditCardRepository(QueensHotelDbContext context, ILogger<CreditCardRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> InsertCreditCardAsync(InsertCreditCardRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing InsertCreditCardSecure stored procedure for Customer_Id: {CustomerId} at {Timestamp}",
                    dto.Customer_Id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var parameters = new[]
                {
                    new SqlParameter("@CardHolderName", SqlDbType.NVarChar, 100) { Value = dto.CardHolderName },
                    new SqlParameter("@CardNumber", SqlDbType.NVarChar, 20) { Value = dto.CardNumber },
                    new SqlParameter("@CardType", SqlDbType.NVarChar, 20) { Value = dto.CardType },
                    new SqlParameter("@ExpiryMonth", SqlDbType.Int) { Value = dto.ExpiryMonth },
                    new SqlParameter("@ExpiryYear", SqlDbType.Int) { Value = dto.ExpiryYear },
                    new SqlParameter("@CVV", SqlDbType.NVarChar, 10) { Value = dto.CVV },
                    new SqlParameter("@IsDefault", SqlDbType.Bit) { Value = dto.IsDefault },
                    new SqlParameter("@Status", SqlDbType.Bit) { Value = dto.Status },
                    new SqlParameter("@Customer_Id", SqlDbType.Int) { Value = dto.Customer_Id },
                    new SqlParameter("@CardType_id", SqlDbType.Int) { Value = dto.CardType_id },
                    new SqlParameter("@CompanyMaster_Id", SqlDbType.Int) { Value = dto.CompanyMaster_Id }
                };

                try
                {
                    // First try with Entity Framework's ExecuteSqlRaw (includes retry logic)
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC [dbo].[InsertCreditCardSecure] @CardHolderName, @CardNumber, @CardType, @ExpiryMonth, @ExpiryYear, @CVV, @IsDefault, @Status, @Customer_Id, @CardType_id, @CompanyMaster_Id",
                        parameters);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("SqlServerRetryingExecutionStrategy"))
                {
                    // If the retry strategy fails, try once more without it
                    _logger.LogWarning("Queens Hotel API: Retry strategy failed, attempting direct execution for credit card insertion for Customer_Id: {CustomerId}", dto.Customer_Id);
                    
                    await ExecuteInsertCreditCardDirectlyAsync(dto);
                }

                _logger.LogInformation("Queens Hotel API: Successfully inserted credit card for Customer_Id: {CustomerId} at {Timestamp}",
                    dto.Customer_Id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return true;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Queens Hotel API: SQL Server error occurred while inserting credit card for Customer_Id: {CustomerId}. Error: {ErrorMessage}",
                    dto.Customer_Id, sqlEx.Message);
                
                // Check if it's a connection-related error
                if (IsConnectionError(sqlEx))
                {
                    throw new InvalidOperationException($"Database connection failed while inserting credit card for Customer_Id {dto.Customer_Id}. Please check your network connection and try again.", sqlEx);
                }
                else
                {
                    throw new InvalidOperationException($"Database error occurred while inserting credit card for Customer_Id {dto.Customer_Id}: {sqlEx.Message}", sqlEx);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Unexpected error occurred while inserting credit card for Customer_Id: {CustomerId}",
                    dto.Customer_Id);
                throw new InvalidOperationException($"An unexpected error occurred while inserting credit card for Customer_Id {dto.Customer_Id}: {ex.Message}", ex);
            }
        }

        private async Task ExecuteInsertCreditCardDirectlyAsync(InsertCreditCardRequestDto dto)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            
            using var command = new SqlCommand("[dbo].[InsertCreditCardSecure]", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };
            
            command.Parameters.Add(new SqlParameter("@CardHolderName", SqlDbType.NVarChar, 100) { Value = dto.CardHolderName });
            command.Parameters.Add(new SqlParameter("@CardNumber", SqlDbType.NVarChar, 20) { Value = dto.CardNumber });
            command.Parameters.Add(new SqlParameter("@CardType", SqlDbType.NVarChar, 20) { Value = dto.CardType });
            command.Parameters.Add(new SqlParameter("@ExpiryMonth", SqlDbType.Int) { Value = dto.ExpiryMonth });
            command.Parameters.Add(new SqlParameter("@ExpiryYear", SqlDbType.Int) { Value = dto.ExpiryYear });
            command.Parameters.Add(new SqlParameter("@CVV", SqlDbType.NVarChar, 10) { Value = dto.CVV });
            command.Parameters.Add(new SqlParameter("@IsDefault", SqlDbType.Bit) { Value = dto.IsDefault });
            command.Parameters.Add(new SqlParameter("@Status", SqlDbType.Bit) { Value = dto.Status });
            command.Parameters.Add(new SqlParameter("@Customer_Id", SqlDbType.Int) { Value = dto.Customer_Id });
            command.Parameters.Add(new SqlParameter("@CardType_id", SqlDbType.Int) { Value = dto.CardType_id });
            command.Parameters.Add(new SqlParameter("@CompanyMaster_Id", SqlDbType.Int) { Value = dto.CompanyMaster_Id });
            
            await command.ExecuteNonQueryAsync();
        }

        private static bool IsConnectionError(SqlException sqlException)
        {
            // Common SQL Server connection error numbers
            var connectionErrorNumbers = new[] { 2, 53, 121, 232, 258, 1231, 1232, 11001, 18456, 4060 };
            return connectionErrorNumbers.Contains(sqlException.Number);
        }
    }
}