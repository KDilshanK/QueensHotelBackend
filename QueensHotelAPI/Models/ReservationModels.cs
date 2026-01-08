using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueensHotelAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? CompanyMaster_Id { get; set; }
        public string PassportId { get; set; } = string.Empty;
        public string NIC { get; set; } = string.Empty;
        public int? CountryOfResidence_id { get; set; }
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
    }

    public class CustomerDetailsResult
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PassportId { get; set; } = string.Empty;
        public string NIC { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
    }

    public class MealPlan
    {
        public int Id { get; set; }
        public string MealPlanCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal CostPerNight { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsFree { get; set; }
        public int? CompanyMaster_Id { get; set; }
    }

    public class Suite
    {
        public int Id { get; set; }
        public string SuiteName { get; set; } = string.Empty;
        public decimal WeeklyRate { get; set; }
        public decimal MonthlyRate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public string Living_area { get; set; } = string.Empty;
        public string Kitchen { get; set; } = string.Empty;
        public string Dining_area { get; set; } = string.Empty;
        public int Bathrooms { get; set; }
        public string Laundry_facilities { get; set; } = string.Empty;
        public string Workspace { get; set; } = string.Empty;
        public string Wifi_entertainment { get; set; } = string.Empty;
        public string Balcony_terrace { get; set; } = string.Empty;
        public string Housekeeping { get; set; } = string.Empty;
        public string Security { get; set; } = string.Empty;
        public int? CompanyMaster_Id { get; set; }
    }

    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int? CompanyMaster_Id { get; set; }
        public int EnumRoomId { get; set; }
        public int FloorId { get; set; }
        public int RoomType_Id { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class RoomDetailsResult
    {
        public string AccommodationType { get; set; } = "Room";
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        public int Capacity { get; set; }
        public int CompanyMaster_Id { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal RatePerNight { get; set; }
        public string FormattedRate { get; set; } = string.Empty;
        public bool IsAcAvailable { get; set; }
        public int RoomTypeStatus { get; set; }
        public int FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string FloorDescription { get; set; } = string.Empty;
        public int RoomStatusId { get; set; }
        public string RoomStatus { get; set; } = string.Empty;
        public string AirConditioningStatus { get; set; } = string.Empty;
        public string DetailedRoomStatus { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public string RateWithCurrency { get; set; } = string.Empty;
        public string AccommodationSummary { get; set; } = string.Empty;
    }

    public class Floor
    {
        public int Id { get; set; }
        public int FloorNumber { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class RoomType
    {
        public int Id { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal RatePerNight { get; set; }
        public bool IsAcAvailable { get; set; }
        public string Status { get; set; }
        public int? CompanyMaster_Id { get; set; }
    }

    // Added the missing EnumRoomStatus model
    public class EnumRoomStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class TravelAgency
    {
        public int Id { get; set; }
        public string AgencyName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double DiscountRate { get; set; }
        public string WebpageUrl { get; set; } = string.Empty;
        public int? CompanyMaster_Id { get; set; }
        //public string Password { get; set; } = string.Empty;
    }

    public class Reservation
    {
        public int Id { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? NumberOfGuests { get; set; }
        public string? Status { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public int Customer_Id { get; set; }
        public int? MealPlan_id { get; set; }
        public int? Suite_id { get; set; }
        public int? Room_ID { get; set; }
        public int? TravalAgency_Id { get; set; }
        public int CompanyMaster_Id { get; set; }
        public int? PaymentMethodI_Id { get; set; }
        //public int? PaymentCardDetails_Id { get; set; }
    }

    public class ReservationDetailsResult
    {
        // Reservation fields
        public int ReservationID { get; set; }
        public DateTime ReservationCheckInDate { get; set; }
        public DateTime ReservationCheckOutDate { get; set; }
        public int GuestCount { get; set; }
        public string ReservationStatus { get; set; } = string.Empty;
        public DateTime ReservationCreatedDate { get; set; }

        // Payment information (new fields from updated SP)
        public int? PaymentMethod { get; set; }
        public int? PaymentCardDetails { get; set; }
        public string? GuestStatus { get; set; }

        // Payment method details (new fields from updated SP)
        public int? PaymentMethodId { get; set; }
        public string? PaymentMethodType { get; set; }

        // Payment card details (new fields from updated SP)  
        public int? PaymentCardId { get; set; }
        public string? CardHolderName { get; set; }
        public int? ExpiryYear { get; set; }
        public int? ExpiryMonth { get; set; }

        // Customer fields
        public int CustomerID { get; set; }
        public string CustomerNIC { get; set; } = string.Empty;
        public string CustomerFirstName { get; set; } = string.Empty;
        public string CustomerLastName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerPassportID { get; set; } = string.Empty;
        public string CustomerDateOfBirth { get; set; } = string.Empty; // as string for flexibility
        public string CustomerGender { get; set; } = string.Empty;

        // MealPlan fields
        public int? MealPlanID { get; set; }
        public decimal? CostPerNight { get; set; }
        public string? Description { get; set; }
        public int? MealPlanInternalId { get; set; }
        public bool? IsFree { get; set; }
        public string? MealPlanCode { get; set; }
        public string? MealPlanStatus { get; set; }

        // Suite fields
        public int? SuiteID { get; set; }
        public string? SuiteName { get; set; }
        public string? SuiteType { get; set; }
        public string? SuiteSize { get; set; }
        public int? SuiteBedrooms { get; set; }
        public int? SuiteBathrooms { get; set; }
        public string? SuiteDescription { get; set; }
        public decimal? SuiteWeeklyRate { get; set; }
        public decimal? SuiteMonthlyRate { get; set; }

        // Room fields
        public int? RoomID { get; set; }
        public string? RoomNumber { get; set; }
        public string? RoomTypeName { get; set; }
        public decimal? RoomRatePerNight { get; set; }
        public bool? HasAirConditioning { get; set; }
        public int? RoomCapacity { get; set; }
        public int? RoomStatusID { get; set; }
        public string? RoomStatusName { get; set; }

        // Floor fields
        public int? FloorID { get; set; }
        public int? FloorNumber { get; set; }
        public string? FloorDescription { get; set; }

        // Travel Agency fields
        public int? TravelAgencyID { get; set; }
        public string? TravelAgencyName { get; set; }
        public string? Address { get; set; }
        public string? ContactPerson { get; set; }
        public decimal? DiscountRate { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? WebpageUrl { get; set; }
    }

    public class SuiteDetailsResult
    {
        public string AccommodationType { get; set; } = "Suite";
        public int SuiteId { get; set; }
        public string SuiteName { get; set; } = string.Empty;
        public string SuiteType { get; set; } = string.Empty;
        public int Status { get; set; }
        public int EnumRoomType { get; set; }
        public int CompanyMaster_Id { get; set; }
        public decimal WeeklyRate { get; set; }
        public decimal MonthlyRate { get; set; }
        public string WeeklyRateFormatted { get; set; } = string.Empty;
        public string MonthlyRateFormatted { get; set; } = string.Empty;
        public string SuiteSize { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public string Description { get; set; } = string.Empty;
        public string LivingArea { get; set; } = string.Empty;
        public string Kitchen { get; set; } = string.Empty;
        public string DiningArea { get; set; } = string.Empty;
        public string LaundryFacilities { get; set; } = string.Empty;
        public string Workspace { get; set; } = string.Empty;
        public string WifiEntertainment { get; set; } = string.Empty;
        public string BalconyTerrace { get; set; } = string.Empty;
        public string Housekeeping { get; set; } = string.Empty;
        public string Security { get; set; } = string.Empty;
        public string AvailabilityStatus { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public string AccommodationSummary { get; set; } = string.Empty;
        public string FullDescription { get; set; } = string.Empty;
    }

    public class CountryOfResidenceResult
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StatusText { get; set; } = string.Empty;
    }

    public class Billing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime BillingDate { get; set; }
        public float TotalAmount { get; set; }
        public bool IsNoShowCharge { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public int Reservation_ID { get; set; }
        public int CompanyMaster_Id { get; set; }
    }

    public class PaymentCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public byte[] CardNumber { get; set; } = Array.Empty<byte>(); // Encrypted
        public string CardType { get; set; } = string.Empty;
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public byte[] CVV { get; set; } = Array.Empty<byte>(); // Encrypted
        public bool IsDefault { get; set; }
        public bool Status { get; set; }
        public int Customer_Id { get; set; }
        public int CardType_id { get; set; }
        public int CompanyMaster_Id { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    }

    public class CardType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

}