using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
    public interface ISuiteRepository
    {
        Task<InsertSuiteResponseDto> InsertSuiteAsync(InsertSuiteRequestDto dto);
        Task<IEnumerable<GetAllSuitesResponseDto>> GetAllSuitesAsync();
    }
}