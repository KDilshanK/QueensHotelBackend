namespace QueensHotelAPI.DTOs
{
    public class InsertSuiteRequestDto
    {
        public string SuiteName { get; set; } = string.Empty;
        public decimal WeeklyRate { get; set; }
        public decimal MonthlyRate { get; set; }
        public int Status { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public string LivingArea { get; set; } = string.Empty;
        public string Kitchen { get; set; } = string.Empty;
        public string DiningArea { get; set; } = string.Empty;
        public int Bathrooms { get; set; }
        public string LaundryFacilities { get; set; } = string.Empty;
        public string Workspace { get; set; } = string.Empty;
        public string WifiEntertainment { get; set; } = string.Empty;
        public string BalconyTerrace { get; set; } = string.Empty;
        public string Housekeeping { get; set; } = string.Empty;
        public string Security { get; set; } = string.Empty;
        public int CompanyMaster_Id { get; set; }
    }
}