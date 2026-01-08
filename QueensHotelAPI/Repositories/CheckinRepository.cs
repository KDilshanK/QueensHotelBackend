using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using System.Data;

namespace QueensHotelAPI.Repositories
{
    public class CheckinRepository : ICheckinRepository
    {
        private readonly QueensHotelDbContext _context;

        public CheckinRepository(QueensHotelDbContext context)
        {
            _context = context;
        }

        public async Task<InsertCheckinResponseDto> InsertCheckinAsync(InsertCheckinRequestDto dto)
        {
            await _context.Database.OpenConnectionAsync();
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[InsertCheckin]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@CheckInTime", SqlDbType.DateTime) { Value = dto.CheckInTime });
                command.Parameters.Add(new SqlParameter("@Reservation_ID", SqlDbType.Int) { Value = dto.Reservation_ID });
                command.Parameters.Add(new SqlParameter("@User_Id", SqlDbType.Int) { Value = dto.User_Id });
                command.Parameters.Add(new SqlParameter("@Room_ID", SqlDbType.Int)
                {
                    Value = (dto.Room_ID == null || dto.Room_ID == 0) ? DBNull.Value : dto.Room_ID
                });
                command.Parameters.Add(new SqlParameter("@Suite_id", SqlDbType.Int)
                {
                    Value = (dto.Suite_id == null || dto.Suite_id == 0) ? DBNull.Value : dto.Suite_id
                });


                await command.ExecuteNonQueryAsync();
                return new InsertCheckinResponseDto
                {
                    Success = true,
                    Message = "Check-in record inserted successfully."
                };
            }
            catch (Exception ex)
            {
                return new InsertCheckinResponseDto
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