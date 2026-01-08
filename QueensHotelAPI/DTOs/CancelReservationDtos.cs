namespace QueensHotelAPI.DTOs
{
    /// <summary>
    /// Request DTO for canceling a reservation
    /// </summary>
    public class CancelReservationRequestDto
    {
        /// <summary>
        /// The ID of the reservation to cancel
        /// </summary>
        public int ReservationID { get; set; }

        /// <summary>
        /// The user who is canceling the reservation
        /// </summary>
        public string CancelledBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO for reservation cancellation result
    /// </summary>
    public class CancelReservationResponseDto
    {
        /// <summary>
        /// Indicates if the cancellation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message about the cancellation result
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the canceled reservation
        /// </summary>
        public int ReservationID { get; set; }

        /// <summary>
        /// Date and time when the reservation was cancelled
        /// </summary>
        public DateTime? CancelledDateTime { get; set; }

        /// <summary>
        /// User who cancelled the reservation
        /// </summary>
        public string CancelledBy { get; set; } = string.Empty;
    }
}