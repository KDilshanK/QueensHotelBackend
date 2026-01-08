namespace QueensHotelAPI.DTOs
{
    public class InsertCheckinRequestDto
    {
        public DateTime CheckInTime { get; set; }
        public int Reservation_ID { get; set; }
        public int User_Id { get; set; }
        public int? Room_ID { get; set; }
        public int? Suite_id { get; set; }
    }
}