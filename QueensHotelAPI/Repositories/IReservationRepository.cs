using QueensHotelAPI.DTOs;
using QueensHotelAPI.Models;

namespace QueensHotelAPI.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<ReservationDetailsResult>> GetReservationDataAsync(
            string? nic = null,
            string? fname = null,
            string? lname = null,
            string? email = null,
            string? number = null,
            int? id = 0);
        Task<int> InsertReservationAsync(CreateReservationDto dto);
        Task<bool> UpdateReservationAsync(UpdateReservationDto dto);
        Task<bool> ReservationExistsAsync(int reservationId);
        Task<bool> CancelReservationAsync(int reservationId, string cancelledBy);
    }
}