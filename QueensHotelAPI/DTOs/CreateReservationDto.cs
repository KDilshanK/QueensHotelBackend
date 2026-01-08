using System.ComponentModel.DataAnnotations;

namespace QueensHotelAPI.DTOs
{
    /// <summary>
    /// DTO for creating new reservations
    /// Author: dilshan-jolanka
    /// Updated date: 2025-08-28 01:45:00
    /// Note: Added Status parameter to match updated stored procedure
    /// </summary>
    public class CreateReservationDto
    {
        [Required(ErrorMessage = "Check-in date is required")]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        public DateTime CheckOutDate { get; set; }

        [Required(ErrorMessage = "Number of guests is required")]
        [Range(1, 50, ErrorMessage = "Number of guests must be between 1 and 50")]
        public int NumberOfGuests { get; set; }

        [Required(ErrorMessage = "Payment method ID is required")]
        public int PaymentMethodI_Id { get; set; }

        [Required(ErrorMessage = "CreateBy field is required for audit tracking")]
        [MaxLength(45, ErrorMessage = "CreateBy cannot exceed 45 characters")]
        public string CreateBy { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer ID is required")]
        public int Customer_Id { get; set; }

        /// <summary>
        /// Status of the reservation (e.g., "Pending", "Confirmed", "Draft")
        /// Max length: 10 characters as per stored procedure parameter definition
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(10, ErrorMessage = "Status cannot exceed 10 characters")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Meal Plan ID - Optional. If set to 0 or null, will be converted to NULL in stored procedure.
        /// </summary>
        public int? MealPlan_id { get; set; }

        /// <summary>
        /// Suite ID - Optional. If set to 0 or null, will be converted to NULL in stored procedure.
        /// </summary>
        public int? Suite_id { get; set; }

        /// <summary>
        /// Room ID - Optional. If set to 0 or null, will be converted to NULL in stored procedure.
        /// </summary>
        public int? Room_ID { get; set; }

        /// <summary>
        /// Travel Agency ID - Optional. If set to 0 or null, will be converted to NULL in stored procedure.
        /// </summary>
        public int? TravalAgency_Id { get; set; }
        // PaymentCardDetails_Id removed - no longer needed for reservation creation
    }
}