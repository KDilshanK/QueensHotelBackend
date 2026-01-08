using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FloorRepository : IFloorRepository
{
    private readonly QueensHotelDbContext _context;

    public FloorRepository(QueensHotelDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FloorDto>> GetAllFloorsAsync()
    {
        var floors = await _context.Floors
            .FromSqlRaw("EXEC GetAllFloors")
            .ToListAsync();

        // Map to DTOs
        return floors.Select(f => new FloorDto
        {
            Id = f.Id,
            FloorNumber = f.FloorNumber,
            Description = f.Description
        }).ToList();
    }
}