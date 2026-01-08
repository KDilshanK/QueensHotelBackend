namespace QueensHotelAPI.DTOs
{
    public class UserLoginResponseDto
    {
        public string LoginStatus { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserType { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public DateTime? LastLoginTime { get; set; }
    }
}