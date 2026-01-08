using QueensHotelAPI.Models;

namespace QueensHotelAPI.Services
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryOfResidenceResult>> GetCountryOfResidenceDataAsync();
    }
}