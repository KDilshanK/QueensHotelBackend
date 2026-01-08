using System.Threading.Tasks;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;
using QueensHotelAPI.Services;

public class CheckOutService : ICheckOutService
{
    private readonly ICheckOutRepository _repository;

    public CheckOutService(ICheckOutRepository repository)
    {
        _repository = repository;
    }

    public async Task<CheckOutResultDto> InsertCheckOutAsync(int checkInId, int userId)
    {
        return await _repository.InsertCheckOutAsync(checkInId, userId);
    }

    public async Task<List<CheckOutDetailsDto>> GetCheckOutDetailsAsync(
        int checkOutId = 0,
        int reservationId = 0,
        int roomId = 0,
        string customerNIC = null,
        string customerEmail = null,
        string customerPhone = null,
        DateTime? checkInDate = null)
    {
        return await _repository.GetCheckOutDetailsAsync(checkOutId, reservationId, roomId, customerNIC, customerEmail, customerPhone, checkInDate);
    }
}