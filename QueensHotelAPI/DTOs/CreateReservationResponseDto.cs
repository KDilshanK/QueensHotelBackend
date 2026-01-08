using System.ComponentModel.DataAnnotations;

namespace QueensHotelAPI.DTOs
{
    /// <summary>
    /// DTO for reservation creation response containing the new reservation ID
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-28 01:15:00
    /// Updated date: 2025-08-28 01:45:00 - Added Status field
    /// Description: Response DTO that includes the newly created reservation ID returned from the stored procedure
    /// </summary>
    public class CreateReservationResponseDto
    {
        /// <summary>
        /// Indicates whether the reservation was created successfully
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The ID of the newly created reservation (returned from stored procedure)
        /// </summary>
        public int? ReservationId { get; set; }

        /// <summary>
        /// Success or error message describing the result
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the reservation was created
        /// </summary>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// User who created the reservation (for audit tracking)
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Check-in date for the reservation
        /// </summary>
        public DateTime CheckInDate { get; set; }

        /// <summary>
        /// Check-out date for the reservation
        /// </summary>
        public DateTime CheckOutDate { get; set; }

        /// <summary>
        /// Number of guests for the reservation
        /// </summary>
        public int NumberOfGuests { get; set; }

        /// <summary>
        /// Customer ID associated with the reservation
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Status of the created reservation (e.g., "Pending", "Confirmed", "Draft")
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}