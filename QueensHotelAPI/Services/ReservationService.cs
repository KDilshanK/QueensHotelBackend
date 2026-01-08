using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repository;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(IReservationRepository repository, ILogger<ReservationService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ReservationDetailsResponse>> GetReservationDataAsync(GetReservationDataRequest request)
        {
            _logger.LogInformation("Queens Hotel API: Processing reservation data request at {Timestamp} by user: {User}",
                "2025-05-29 05:30:54", "dilshan-jolanka");

            var results = await _repository.GetReservationDataAsync(
                request.NIC,
                request.Fname,
                request.Lname,
                request.Email,
                request.Number,
                request.Id // Add this if your repository expects Id filter
            );

            return results.Select(r => new ReservationDetailsResponse
            {
                ReservationId = r.ReservationID,
                CheckInDate = r.ReservationCheckInDate,
                CheckOutDate = r.ReservationCheckOutDate,
                NumberOfGuests = r.GuestCount,
                Status = r.ReservationStatus,
                CreatedDateTime = r.ReservationCreatedDate,
                CreditCardProvided = r.PaymentCardDetails.HasValue,

                // Payment information (new fields from updated SP)
                Payment = new PaymentInfo
                {
                    PaymentMethodId = r.PaymentMethodId,
                    PaymentMethodType = r.PaymentMethodType ?? string.Empty,
                    PaymentCardDetailsId = r.PaymentCardDetails,
                    GuestStatus = r.GuestStatus ?? string.Empty,
                    PaymentCard = r.PaymentCardId.HasValue ? new PaymentCardInfo
                    {
                        CardId = r.PaymentCardId,
                        CardHolderName = r.CardHolderName ?? string.Empty,
                        ExpiryMonth = r.ExpiryMonth,
                        ExpiryYear = r.ExpiryYear,
                        MaskedCardNumber = !string.IsNullOrEmpty(r.CardHolderName) ? "**** **** **** ****" : string.Empty
                    } : null
                },

                Customer = new CustomerInfo
                {
                    Id = r.CustomerID,
                    NIC = r.CustomerNIC,
                    FirstName = r.CustomerFirstName,
                    LastName = r.CustomerLastName,
                    Email = r.CustomerEmail,
                    Phone = r.CustomerPhone,
                    Address = r.CustomerAddress,
                    PassportId = r.CustomerPassportID,
                    DateOfBirth = r.CustomerDateOfBirth,
                    Gender = r.CustomerGender
                },

                MealPlan = r.MealPlanID.HasValue ? new MealPlanInfo
                {
                    Id = r.MealPlanID.Value,
                    Description = r.Description ?? string.Empty,
                    Code = r.MealPlanCode ?? string.Empty,
                    CostPerNight = r.CostPerNight ?? 0,
                    Status = r.MealPlanStatus ?? string.Empty,
                    IsFree = r.IsFree ?? false
                } : null,

                Suite = r.SuiteID.HasValue ? new SuiteInfo
                {
                    Id = r.SuiteID.Value,
                    Name = r.SuiteName ?? string.Empty,
                    Type = r.SuiteType ?? string.Empty,
                    Size = r.SuiteSize ?? string.Empty,
                    Bedrooms = r.SuiteBedrooms ?? 0,
                    Bathrooms = r.SuiteBathrooms ?? 0,
                    Description = r.SuiteDescription ?? string.Empty,
                    WeeklyRate = r.SuiteWeeklyRate ?? 0,
                    MonthlyRate = r.SuiteMonthlyRate ?? 0
                } : null,

                Room = r.RoomID.HasValue ? new RoomInfo
                {
                    Id = r.RoomID.Value,
                    RoomNumber = r.RoomNumber ?? string.Empty,
                    Type = r.RoomTypeName ?? string.Empty,
                    RatePerNight = r.RoomRatePerNight ?? 0,
                    IsAcAvailable = r.HasAirConditioning ?? false,
                    Capacity = r.RoomCapacity ?? 0,
                    StatusId = r.RoomStatusID ?? 0,
                    StatusName = r.RoomStatusName ?? string.Empty,
                    FloorId = r.FloorID ?? 0,
                    FloorNumber = r.FloorNumber ?? 0,
                    FloorDescription = r.FloorDescription ?? string.Empty
                } : null,

                TravelAgency = r.TravelAgencyID.HasValue ? new TravelAgencyInfo
                {
                    Id = r.TravelAgencyID.Value,
                    Name = r.TravelAgencyName ?? string.Empty,
                    Address = r.Address ?? string.Empty,
                    ContactPerson = r.ContactPerson ?? string.Empty,
                    DiscountRate = r.DiscountRate ?? 0,
                    Email = r.Email ?? string.Empty,
                    Phone = r.Phone ?? string.Empty,
                    WebpageUrl = r.WebpageUrl ?? string.Empty
                } : null
            });
        }

        public async Task<CreateReservationResponseDto> InsertReservationAsync(CreateReservationDto dto)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing reservation creation request at {Timestamp} by user: {CreateBy}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreateBy);

                var reservationId = await _repository.InsertReservationAsync(dto);

                if (reservationId > 0)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully created reservation with ID {ReservationId} and Status {Status} at {Timestamp} by user: {CreateBy}",
                        reservationId, dto.Status, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreateBy);

                    return new CreateReservationResponseDto
                    {
                        Success = true,
                        ReservationId = reservationId,
                        Message = "Reservation created successfully",
                        CreatedDateTime = DateTime.UtcNow,
                        CreatedBy = dto.CreateBy,
                        CheckInDate = dto.CheckInDate,
                        CheckOutDate = dto.CheckOutDate,
                        NumberOfGuests = dto.NumberOfGuests,
                        CustomerId = dto.Customer_Id,
                        Status = dto.Status
                    };
                }
                else
                {
                    _logger.LogWarning("Queens Hotel API: Failed to create reservation - no ID returned at {Timestamp} by user: {CreateBy}",
                        DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreateBy);

                    return new CreateReservationResponseDto
                    {
                        Success = false,
                        ReservationId = null,
                        Message = "Reservation creation failed - no reservation ID returned",
                        CreatedDateTime = DateTime.UtcNow,
                        CreatedBy = dto.CreateBy,
                        CheckInDate = dto.CheckInDate,
                        CheckOutDate = dto.CheckOutDate,
                        NumberOfGuests = dto.NumberOfGuests,
                        CustomerId = dto.Customer_Id,
                        Status = dto.Status
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during reservation creation at {Timestamp} by user: {CreateBy}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreateBy);

                return new CreateReservationResponseDto
                {
                    Success = false,
                    ReservationId = null,
                    Message = $"Error occurred while creating reservation: {ex.Message}",
                    CreatedDateTime = DateTime.UtcNow,
                    CreatedBy = dto.CreateBy,
                    CheckInDate = dto.CheckInDate,
                    CheckOutDate = dto.CheckOutDate,
                    NumberOfGuests = dto.NumberOfGuests,
                    CustomerId = dto.Customer_Id,
                    Status = dto.Status
                };
            }
        }

        public async Task<bool> UpdateReservationAsync(UpdateReservationDto dto)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing reservation update request for ReservationID: {ReservationId} at {Timestamp} by user: {UserId}",
                    dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.UserId);

                // Business validation
                if (dto.ReservationId <= 0)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid reservation ID provided: {ReservationId}", dto.ReservationId);
                    throw new ArgumentException($"Invalid reservation ID: {dto.ReservationId}. Reservation ID must be greater than 0.");
                }

                if (dto.CheckInDate >= dto.CheckOutDate)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid date range - CheckIn: {CheckIn}, CheckOut: {CheckOut} for reservation {ReservationId}",
                        dto.CheckInDate, dto.CheckOutDate, dto.ReservationId);
                    throw new ArgumentException("Check-in date must be before check-out date.");
                }

                // Log the zero-to-null conversion that will happen
                var fieldsToBeNull = new List<string>();
                if (dto.MealPlan_id == 0) fieldsToBeNull.Add("MealPlan_id");
                if (dto.Suite_id == 0) fieldsToBeNull.Add("Suite_id");
                if (dto.Room_ID == 0) fieldsToBeNull.Add("Room_ID");
                if (dto.TravalAgency_Id == 0) fieldsToBeNull.Add("TravalAgency_Id");

                if (fieldsToBeNull.Any())
                {
                    _logger.LogInformation("Queens Hotel API: Converting zero values to NULL for fields: {Fields} in reservation {ReservationId}",
                        string.Join(", ", fieldsToBeNull), dto.ReservationId);
                }

                // Call repository to update the reservation
                var result = await _repository.UpdateReservationAsync(dto);

                if (result)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully updated reservation {ReservationId} at {Timestamp} by user: {UserId}",
                        dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.UserId);
                }
                else
                {
                    _logger.LogWarning("Queens Hotel API: Failed to update reservation {ReservationId} at {Timestamp} by user: {UserId}",
                        dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.UserId);
                }

                return result;
            }
            catch (ArgumentException)
            {
                // Re-throw argument exceptions as they should be handled by the controller
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred while updating reservation {ReservationId} at {Timestamp}",
                    dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                throw;
            }
        }

        public async Task<bool> ReservationExistsAsync(int reservationId)
        {
            return await _repository.ReservationExistsAsync(reservationId);
        }

        public async Task<CancelReservationResponseDto> CancelReservationAsync(CancelReservationRequestDto request)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing reservation cancellation request for ReservationID: {ReservationId} at {Timestamp} by user: {User}",
                    request.ReservationID, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), request.CancelledBy);

                // Check if reservation exists first (with shorter timeout)
                bool exists;
                try
                {
                    exists = await _repository.ReservationExistsAsync(request.ReservationID);
                }
                catch (Exception ex) when (ex.Message.Contains("connection") || ex.Message.Contains("timeout"))
                {
                    _logger.LogWarning(ex, "Queens Hotel API: Database connectivity issue during existence check for reservation {ReservationId}", request.ReservationID);
                    
                    return new CancelReservationResponseDto
                    {
                        Success = false,
                        Message = $"Unable to connect to database to verify reservation {request.ReservationID}. Please check your internet connection and try again later.",
                        ReservationID = request.ReservationID,
                        CancelledBy = request.CancelledBy,
                        CancelledDateTime = DateTime.UtcNow
                    };
                }

                if (!exists)
                {
                    return new CancelReservationResponseDto
                    {
                        Success = false,
                        Message = $"Reservation with ID {request.ReservationID} not found",
                        ReservationID = request.ReservationID,
                        CancelledBy = request.CancelledBy,
                        CancelledDateTime = DateTime.UtcNow
                    };
                }

                // Attempt to cancel the reservation
                var result = await _repository.CancelReservationAsync(request.ReservationID, request.CancelledBy);

                if (result)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully processed cancellation for reservation {ReservationId} at {Timestamp} by user: {User}",
                        request.ReservationID, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), request.CancelledBy);

                    return new CancelReservationResponseDto
                    {
                        Success = true,
                        Message = $"Reservation {request.ReservationID} has been successfully cancelled",
                        ReservationID = request.ReservationID,
                        CancelledBy = request.CancelledBy,
                        CancelledDateTime = DateTime.UtcNow
                    };
                }
                else
                {
                    return new CancelReservationResponseDto
                    {
                        Success = false,
                        Message = $"Failed to cancel reservation {request.ReservationID}",
                        ReservationID = request.ReservationID,
                        CancelledBy = request.CancelledBy,
                        CancelledDateTime = DateTime.UtcNow
                    };
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("connection"))
            {
                _logger.LogError(ex, "Queens Hotel API: Database connection error during reservation cancellation for ReservationID: {ReservationId}",
                    request.ReservationID);

                return new CancelReservationResponseDto
                {
                    Success = false,
                    Message = $"Database connection issue occurred while cancelling reservation {request.ReservationID}. Please ensure you have internet connectivity and try again.",
                    ReservationID = request.ReservationID,
                    CancelledBy = request.CancelledBy,
                    CancelledDateTime = DateTime.UtcNow
                };
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("retry"))
            {
                _logger.LogError(ex, "Queens Hotel API: Database retry limit exceeded during reservation cancellation for ReservationID: {ReservationId}",
                    request.ReservationID);

                return new CancelReservationResponseDto
                {
                    Success = false,
                    Message = $"The database service is temporarily unavailable. The cancellation for reservation {request.ReservationID} could not be completed at this time. Please try again in a few minutes.",
                    ReservationID = request.ReservationID,
                    CancelledBy = request.CancelledBy,
                    CancelledDateTime = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during reservation cancellation for ReservationID: {ReservationId} at {Timestamp}",
                    request.ReservationID, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return new CancelReservationResponseDto
                {
                    Success = false,
                    Message = $"An error occurred while cancelling reservation {request.ReservationID}. Please contact support if the problem persists.",
                    ReservationID = request.ReservationID,
                    CancelledBy = request.CancelledBy,
                    CancelledDateTime = DateTime.UtcNow
                };
            }
        }
    }
}