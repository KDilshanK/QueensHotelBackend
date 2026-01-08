using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger<RoomService> _logger;

        public RoomService(IRoomRepository roomRepository, ILogger<RoomService> logger)
        {
            _roomRepository = roomRepository;
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
            return await _roomRepository.GetRoomDetailsDataAsync(type, roomId, roomNumber, floorId, roomTypeId, accommodation);
        }

        public async Task<InsertRoomResponseDto> InsertRoomAsync(InsertRoomRequestDto dto)
        {
            return await _roomRepository.InsertRoomAsync(dto);
        }
    }
}