using QueensHotelAPI.Models;

namespace QueensHotelAPI.Repositories
{
    public interface ICountryRepository
    {
        Task<IEnumerable<CountryOfResidenceResult>> GetCountryOfResidenceDataAsync();
    }
}