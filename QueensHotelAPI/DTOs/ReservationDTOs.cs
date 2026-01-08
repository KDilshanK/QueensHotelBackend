namespace QueensHotelAPI.DTOs
{
    public class GetReservationDataRequest
    {
        public string? NIC { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Email { get; set; }
        public string? Number { get; set; }
        public int? Id { get; set; }
    }

    public class ReservationDetailsResponse
    {
        public int ReservationId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool CreditCardProvided { get; set; }
        public string CreateBy { get; set; } = string.Empty;
        public DateTime CreatedDateTime { get; set; }

        // Payment information (new fields)
        public PaymentInfo? Payment { get; set; }

        public CustomerInfo Customer { get; set; } = new();
        public MealPlanInfo? MealPlan { get; set; }
        public SuiteInfo? Suite { get; set; }
        public RoomInfo? Room { get; set; }
        public TravelAgencyInfo? TravelAgency { get; set; }
    }

    // New payment information class
    public class PaymentInfo
    {
        public int? PaymentMethodId { get; set; }
        public string PaymentMethodType { get; set; } = string.Empty;
        public int? PaymentCardDetailsId { get; set; }
        public PaymentCardInfo? PaymentCard { get; set; }
        public string GuestStatus { get; set; } = string.Empty;
    }

    // New payment card information class
    public class PaymentCardInfo
    {
        public int? CardId { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public int? ExpiryMonth { get; set; }
        public int? ExpiryYear { get; set; }
        public string MaskedCardNumber { get; set; } = string.Empty; // For display purposes
    }

    public class CustomerInfo
    {
        public int Id { get; set; }
        public string NIC { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PassportId { get; set; } = string.Empty;
        public string DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class MealPlanInfo
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal CostPerNight { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsFree { get; set; }
    }

    public class SuiteInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal WeeklyRate { get; set; }
        public decimal MonthlyRate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public string LivingArea { get; set; } = string.Empty;
        public string Kitchen { get; set; } = string.Empty;
        public string DiningArea { get; set; } = string.Empty;
        public int Bathrooms { get; set; }
        public string LaundryFacilities { get; set; } = string.Empty;
        public string Workspace { get; set; } = string.Empty;
        public string WifiEntertainment { get; set; } = string.Empty;
        public string BalconyTerrace { get; set; } = string.Empty;
        public string Housekeeping { get; set; } = string.Empty;
        public string Security { get; set; } = string.Empty;
    }

    public class RoomInfo
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // maps to RoomTypeName
        public decimal RatePerNight { get; set; }        // maps to RoomRatePerNight
        public bool IsAcAvailable { get; set; }          // maps to HasAirConditioning
        public int Capacity { get; set; }
        public int StatusId { get; set; }                // maps to RoomStatusID
        public string StatusName { get; set; } = string.Empty; // maps to RoomStatusName
        public int FloorId { get; set; }                 // maps to FloorID
        public int FloorNumber { get; set; }
        public string FloorDescription { get; set; } = string.Empty;
    }

    public class TravelAgencyInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public decimal DiscountRate { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string WebpageUrl { get; set; } = string.Empty;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int? Count { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string ApiVersion { get; set; } = "1.0.0";
        public string ProcessedBy { get; set; } = "dilshan-jolanka";
    }
}