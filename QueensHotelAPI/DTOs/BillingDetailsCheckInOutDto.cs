using System;
namespace QueensHotelAPI.DTOs
{
    public class BillingDetailsCheckInOutDto
    {
        public int CheckInID { get; set; }
        public string? ActualCheckInDateTime { get; set; }
        public int? CheckOutID { get; set; }
        public string? ActualCheckOutTime { get; set; }
        public string? ActualCheckOutDateTime { get; set; }
        public int NightsBilled { get; set; }
        public int LateCheckoutFlag { get; set; }
        public string? LateCheckout { get; set; }
        public int DaysLateCheckingOut { get; set; }
        public int DaysLateCheckingIn { get; set; }
        public int AdditionalNights { get; set; }
    }
}

