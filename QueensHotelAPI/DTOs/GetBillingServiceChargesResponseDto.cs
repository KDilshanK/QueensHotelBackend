using System;
using System.Text.Json.Serialization;

namespace QueensHotelAPI.DTOs
{
    /// <summary>
    /// DTO for GetBillingServiceCharges stored procedure response
    /// Author: dilshan-jolanka
    /// Create date: 2025-09-06
    /// </summary>
    public class GetBillingServiceChargesResponseDto
    {
        // Billing_ServiceCharge fields
        [JsonPropertyName("billingServiceChargeId")]
        public int BillingServiceChargeId { get; set; }

        [JsonPropertyName("billingId")]
        public int BillingId { get; set; }

        [JsonPropertyName("serviceChargeId")]
        public int ServiceChargeId { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        [JsonPropertyName("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        // Billing information
        [JsonPropertyName("billingInfo")]
        public BillingServiceChargesBillingDto? BillingInfo { get; set; }

        // ServiceCharge information
        [JsonPropertyName("serviceChargeInfo")]
        public BillingServiceChargesServiceChargeDto? ServiceChargeInfo { get; set; }
    }

    /// <summary>
    /// Billing information within billing service charges
    /// </summary>
    public class BillingServiceChargesBillingDto
    {
        [JsonPropertyName("billingDate")]
        public DateTime? BillingDate { get; set; }

        [JsonPropertyName("totalAmount")]
        public decimal? TotalAmount { get; set; }

        [JsonPropertyName("isNoShowCharge")]
        public bool? IsNoShowCharge { get; set; }

        [JsonPropertyName("paymentStatus")]
        public string PaymentStatus { get; set; } = string.Empty;

        [JsonPropertyName("paymentMethod")]
        public int? PaymentMethod { get; set; }

        [JsonPropertyName("reservationId")]
        public int? ReservationId { get; set; }

        [JsonPropertyName("companyMasterId")]
        public int? CompanyMasterId { get; set; }
    }

    /// <summary>
    /// ServiceCharge information within billing service charges
    /// </summary>
    public class BillingServiceChargesServiceChargeDto
    {
        [JsonPropertyName("serviceType")]
        public string ServiceType { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal? Amount { get; set; }

        [JsonPropertyName("serviceDate")]
        public DateTime? ServiceDate { get; set; }

        [JsonPropertyName("isFree")]
        public bool? IsFree { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}