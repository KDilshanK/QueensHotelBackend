using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    public interface ICheckOutService
    {
        Task<List<CheckOutDetailsDto>> GetCheckOutDetailsAsync(
        int checkOutId = 0,
        int reservationId = 0,
        int roomId = 0,
        string customerNIC = null,
        string customerEmail = null,
        string customerPhone = null,
        DateTime? checkInDate = null
    );
        Task<CheckOutResultDto> InsertCheckOutAsync(int checkInId, int userId);
    }
}