using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDetailsResponse>> GetReservationDataAsync(GetReservationDataRequest request);
        Task<CreateReservationResponseDto> InsertReservationAsync(CreateReservationDto dto);
        Task<bool> UpdateReservationAsync(UpdateReservationDto dto);
        Task<bool> ReservationExistsAsync(int reservationId);
        Task<CancelReservationResponseDto> CancelReservationAsync(CancelReservationRequestDto request);
    }
}