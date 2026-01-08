using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    public interface ICheckinService
    {
        Task<InsertCheckinResponseDto> InsertCheckinAsync(InsertCheckinRequestDto dto);
    }
}