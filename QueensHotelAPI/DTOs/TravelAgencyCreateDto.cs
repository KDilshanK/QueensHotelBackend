using System;
namespace QueensHotelAPI.DTOs
{
    public class TravelAgencyCreateDto
    {
        public int Id { get; set; }
        public string AgencyName { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public double DiscountRate { get; set; }
        public string WebpageUrl { get; set; }
        public int CompanyMaster_Id { get; set; }
        public string? Password { get; set; }
    }
}

