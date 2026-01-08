using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.Models;
using System.Data;

namespace QueensHotelAPI.Repositories
{
    public class RoomTypeRepository
    {
        private readonly QueensHotelDbContext _context;
        private readonly ILogger<RoomTypeRepository> _logger;

        public RoomTypeRepository(QueensHotelDbContext context, ILogger<RoomTypeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RoomType>> GetRoomTypesAsync(int? id = null)
        {
            var results = new List<RoomType>();
            await _context.Database.OpenConnectionAsync();

            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[GetRoomTypes]";
                command.CommandType = CommandType.StoredProcedure;

                var idParam = command.CreateParameter();
                idParam.ParameterName = "@Id";
                idParam.Value = id.HasValue ? (object)id.Value : DBNull.Value;
                idParam.DbType = DbType.Int32;
                command.Parameters.Add(idParam);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(new RoomType
                    {
                        Id = reader.IsDBNull(reader.GetOrdinal("id"))? 0: reader.GetInt32(reader.GetOrdinal("id")),
                        RoomTypeName = reader.IsDBNull(reader.GetOrdinal("RoomType")) ? string.Empty : reader.GetString(reader.GetOrdinal("RoomType")),
                        RatePerNight = reader.IsDBNull(reader.GetOrdinal("RatePerNight")) ? 0 : Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("RatePerNight"))),
                        IsAcAvailable = !reader.IsDBNull(reader.GetOrdinal("IsAcAvailable")) && reader.GetBoolean(reader.GetOrdinal("IsAcAvailable")),
                        Status = reader.IsDBNull(reader.GetOrdinal("status"))? string.Empty: reader.GetValue(reader.GetOrdinal("status")).ToString(),
                        CompanyMaster_Id = reader.IsDBNull(reader.GetOrdinal("CompanyMaster_Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("CompanyMaster_Id"))
                    });
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
    }
}