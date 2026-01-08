using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Service implementation for Credit Card business logic
    /// </summary>
    public class CreditCardService : ICreditCardService
    {
        private readonly ICreditCardRepository _repository;
        private readonly ILogger<CreditCardService> _logger;

        public CreditCardService(ICreditCardRepository repository, ILogger<CreditCardService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<InsertCreditCardResponseDto> InsertCreditCardAsync(InsertCreditCardRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing credit card insertion request for Customer_Id: {CustomerId} at {Timestamp}",
                    dto.Customer_Id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                // Validate card number length and format
                if (string.IsNullOrWhiteSpace(dto.CardNumber) || dto.CardNumber.Length < 13 || dto.CardNumber.Length > 19)
                {
                    return new InsertCreditCardResponseDto
                    {
                        Success = false,
                        Message = "Invalid card number format. Card number must be between 13 and 19 digits.",
                        Customer_Id = dto.Customer_Id,
                        CreatedDateTime = DateTime.UtcNow
                    };
                }

                // Validate expiry date
                var currentDate = DateTime.Now;
                if (dto.ExpiryYear < currentDate.Year || 
                    (dto.ExpiryYear == currentDate.Year && dto.ExpiryMonth < currentDate.Month))
                {
                    return new InsertCreditCardResponseDto
                    {
                        Success = false,
                        Message = "Card expiry date cannot be in the past.",
                        Customer_Id = dto.Customer_Id,
                        CreatedDateTime = DateTime.UtcNow
                    };
                }

                // Validate CVV
                if (string.IsNullOrWhiteSpace(dto.CVV) || dto.CVV.Length < 3 || dto.CVV.Length > 4)
                {
                    return new InsertCreditCardResponseDto
                    {
                        Success = false,
                        Message = "Invalid CVV format. CVV must be 3 or 4 digits.",
                        Customer_Id = dto.Customer_Id,
                        CreatedDateTime = DateTime.UtcNow
                    };
                }

                // Validate cardholder name
                if (string.IsNullOrWhiteSpace(dto.CardHolderName) || dto.CardHolderName.Length < 2)
                {
                    return new InsertCreditCardResponseDto
                    {
                        Success = false,
                        Message = "Card holder name is required and must be at least 2 characters long.",
                        Customer_Id = dto.Customer_Id,
                        CreatedDateTime = DateTime.UtcNow
                    };
                }

                // Attempt to insert the credit card
                var result = await _repository.InsertCreditCardAsync(dto);

                if (result)
                {
                    // Create masked card number for response (show only last 4 digits)
                    var maskedCardNumber = "**** **** **** " + dto.CardNumber.Substring(dto.CardNumber.Length - 4);

                    _logger.LogInformation("Queens Hotel API: Successfully processed credit card insertion for Customer_Id: {CustomerId} at {Timestamp}",
                        dto.Customer_Id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    return new InsertCreditCardResponseDto
                    {
                        Success = true,
                        Message = $"Credit card has been successfully added for customer {dto.Customer_Id}",
                        Customer_Id = dto.Customer_Id,
                        MaskedCardNumber = maskedCardNumber,
                        CardHolderName = dto.CardHolderName,
                        CreatedDateTime = DateTime.UtcNow
                    };
                }
                else
                {
                    return new InsertCreditCardResponseDto
                    {
                        Success = false,
                        Message = $"Failed to add credit card for customer {dto.Customer_Id}",
                        Customer_Id = dto.Customer_Id,
                        CreatedDateTime = DateTime.UtcNow
                    };
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("connection"))
            {
                _logger.LogError(ex, "Queens Hotel API: Database connection error during credit card insertion for Customer_Id: {CustomerId}",
                    dto.Customer_Id);

                return new InsertCreditCardResponseDto
                {
                    Success = false,
                    Message = $"Database connection issue occurred while adding credit card for customer {dto.Customer_Id}. Please ensure you have internet connectivity and try again.",
                    Customer_Id = dto.Customer_Id,
                    CreatedDateTime = DateTime.UtcNow
                };
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("retry"))
            {
                _logger.LogError(ex, "Queens Hotel API: Database retry limit exceeded during credit card insertion for Customer_Id: {CustomerId}",
                    dto.Customer_Id);

                return new InsertCreditCardResponseDto
                {
                    Success = false,
                    Message = $"The database service is temporarily unavailable. The credit card insertion for customer {dto.Customer_Id} could not be completed at this time. Please try again in a few minutes.",
                    Customer_Id = dto.Customer_Id,
                    CreatedDateTime = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during credit card insertion for Customer_Id: {CustomerId} at {Timestamp}",
                    dto.Customer_Id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return new InsertCreditCardResponseDto
                {
                    Success = false,
                    Message = $"An error occurred while adding credit card for customer {dto.Customer_Id}. Please contact support if the problem persists.",
                    Customer_Id = dto.Customer_Id,
                    CreatedDateTime = DateTime.UtcNow
                };
            }
        }
    }
}