using System;
namespace QueensHotelAPI.DTOs
{
    public class BillingDetailsServiceChargeSummaryDto
    {
        public double TotalServiceCharges { get; set; }
        public int FreeServicesCount { get; set; }
        public int PaidServicesCount { get; set; }
        public int TotalServiceItems { get; set; }
    }
}

