using System;
namespace QueensHotelAPI.DTOs
{
    public class BillingDetailsSummaryDto
    {
        public int BillingID { get; set; }
        public double GrandTotal { get; set; }
        public double AccommodationCharge { get; set; }
        public double ServiceChargesTotal { get; set; }
        public string? LateCheckoutFeeStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethodName { get; set; }
        public string? BillingDate { get; set; }
        public string? ProcessedBy { get; set; }
        public string? CurrentDateTime { get; set; }
        public string? CurrentUser { get; set; }
    }
}

