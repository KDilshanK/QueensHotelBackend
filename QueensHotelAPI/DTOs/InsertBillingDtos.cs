using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QueensHotelAPI.DTOs
{
    /// <summary>
    /// Request DTO for inserting billing information
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-27 14:50:00
    /// </summary>
    public class InsertBillingRequestDto
    {
        [Required(ErrorMessage = "Billing date is required")]
        [JsonPropertyName("billingDate")]
        public DateTime BillingDate { get; set; }

        [Required(ErrorMessage = "Total amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0")]
        [JsonPropertyName("totalAmount")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "IsNoShowCharge flag is required")]
        [JsonPropertyName("isNoShowCharge")]
        public bool IsNoShowCharge { get; set; }

        [Required(ErrorMessage = "Payment status is required")]
        [MaxLength(50, ErrorMessage = "Payment status cannot exceed 50 characters")]
        [JsonPropertyName("paymentStatus")]
        public string PaymentStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment method is required")]
        [JsonPropertyName("paymentMethod")]
        public int PaymentMethod { get; set; }

        [Required(ErrorMessage = "CreatedBy is required")]
        [MaxLength(100, ErrorMessage = "CreatedBy cannot exceed 100 characters")]
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reservation ID is required")]
        [JsonPropertyName("reservationId")]
        public int ReservationId { get; set; }
    }

    /// <summary>
    /// Response DTO for billing insertion result
    /// </summary>
    public class InsertBillingResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("billingId")]
        public int? BillingId { get; set; }

        [JsonPropertyName("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }
    }
}