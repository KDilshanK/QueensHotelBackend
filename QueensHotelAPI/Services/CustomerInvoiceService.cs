using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Service implementation for Customer Invoice business logic
    /// </summary>
    public class CustomerInvoiceService : ICustomerInvoiceService
    {
        private readonly ICustomerInvoiceRepository _repository;
        private readonly ILogger<CustomerInvoiceService> _logger;

        public CustomerInvoiceService(ICustomerInvoiceRepository repository, ILogger<CustomerInvoiceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<GenerateCustomerInvoiceResponseDto> GenerateCustomerInvoiceAsync(int billingId)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing customer invoice generation request for BillingId: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                // Validate billing ID
                if (billingId <= 0)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid billing ID provided: {BillingId}", billingId);
                    
                    // Return empty response with error indication
                    var errorResponse = new GenerateCustomerInvoiceResponseDto();
                    errorResponse.InvoiceHeader.InvoiceNumber = "ERROR";
                    return errorResponse;
                }

                // Generate the invoice
                var result = await _repository.GenerateCustomerInvoiceAsync(billingId);

                // Check if the stored procedure returned an error (based on your SP logic)
                if (result.InvoiceHeader.InvoiceNumber == "ERROR" || string.IsNullOrEmpty(result.InvoiceHeader.InvoiceNumber))
                {
                    _logger.LogWarning("Queens Hotel API: Invoice generation failed for BillingId: {BillingId} - Invalid Billing ID or no data found", billingId);
                    return result; // Return the error response from SP
                }

                _logger.LogInformation("Queens Hotel API: Successfully processed invoice generation for BillingId: {BillingId}, Invoice: {InvoiceNumber} at {Timestamp}",
                    billingId, result.InvoiceHeader.InvoiceNumber, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return result;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("connection"))
            {
                _logger.LogError(ex, "Queens Hotel API: Database connection error during invoice generation for BillingId: {BillingId}",
                    billingId);

                // Return error response
                var errorResponse = new GenerateCustomerInvoiceResponseDto();
                errorResponse.InvoiceHeader.InvoiceNumber = "CONNECTION_ERROR";
                return errorResponse;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("retry"))
            {
                _logger.LogError(ex, "Queens Hotel API: Database retry limit exceeded during invoice generation for BillingId: {BillingId}",
                    billingId);

                // Return error response
                var errorResponse = new GenerateCustomerInvoiceResponseDto();
                errorResponse.InvoiceHeader.InvoiceNumber = "RETRY_ERROR";
                return errorResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during invoice generation for BillingId: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                // Return error response
                var errorResponse = new GenerateCustomerInvoiceResponseDto();
                errorResponse.InvoiceHeader.InvoiceNumber = "SYSTEM_ERROR";
                return errorResponse;
            }
        }
    }
}