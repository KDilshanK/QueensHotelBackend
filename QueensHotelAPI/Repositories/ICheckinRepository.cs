using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
    public interface ICheckinRepository
    {
        Task<InsertCheckinResponseDto> InsertCheckinAsync(InsertCheckinRequestDto dto);
    }
}