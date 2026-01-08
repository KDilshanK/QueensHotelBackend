using System.ComponentModel.DataAnnotations;

namespace QueensHotelAPI.DTOs
{
    /// <summary>
    /// DTO for updating reservation information
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-27 13:55:00
    /// Updated date: 2025-08-27 20:15:00
    /// 
    /// Note: For optional foreign key fields (MealPlan_id, Suite_id, Room_ID, TravalAgency_Id, PaymentCardDetails_Id),
    /// if the value is 0, it will be converted to NULL when passed to the stored procedure.
    /// This allows the frontend to send 0 to indicate "no selection" or "remove association".
    /// </summary>
    public class UpdateReservationDto
    {
        [Required(ErrorMessage = "Reservation ID is required")]
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Check-in date is required")]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        public DateTime CheckOutDate { get; set; }

        [Required(ErrorMessage = "Number of guests is required")]
        [Range(1, 50, ErrorMessage = "Number of guests must be between 1 and 50")]
        public int NumberOfGuests { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(45, ErrorMessage = "Status cannot exceed 45 characters")]
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment method ID is required")]
        public int PaymentMethodI_Id { get; set; }

        /// <summary>
        /// Meal Plan ID - Optional. If set to 0, will be converted to NULL in stored procedure.
        /// </summary>
        public int? MealPlan_id { get; set; }

        /// <summary>
        /// Suite ID - Optional. If set to 0, will be converted to NULL in stored procedure.
        /// </summary>
        public int? Suite_id { get; set; }

        /// <summary>
        /// Room ID - Optional. If set to 0, will be converted to NULL in stored procedure.
        /// </summary>
        public int? Room_ID { get; set; }

        /// <summary>
        /// Travel Agency ID - Optional. If set to 0, will be converted to NULL in stored procedure.
        /// </summary>
        public int? TravalAgency_Id { get; set; }

        /// <summary>
        /// Payment Card Details ID - Optional. If set to 0, will be converted to NULL in stored procedure.
        /// </summary>
        //public int? PaymentCardDetails_Id { get; set; }

        [Required(ErrorMessage = "User ID is required for audit tracking")]
        [MaxLength(45, ErrorMessage = "User ID cannot exceed 45 characters")]
        public string UserId { get; set; } = string.Empty;
    }
}