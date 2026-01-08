namespace QueensHotelAPI.DTOs
{
    public class InsertRoomRequestDto
    {
        public string RoomNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int CompanyMaster_Id { get; set; }
        public int EnumRoomId { get; set; }
        public int FloorId { get; set; }
        public int RoomType_Id { get; set; }
    }
}