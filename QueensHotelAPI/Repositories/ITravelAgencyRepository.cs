using System.Collections.Generic;
using System.Threading.Tasks;
using QueensHotelAPI.DTOs;

public interface ITravelAgencyRepository
{
    Task<int> InsertTravelAgencyAsync(TravelAgencyCreateDto dto);
    Task<IEnumerable<TravelAgencyDto>> GetTravelAgencyDataAsync(
        string agencyName = null,
        string contactPerson = null,
        string phone = null,
        string email = null,
        string webpageUrl = null
    );
}