namespace QueensHotelAPI.DTOs
{
    public class UpdateCustomerResponseDto
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int CustomerId { get; set; }
    }
}