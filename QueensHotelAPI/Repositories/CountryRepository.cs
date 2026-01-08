using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.Models;
using System.Data;

namespace QueensHotelAPI.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly QueensHotelDbContext _context;
        private readonly ILogger<CountryRepository> _logger;

        public CountryRepository(QueensHotelDbContext context, ILogger<CountryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CountryOfResidenceResult>> GetCountryOfResidenceDataAsync()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing GetCountryOfResidence_Data stored procedure at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var results = new List<CountryOfResidenceResult>();

                // Use Entity Framework's ExecuteSqlRaw for better retry handling
                try
                {
                    // First try with Entity Framework's built-in connection management and retry logic
                    var parameters = new SqlParameter[0]; // No parameters needed for this stored procedure
                    
                    // Use FromSqlRaw to execute stored procedure and let EF handle connections
                    await using var command = _context.Database.GetDbConnection().CreateCommand();
                    command.CommandText = "[dbo].[GetCountryOfResidence_Data]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 120; // Increase timeout for Azure

                    // Let Entity Framework manage the connection
                    if (_context.Database.GetDbConnection().State != ConnectionState.Open)
                    {
                        await _context.Database.OpenConnectionAsync();
                    }

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        results.Add(new CountryOfResidenceResult
                        {
                            CountryId = reader["CountryId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CountryId"]),
                            CountryName = reader["CountryName"]?.ToString() ?? "",
                            Status = reader["status"] == DBNull.Value ? 0 : Convert.ToInt32(reader["status"]),
                            StatusText = reader["StatusText"]?.ToString() ?? ""
                        });
                    }
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("SqlServerRetryingExecutionStrategy"))
                {
                    // If the retry strategy fails, try once more with direct connection
                    _logger.LogWarning("Queens Hotel API: Retry strategy failed for GetCountryOfResidence_Data, attempting direct execution");
                    
                    results = (List<CountryOfResidenceResult>)await ExecuteGetCountryDataDirectlyAsync();
                }

                _logger.LogInformation("Queens Hotel API: Successfully retrieved {Count} country records at {Timestamp}",
                    results.Count, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return results;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Queens Hotel API: SQL Server error occurred while executing GetCountryOfResidence_Data stored procedure. Error: {ErrorMessage}",
                    sqlEx.Message);
                
                // Check if it's a connection-related error
                if (IsConnectionError(sqlEx))
                {
                    throw new InvalidOperationException("Database connection failed while retrieving country data. Please check your network connection and try again.", sqlEx);
                }
                else
                {
                    throw new InvalidOperationException($"Database error occurred while retrieving country data: {sqlEx.Message}", sqlEx);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Unexpected error occurred while executing GetCountryOfResidence_Data");
                throw new InvalidOperationException($"An unexpected error occurred while retrieving country data: {ex.Message}", ex);
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }

        private async Task<IEnumerable<CountryOfResidenceResult>> ExecuteGetCountryDataDirectlyAsync()
        {
            var results = new List<CountryOfResidenceResult>();
            
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            
            using var command = new SqlCommand("[dbo].[GetCountryOfResidence_Data]", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new CountryOfResidenceResult
                {
                    CountryId = reader["CountryId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CountryId"]),
                    CountryName = reader["CountryName"]?.ToString() ?? "",
                    Status = reader["status"] == DBNull.Value ? 0 : Convert.ToInt32(reader["status"]),
                    StatusText = reader["StatusText"]?.ToString() ?? ""
                });
            }

            return results;
        }

        private static bool IsConnectionError(SqlException sqlException)
        {
            // Common SQL Server connection error numbers
            var connectionErrorNumbers = new[] { 2, 53, 121, 232, 258, 1231, 1232, 11001, 18456, 4060 }; // Added 4060 for login failures
            return connectionErrorNumbers.Contains(sqlException.Number);
        }
    }
}