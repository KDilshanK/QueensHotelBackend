using QueensHotelAPI.DTOs;
using QueensHotelAPI.Models;

namespace QueensHotelAPI.Repositories
{
    /// <summary>
    /// Repository interface for Room data operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-06-05 17:30:00
    /// Description: Interface for executing GetRoomDetails_Data stored procedure
    /// </summary>
    public interface IRoomRepository
    {
        Task<IEnumerable<object>> GetRoomDetailsDataAsync(
            string? type = "all",
            int roomId = 0,
            int roomNumber = 0,
            int floorId = 0,
            int roomTypeId = 0,
            string accommodation = "Room");
        Task<InsertRoomResponseDto> InsertRoomAsync(InsertRoomRequestDto dto);
    }
}