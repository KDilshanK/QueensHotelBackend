using System;
using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    public interface ITravelAgencyService
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
}

