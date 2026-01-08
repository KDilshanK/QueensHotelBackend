namespace QueensHotelAPI.DTOs
{
    /// <summary>
    /// Request DTO for generating customer invoice
    /// </summary>
    public class GenerateCustomerInvoiceRequestDto
    {
        /// <summary>
        /// The billing ID to generate invoice for
        /// </summary>
        public int BillingId { get; set; }
    }

    /// <summary>
    /// Complete response DTO containing all invoice information
    /// </summary>
    public class GenerateCustomerInvoiceResponseDto
    {
        /// <summary>
        /// Invoice header information
        /// </summary>
        public InvoiceHeaderDto InvoiceHeader { get; set; } = new();

        /// <summary>
        /// Customer information
        /// </summary>
        public InvoiceCustomerDto Customer { get; set; } = new();

        /// <summary>
        /// Stay information
        /// </summary>
        public InvoiceStayDto StayInformation { get; set; } = new();

        /// <summary>
        /// Charges breakdown
        /// </summary>
        public List<InvoiceChargeDto> ChargesBreakdown { get; set; } = new();

        /// <summary>
        /// Detailed service charges
        /// </summary>
        public List<InvoiceServiceChargeDto> ServiceCharges { get; set; } = new();

        /// <summary>
        /// Payment information
        /// </summary>
        public List<InvoicePaymentDto> Payments { get; set; } = new();

        /// <summary>
        /// Service charges grouped by type
        /// </summary>
        public List<InvoiceServiceChargeGroupDto> ServiceChargesByType { get; set; } = new();

        /// <summary>
        /// Service charges by date
        /// </summary>
        public List<InvoiceServiceChargeByDateDto> ServiceChargesByDate { get; set; } = new();

        /// <summary>
        /// Overall service charge statistics
        /// </summary>
        public InvoiceServiceChargeStatsDto ServiceChargeStatistics { get; set; } = new();
    }

    /// <summary>
    /// Invoice header information
    /// </summary>
    public class InvoiceHeaderDto
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public string InvoiceDate { get; set; } = string.Empty;
        public int BillingID { get; set; }
        public int ReservationID { get; set; }
        public decimal BillingAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string BillingDate { get; set; } = string.Empty;
    }

    /// <summary>
    /// Customer information for invoice
    /// </summary>
    public class InvoiceCustomerDto
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    /// <summary>
    /// Stay information for invoice
    /// </summary>
    public class InvoiceStayDto
    {
        public string Accommodation { get; set; } = string.Empty;
        public string CheckInDateTime { get; set; } = string.Empty;
        public string CheckOutDateTime { get; set; } = string.Empty;
        public int NightsStayed { get; set; }
        public string LateCheckout { get; set; } = string.Empty;
    }

    /// <summary>
    /// Charge breakdown item
    /// </summary>
    public class InvoiceChargeDto
    {
        public string ChargeType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// Service charge detail for invoice
    /// </summary>
    public class InvoiceServiceChargeDto
    {
        public int ServiceChargeID { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ServiceDate { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string IsFree { get; set; } = string.Empty;
    }

    /// <summary>
    /// Payment information for invoice
    /// </summary>
    public class InvoicePaymentDto
    {
        public int PaymentID { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentDate { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }

    /// <summary>
    /// Service charges grouped by type
    /// </summary>
    public class InvoiceServiceChargeGroupDto
    {
        public string ServiceType { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
        public int FreeItems { get; set; }
        public decimal PercentageOfServiceCharges { get; set; }
    }

    /// <summary>
    /// Service charges by date
    /// </summary>
    public class InvoiceServiceChargeByDateDto
    {
        public string ServiceDay { get; set; } = string.Empty;
        public int ServiceCount { get; set; }
        public decimal DailyTotal { get; set; }
        public string ServicesUsed { get; set; } = string.Empty;
    }

    /// <summary>
    /// Service charge statistics
    /// </summary>
    public class InvoiceServiceChargeStatsDto
    {
        public int TotalServiceItems { get; set; }
        public int PaidServiceItems { get; set; }
        public int FreeServiceItems { get; set; }
        public decimal TotalServiceCharges { get; set; }
        public decimal AverageChargePerPaidItem { get; set; }
        public decimal HighestServiceCharge { get; set; }
        public decimal LowestServiceCharge { get; set; }
        public decimal PercentageOfBill { get; set; }
    }
}