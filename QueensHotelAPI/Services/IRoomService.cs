using QueensHotelAPI.DTOs;
using QueensHotelAPI.Models;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Service interface for Room business logic
    /// Author: dilshan-jolanka
    /// Create date: 2025-06-05 17:30:00
    /// Description: Business layer interface for room operations
    /// </summary>
    public interface IRoomService
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