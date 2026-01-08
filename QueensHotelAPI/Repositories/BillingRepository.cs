using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using System.Data;

namespace QueensHotelAPI.Repositories
{
    /// <summary>
    /// Repository implementation for Billing operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-27 14:50:00
    /// </summary>
    public class BillingRepository : IBillingRepository
    {
        private readonly QueensHotelDbContext _context;
        private readonly ILogger<BillingRepository> _logger;

        public BillingRepository(QueensHotelDbContext context, ILogger<BillingRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<InsertBillingResponseDto> InsertBillingAsync(InsertBillingRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing InsertBilling stored procedure for Reservation ID: {ReservationId} at {Timestamp} by user: {User}",
                    dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreatedBy);

                var createdDateTime = DateTime.Now;

                try
                {
                    // First try with Entity Framework's ExecuteSqlRaw (includes retry logic)
                    return await ExecuteInsertBillingAsync(dto, createdDateTime);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("SqlServerRetryingExecutionStrategy"))
                {
                    // If the retry strategy fails, try once more without it
                    _logger.LogWarning("Queens Hotel API: Retry strategy failed, attempting direct execution for billing insertion for Reservation ID: {ReservationId}", dto.ReservationId);
                    
                    return await ExecuteInsertBillingDirectlyAsync(dto, createdDateTime);
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Queens Hotel API: SQL Server error occurred while inserting billing for Reservation ID: {ReservationId}. Error: {ErrorMessage}",
                    dto.ReservationId, sqlEx.Message);
                
                // Check if it's a connection-related error
                if (IsConnectionError(sqlEx))
                {
                    return new InsertBillingResponseDto
                    {
                        Success = false,
                        Message = $"Database connection failed while inserting billing for Reservation ID {dto.ReservationId}. Please check your network connection and try again.",
                        BillingId = null,
                        CreatedDateTime = DateTime.Now
                    };
                }
                else
                {
                    return new InsertBillingResponseDto
                    {
                        Success = false,
                        Message = $"Database error occurred while inserting billing: {sqlEx.Message}",
                        BillingId = null,
                        CreatedDateTime = DateTime.Now
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Unexpected error occurred while inserting billing for Reservation ID: {ReservationId}",
                    dto.ReservationId);
                
                return new InsertBillingResponseDto
                {
                    Success = false,
                    Message = $"An unexpected error occurred while inserting billing: {ex.Message}",
                    BillingId = null,
                    CreatedDateTime = DateTime.Now
                };
            }
        }

        private async Task<InsertBillingResponseDto> ExecuteInsertBillingAsync(InsertBillingRequestDto dto, DateTime createdDateTime)
        {
            await _context.Database.OpenConnectionAsync();
            
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[InsertBilling]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 120;

                // Add parameters matching the stored procedure
                command.Parameters.Add(new SqlParameter("@BillingDate", SqlDbType.Date) { Value = dto.BillingDate });
                command.Parameters.Add(new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Value = dto.TotalAmount, Precision = 18, Scale = 2 });
                command.Parameters.Add(new SqlParameter("@IsNoShowCharge", SqlDbType.Bit) { Value = dto.IsNoShowCharge });
                command.Parameters.Add(new SqlParameter("@PaymentStatus", SqlDbType.VarChar, 50) { Value = dto.PaymentStatus });
                command.Parameters.Add(new SqlParameter("@PaymentMethod", SqlDbType.Int) { Value = dto.PaymentMethod });
                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.VarChar, 100) { Value = dto.CreatedBy });
                command.Parameters.Add(new SqlParameter("@CreatedDatetime", SqlDbType.DateTime) { Value = createdDateTime });
                command.Parameters.Add(new SqlParameter("@Reservation_ID", SqlDbType.Int) { Value = dto.ReservationId });

                await command.ExecuteNonQueryAsync();

                // Get the inserted billing ID by querying the latest record for this reservation
                var billingId = await GetLatestBillingIdAsync(dto.ReservationId, createdDateTime);

                _logger.LogInformation("Queens Hotel API: Successfully inserted billing for Reservation ID: {ReservationId}, Billing ID: {BillingId} at {Timestamp} by user: {User}",
                    dto.ReservationId, billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreatedBy);

                return new InsertBillingResponseDto
                {
                    Success = true,
                    Message = "Billing inserted successfully",
                    BillingId = billingId,
                    CreatedDateTime = createdDateTime
                };
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }

        private async Task<InsertBillingResponseDto> ExecuteInsertBillingDirectlyAsync(InsertBillingRequestDto dto, DateTime createdDateTime)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            
            using var command = new SqlCommand("[dbo].[InsertBilling]", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };
            
            // Add parameters matching the stored procedure
            command.Parameters.Add(new SqlParameter("@BillingDate", SqlDbType.Date) { Value = dto.BillingDate });
            command.Parameters.Add(new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Value = dto.TotalAmount, Precision = 18, Scale = 2 });
            command.Parameters.Add(new SqlParameter("@IsNoShowCharge", SqlDbType.Bit) { Value = dto.IsNoShowCharge });
            command.Parameters.Add(new SqlParameter("@PaymentStatus", SqlDbType.VarChar, 50) { Value = dto.PaymentStatus });
            command.Parameters.Add(new SqlParameter("@PaymentMethod", SqlDbType.Int) { Value = dto.PaymentMethod });
            command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.VarChar, 100) { Value = dto.CreatedBy });
            command.Parameters.Add(new SqlParameter("@CreatedDatetime", SqlDbType.DateTime) { Value = createdDateTime });
            command.Parameters.Add(new SqlParameter("@Reservation_ID", SqlDbType.Int) { Value = dto.ReservationId });
            
            await command.ExecuteNonQueryAsync();

            // Get the inserted billing ID
            var billingId = await GetLatestBillingIdDirectAsync(dto.ReservationId, createdDateTime, connection);

            _logger.LogInformation("Queens Hotel API: Successfully inserted billing (direct) for Reservation ID: {ReservationId}, Billing ID: {BillingId} at {Timestamp} by user: {User}",
                dto.ReservationId, billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreatedBy);

            return new InsertBillingResponseDto
            {
                Success = true,
                Message = "Billing inserted successfully",
                BillingId = billingId,
                CreatedDateTime = createdDateTime
            };
        }

        private async Task<int?> GetLatestBillingIdAsync(int reservationId, DateTime createdDateTime)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = @"
                    SELECT TOP 1 Id 
                    FROM [dbo].[Billing] 
                    WHERE Reservation_ID = @ReservationId 
                    AND CreatedDatetime = @CreatedDatetime 
                    ORDER BY Id DESC";
                
                command.Parameters.Add(new SqlParameter("@ReservationId", SqlDbType.Int) { Value = reservationId });
                command.Parameters.Add(new SqlParameter("@CreatedDatetime", SqlDbType.DateTime) { Value = createdDateTime });

                var result = await command.ExecuteScalarAsync();
                return result as int?;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Queens Hotel API: Failed to retrieve billing ID for Reservation ID: {ReservationId}", reservationId);
                return null;
            }
        }

        private async Task<int?> GetLatestBillingIdDirectAsync(int reservationId, DateTime createdDateTime, SqlConnection connection)
        {
            try
            {
                using var command = new SqlCommand(@"
                    SELECT TOP 1 Id 
                    FROM [dbo].[Billing] 
                    WHERE Reservation_ID = @ReservationId 
                    AND CreatedDatetime = @CreatedDatetime 
                    ORDER BY Id DESC", connection);
                
                command.Parameters.Add(new SqlParameter("@ReservationId", SqlDbType.Int) { Value = reservationId });
                command.Parameters.Add(new SqlParameter("@CreatedDatetime", SqlDbType.DateTime) { Value = createdDateTime });

                var result = await command.ExecuteScalarAsync();
                return result as int?;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Queens Hotel API: Failed to retrieve billing ID (direct) for Reservation ID: {ReservationId}", reservationId);
                return null;
            }
        }

        private static bool IsConnectionError(SqlException sqlException)
        {
            // Common SQL Server connection error numbers
            var connectionErrorNumbers = new[] { 2, 53, 121, 232, 258, 1231, 1232, 11001, 18456, 4060 };
            return connectionErrorNumbers.Contains(sqlException.Number);
        }

        public async Task<GetBillingInfoResponseDto?> GetBillingInfoAsync(int billingId)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing GetBillingInfo stored procedure for Billing ID: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                try
                {
                    // First try with Entity Framework's ExecuteSqlRaw (includes retry logic)
                    return await ExecuteGetBillingInfoAsync(billingId);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("SqlServerRetryingExecutionStrategy"))
                {
                    // If the retry strategy fails, try once more without it
                    _logger.LogWarning("Queens Hotel API: Retry strategy failed, attempting direct execution for GetBillingInfo for Billing ID: {BillingId}", billingId);
                    
                    return await ExecuteGetBillingInfoDirectlyAsync(billingId);
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Queens Hotel API: SQL Server error occurred while retrieving billing info for Billing ID: {BillingId}. Error: {ErrorMessage}",
                    billingId, sqlEx.Message);
                
                // Check if it's a connection-related error
                if (IsConnectionError(sqlEx))
                {
                    throw new InvalidOperationException($"Database connection failed while retrieving billing info for Billing ID {billingId}. Please check your network connection and try again.", sqlEx);
                }
                else
                {
                    throw new InvalidOperationException($"Database error occurred while retrieving billing info: {sqlEx.Message}", sqlEx);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Unexpected error occurred while retrieving billing info for Billing ID: {BillingId}",
                    billingId);
                throw new InvalidOperationException($"An unexpected error occurred while retrieving billing info for Billing ID {billingId}: {ex.Message}", ex);
            }
        }

        private async Task<GetBillingInfoResponseDto?> ExecuteGetBillingInfoAsync(int billingId)
        {
            await _context.Database.OpenConnectionAsync();
            
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[GetBillingInfo]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 120;

                command.Parameters.Add(new SqlParameter("@Billing_Id", SqlDbType.Int) { Value = billingId });

                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    var result = MapReaderToBillingInfo(reader);
                    
                    _logger.LogInformation("Queens Hotel API: Successfully retrieved billing info for Billing ID: {BillingId} at {Timestamp}",
                        billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    return result;
                }

                _logger.LogInformation("Queens Hotel API: No billing record found for Billing ID: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                
                return null;
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }

        private async Task<GetBillingInfoResponseDto?> ExecuteGetBillingInfoDirectlyAsync(int billingId)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            
            using var command = new SqlCommand("[dbo].[GetBillingInfo]", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };
            
            command.Parameters.Add(new SqlParameter("@Billing_Id", SqlDbType.Int) { Value = billingId });
            
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var result = MapReaderToBillingInfo(reader);
                
                _logger.LogInformation("Queens Hotel API: Successfully retrieved billing info (direct) for Billing ID: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                
                return result;
            }

            _logger.LogInformation("Queens Hotel API: No billing record found (direct) for Billing ID: {BillingId} at {Timestamp}",
                billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            
            return null;
        }

        private static GetBillingInfoResponseDto MapReaderToBillingInfo(IDataReader reader)
        {
            return new GetBillingInfoResponseDto
            {
                // Billing Information
                BillingId = GetSafeInt32(reader, "BillingId"),
                BillingDate = GetSafeDateTime(reader, "BillingDate"),
                TotalAmount = GetSafeTotalAmount(reader), // Use specific method for TotalAmount
                IsNoShowCharge = GetSafeBoolean(reader, "IsNoShowCharge"),
                PaymentStatus = GetSafeString(reader, "PaymentStatus"),
                PaymentMethod = GetSafeString(reader, "PaymentMethod"),
                PaymentMethodType = GetSafeString(reader, "Type"),
                CreatedBy = GetSafeString(reader, "CreatedBy"),
                CreatedDateTime = GetSafeDateTime(reader, "CreatedDatetime"),
                ReservationId = GetSafeInt32(reader, "Reservation_ID"),
                BillingCompanyMasterId = GetSafeInt32(reader, "BillingCompanyMaster_Id"),

                // Reservation Information
                ReservationInfo = new BillingReservationInfoDto
                {
                    CheckInDate = GetSafeNullableDateTime(reader, "CheckInDate"),
                    CheckOutDate = GetSafeNullableDateTime(reader, "CheckOutDate"),
                    NumberOfGuests = GetSafeNullableInt32(reader, "NumberOfGuests"),
                    ReservationStatus = GetSafeString(reader, "ReservationStatus"),
                    CreateBy = GetSafeString(reader, "CreateBy"),
                    CreatedDateTime = GetSafeNullableDateTime(reader, "CreatedDateTime"),
                    CustomerId = GetSafeInt32(reader, "Customer_Id"),
                    MealPlanId = GetSafeNullableInt32(reader, "MealPlan_id"),
                    SuiteId = GetSafeNullableInt32(reader, "Suite_id"),
                    SuiteName = GetSafeString(reader, "SuiteName"),
                    RoomId = GetSafeNullableInt32(reader, "Room_ID"),
                    RoomType = GetSafeString(reader, "RoomType"),
                    RoomRatePerNight = GetSafeNullableDecimal(reader, "RatePerNight"),
                    TravelAgencyId = GetSafeNullableInt32(reader, "TravalAgency_Id"),
                    ReservationCompanyMasterId = GetSafeInt32(reader, "ReservationCompanyMaster_Id"),
                    PaymentMethodId = GetSafeNullableInt32(reader, "PaymentMethodI_Id"),
                    PaymentCardDetailsId = GetSafeNullableInt32(reader, "PaymentCardDetails_Id")
                },

                // Customer Information
                CustomerInfo = new BillingCustomerInfoDto
                {
                    FirstName = GetSafeString(reader, "FirstName"),
                    LastName = GetSafeString(reader, "LName"),
                    EmailAddress = GetSafeString(reader, "EmailAddress"),
                    Phone = GetSafeString(reader, "Phone"),
                    Address = GetSafeString(reader, "Address"),
                    CustomerStatus = GetSafeString(reader, "CustomerStatus"),
                    CustomerCompanyMasterId = GetSafeNullableInt32(reader, "CustomerCompanyMaster_Id"),
                    PassportId = GetSafeString(reader, "PassportId"),
                    NIC = GetSafeString(reader, "NIC"),
                    CountryOfResidenceId = GetSafeNullableInt32(reader, "CountryOfResidence_id"),
                    DateOfBirth = GetSafeNullableDateTime(reader, "DOB"),
                    Gender = GetSafeString(reader, "Gender")
                }
            };
        }

        // Specific method to handle TotalAmount with multiple fallback attempts
        private static decimal GetSafeTotalAmount(IDataReader reader)
        {
            // Try different possible column names for TotalAmount
            string[] possibleColumnNames = { "TotalAmount", "B.TotalAmount", "totalamount", "Total_Amount" };
            
            foreach (var columnName in possibleColumnNames)
            {
                try
                {
                    var ordinal = reader.GetOrdinal(columnName);
                    var value = reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
                    System.Diagnostics.Debug.WriteLine($"Successfully retrieved TotalAmount using column name '{columnName}': {value}");
                    return value;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to get TotalAmount using column name '{columnName}': {ex.Message}");
                    continue;
                }
            }

            // Try using indexer as last resort
            try
            {
                var value = reader["TotalAmount"];
                if (value != null && value != DBNull.Value)
                {
                    var result = Convert.ToDecimal(value);
                    System.Diagnostics.Debug.WriteLine($"Successfully retrieved TotalAmount using indexer: {result}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to get TotalAmount using indexer: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("All attempts to retrieve TotalAmount failed, returning 0");
            return 0;
        }

        public async Task<List<GetBillingInfoResponseDto>> GetBillingInfoListAsync(int billingId)
        {
            var results = new List<GetBillingInfoResponseDto>();
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing GetBillingInfo stored procedure for Billing ID: {BillingId} (list mode) at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                await _context.Database.OpenConnectionAsync();
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[GetBillingInfo]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 120;
                command.Parameters.Add(new SqlParameter("@Billing_Id", SqlDbType.Int) { Value = billingId });

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(MapReaderToBillingInfo(reader));
                }
                _logger.LogInformation("Queens Hotel API: Retrieved {Count} billing records for Billing ID: {BillingId} at {Timestamp}",
                    results.Count, billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred while retrieving billing info list for Billing ID: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                throw;
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
            return results;
        }

        // Helper methods for safe data extraction
        private static string GetSafeString(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static int GetSafeInt32(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private static int? GetSafeNullableInt32(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
            }
            catch
            {
                return null;
            }
        }

        private static decimal GetSafeDecimal(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
            }
            catch (Exception ex)
            {
                // Add logging to identify which column is failing
                System.Diagnostics.Debug.WriteLine($"GetSafeDecimal failed for column '{columnName}': {ex.Message}");
                // Try alternative approach using indexer
                try
                {
                    var value = reader[columnName];
                    if (value == null || value == DBNull.Value)
                        return 0;
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return 0;
                }
            }
        }

        private static decimal? GetSafeNullableDecimal(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);
            }
            catch
            {
                return null;
            }
        }

        private static DateTime GetSafeDateTime(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private static DateTime? GetSafeNullableDateTime(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
            }
            catch
            {
                return null;
            }
        }

        private static bool GetSafeBoolean(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? false : reader.GetBoolean(ordinal);
            }
            catch
            {
                return false;
            }
        }
    }
}