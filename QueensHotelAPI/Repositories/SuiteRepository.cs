using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using System.Data;

namespace QueensHotelAPI.Repositories
{
    public class SuiteRepository : ISuiteRepository
    {
        private readonly QueensHotelDbContext _context;
        private readonly ILogger<SuiteRepository> _logger;

        public SuiteRepository(QueensHotelDbContext context, ILogger<SuiteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<InsertSuiteResponseDto> InsertSuiteAsync(InsertSuiteRequestDto dto)
        {
            await _context.Database.OpenConnectionAsync();
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[InsertSuite]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@SuiteName", SqlDbType.NVarChar, 100) { Value = dto.SuiteName });
                command.Parameters.Add(new SqlParameter("@WeeklyRate", SqlDbType.Decimal) { Value = dto.WeeklyRate });
                command.Parameters.Add(new SqlParameter("@MonthlyRate", SqlDbType.Decimal) { Value = dto.MonthlyRate });
                command.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int) { Value = dto.Status });
                command.Parameters.Add(new SqlParameter("@type", SqlDbType.VarChar, 255) { Value = dto.Type });
                command.Parameters.Add(new SqlParameter("@description", SqlDbType.VarChar, 255) { Value = dto.Description });
                command.Parameters.Add(new SqlParameter("@size", SqlDbType.VarChar, 255) { Value = dto.Size });
                command.Parameters.Add(new SqlParameter("@bedrooms", SqlDbType.Int) { Value = dto.Bedrooms });
                command.Parameters.Add(new SqlParameter("@living_area", SqlDbType.VarChar, 255) { Value = dto.LivingArea });
                command.Parameters.Add(new SqlParameter("@kitchen", SqlDbType.VarChar, 255) { Value = dto.Kitchen });
                command.Parameters.Add(new SqlParameter("@dining_area", SqlDbType.VarChar, 255) { Value = dto.DiningArea });
                command.Parameters.Add(new SqlParameter("@bathrooms", SqlDbType.Int) { Value = dto.Bathrooms });
                command.Parameters.Add(new SqlParameter("@laundry_facilities", SqlDbType.VarChar, 255) { Value = dto.LaundryFacilities });
                command.Parameters.Add(new SqlParameter("@workspace", SqlDbType.VarChar, 255) { Value = dto.Workspace });
                command.Parameters.Add(new SqlParameter("@wifi_entertainment", SqlDbType.VarChar, 255) { Value = dto.WifiEntertainment });
                command.Parameters.Add(new SqlParameter("@balcony_terrace", SqlDbType.VarChar, 255) { Value = dto.BalconyTerrace });
                command.Parameters.Add(new SqlParameter("@housekeeping", SqlDbType.VarChar, 255) { Value = dto.Housekeeping });
                command.Parameters.Add(new SqlParameter("@security", SqlDbType.VarChar, 255) { Value = dto.Security });
                command.Parameters.Add(new SqlParameter("@CompanyMaster_Id", SqlDbType.Int) { Value = dto.CompanyMaster_Id });

                await command.ExecuteNonQueryAsync();
                return new InsertSuiteResponseDto
                {
                    Success = true,
                    Message = "Suite inserted successfully."
                };
            }
            catch (Exception ex)
            {
                return new InsertSuiteResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                    await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task<IEnumerable<GetAllSuitesResponseDto>> GetAllSuitesAsync()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing GetRoomDetails_Data with @Accommodation='Suite' at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var results = new List<GetAllSuitesResponseDto>();

                await _context.Database.OpenConnectionAsync();

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[GetRoomDetails_Data]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;

                // Add parameters for Suite retrieval
                command.Parameters.Add(new SqlParameter("@Type", SqlDbType.VarChar, 20) { Value = "all" });
                command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int) { Value = 0 });
                command.Parameters.Add(new SqlParameter("@RoomNumber", SqlDbType.Int) { Value = 0 });
                command.Parameters.Add(new SqlParameter("@FloorId", SqlDbType.Int) { Value = 0 });
                command.Parameters.Add(new SqlParameter("@RoomTypeId", SqlDbType.Int) { Value = 0 });
                command.Parameters.Add(new SqlParameter("@Accommodation", SqlDbType.VarChar, 10) { Value = "Suite" });

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(new GetAllSuitesResponseDto
                    {
                        Id = GetSafeInt32(reader, "SuiteId"),
                        SuiteName = GetSafeString(reader, "SuiteName"),
                        WeeklyRate = GetSafeDecimal(reader, "WeeklyRate"),
                        MonthlyRate = GetSafeDecimal(reader, "MonthlyRate"),
                        Status = GetSafeString(reader, "AvailabilityStatus"),
                        Type = GetSafeString(reader, "SuiteType"),
                        Description = GetSafeString(reader, "Description"),
                        Size = GetSafeString(reader, "SuiteSize"),
                        Bedrooms = GetSafeInt32(reader, "Bedrooms"),
                        Living_area = GetSafeString(reader, "LivingArea"),
                        Kitchen = GetSafeString(reader, "Kitchen"),
                        Dining_area = GetSafeString(reader, "DiningArea"),
                        Bathrooms = GetSafeInt32(reader, "Bathrooms"),
                        Laundry_facilities = GetSafeString(reader, "LaundryFacilities"),
                        Workspace = GetSafeString(reader, "Workspace"),
                        Wifi_entertainment = GetSafeString(reader, "WifiEntertainment"),
                        Balcony_terrace = GetSafeString(reader, "BalconyTerrace"),
                        Housekeeping = GetSafeString(reader, "Housekeeping"),
                        Security = GetSafeString(reader, "Security"),
                        CompanyMaster_Id = GetSafeNullableInt32(reader, "CompanyMaster_Id")
                    });
                }

                _logger.LogInformation("Queens Hotel API: Successfully retrieved {Count} suite records at {Timestamp} by user: {User}",
                    results.Count, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                
                return results;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Queens Hotel API: SQL Server error occurred while executing GetRoomDetails_Data for Suites at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                throw new InvalidOperationException("Database error occurred while retrieving suite data", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Unexpected error occurred while executing GetAllSuites at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                throw;
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
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
            catch
            {
                return 0;
            }
        }
    }
}