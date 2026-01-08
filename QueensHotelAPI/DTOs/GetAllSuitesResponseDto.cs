namespace QueensHotelAPI.DTOs
{
    /// <summary>
    /// Response DTO for GetAllSuites endpoint
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-28 00:15:00
    /// Description: Contains all suite information returned from GetAllSuites stored procedure
    /// </summary>
    public class GetAllSuitesResponseDto
    {
        public int Id { get; set; }
        public string SuiteName { get; set; } = string.Empty;
        public decimal WeeklyRate { get; set; }
        public decimal MonthlyRate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public string Living_area { get; set; } = string.Empty;
        public string Kitchen { get; set; } = string.Empty;
        public string Dining_area { get; set; } = string.Empty;
        public int Bathrooms { get; set; }
        public string Laundry_facilities { get; set; } = string.Empty;
        public string Workspace { get; set; } = string.Empty;
        public string Wifi_entertainment { get; set; } = string.Empty;
        public string Balcony_terrace { get; set; } = string.Empty;
        public string Housekeeping { get; set; } = string.Empty;
        public string Security { get; set; } = string.Empty;
        public int? CompanyMaster_Id { get; set; }
    }
}