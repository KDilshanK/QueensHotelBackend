using System.Collections.Generic;
using System.Threading.Tasks;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;
using QueensHotelAPI.Services;

public class TravelAgencyService : ITravelAgencyService
{
    private readonly ITravelAgencyRepository _repository;

    public TravelAgencyService(ITravelAgencyRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> InsertTravelAgencyAsync(TravelAgencyCreateDto dto)
    {
        return await _repository.InsertTravelAgencyAsync(dto);
    }

    public async Task<IEnumerable<TravelAgencyDto>> GetTravelAgencyDataAsync(
        string agencyName = null,
        string contactPerson = null,
        string phone = null,
        string email = null,
        string webpageUrl = null)
    {
        return await _repository.GetTravelAgencyDataAsync(agencyName, contactPerson, phone, email, webpageUrl);
    }
}