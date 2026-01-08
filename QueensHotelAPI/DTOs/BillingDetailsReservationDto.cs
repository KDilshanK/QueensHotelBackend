using System;
namespace QueensHotelAPI.DTOs
{
    public class BillingDetailsReservationDto
    {
        public int ReservationID { get; set; }
        public int CustomerID { get; set; }
        public string? ReservationCheckInDate { get; set; }
        public string? ReservationCheckOutDate { get; set; }
        public int ReservationNights { get; set; }
        public string? ReservationStatus { get; set; }
    }
}

