namespace QueensHotelAPI.DTOs
{
    public class UpdateCustomerRequestDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? Status { get; set; }
        public int? CompanyMaster_Id { get; set; }
        public string? PassportId { get; set; }
        public string? NIC { get; set; }
        public int? CountryOfResidence_id { get; set; }
        public string? DOB { get; set; }
        public string? Gender { get; set; }
    }
}