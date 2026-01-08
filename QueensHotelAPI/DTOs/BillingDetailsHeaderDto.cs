using System;
namespace QueensHotelAPI.DTOs
{
    public class BillingDetailsHeaderDto
    {
        public int BillingID { get; set; }
        public string? BillingDate { get; set; }
        public double? TotalAmount { get; set; }
        public bool? IsNoShowCharge { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDateTime { get; set; }
        public int? Reservation_ID { get; set; }
        public int? CompanyMaster_Id { get; set; }
    }
}

