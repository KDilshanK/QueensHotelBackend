using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
    public interface ICheckOutRepository
    {
        //Task<InsertResponseDto> InsertCheckOutAsync(InsertCheckOutRequestDto dto);
        Task<CheckOutResultDto> InsertCheckOutAsync(int checkInId, int userId);
        Task<List<CheckOutDetailsDto>> GetCheckOutDetailsAsync(
        int checkOutId = 0,
        int reservationId = 0,
        int roomId = 0,
        string customerNIC = null,
        string customerEmail = null,
        string customerPhone = null,
        DateTime? checkInDate = null
    );
    }
}