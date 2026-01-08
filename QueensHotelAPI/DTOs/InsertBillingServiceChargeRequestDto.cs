using System;
using System.ComponentModel.DataAnnotations;

namespace QueensHotelAPI.DTOs
{
    public class InsertBillingServiceChargeRequestDto
    {
        [Required(ErrorMessage = "Billing ID is required")]
        public int BillingId { get; set; }
        
        [Required(ErrorMessage = "Service Charge ID is required")]
        public int ServiceChargeId { get; set; }
    }
}

