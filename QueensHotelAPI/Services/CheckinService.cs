using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    public class CheckinService : ICheckinService
    {
        private readonly ICheckinRepository _checkinRepository;

        public CheckinService(ICheckinRepository checkinRepository)
        {
            _checkinRepository = checkinRepository;
        }

        public async Task<InsertCheckinResponseDto> InsertCheckinAsync(InsertCheckinRequestDto dto)
        {
            return await _checkinRepository.InsertCheckinAsync(dto);
        }
    }
}