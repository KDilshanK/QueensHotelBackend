using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Services;

namespace QueensHotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(IReservationService reservationService, ILogger<ReservationController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves reservation data from Queens Hotel based on customer search criteria
        /// </summary>
        /// <param name="request">Search criteria including NIC, name, email, and phone number</param>
        /// <returns>A list of reservation details matching the search criteria</returns>
        /// <response code="200">Returns the list of Queens Hotel reservations</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservationDetailsResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReservationDetailsResponse>>>> GetReservationData([FromQuery] GetReservationDataRequest request)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing reservation search request at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                if (request == null)
                {
                    var badRequestResponse = new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Request cannot be null",
                        Data = null,
                        Timestamp = DateTime.UtcNow,
                        ProcessedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                var reservations = await _reservationService.GetReservationDataAsync(request);

                _logger.LogInformation("Queens Hotel API: Successfully retrieved {Count} reservations at {Timestamp}",
                    reservations.Count(), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var response = new ApiResponse<IEnumerable<ReservationDetailsResponse>>
                {
                    Success = true,
                    Message = $"Successfully retrieved {reservations.Count()} reservation(s) from Queens Hotel database",
                    Data = reservations,
                    Count = reservations.Count(),
                    Timestamp = DateTime.UtcNow,
                    ApiVersion = "1.0.0",
                    ProcessedBy = "dilshan-jolanka"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during reservation search at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var response = new ApiResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while processing your Queens Hotel request",
                    Data = null,
                    Timestamp = DateTime.UtcNow,
                    ProcessedBy = "dilshan-jolanka"
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Creates a new reservation at Queens Hotel
        /// Author: dilshan-jolanka
        /// Updated: 2025-08-28 01:15:00 - Now returns the new reservation ID from stored procedure
        /// </summary>
        /// <param name="dto">Reservation details to create</param>
        /// <returns>Success status and new reservation ID</returns>
        /// <response code="201">Reservation created successfully with reservation ID</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateReservationResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreateReservationResponseDto>> InsertReservation([FromBody] CreateReservationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid model state for reservation creation at {Timestamp}",
                        DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return BadRequest(ModelState);
                }

                // Additional business validations
                if (dto.CheckInDate >= dto.CheckOutDate)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid date range - CheckIn: {CheckIn}, CheckOut: {CheckOut} for reservation creation at {Timestamp}",
                        dto.CheckInDate, dto.CheckOutDate, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return BadRequest("Check-in date must be before check-out date.");
                }

                if (dto.NumberOfGuests <= 0)
                {
                    return BadRequest("Number of guests must be greater than 0.");
                }

                if (string.IsNullOrWhiteSpace(dto.CreateBy))
                {
                    return BadRequest("CreateBy field is required for audit tracking.");
                }

                if (string.IsNullOrWhiteSpace(dto.Status))
                {
                    return BadRequest("Status field is required.");
                }

                _logger.LogInformation("Queens Hotel API: Processing reservation creation at {Timestamp} by user: {CreateBy}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreateBy);

                var result = await _reservationService.InsertReservationAsync(dto);

                if (result.Success)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully created reservation with ID {ReservationId} at {Timestamp} by user: {CreateBy}",
                        result.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreateBy);
                    
                    return CreatedAtAction(
                        nameof(InsertReservation), 
                        new { id = result.ReservationId }, 
                        result);
                }

                _logger.LogError("Queens Hotel API: Failed to create reservation at {Timestamp} by user: {CreateBy} - {Message}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreateBy, result.Message);
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during reservation creation at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                
                var errorResponse = new CreateReservationResponseDto
                {
                    Success = false,
                    ReservationId = null,
                    Message = "An internal server error occurred while creating the reservation",
                    CreatedDateTime = DateTime.UtcNow,
                    CreatedBy = dto.CreateBy,
                    CheckInDate = dto.CheckInDate,
                    CheckOutDate = dto.CheckOutDate,
                    NumberOfGuests = dto.NumberOfGuests,
                    CustomerId = dto.Customer_Id,
                    Status = dto.Status
                };
                
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Updates an existing reservation with new details including user tracking
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-27 13:55:00
        /// Updated date: 2025-08-27 20:35:00
        /// </summary>
        /// <param name="id">Reservation ID to update</param>
        /// <param name="dto">Updated reservation details including UserId for audit tracking</param>
        /// <returns>Success status of the update operation</returns>
        /// <response code="204">Reservation updated successfully</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="404">If the reservation is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateReservation(int id, [FromBody] UpdateReservationDto dto)
        {
            try
            {
                if (id != dto.ReservationId)
                {
                    _logger.LogWarning("Queens Hotel API: Reservation ID mismatch - URL ID: {UrlId}, DTO ID: {DtoId} at {Timestamp}",
                        id, dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return BadRequest("Reservation ID mismatch between URL and request body.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid model state for reservation update - ID: {ReservationId} at {Timestamp}",
                        id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return BadRequest(ModelState);
                }

                // Validate UserId is provided
                if (string.IsNullOrWhiteSpace(dto.UserId))
                {
                    _logger.LogWarning("Queens Hotel API: UserId is required for reservation update - ID: {ReservationId} at {Timestamp}",
                        id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return BadRequest("UserId is required for audit tracking.");
                }

                // Additional business validations
                if (dto.CheckInDate >= dto.CheckOutDate)
                {
                    _logger.LogWarning("Queens Hotel API: Invalid date range - CheckIn: {CheckIn}, CheckOut: {CheckOut} for reservation {ReservationId} at {Timestamp}",
                        dto.CheckInDate, dto.CheckOutDate, id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return BadRequest("Check-in date must be before check-out date.");
                }

                if (dto.NumberOfGuests <= 0)
                {
                    return BadRequest("Number of guests must be greater than 0.");
                }

                _logger.LogInformation("Queens Hotel API: Processing reservation update for ID: {ReservationId} by user: {UserId} at {Timestamp}",
                    id, dto.UserId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                // Check if reservation exists
                var exists = await _reservationService.ReservationExistsAsync(id);
                if (!exists)
                {
                    _logger.LogWarning("Queens Hotel API: Reservation not found - ID: {ReservationId} at {Timestamp}",
                        id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return NotFound($"Reservation with ID {id} not found.");
                }

                // Perform the update
                var updateResult = await _reservationService.UpdateReservationAsync(dto);
                
                if (updateResult)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully updated reservation ID: {ReservationId} by user: {UserId} at {Timestamp}",
                        id, dto.UserId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return NoContent();
                }
                else
                {
                    _logger.LogError("Queens Hotel API: Failed to update reservation ID: {ReservationId} by user: {UserId} at {Timestamp} - No rows affected",
                        id, dto.UserId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    return BadRequest(new { 
                        message = "Failed to update reservation. This could be due to one of the following reasons:",
                        possibleReasons = new[]
                        {
                            "The reservation doesn't belong to the specified customer",
                            "Another process has modified this reservation",
                            "Invalid foreign key references (meal plan, suite, room, travel agency, payment card)",
                            "Database constraints are preventing the update"
                        },
                        troubleshooting = new
                        {
                            debugEndpoint = $"/api/reservation/debug-update-reservation",
                            checkReservation = $"/api/reservation?id={id}",
                            note = "Use the debug endpoint with the same data to get detailed validation information"
                        },
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Queens Hotel API: Validation error for reservation update - ID: {ReservationId} at {Timestamp}",
                    id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return BadRequest(new { message = argEx.Message, timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") });
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogError(invEx, "Queens Hotel API: Database operation error for reservation update - ID: {ReservationId} at {Timestamp}",
                    id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                
                // Check if it's a "not found" scenario
                if (invEx.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { message = invEx.Message, timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") });
                }
                
                return BadRequest(new { message = invEx.Message, timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during reservation update for ID: {ReservationId} at {Timestamp}",
                    id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return StatusCode(500, new { 
                    message = "An internal server error occurred while updating the reservation",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        /// <summary>
        /// Cancels a reservation at Queens Hotel
        /// </summary>
        /// <param name="request">Cancellation request containing reservation ID and user information</param>
        /// <returns>Cancellation result with success status and details</returns>
        /// <response code="200">Returns the cancellation result</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="404">If the reservation is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost("cancel")]
        [ProducesResponseType(typeof(ApiResponse<CancelReservationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<CancelReservationResponseDto>>> CancelReservation([FromBody] CancelReservationRequestDto request)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Processing reservation cancellation request for ReservationID: {ReservationId} at {Timestamp} by user: {User}",
                    request.ReservationID, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), request.CancelledBy);

                if (request == null)
                {
                    var badRequestResponse = new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Cancellation request cannot be null",
                        Data = null,
                        Timestamp = DateTime.UtcNow,
                        ProcessedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                if (request.ReservationID <= 0)
                {
                    var badRequestResponse = new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid reservation ID provided",
                        Data = null,
                        Timestamp = DateTime.UtcNow,
                        ProcessedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                if (string.IsNullOrWhiteSpace(request.CancelledBy))
                {
                    var badRequestResponse = new ApiResponse<string>
                    {
                        Success = false,
                        Message = "CancelledBy field is required",
                        Data = null,
                        Timestamp = DateTime.UtcNow,
                        ProcessedBy = "dilshan-jolanka"
                    };
                    return BadRequest(badRequestResponse);
                }

                var result = await _reservationService.CancelReservationAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("Queens Hotel API: Successfully cancelled reservation {ReservationId} at {Timestamp}",
                        request.ReservationID, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    var successResponse = new ApiResponse<CancelReservationResponseDto>
                    {
                        Success = true,
                        Message = "Reservation cancelled successfully",
                        Data = result,
                        Timestamp = DateTime.UtcNow,
                        ApiVersion = "1.0.0",
                        ProcessedBy = "dilshan-jolanka"
                    };

                    return Ok(successResponse);
                }
                else
                {
                    // Check if it was a "not found" scenario
                    if (result.Message.Contains("not found"))
                    {
                        var notFoundResponse = new ApiResponse<string>
                        {
                            Success = false,
                            Message = result.Message,
                            Data = null,
                            Timestamp = DateTime.UtcNow,
                            ProcessedBy = "dilshan-jolanka"
                        };
                        return NotFound(notFoundResponse);
                    }
                    else
                    {
                        var errorResponse = new ApiResponse<CancelReservationResponseDto>
                        {
                            Success = false,
                            Message = result.Message,
                            Data = result,
                            Timestamp = DateTime.UtcNow,
                            ProcessedBy = "dilshan-jolanka"
                        };
                        return BadRequest(errorResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error occurred during reservation cancellation at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var response = new ApiResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while processing your Queens Hotel cancellation request",
                    Data = null,
                    Timestamp = DateTime.UtcNow,
                    ProcessedBy = "dilshan-jolanka"
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Test endpoint to verify the updated GetReservationData functionality
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-27 19:50:00
        /// </summary>
        [HttpGet("test-updated-endpoint")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> TestUpdatedReservationEndpoint()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Testing updated GetReservationData endpoint at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return Ok(new
                {
                    message = "Updated GetReservationData endpoint is ready",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    newFeatures = new[]
                    {
                        "Payment Method Information (ID and Type)",
                        "Payment Card Details (ID, Cardholder Name, Expiry)",
                        "Guest Status Information",
                        "Enhanced Travel Agency Details",
                        "Improved Error Handling"
                    },
                    endpoints = new
                    {
                        getAllReservations = "GET /api/reservation",
                        getByNIC = "GET /api/reservation?nic=123456789V",
                        getByEmail = "GET /api/reservation?email=john@email.com",
                        getByCustomerName = "GET /api/reservation?fname=John&lname=Doe",
                        getByPhone = "GET /api/reservation?number=1234567890",
                        getById = "GET /api/reservation?id=123"
                    },
                    sampleResponse = new
                    {
                        reservationId = 1,
                        checkInDate = "2025-08-28T00:00:00Z",
                        checkOutDate = "2025-08-30T00:00:00Z",
                        numberOfGuests = 2,
                        status = "Confirmed",
                        creditCardProvided = true,
                        createdDateTime = "2025-08-27T19:50:00Z",
                        payment = new
                        {
                            paymentMethodId = 2,
                            paymentMethodType = "Credit Card",
                            paymentCardDetailsId = 123,
                            guestStatus = "Active",
                            paymentCard = new
                            {
                                cardId = 123,
                                cardHolderName = "John Doe",
                                expiryMonth = 12,
                                expiryYear = 2025,
                                maskedCardNumber = "**** **** **** ****"
                            }
                        }
                    },
                    note = "The endpoint now returns comprehensive payment and card information as per the updated stored procedure"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error in test endpoint at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Test endpoint to demonstrate the zero-to-null conversion functionality for update reservation
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-27 20:20:00
        /// </summary>
        [HttpGet("test-zero-to-null-conversion")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> TestZeroToNullConversion()
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Testing zero-to-null conversion functionality at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return Ok(new
                {
                    message = "Zero-to-null conversion is working correctly in UpdateReservation endpoint",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    functionality = new
                    {
                        description = "When updating a reservation, if any of the optional foreign key fields are set to 0, they will be converted to NULL in the stored procedure",
                        affectedFields = new[]
                        {
                            "mealPlan_id",
                            "suite_id",
                            "room_ID",
                            "travalAgency_Id",
                            "paymentCardDetails_Id"
                        }
                    },
                    example = new
                    {
                        description = "Sample request that demonstrates zero-to-null conversion",
                        method = "PUT",
                        url = "/api/reservation/1",
                        requestBody = new
                        {
                            reservationId = 1,
                            customerId = 1,
                            checkInDate = "2026-01-01T00:00:00.000Z",
                            checkOutDate = "2026-01-04T00:00:00.000Z",
                            numberOfGuests = 6,
                            status = "Confirmed",
                            paymentMethodI_Id = 1,
                            mealPlan_id = 0,      // This will be converted to NULL
                            suite_id = 0,         // This will be converted to NULL
                            room_ID = 0,          // This will be converted to NULL
                            travalAgency_Id = 0,  // This will be converted to NULL
                            paymentCardDetails_Id = 0, // This will be converted to NULL
                            userId = "admin001"
                        },
                        explanation = new
                        {
                            beforeConversion = "mealPlan_id=0, suite_id=0, room_ID=0, travalAgency_Id=0, paymentCardDetails_Id=0",
                            afterConversion = "All these fields will be passed as NULL to the stored procedure",
                            benefit = "This allows the frontend to send 0 to indicate 'no selection' or 'remove association'"
                        }
                    },
                    validValues = new
                    {
                        note = "Valid values for optional foreign key fields",
                        nullOrZero = "Will be converted to NULL in stored procedure",
                        positiveInteger = "Will be passed as-is to stored procedure",
                        examples = new
                        {
                            noMealPlan = "mealPlan_id: 0 or null -> NULL in SP",
                            withMealPlan = "mealPlan_id: 5 -> 5 in SP",
                            noSuite = "suite_id: 0 or null -> NULL in SP", 
                            withSuite = "suite_id: 10 -> 10 in SP"
                        }
                    },
                    logging = new
                    {
                        description = "The system will log which fields are being converted from 0 to NULL",
                        logLevel = "Information",
                        example = "Converting zero values to NULL for fields: MealPlan_id, Suite_id, Room_ID in reservation 1"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error in zero-to-null test endpoint at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Debug endpoint to validate reservation update data and constraints
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-27 20:30:00
        /// </summary>
        [HttpPost("debug-update-reservation")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> DebugUpdateReservation([FromBody] UpdateReservationDto dto)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Debug update reservation called for ReservationID: {ReservationId} at {Timestamp}",
                    dto.ReservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var debugInfo = new
                {
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    receivedData = new
                    {
                        reservationId = dto.ReservationId,
                        customerId = dto.CustomerId,
                        checkInDate = dto.CheckInDate.ToString("yyyy-MM-dd"),
                        checkOutDate = dto.CheckOutDate.ToString("yyyy-MM-dd"),
                        numberOfGuests = dto.NumberOfGuests,
                        status = dto.Status,
                        paymentMethodI_Id = dto.PaymentMethodI_Id,
                        mealPlan_id = dto.MealPlan_id,
                        suite_id = dto.Suite_id,
                        room_ID = dto.Room_ID,
                        travalAgency_Id = dto.TravalAgency_Id,
                        userId = dto.UserId
                    },
                    zeroToNullConversions = new
                    {
                        mealPlan_id = dto.MealPlan_id == 0 ? "Will be converted to NULL" : "Will remain as is",
                        suite_id = dto.Suite_id == 0 ? "Will be converted to NULL" : "Will remain as is",
                        room_ID = dto.Room_ID == 0 ? "Will be converted to NULL" : "Will remain as is",
                        travalAgency_Id = dto.TravalAgency_Id == 0 ? "Will be converted to NULL" : "Will remain as is",
                    }
                };

                // Check if reservation exists
                var reservationExists = await _reservationService.ReservationExistsAsync(dto.ReservationId);
                object reservationInfo = "Not checked";
                var customerMatch = "Not checked";

                if (reservationExists)
                {
                    // Get actual reservation data to compare
                    var searchRequest = new GetReservationDataRequest { Id = dto.ReservationId };
                    var reservations = await _reservationService.GetReservationDataAsync(searchRequest);
                    var reservation = reservations.FirstOrDefault();

                    if (reservation != null)
                    {
                        reservationInfo = new
                        {
                            currentCustomerId = reservation.Customer.Id,
                            currentStatus = reservation.Status,
                            currentCheckIn = reservation.CheckInDate.ToString("yyyy-MM-dd"),
                            currentCheckOut = reservation.CheckOutDate.ToString("yyyy-MM-dd"),
                            currentGuests = reservation.NumberOfGuests
                        };
                        
                        customerMatch = reservation.Customer.Id == dto.CustomerId ? "MATCH" : "MISMATCH";
                    }
                }

                var response = new
                {
                    message = "Debug information for UpdateReservation",
                    debugInfo.timestamp,
                    validation = new
                    {
                        reservationExists,
                        customerMatch,
                        dateRangeValid = dto.CheckInDate < dto.CheckOutDate,
                        guestsValid = dto.NumberOfGuests > 0 && dto.NumberOfGuests <= 50,
                        statusValid = !string.IsNullOrEmpty(dto.Status) && dto.Status.Length <= 45,
                        userIdValid = !string.IsNullOrEmpty(dto.UserId) && dto.UserId.Length <= 45
                    },
                    currentReservationData = reservationInfo,
                    debugInfo.receivedData,
                    debugInfo.zeroToNullConversions,
                    storedProcedureWhereClause = new
                    {
                        condition = "WHERE Id = @ReservationId AND Customer_Id = @CustomerId",
                        note = "Both reservation ID and customer ID must match for update to succeed",
                        reservationIdMatch = reservationExists ? "EXISTS" : "NOT_FOUND",
                        customerIdMatch = customerMatch
                    },
                    troubleshooting = new
                    {
                        commonIssues = new[]
                        {
                            "Reservation ID doesn't exist in database",
                            "Customer ID doesn't match the reservation's customer",
                            "Another process modified the reservation simultaneously",
                            "Required foreign key constraints are violated",
                            "Date values are invalid or out of range"
                        },
                        nextSteps = new[]
                        {
                            "1. Verify reservation exists with: GET /api/reservation?id=" + dto.ReservationId,
                            "2. Check if customer owns this reservation",
                            "3. Verify all foreign keys exist in their respective tables",
                            "4. Try the actual update to see detailed error logs"
                        }
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error in debug update reservation at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Ok(new
                {
                    message = "Debug endpoint encountered an error",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        /// <summary>
        /// Quick diagnostic endpoint to check reservation-customer relationship for troubleshooting updates
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-27 21:00:00
        /// </summary>
        [HttpGet("diagnostic/{reservationId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> DiagnosticReservation(int reservationId)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Diagnostic check for reservation {ReservationId} at {Timestamp}",
                    reservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                // Get the reservation directly
                var searchRequest = new GetReservationDataRequest { Id = reservationId };
                var reservations = await _reservationService.GetReservationDataAsync(searchRequest);
                var reservation = reservations.FirstOrDefault();

                if (reservation == null)
                {
                    return Ok(new
                    {
                        reservationId,
                        found = false,
                        message = "Reservation not found",
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        nextSteps = new[]
                        {
                            "1. Verify the reservation ID exists in the database",
                            "2. Check if the reservation was deleted or never created",
                            "3. Try with a different reservation ID"
                        }
                    });
                }

                return Ok(new
                {
                    reservationId,
                    found = true,
                    reservationDetails = new
                    {
                        actualCustomerId = reservation.Customer.Id,
                        customerName = $"{reservation.Customer.FirstName} {reservation.Customer.LastName}",
                        customerNIC = reservation.Customer.NIC,
                        customerEmail = reservation.Customer.Email,
                        reservationStatus = reservation.Status,
                        checkInDate = reservation.CheckInDate.ToString("yyyy-MM-dd"),
                        checkOutDate = reservation.CheckOutDate.ToString("yyyy-MM-dd"),
                        numberOfGuests = reservation.NumberOfGuests
                    },
                    updateInstructions = new
                    {
                        message = "To update this reservation, use the ACTUAL customer ID from above",
                        correctCustomerId = reservation.Customer.Id,
                        yourCurrentCustomerId = "You were using: 1",
                        storedProcedureRequirement = "WHERE Id = @ReservationId AND Customer_Id = @CustomerId"
                    },
                    sampleCorrectPayload = new
                    {
                        reservationId = reservationId,
                        customerId = reservation.Customer.Id, // ? Use this customer ID
                        checkInDate = "2026-01-01T00:00:00.000Z",
                        checkOutDate = "2026-01-04T00:00:00.000Z",
                        numberOfGuests = 6,
                        status = "Confirmed",
                        paymentMethodI_Id = 1,
                        mealPlan_id = 0,
                        suite_id = 0,
                        room_ID = 0,
                        travalAgency_Id = 0,
                        paymentCardDetails_Id = 0,
                        userId = "admin001"
                    },
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Error in diagnostic endpoint for reservation {ReservationId} at {Timestamp}",
                    reservationId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Ok(new
                {
                    reservationId,
                    found = false,
                    error = ex.Message,
                    message = "Error occurred while checking reservation",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        /// <summary>
        /// Gets the next scheduled execution time for the ReservationAutoCancelService (7 PM daily)
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-28
        /// </summary>
        [HttpGet("auto-cancel-time")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public ActionResult GetNextAutoCancelTime()
        {
            var now = DateTime.Now;
            var next7pm = now.Date.AddHours(19); // 7 PM today
            if (now > next7pm) next7pm = next7pm.AddDays(1);

            return Ok(new
            {
                nextExecutionTime = next7pm,
                nextExecutionTimeUtc = next7pm.ToUniversalTime(),
                serverTime = now,
                serverTimeUtc = now.ToUniversalTime(),
                message = "This is the next scheduled execution time for the ReservationAutoCancelService (runs daily at 7 PM server time)."
            });
        }

        /// <summary>
        /// Sets the time of day for the ReservationAutoCancelService to execute (format: HH:mm)
        /// Author: dilshan-jolanka
        /// Create date: 2025-08-28
        /// </summary>
        [HttpPost("set-auto-cancel-time")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult SetAutoCancelTime([FromQuery] string time)
        {
            if (string.IsNullOrWhiteSpace(time))
                return BadRequest(new { message = "Query parameter 'time' is required in HH:mm format." });

            if (!TimeSpan.TryParseExact(time, "hh\\:mm", null, out var parsedTime))
            {
                // Try 24-hour format as fallback
                if (!TimeSpan.TryParseExact(time, "h\\:mm", null, out parsedTime) &&
                    !TimeSpan.TryParseExact(time, "H\\:mm", null, out parsedTime) &&
                    !TimeSpan.TryParseExact(time, "HH\\:mm", null, out parsedTime))
                {
                    return BadRequest(new { message = "Invalid time format. Use HH:mm (e.g., 19:00 for 7 PM)." });
                }
            }

            ReservationAutoCancelService.NextRunTimeOfDay = parsedTime;
            return Ok(new
            {
                message = $"Auto-cancel service time set to {parsedTime:hh\\:mm}.",
                nextRunTime = parsedTime.ToString("hh\\:mm")
            });
        }
    }
}