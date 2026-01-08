using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;
using System.Threading.Tasks;

public class CheckOutRepository : ICheckOutRepository
{
    private readonly QueensHotelDbContext _context;

    public CheckOutRepository(QueensHotelDbContext context)
    {
        _context = context;
    }

    public async Task<CheckOutResultDto> InsertCheckOutAsync(int checkInId, int userId)
    {
        var results = await _context.CheckOutResultDto
            .FromSqlRaw(
                "EXEC [dbo].[InsertCheckOut] @CheckIn_Id, @User_Id",
                new SqlParameter("@CheckIn_Id", checkInId),
                new SqlParameter("@User_Id", userId)
            )
            .ToListAsync();

        // Get the first result (should always be one row)
        return results.FirstOrDefault();
    }

    public async Task<List<CheckOutDetailsDto>> GetCheckOutDetailsAsync(
    int checkOutId = 0,
    int reservationId = 0,
    int roomId = 0,
    string customerNIC = null,
    string customerEmail = null,
    string customerPhone = null,
    DateTime? checkInDate = null)
    {
        var parameters = new List<SqlParameter>
    {
        new SqlParameter("@CheckOutId", checkOutId),
        new SqlParameter("@ReservationId", reservationId),
        new SqlParameter("@RoomId", roomId),
        new SqlParameter("@CustomerNIC", (object?)customerNIC ?? DBNull.Value),
        new SqlParameter("@CustomerEmail", (object?)customerEmail ?? DBNull.Value),
        new SqlParameter("@CustomerPhone", (object?)customerPhone ?? DBNull.Value),
        new SqlParameter("@CheckInDate", (object?)checkInDate ?? DBNull.Value)
    };

        var result = await _context.CheckOutDetailsDto
            .FromSqlRaw(
                "EXEC [dbo].[GetCheckOutDetails] @CheckOutId, @ReservationId, @RoomId, @CustomerNIC, @CustomerEmail, @CustomerPhone, @CheckInDate",
                parameters.ToArray()
            ).ToListAsync();

        return result;
    }
}