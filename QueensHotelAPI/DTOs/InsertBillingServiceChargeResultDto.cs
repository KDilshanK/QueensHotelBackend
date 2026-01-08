using System;
namespace QueensHotelAPI.DTOs
{
    public class InsertBillingServiceChargeResultDto
    {
        public int Result { get; set; }
        public string? Message { get; set; }
        public int? ServiceChargeID { get; set; }
        public string? ServiceType { get; set; }
        public decimal? Amount { get; set; }
        public bool? IsFree { get; set; }
        public decimal? UpdatedBillingTotal { get; set; }
        // Error fields (only if needed)
        public int? ErrorNumber { get; set; }
        public int? ErrorLine { get; set; }
        public string? ErrorProcedure { get; set; }
    }
}

