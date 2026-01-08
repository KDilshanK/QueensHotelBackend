using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Service implementation for Billing business logic
    /// Author: dilshan-jolanka
    /// Create date: 2025-08-27 14:50:00
    /// </summary>
    public class BillingService : IBillingService
    {
        private readonly IBillingRepository _repository;
        private readonly ILogger<BillingService> _logger;

        public BillingService(IBillingRepository repository, ILogger<BillingService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<InsertBillingResponseDto> InsertBillingAsync(InsertBillingRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing billing insertion request for Reservation ID: {ReservationId} at {Timestamp} by user: {User}",
                    dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreatedBy);

                // Business validation
                if (dto.ReservationId <= 0)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid reservation ID provided: {ReservationId}", dto.ReservationId);
                    
                    return new InsertBillingResponseDto
                    {
                        Success = false,
                        Message = "Invalid reservation ID provided. Reservation ID must be greater than 0.",
                        BillingId = null,
                        CreatedDateTime = DateTime.Now
                    };
                }

                if (dto.TotalAmount < 0)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid total amount provided: {TotalAmount}", dto.TotalAmount);
                    
                    return new InsertBillingResponseDto
                    {
                        Success = false,
                        Message = "Total amount cannot be negative.",
                        BillingId = null,
                        CreatedDateTime = DateTime.Now
                    };
                }

                if (dto.BillingDate > DateTime.Today.AddDays(1))
                {
                    _logger.LogWarning("Queens Hotel API: Future billing date provided: {BillingDate}", dto.BillingDate);
                    
                    return new InsertBillingResponseDto
                    {
                        Success = false,
                        Message = "Billing date cannot be more than one day in the future.",
                        BillingId = null,
                        CreatedDateTime = DateTime.Now
                    };
                }

                if (string.IsNullOrWhiteSpace(dto.PaymentStatus))
                {
                    _logger.LogWarning("Queens Hotel API: Empty payment status provided for Reservation ID: {ReservationId}", dto.ReservationId);
                    
                    return new InsertBillingResponseDto
                    {
                        Success = false,
                        Message = "Payment status is required.",
                        BillingId = null,
                        CreatedDateTime = DateTime.Now
                    };
                }

                if (string.IsNullOrWhiteSpace(dto.CreatedBy))
                {
                    _logger.LogWarning("Queens Hotel API: Empty CreatedBy provided for Reservation ID: {ReservationId}", dto.ReservationId);
                    
                    return new InsertBillingResponseDto
                    {
                        Success = false,
                        Message = "CreatedBy field is required for audit tracking.",
                        BillingId = null,
                        CreatedDateTime = DateTime.Now
                    };
                }

                // Validate payment method (assuming common payment method IDs)
                if (dto.PaymentMethod < 1 || dto.PaymentMethod > 10)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid payment method provided: {PaymentMethod}", dto.PaymentMethod);
                    
                    return new InsertBillingResponseDto
                    {
                        Success = false,
                        Message = "Invalid payment method. Payment method must be between 1 and 10.",
                        BillingId = null,
                        CreatedDateTime = DateTime.Now
                    };
                }

                // Process the billing insertion
                var result = await _repository.InsertBillingAsync(dto);

                if (result.Success)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully processed billing insertion for Reservation ID: {ReservationId}, Billing ID: {BillingId} at {Timestamp} by user: {User}",
                        dto.ReservationId, result.BillingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreatedBy);
                }
                else
                {
                    _logger.LogWarning("Queens Hotel API: Failed to process billing insertion for Reservation ID: {ReservationId} at {Timestamp} by user: {User}. Reason: {Message}",
                        dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreatedBy, result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during billing insertion for Reservation ID: {ReservationId} at {Timestamp} by user: {User}",
                    dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreatedBy);

                return new InsertBillingResponseDto
                {
                    Success = false,
                    Message = $"An error occurred while processing the billing insertion: {ex.Message}",
                    BillingId = null,
                    CreatedDateTime = DateTime.Now
                };
            }
        }

        public async Task<GetBillingInfoResponseDto?> GetBillingInfoAsync(int billingId)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing GetBillingInfo request for Billing ID: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                // Business validation
                if (billingId < 0)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid billing ID provided: {BillingId}", billingId);
                    throw new ArgumentException($"Invalid billing ID provided. Billing ID must be greater than 0.", nameof(billingId));
                }

                // Retrieve billing information
                var result = await _repository.GetBillingInfoAsync(billingId);

                if (result != null)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully retrieved billing info for Billing ID: {BillingId} at {Timestamp}",
                        billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    _logger.LogInformation("Queens Hotel API: No billing information found for Billing ID: {BillingId} at {Timestamp}",
                        billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                return result;
            }
            catch (ArgumentException)
            {
                // Re-throw argument exceptions as they are validation errors
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred while retrieving billing info for Billing ID: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                throw new InvalidOperationException($"An error occurred while retrieving billing information for Billing ID {billingId}: {ex.Message}", ex);
            }
        }

        public async Task<List<GetBillingInfoResponseDto>> GetBillingInfoListAsync(int billingId)
        {
            _logger.LogInformation("Queens Hotel API: Processing GetBillingInfoList request for Billing ID: {BillingId} at {Timestamp}",
                billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            return await _repository.GetBillingInfoListAsync(billingId);
        }
    }
}