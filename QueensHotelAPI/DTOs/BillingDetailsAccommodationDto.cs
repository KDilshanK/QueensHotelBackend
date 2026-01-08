using System;
namespace QueensHotelAPI.DTOs
{
    public class BillingDetailsAccommodationDto
    {
        public string? AccommodationType { get; set; }
        public string? AccommodationNumber { get; set; }
        public string? AccommodationCategory { get; set; }
        public double? RatePerNight { get; set; }
        public double? TotalAccommodationCharge { get; set; }
    }
}

