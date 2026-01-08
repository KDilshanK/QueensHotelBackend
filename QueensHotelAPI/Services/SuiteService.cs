using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    public class SuiteService : ISuiteService
    {
        private readonly ISuiteRepository _suiteRepository;
        private readonly ILogger<SuiteService> _logger;

        public SuiteService(ISuiteRepository suiteRepository, ILogger<SuiteService> logger)
        {
            _suiteRepository = suiteRepository;
            _logger = logger;
        }

        public async Task<InsertSuiteResponseDto> InsertSuiteAsync(InsertSuiteRequestDto dto)
        {
            return await _suiteRepository.InsertSuiteAsync(dto);
        }

        public async Task<IEnumerable<GetAllSuitesResponseDto>> GetAllSuitesAsync()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing GetAllSuites request at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var suites = await _suiteRepository.GetAllSuitesAsync();

                _logger.LogInformation("Queens Hotel API: Successfully retrieved {Count} suites at {Timestamp} by user: {User}",
                    suites.Count(), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                return suites;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during GetAllSuites service operation at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                throw;
            }
        }
    }
}