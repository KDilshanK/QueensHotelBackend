using System;
namespace QueensHotelAPI.DTOs
{
    public class BillingDetailsServiceChargeDto
    {
        public int ServiceChargeID { get; set; }
        public string? ServiceType { get; set; }
        public double Amount { get; set; }
        public string? ServiceDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDateTime { get; set; }
        public int IsFree { get; set; }
        public int BillingServiceChargeID { get; set; }
        public string? DateAdded { get; set; }
        public int ItemNumber { get; set; }
    }
}

