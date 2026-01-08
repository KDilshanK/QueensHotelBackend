using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    public interface ISuiteService
    {
        Task<InsertSuiteResponseDto> InsertSuiteAsync(InsertSuiteRequestDto dto);
        Task<IEnumerable<GetAllSuitesResponseDto>> GetAllSuitesAsync();
    }
}