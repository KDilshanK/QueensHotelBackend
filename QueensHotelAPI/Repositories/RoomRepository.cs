using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Models;
using System.Data;

namespace QueensHotelAPI.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly QueensHotelDbContext _context;
        private readonly ILogger<RoomRepository> _logger;

        public RoomRepository(QueensHotelDbContext context, ILogger<RoomRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<object>> GetRoomDetailsDataAsync(
            string? type = "all",
            int roomId = 0,
            int roomNumber = 0,
            int floorId = 0,
            int roomTypeId = 0,
            string accommodation = "Room")
        {
            var results = new List<object>();
            await _context.Database.OpenConnectionAsync();
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[GetRoomDetails_Data]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@Type", SqlDbType.VarChar, 20) { Value = (object?)type ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@RoomId", SqlDbType.Int) { Value = roomId });
                command.Parameters.Add(new SqlParameter("@RoomNumber", SqlDbType.Int) { Value = roomNumber });
                command.Parameters.Add(new SqlParameter("@FloorId", SqlDbType.Int) { Value = floorId });
                command.Parameters.Add(new SqlParameter("@RoomTypeId", SqlDbType.Int) { Value = roomTypeId });
                command.Parameters.Add(new SqlParameter("@Accommodation", SqlDbType.VarChar, 10) { Value = (object?)accommodation ?? "Room" });

                using var reader = await command.ExecuteReaderAsync();
                if (accommodation.ToLower() == "suite")
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new SuiteDetailsResult
                        {
                            AccommodationType = reader["AccommodationType"]?.ToString() ?? "Suite",
                            SuiteId = reader["SuiteId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SuiteId"]),
                            SuiteName = reader["SuiteName"]?.ToString() ?? "",
                            SuiteType = reader["SuiteType"]?.ToString() ?? "",
                            Status = reader["Status"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Status"]),
                            EnumRoomType = reader["EnumRoomType"] == DBNull.Value ? 0 : Convert.ToInt32(reader["EnumRoomType"]),
                            CompanyMaster_Id = reader["CompanyMaster_Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CompanyMaster_Id"]),
                            WeeklyRate = reader["WeeklyRate"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WeeklyRate"]),
                            MonthlyRate = reader["MonthlyRate"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MonthlyRate"]),
                            WeeklyRateFormatted = reader["WeeklyRateFormatted"]?.ToString() ?? "",
                            MonthlyRateFormatted = reader["MonthlyRateFormatted"]?.ToString() ?? "",
                            SuiteSize = reader["SuiteSize"]?.ToString() ?? "",
                            Bedrooms = reader["Bedrooms"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Bedrooms"]),
                            Bathrooms = reader["Bathrooms"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Bathrooms"]),
                            Description = reader["Description"]?.ToString() ?? "",
                            LivingArea = reader["LivingArea"]?.ToString() ?? "",
                            Kitchen = reader["Kitchen"]?.ToString() ?? "",
                            DiningArea = reader["DiningArea"]?.ToString() ?? "",
                            LaundryFacilities = reader["LaundryFacilities"]?.ToString() ?? "",
                            Workspace = reader["Workspace"]?.ToString() ?? "",
                            WifiEntertainment = reader["WifiEntertainment"]?.ToString() ?? "",
                            BalconyTerrace = reader["BalconyTerrace"]?.ToString() ?? "",
                            Housekeeping = reader["Housekeeping"]?.ToString() ?? "",
                            Security = reader["Security"]?.ToString() ?? "",
                            AvailabilityStatus = reader["AvailabilityStatus"]?.ToString() ?? "",
                            BookingStatus = reader["BookingStatus"]?.ToString() ?? "",
                            AccommodationSummary = reader["AccommodationSummary"]?.ToString() ?? "",
                            FullDescription = reader["FullDescription"]?.ToString() ?? "",
                        });
                    }
                }
                else
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new RoomDetailsResult
                        {
                            AccommodationType = reader["AccommodationType"]?.ToString() ?? "Room",
                            RoomId = reader["RoomId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoomId"]),
                            RoomNumber = reader["RoomNumber"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoomNumber"]),
                            Capacity = reader["Capacity"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Capacity"]),
                            CompanyMaster_Id = reader["CompanyMaster_Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CompanyMaster_Id"]),
                            RoomTypeId = reader["RoomTypeId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoomTypeId"]),
                            RoomTypeName = reader["RoomTypeName"]?.ToString() ?? "",
                            RatePerNight = reader["RatePerNight"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RatePerNight"]),
                            FormattedRate = reader["FormattedRate"]?.ToString() ?? "",
                            IsAcAvailable = reader["IsAcAvailable"] != DBNull.Value && (
                                reader["IsAcAvailable"].ToString() == "1" ||
                                reader["IsAcAvailable"].ToString().ToLower() == "true"),
                            RoomTypeStatus = reader["RoomTypeStatus"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoomTypeStatus"]),
                            FloorId = reader["FloorId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["FloorId"]),
                            FloorNumber = reader["FloorNumber"] == DBNull.Value ? 0 : Convert.ToInt32(reader["FloorNumber"]),
                            FloorDescription = reader["FloorDescription"]?.ToString() ?? "",
                            RoomStatusId = reader["RoomStatusId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoomStatusId"]),
                            RoomStatus = reader["RoomStatus"]?.ToString() ?? "",
                            AirConditioningStatus = reader["AirConditioningStatus"]?.ToString() ?? "",
                            DetailedRoomStatus = reader["DetailedRoomStatus"]?.ToString() ?? "",
                            BookingStatus = reader["BookingStatus"]?.ToString() ?? "",
                            RateWithCurrency = reader["RateWithCurrency"]?.ToString() ?? "",
                            AccommodationSummary = reader["AccommodationSummary"]?.ToString() ?? "",
                        });
                    }
                }
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

        public async Task<InsertRoomResponseDto> InsertRoomAsync(InsertRoomRequestDto dto)
        {
            await _context.Database.OpenConnectionAsync();
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[InsertRoom]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@RoomNumber", SqlDbType.NVarChar, 10) { Value = dto.RoomNumber });
                command.Parameters.Add(new SqlParameter("@Capacity", SqlDbType.Int) { Value = dto.Capacity });
                command.Parameters.Add(new SqlParameter("@CompanyMaster_Id", SqlDbType.Int) { Value = dto.CompanyMaster_Id });
                command.Parameters.Add(new SqlParameter("@EnumRoomId", SqlDbType.Int) { Value = dto.EnumRoomId });
                command.Parameters.Add(new SqlParameter("@FloorId", SqlDbType.Int) { Value = dto.FloorId });
                command.Parameters.Add(new SqlParameter("@RoomType_Id", SqlDbType.Int) { Value = dto.RoomType_Id });

                await command.ExecuteNonQueryAsync();
                return new InsertRoomResponseDto
                {
                    Success = true,
                    Message = "Room inserted successfully."
                };
            }
            catch (Exception ex)
            {
                return new InsertRoomResponseDto
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
    }
}