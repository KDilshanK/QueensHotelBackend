using System;
namespace QueensHotelAPI.DTOs
{
    public class BillingDetailsCustomerDto
    {
        public int CustomerID { get; set; }
        public string? FirstName { get; set; }
        public string? LName { get; set; }
        public string? CustomerName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}

