using System;
namespace QueensHotelAPI.DTOs
{
    public class GetBillingDetailsResultDto
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public BillingDetailsHeaderDto? Header { get; set; }
        public BillingDetailsReservationDto? Reservation { get; set; }
        public BillingDetailsCustomerDto? Customer { get; set; }
        public BillingDetailsCheckInOutDto? CheckInOut { get; set; }
        public BillingDetailsAccommodationDto? Accommodation { get; set; }
        public List<BillingDetailsServiceChargeDto>? ServiceCharges { get; set; }
        public BillingDetailsServiceChargeSummaryDto? ServiceChargeSummary { get; set; }
        public BillingDetailsSummaryDto? Summary { get; set; }
    }
}

