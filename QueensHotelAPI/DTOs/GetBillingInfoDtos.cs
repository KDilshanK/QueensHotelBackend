using System.Text.Json.Serialization;

namespace QueensHotelAPI.DTOs
{
    /// <summary>
    /// DTO for GetBillingInfo stored procedure response
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-27 15:40:00
    /// </summary>
    public class GetBillingInfoResponseDto
    {
        // Billing Information
        [JsonPropertyName("billingId")]
        public int BillingId { get; set; }

        [JsonPropertyName("billingDate")]
        public DateTime BillingDate { get; set; }

        [JsonPropertyName("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("isNoShowCharge")]
        public bool IsNoShowCharge { get; set; }

        [JsonPropertyName("paymentStatus")]
        public string PaymentStatus { get; set; } = string.Empty;

        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; } = string.Empty;

        [JsonPropertyName("paymentMethodType")]
        public string PaymentMethodType { get; set; } = string.Empty;

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        [JsonPropertyName("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonPropertyName("reservationId")]
        public int ReservationId { get; set; }

        [JsonPropertyName("billingCompanyMasterId")]
        public int BillingCompanyMasterId { get; set; }

        // Reservation Information
        [JsonPropertyName("reservationInfo")]
        public BillingReservationInfoDto ReservationInfo { get; set; } = new();

        // Customer Information
        [JsonPropertyName("customerInfo")]
        public BillingCustomerInfoDto CustomerInfo { get; set; } = new();
    }

    /// <summary>
    /// Reservation information within billing info
    /// </summary>
    public class BillingReservationInfoDto
    {
        [JsonPropertyName("checkInDate")]
        public DateTime? CheckInDate { get; set; }

        [JsonPropertyName("checkOutDate")]
        public DateTime? CheckOutDate { get; set; }

        [JsonPropertyName("numberOfGuests")]
        public int? NumberOfGuests { get; set; }

        [JsonPropertyName("reservationStatus")]
        public string ReservationStatus { get; set; } = string.Empty;

        [JsonPropertyName("createBy")]
        public string CreateBy { get; set; } = string.Empty;

        [JsonPropertyName("createdDateTime")]
        public DateTime? CreatedDateTime { get; set; }

        [JsonPropertyName("customerId")]
        public int CustomerId { get; set; }

        [JsonPropertyName("mealPlanId")]
        public int? MealPlanId { get; set; }

        [JsonPropertyName("suiteId")]
        public int? SuiteId { get; set; }

        [JsonPropertyName("suiteName")]
        public string SuiteName { get; set; } = string.Empty;

        [JsonPropertyName("roomId")]
        public int? RoomId { get; set; }

        [JsonPropertyName("roomType")]
        public string RoomType { get; set; } = string.Empty;

        [JsonPropertyName("roomRatePerNight")]
        public decimal? RoomRatePerNight { get; set; }

        [JsonPropertyName("travelAgencyId")]
        public int? TravelAgencyId { get; set; }

        [JsonPropertyName("reservationCompanyMasterId")]
        public int ReservationCompanyMasterId { get; set; }

        [JsonPropertyName("paymentMethodId")]
        public int? PaymentMethodId { get; set; }

        [JsonPropertyName("paymentCardDetailsId")]
        public int? PaymentCardDetailsId { get; set; }
    }

    /// <summary>
    /// Customer information within billing info
    /// </summary>
    public class BillingCustomerInfoDto
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("customerStatus")]
        public string CustomerStatus { get; set; } = string.Empty;

        [JsonPropertyName("customerCompanyMasterId")]
        public int? CustomerCompanyMasterId { get; set; }

        [JsonPropertyName("passportId")]
        public string PassportId { get; set; } = string.Empty;

        [JsonPropertyName("nic")]
        public string NIC { get; set; } = string.Empty;

        [JsonPropertyName("countryOfResidenceId")]
        public int? CountryOfResidenceId { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;
    }
}