using QueensHotelAPI.Models;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    public class RoomTypeService
    {
        private readonly RoomTypeRepository _repository;

        public RoomTypeService(RoomTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RoomType>> GetAllRoomTypesAsync()
        {
            return await _repository.GetRoomTypesAsync(null);
        }

        public async Task<RoomType?> GetRoomTypeByIdAsync(int id)
        {
            return (await _repository.GetRoomTypesAsync(id)).FirstOrDefault();
        }
    }
}