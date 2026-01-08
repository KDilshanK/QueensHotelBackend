using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using QueensHotelAPI.Data;
using QueensHotelAPI.Models;
using System.Data;
using System.Data.Common;
using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly QueensHotelDbContext _context;
        private readonly ILogger<ReservationRepository> _logger;

        public ReservationRepository(QueensHotelDbContext context, ILogger<ReservationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ReservationDetailsResult>> GetReservationDataAsync(
            string? nic = null,
            string? fname = null,
            string? lname = null,
            string? email = null,
            string? number = null,
            int? id = 0)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing GetReservationData stored procedure at {Timestamp} by user: {User}",
                    "2025-05-29 06:53:37", "dilshan-jolanka");

                var results = new List<ReservationDetailsResult>();

                await _context.Database.OpenConnectionAsync();

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[GetReservationData]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;

                // Create parameters using the command's CreateParameter method
                var nicParam = command.CreateParameter();
                nicParam.ParameterName = "@NIC";
                nicParam.Value = (object?)nic ?? "ALL";
                nicParam.DbType = DbType.String;
                command.Parameters.Add(nicParam);

                var fnameParam = command.CreateParameter();
                fnameParam.ParameterName = "@Fname";
                fnameParam.Value = (object?)fname ?? "ALL";
                fnameParam.DbType = DbType.String;
                command.Parameters.Add(fnameParam);

                var lnameParam = command.CreateParameter();
                lnameParam.ParameterName = "@Lname";
                lnameParam.Value = (object?)lname ?? "ALL";
                lnameParam.DbType = DbType.String;
                command.Parameters.Add(lnameParam);

                var emailParam = command.CreateParameter();
                emailParam.ParameterName = "@Email";
                emailParam.Value = (object?)email ?? "ALL";
                emailParam.DbType = DbType.String;
                command.Parameters.Add(emailParam);

                var numberParam = command.CreateParameter();
                numberParam.ParameterName = "@Number";
                numberParam.Value = (object?)number ?? "ALL";
                numberParam.DbType = DbType.String;
                command.Parameters.Add(numberParam);

                var idParam = command.CreateParameter();
                idParam.ParameterName = "@Id";
                idParam.Value = (object?)id ?? 0;
                idParam.DbType = DbType.Int32;
                command.Parameters.Add(idParam);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(MapReaderToResult(reader));
                }

                _logger.LogInformation("Queens Hotel API: Successfully retrieved {Count} reservation records at {Timestamp} by user: {User}",
                    results.Count, "2025-05-29 06:53:37", "dilshan-jolanka");
                return results;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Queens Hotel API: SQL Server error occurred while executing GetReservationData stored procedure at {Timestamp}",
                    "2025-05-29 06:53:37");
                throw new InvalidOperationException("Database error occurred while retrieving Queens Hotel reservation data", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Unexpected error occurred while executing GetReservationData at {Timestamp} by user: {User}",
                    "2025-05-29 06:53:37", "dilshan-jolanka");
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

        private static ReservationDetailsResult MapReaderToResult(IDataReader reader)
        {
            return new ReservationDetailsResult
            {
                ReservationID = reader.GetInt32(reader.GetOrdinal("ReservationID")),
                ReservationCheckInDate = reader.GetDateTime(reader.GetOrdinal("ReservationCheckInDate")),
                ReservationCheckOutDate = reader.GetDateTime(reader.GetOrdinal("ReservationCheckOutDate")),
                GuestCount = reader.GetInt32(reader.GetOrdinal("GuestCount")),
                ReservationStatus = reader.IsDBNull(reader.GetOrdinal("ReservationStatus"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("ReservationStatus")),
                ReservationCreatedDate = reader.GetDateTime(reader.GetOrdinal("ReservationCreatedDate")),

                // Payment information (new fields from updated SP)
                PaymentMethod = GetSafeNullableInt32(reader, "PaymentMethod"),
                PaymentCardDetails = GetSafeNullableInt32(reader, "PaymentCardDetails"),
                GuestStatus = GetSafeString(reader, "GuestStatus"),

                // Payment method details (new fields from updated SP) - handling duplicate 'id' columns
                PaymentMethodId = GetSafeNullableInt32WithAlias(reader, "id", 0), // First 'id' column (PM.id)
                PaymentMethodType = GetSafeString(reader, "Type"),

                // Payment card details (new fields from updated SP) - handling duplicate 'id' columns
                PaymentCardId = GetSafeNullableInt32WithAlias(reader, "id", 1), // Second 'id' column (PC.id)
                CardHolderName = GetSafeString(reader, "CardHolderName"),
                ExpiryYear = GetSafeNullableInt32(reader, "ExpiryYear"),
                ExpiryMonth = GetSafeNullableInt32(reader, "ExpiryMonth"),

                CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                CustomerNIC = reader.IsDBNull(reader.GetOrdinal("CustomerNIC"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("CustomerNIC")),
                CustomerFirstName = reader.IsDBNull(reader.GetOrdinal("CustomerFirstName"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("CustomerFirstName")),
                CustomerLastName = reader.IsDBNull(reader.GetOrdinal("CustomerLastName"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("CustomerLastName")),
                CustomerEmail = reader.IsDBNull(reader.GetOrdinal("CustomerEmail"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("CustomerEmail")),
                CustomerPhone = reader.IsDBNull(reader.GetOrdinal("CustomerPhone"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("CustomerPhone")),
                CustomerAddress = reader.IsDBNull(reader.GetOrdinal("CustomerAddress"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("CustomerAddress")),
                CustomerPassportID = reader.IsDBNull(reader.GetOrdinal("CustomerPassportID"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("CustomerPassportID")),
                CustomerDateOfBirth = reader.IsDBNull(reader.GetOrdinal("CustomerDateOfBirth"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("CustomerDateOfBirth")).ToString(),
                CustomerGender = reader.IsDBNull(reader.GetOrdinal("CustomerGender"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("CustomerGender")),

                MealPlanID = reader.IsDBNull(reader.GetOrdinal("MealPlanID"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("MealPlanID")),
                CostPerNight = reader.IsDBNull(reader.GetOrdinal("CostPerNight"))
                    ? (decimal?)null
                    : Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("CostPerNight"))),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("Description")).ToString(),
                MealPlanInternalId = GetSafeNullableInt32WithAlias(reader, "id", 2), // Third 'id' column (MP.id)
                IsFree = reader.IsDBNull(reader.GetOrdinal("IsFree"))
                    ? (bool?)null
                    : reader.GetBoolean(reader.GetOrdinal("IsFree")),
                MealPlanCode = reader.IsDBNull(reader.GetOrdinal("MealPlanCode"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("MealPlanCode")).ToString(),
                MealPlanStatus = reader.IsDBNull(reader.GetOrdinal("status"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("status")).ToString(),

                SuiteID = reader.IsDBNull(reader.GetOrdinal("SuiteID"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("SuiteID")),
                SuiteName = reader.IsDBNull(reader.GetOrdinal("SuiteName"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("SuiteName")).ToString(),
                SuiteType = reader.IsDBNull(reader.GetOrdinal("SuiteType"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("SuiteType")).ToString(),
                SuiteSize = reader.IsDBNull(reader.GetOrdinal("SuiteSize"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("SuiteSize")).ToString(),
                SuiteBedrooms = reader.IsDBNull(reader.GetOrdinal("SuiteBedrooms"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("SuiteBedrooms")),
                SuiteBathrooms = reader.IsDBNull(reader.GetOrdinal("SuiteBathrooms"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("SuiteBathrooms")),
                SuiteDescription = reader.IsDBNull(reader.GetOrdinal("SuiteDescription"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("SuiteDescription")).ToString(),
                SuiteWeeklyRate = reader.IsDBNull(reader.GetOrdinal("SuiteWeeklyRate"))
                    ? (decimal?)null
                    : Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("SuiteWeeklyRate"))),
                SuiteMonthlyRate = reader.IsDBNull(reader.GetOrdinal("SuiteMonthlyRate"))
                    ? (decimal?)null
                    : Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("SuiteMonthlyRate"))),

                RoomID = reader.IsDBNull(reader.GetOrdinal("RoomID"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("RoomID")),
                RoomNumber = reader.IsDBNull(reader.GetOrdinal("RoomNumber"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("RoomNumber")).ToString(),
                RoomTypeName = reader.IsDBNull(reader.GetOrdinal("RoomTypeName"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("RoomTypeName")).ToString(),
                RoomRatePerNight = reader.IsDBNull(reader.GetOrdinal("RoomRatePerNight"))
                    ? (decimal?)null
                    : Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("RoomRatePerNight"))),
                HasAirConditioning = reader.IsDBNull(reader.GetOrdinal("HasAirConditioning"))
                    ? (bool?)null
                    : reader.GetBoolean(reader.GetOrdinal("HasAirConditioning")),
                RoomCapacity = reader.IsDBNull(reader.GetOrdinal("RoomCapacity"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("RoomCapacity")),
                RoomStatusID = reader.IsDBNull(reader.GetOrdinal("RoomStatusID"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("RoomStatusID")),
                RoomStatusName = reader.IsDBNull(reader.GetOrdinal("RoomStatusName"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("RoomStatusName")).ToString(),

                FloorID = reader.IsDBNull(reader.GetOrdinal("FloorID"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("FloorID")),
                FloorNumber = reader.IsDBNull(reader.GetOrdinal("FloorNumber"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("FloorNumber")),
                FloorDescription = reader.IsDBNull(reader.GetOrdinal("FloorDescription"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("FloorDescription")).ToString(),

                TravelAgencyID = reader.IsDBNull(reader.GetOrdinal("TravelAgencyID"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("TravelAgencyID")),
                TravelAgencyName = reader.IsDBNull(reader.GetOrdinal("TravelAgencyName"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("TravelAgencyName")).ToString(),
                Address = reader.IsDBNull(reader.GetOrdinal("Address"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("Address")).ToString(),
                ContactPerson = reader.IsDBNull(reader.GetOrdinal("ContactPerson"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("ContactPerson")).ToString(),
                DiscountRate = reader.IsDBNull(reader.GetOrdinal("DiscountRate"))
                    ? (decimal?)null
                    : Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("DiscountRate"))),
                Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("Email")).ToString(),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("Phone")).ToString(),
                WebpageUrl = reader.IsDBNull(reader.GetOrdinal("WebpageUrl"))
                    ? string.Empty
                    : reader.GetValue(reader.GetOrdinal("WebpageUrl")).ToString(),
            };
        }

        // Helper method to handle duplicate column names by ordinal position
        private static int? GetSafeNullableInt32WithAlias(IDataReader reader, string columnName, int occurrence)
        {
            try
            {
                var columnCount = reader.FieldCount;
                var occurrenceFound = 0;
                
                for (int i = 0; i < columnCount; i++)
                {
                    if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (occurrenceFound == occurrence)
                        {
                            return reader.IsDBNull(i) ? null : reader.GetInt32(i);
                        }
                        occurrenceFound++;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        // Additional helper methods for safe data extraction
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

        private static bool? GetSafeNullableBoolean(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetBoolean(ordinal);
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> InsertReservationAsync(CreateReservationDto dto)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing InsertReservation stored procedure at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreateBy);

                await _context.Database.OpenConnectionAsync();

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[InsertReservation]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 120;

                command.Parameters.Add(new SqlParameter("@CheckInDate", SqlDbType.Date) { Value = dto.CheckInDate });
                command.Parameters.Add(new SqlParameter("@CheckOutDate", SqlDbType.Date) { Value = dto.CheckOutDate });
                command.Parameters.Add(new SqlParameter("@NumberOfGuests", SqlDbType.Int) { Value = dto.NumberOfGuests });
                command.Parameters.Add(new SqlParameter("@PaymentMethodI_Id", SqlDbType.Int) { Value = dto.PaymentMethodI_Id });
                command.Parameters.Add(new SqlParameter("@CreateBy", SqlDbType.VarChar, 45) { Value = dto.CreateBy });
                command.Parameters.Add(new SqlParameter("@Customer_Id", SqlDbType.Int) { Value = dto.Customer_Id });
                
                // NEW: Add Status parameter to match updated stored procedure
                command.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 10) { Value = dto.Status });
                
                // Use ConvertZeroToNull for optional foreign key fields
                command.Parameters.Add(new SqlParameter("@MealPlan_id", SqlDbType.Int) 
                { 
                    Value = ConvertZeroToNull(dto.MealPlan_id)
                });
                command.Parameters.Add(new SqlParameter("@Suite_id", SqlDbType.Int) 
                { 
                    Value = ConvertZeroToNull(dto.Suite_id)
                });
                command.Parameters.Add(new SqlParameter("@Room_ID", SqlDbType.Int) 
                { 
                    Value = ConvertZeroToNull(dto.Room_ID)
                });
                command.Parameters.Add(new SqlParameter("@TravalAgency_Id", SqlDbType.Int) 
                { 
                    Value = ConvertZeroToNull(dto.TravalAgency_Id)
                });

                _logger.LogInformation("Queens Hotel API: InsertReservation parameters - CheckInDate: {CheckInDate}, CheckOutDate: {CheckOutDate}, NumberOfGuests: {NumberOfGuests}, PaymentMethodI_Id: {PaymentMethodI_Id}, Customer_Id: {Customer_Id}, Status: {Status}, MealPlan_id: {MealPlan_id}, Suite_id: {Suite_id}, Room_ID: {Room_ID}, TravalAgency_Id: {TravalAgency_Id}, CreateBy: {CreateBy}",
                    dto.CheckInDate.ToString("yyyy-MM-dd"),
                    dto.CheckOutDate.ToString("yyyy-MM-dd"),
                    dto.NumberOfGuests,
                    dto.PaymentMethodI_Id,
                    dto.Customer_Id,
                    dto.Status,
                    dto.MealPlan_id == 0 ? "NULL" : dto.MealPlan_id?.ToString() ?? "NULL",
                    dto.Suite_id == 0 ? "NULL" : dto.Suite_id?.ToString() ?? "NULL",
                    dto.Room_ID == 0 ? "NULL" : dto.Room_ID?.ToString() ?? "NULL",
                    dto.TravalAgency_Id == 0 ? "NULL" : dto.TravalAgency_Id?.ToString() ?? "NULL",
                    dto.CreateBy);

                // Execute and capture the returned reservation ID
                using var reader = await command.ExecuteReaderAsync();
                int newReservationId = 0;
                
                if (await reader.ReadAsync())
                {
                    newReservationId = reader.GetInt32("ReservationId");
                }
                
                _logger.LogInformation("Queens Hotel API: Successfully inserted reservation with ID {ReservationId} and Status {Status} at {Timestamp} by user: {User}",
                    newReservationId, dto.Status, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreateBy);
                
                return newReservationId;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Queens Hotel API: SQL Server error occurred while inserting reservation. Error Number: {ErrorNumber}, Error Message: {ErrorMessage}",
                    sqlEx.Number, sqlEx.Message);
                throw new InvalidOperationException($"Database error occurred while creating reservation: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Unexpected error occurred while inserting reservation");
                throw;
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                    await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task<bool> UpdateReservationAsync(UpdateReservationDto dto)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing UpdateReservation stored procedure for ReservationID: {ReservationId} at {Timestamp} by user: {UserId}",
                    dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.UserId);

                await _context.Database.OpenConnectionAsync();

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[UpdateReservation]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 120; // Increased timeout for better reliability

                // Required parameters (cannot be null)
                command.Parameters.Add(new SqlParameter("@ReservationId", SqlDbType.Int) { Value = dto.ReservationId });
                command.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = dto.CustomerId });
                command.Parameters.Add(new SqlParameter("@CheckInDate", SqlDbType.Date) { Value = dto.CheckInDate });
                command.Parameters.Add(new SqlParameter("@CheckOutDate", SqlDbType.Date) { Value = dto.CheckOutDate });
                command.Parameters.Add(new SqlParameter("@NumberOfGuests", SqlDbType.Int) { Value = dto.NumberOfGuests });
                command.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 45) { Value = dto.Status });
                command.Parameters.Add(new SqlParameter("@PaymentMethodI_Id", SqlDbType.Int) { Value = dto.PaymentMethodI_Id });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar, 45) { Value = dto.UserId });

                // Optional parameters - Convert 0 to NULL for stored procedure
                command.Parameters.Add(new SqlParameter("@MealPlan_id", SqlDbType.Int) 
                { 
                    Value = ConvertZeroToNull(dto.MealPlan_id)
                });
                command.Parameters.Add(new SqlParameter("@Suite_id", SqlDbType.Int) 
                { 
                    Value = ConvertZeroToNull(dto.Suite_id)
                });
                command.Parameters.Add(new SqlParameter("@Room_ID", SqlDbType.Int) 
                { 
                    Value = ConvertZeroToNull(dto.Room_ID)
                });
                command.Parameters.Add(new SqlParameter("@TravalAgency_Id", SqlDbType.Int) 
                { 
                    Value = ConvertZeroToNull(dto.TravalAgency_Id)
                });
                //command.Parameters.Add(new SqlParameter("@PaymentCardDetails_Id", SqlDbType.Int) 
                //{ 
                //    Value = ConvertZeroToNull(dto.PaymentCardDetails_Id)
                //});

                // Log the parameters being passed (excluding sensitive data)
                _logger.LogInformation("Queens Hotel API: UpdateReservation parameters - ReservationId: {ReservationId}, CustomerId: {CustomerId}, CheckInDate: {CheckInDate}, CheckOutDate: {CheckOutDate}, NumberOfGuests: {NumberOfGuests}, Status: {Status}, PaymentMethodI_Id: {PaymentMethodI_Id}, MealPlan_id: {MealPlanId}, Suite_id: {SuiteId}, Room_ID: {RoomId}, TravalAgency_Id: {TravelAgencyId}, PaymentCardDetails_Id: {PaymentCardDetailsId}, UserId: {UserId}",
                    dto.ReservationId, 
                    dto.CustomerId,
                    dto.CheckInDate.ToString("yyyy-MM-dd"),
                    dto.CheckOutDate.ToString("yyyy-MM-dd"),
                    dto.NumberOfGuests,
                    dto.Status,
                    dto.PaymentMethodI_Id,
                    dto.MealPlan_id == 0 ? "NULL" : dto.MealPlan_id?.ToString() ?? "NULL",
                    dto.Suite_id == 0 ? "NULL" : dto.Suite_id?.ToString() ?? "NULL",
                    dto.Room_ID == 0 ? "NULL" : dto.Room_ID?.ToString() ?? "NULL",
                    dto.TravalAgency_Id == 0 ? "NULL" : dto.TravalAgency_Id?.ToString() ?? "NULL",
                    dto.UserId);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                
                // FIXED: The success condition should be rowsAffected > 0, not >= 0
                // If rowsAffected is 0, it means no rows were updated (likely due to WHERE clause not matching)
                var success = rowsAffected > 0;
                
                _logger.LogInformation("Queens Hotel API: UpdateReservation stored procedure completed - RowsAffected: {RowsAffected}, Success: {Success} for ReservationID: {ReservationId} at {Timestamp}",
                    rowsAffected, success, dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                if (!success)
                {
                    _logger.LogWarning("Queens Hotel API: UpdateReservation failed - No rows were affected. This could mean the reservation with ID {ReservationId} and CustomerID {CustomerId} was not found, or another process has modified it.",
                        dto.ReservationId, dto.CustomerId);
                    
                    // Check if the reservation exists at all
                    var exists = await _context.Reservation.AnyAsync(r => r.Id == dto.ReservationId);
                    if (!exists)
                    {
                        _logger.LogWarning("Queens Hotel API: Reservation with ID {ReservationId} does not exist", dto.ReservationId);
                        throw new InvalidOperationException($"Reservation with ID {dto.ReservationId} was not found");
                    }
                    
                    // Check if the reservation belongs to the specified customer
                    var belongsToCustomer = await _context.Reservation.AnyAsync(r => r.Id == dto.ReservationId && r.Customer_Id == dto.CustomerId);
                    if (!belongsToCustomer)
                    {
                        _logger.LogWarning("Queens Hotel API: Reservation {ReservationId} does not belong to customer {CustomerId}", dto.ReservationId, dto.CustomerId);
                        throw new InvalidOperationException($"Reservation {dto.ReservationId} does not belong to customer {dto.CustomerId}");
                    }
                }

                return success;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Queens Hotel API: SQL Server error occurred while updating reservation {ReservationId}. Error Number: {ErrorNumber}, Error Message: {ErrorMessage}",
                    dto.ReservationId, sqlEx.Number, sqlEx.Message);
                throw new InvalidOperationException($"Database error occurred while updating reservation {dto.ReservationId}: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Unexpected error occurred while updating reservation {ReservationId}",
                    dto.ReservationId);
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

        /// <summary>
        /// Helper method to convert 0 values to NULL for optional foreign key fields
        /// This ensures that when frontend sends 0 (indicating no selection), it gets converted to NULL for the stored procedure
        /// </summary>
        /// <param name="value">The nullable integer value from DTO</param>
        /// <returns>DBNull.Value if the value is null or 0, otherwise the actual value</returns>
        private static object ConvertZeroToNull(int? value)
        {
            // Convert null or 0 to DBNull.Value, otherwise return the actual value
            return (!value.HasValue || value.Value == 0) ? DBNull.Value : value.Value;
        }

        public async Task<bool> ReservationExistsAsync(int reservationId)
        {
            try
            {
                // Use Entity Framework's LINQ instead of raw SQL for better retry handling
                var exists = await _context.Reservation.AnyAsync(r => r.Id == reservationId);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error checking if reservation {ReservationId} exists", reservationId);
                throw;
            }
        }

        public async Task<bool> CancelReservationAsync(int reservationId, string cancelledBy)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing CancelReservation stored procedure for ReservationID: {ReservationId} at {Timestamp} by user: {User}",
                    reservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), cancelledBy);

                // Use a more direct approach with minimal retry interference
                var parameters = new[]
                {
                    new SqlParameter("@ReservationID", SqlDbType.Int) { Value = reservationId },
                    new SqlParameter("@CancelledBy", SqlDbType.VarChar, 45) { Value = cancelledBy }
                };

                try
                {
                    // First try with Entity Framework's ExecuteSqlRaw (includes retry logic)
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC [dbo].[CancelReservation] @ReservationID, @CancelledBy", 
                        parameters);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("SqlServerRetryingExecutionStrategy"))
                {
                    // If the retry strategy fails, try once more without it
                    _logger.LogWarning("Queens Hotel API: Retry strategy failed, attempting direct execution for reservation {ReservationId}", reservationId);
                    
                    await ExecuteCancelReservationDirectlyAsync(reservationId, cancelledBy);
                }

                _logger.LogInformation("Queens Hotel API: Successfully cancelled reservation {ReservationId} at {Timestamp} by user: {User}",
                    reservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), cancelledBy);

                return true;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Queens Hotel API: SQL Server error occurred while cancelling reservation {ReservationId}. Error: {ErrorMessage}",
                    reservationId, sqlEx.Message);
                
                // Check if it's a connection-related error
                if (IsConnectionError(sqlEx))
                {
                    throw new InvalidOperationException($"Database connection failed while cancelling reservation {reservationId}. Please check your network connection and database server availability.", sqlEx);
                }
                else
                {
                    throw new InvalidOperationException($"Database error occurred while cancelling reservation {reservationId}: {sqlEx.Message}", sqlEx);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Unexpected error occurred while cancelling reservation {ReservationId}",
                    reservationId);
                throw new InvalidOperationException($"An unexpected error occurred while cancelling reservation {reservationId}: {ex.Message}", ex);
            }
        }

        private async Task ExecuteCancelReservationDirectlyAsync(int reservationId, string cancelledBy)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            
            using var command = new SqlCommand("[dbo].[CancelReservation]", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };
            
            command.Parameters.Add(new SqlParameter("@ReservationID", SqlDbType.Int) { Value = reservationId });
            command.Parameters.Add(new SqlParameter("@CancelledBy", SqlDbType.VarChar, 45) { Value = cancelledBy });
            
            await command.ExecuteNonQueryAsync();
        }

        private static bool IsConnectionError(SqlException sqlException)
        {
            // Common SQL Server connection error numbers
            var connectionErrorNumbers = new[] { 2, 53, 121, 232, 258, 1231, 1232, 11001, 18456 };
            return connectionErrorNumbers.Contains(sqlException.Number);
        }
    }
}