using System;
namespace QueensHotelAPI.DTOs
{
    public class CustomerLoginRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}