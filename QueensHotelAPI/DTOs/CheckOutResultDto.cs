using System;
namespace QueensHotelAPI.DTOs
{
    public class CheckOutResultDto
    {
        public int CheckOutID { get; set; }
        public int BillingID { get; set; }
        public int? CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public int? ReservationID { get; set; }
        public string? ReservationCheckInDateTime { get; set; }
        public string? ReservationCheckOutDateTime { get; set; }
        public int? ReservedNights { get; set; }
        public string? ActualCheckInDateTime { get; set; }
        public string? ActualCheckOutDateTime { get; set; }
        public int? ActualNights { get; set; }
        public int? DaysLateCheckingIn { get; set; }
        public int? DaysLateCheckingOut { get; set; }
        public int? AdditionalNights { get; set; }
        public bool? LateCheckoutFlag { get; set; }
        public int? TotalNightsBilled { get; set; }
        public decimal? AccommodationCharge { get; set; }
        public decimal? TotalBilled { get; set; }
        public string? CheckoutStatus { get; set; }
        public string? BillingStatus { get; set; }
        public string? AccommodationType { get; set; }
        public string? AccommodationNumber { get; set; }
        public string? ProcessedBy { get; set; }
        public string? ProcessedDateTime { get; set; }
        public string? HotelPolicy { get; set; }
        public string? BillingNote { get; set; }
        public string? Result { get; set; }
        public int? ErrorNumber { get; set; }
        public int? ErrorLine { get; set; }
        public string? ErrorProcedure { get; set; } // <-- nullable!
    }
}

