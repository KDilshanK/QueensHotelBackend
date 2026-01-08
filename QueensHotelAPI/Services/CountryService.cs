using QueensHotelAPI.Models;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<IEnumerable<CountryOfResidenceResult>> GetCountryOfResidenceDataAsync()
        {
            return await _countryRepository.GetCountryOfResidenceDataAsync();
        }
    }
}