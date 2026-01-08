using System;
namespace QueensHotelAPI.DTOs
{
    public class CustomerLoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
    }
}