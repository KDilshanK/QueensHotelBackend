namespace QueensHotelAPI.DTOs
{
    /// <summary>
    /// Request DTO for inserting credit card information securely
    /// </summary>
    public class InsertCreditCardRequestDto
    {
        /// <summary>
        /// Name of the card holder as it appears on the card
        /// </summary>
        public string CardHolderName { get; set; } = string.Empty;

        /// <summary>
        /// Credit card number (will be encrypted in database)
        /// </summary>
        public string CardNumber { get; set; } = string.Empty;

        /// <summary>
        /// Type of the credit card (Visa, MasterCard, etc.)
        /// </summary>
        public string CardType { get; set; } = string.Empty;

        /// <summary>
        /// Expiry month of the card (1-12)
        /// </summary>
        public int ExpiryMonth { get; set; }

        /// <summary>
        /// Expiry year of the card (YYYY format)
        /// </summary>
        public int ExpiryYear { get; set; }

        /// <summary>
        /// Card verification value (will be encrypted in database)
        /// </summary>
        public string CVV { get; set; } = string.Empty;

        /// <summary>
        /// Whether this card should be set as default for the customer
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Status of the card (active/inactive)
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// ID of the customer who owns this card
        /// </summary>
        public int Customer_Id { get; set; }

        /// <summary>
        /// ID of the card type reference
        /// </summary>
        public int CardType_id { get; set; }

        /// <summary>
        /// ID of the company master record
        /// </summary>
        public int CompanyMaster_Id { get; set; } = 1;
    }

    /// <summary>
    /// Response DTO for credit card insertion result
    /// </summary>
    public class InsertCreditCardResponseDto
    {
        /// <summary>
        /// Indicates if the insertion was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message about the insertion result
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// ID of the customer for whom the card was added
        /// </summary>
        public int Customer_Id { get; set; }

        /// <summary>
        /// Masked card number for confirmation (last 4 digits only)
        /// </summary>
        public string MaskedCardNumber { get; set; } = string.Empty;

        /// <summary>
        /// Card holder name for confirmation
        /// </summary>
        public string CardHolderName { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the card was added
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
    }
}