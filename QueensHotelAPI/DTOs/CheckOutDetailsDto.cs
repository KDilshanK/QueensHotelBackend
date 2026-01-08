using System;
namespace QueensHotelAPI.DTOs
{
    public class CheckOutDetailsDto
    {
        public int CheckOutId { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public bool LateCheckout { get; set; }
        public int TotalNights { get; set; }
        public DateTime CheckOutDateTime { get; set; }
        public int CheckInId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime ActualCheckInDate { get; set; }
        public DateTime CalculatedCheckOutDate { get; set; }
        public int ReservationId { get; set; }
        public DateTime ReservedCheckInDate { get; set; }
        public DateTime ReservedCheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string? ReservationStatus { get; set; }
        public int CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LName { get; set; }
        public string? CustomerFullName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }
        public string? PassportId { get; set; }
        public string? NIC { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? CountryOfResidence { get; set; }
        public string? AccommodationType { get; set; }
        public int? RoomId { get; set; }
        public int? RoomNumber { get; set; }
        public int? RoomCapacity { get; set; }
        public string? RoomType { get; set; }
        public double? RatePerNight { get; set; }
        public string? AirConditioned { get; set; }
        public string? RoomStatus { get; set; }
        public int? FloorId { get; set; }
        public int? FloorNumber { get; set; }
        public string? FloorDescription { get; set; }
        public int? SuiteId { get; set; }
        public string? SuiteName { get; set; }
        public string? SuiteType { get; set; }
        public string? SuiteSize { get; set; }
        public int? SuiteBedrooms { get; set; }
        public int? SuiteBathrooms { get; set; }
        public string? SuiteDescription { get; set; }
        public decimal? SuiteWeeklyRate { get; set; }
        public decimal? SuiteMonthlyRate { get; set; }
        public int CheckoutProcessedByUserId { get; set; }
        public string? CheckoutProcessedByUserLogin { get; set; }
        public int CheckInProcessedByUserId { get; set; }
        public string? CheckInProcessedByUserLogin { get; set; }
    }
}

