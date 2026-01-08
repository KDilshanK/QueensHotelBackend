using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using System.Data;

namespace QueensHotelAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly QueensHotelDbContext _context;

        public UserRepository(QueensHotelDbContext context)
        {
            _context = context;
        }

        public async Task<UserLoginResponseDto?> UserLoginAsync(UserLoginRequestDto dto)
        {
            await _context.Database.OpenConnectionAsync();
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[UserLogin]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.VarChar, 50) { Value = dto.UserId });
                command.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 50) { Value = dto.Password });

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new UserLoginResponseDto
                    {
                        LoginStatus = reader["LoginStatus"]?.ToString() ?? "",
                        Message = reader["Message"]?.ToString() ?? "",
                        UserId = reader["UserId"] == DBNull.Value ? null : Convert.ToInt32(reader["UserId"]),
                        UserName = reader["UserName"]?.ToString(),
                        UserType = reader["UserType"]?.ToString(),
                        CompanyId = reader["CompanyId"] == DBNull.Value ? null : Convert.ToInt32(reader["CompanyId"]),
                        CompanyName = reader["CompanyName"]?.ToString(),
                        LastLoginTime = reader["LastLoginTime"] == DBNull.Value ? null : Convert.ToDateTime(reader["LastLoginTime"])
                    };
                }
                return null;
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                    await _context.Database.CloseConnectionAsync();
            }
        }
    }
}