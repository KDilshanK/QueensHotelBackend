using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using System.Data;

namespace QueensHotelAPI.Repositories
{
    /// <summary>
    /// Repository implementation for Customer Invoice operations
    /// </summary>
    public class CustomerInvoiceRepository : ICustomerInvoiceRepository
    {
        private readonly QueensHotelDbContext _context;
        private readonly ILogger<CustomerInvoiceRepository> _logger;

        public CustomerInvoiceRepository(QueensHotelDbContext context, ILogger<CustomerInvoiceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GenerateCustomerInvoiceResponseDto> GenerateCustomerInvoiceAsync(int billingId)
        {
            try
            {
                _logger.LogInformation("Queens Hotel API: Executing GenerateCustomerInvoice stored procedure for BillingId: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                var result = new GenerateCustomerInvoiceResponseDto();

                var parameters = new[]
                {
                    new SqlParameter("@Billing_Id", SqlDbType.Int) { Value = billingId }
                };

                try
                {
                    // First try with Entity Framework's ExecuteSqlRaw (includes retry logic)
                    await ExecuteGenerateInvoiceAsync(billingId, result);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("SqlServerRetryingExecutionStrategy"))
                {
                    // If the retry strategy fails, try once more without it
                    _logger.LogWarning("Queens Hotel API: Retry strategy failed, attempting direct execution for invoice generation for BillingId: {BillingId}", billingId);
                    
                    await ExecuteGenerateInvoiceDirectlyAsync(billingId, result);
                }

                _logger.LogInformation("Queens Hotel API: Successfully generated invoice for BillingId: {BillingId} at {Timestamp}",
                    billingId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return result;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Queens Hotel API: SQL Server error occurred while generating invoice for BillingId: {BillingId}. Error: {ErrorMessage}",
                    billingId, sqlEx.Message);
                
                // Check if it's a connection-related error
                if (IsConnectionError(sqlEx))
                {
                    throw new InvalidOperationException($"Database connection failed while generating invoice for BillingId {billingId}. Please check your network connection and try again.", sqlEx);
                }
                else
                {
                    throw new InvalidOperationException($"Database error occurred while generating invoice for BillingId {billingId}: {sqlEx.Message}", sqlEx);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Queens Hotel API: Unexpected error occurred while generating invoice for BillingId: {BillingId}",
                    billingId);
                throw new InvalidOperationException($"An unexpected error occurred while generating invoice for BillingId {billingId}: {ex.Message}", ex);
            }
        }

        private async Task ExecuteGenerateInvoiceAsync(int billingId, GenerateCustomerInvoiceResponseDto result)
        {
            await _context.Database.OpenConnectionAsync();
            
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[GenerateCustomerInvoice]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 120;

                command.Parameters.Add(new SqlParameter("@Billing_Id", SqlDbType.Int) { Value = billingId });

                using var reader = await command.ExecuteReaderAsync();
                
                // Read result set 1: Invoice Header
                if (await reader.ReadAsync())
                {
                    result.InvoiceHeader = new InvoiceHeaderDto
                    {
                        InvoiceNumber = reader["InvoiceNumber"]?.ToString() ?? string.Empty,
                        InvoiceDate = reader["InvoiceDate"]?.ToString() ?? string.Empty,
                        BillingID = reader["BillingID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BillingID"]),
                        ReservationID = reader["ReservationID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ReservationID"]),
                        BillingAmount = reader["BillingAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BillingAmount"]),
                        PaymentStatus = reader["PaymentStatus"]?.ToString() ?? string.Empty,
                        PaymentMethod = reader["PaymentMethod"]?.ToString() ?? string.Empty,
                        BillingDate = reader["BillingDate"]?.ToString() ?? string.Empty
                    };
                }

                // Move to result set 2: Customer Information
                await reader.NextResultAsync();
                if (await reader.ReadAsync())
                {
                    result.Customer = new InvoiceCustomerDto
                    {
                        CustomerID = reader["CustomerID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CustomerID"]),
                        FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                        LastName = reader["LastName"]?.ToString() ?? string.Empty,
                        CustomerName = reader["CustomerName"]?.ToString() ?? string.Empty,
                        EmailAddress = reader["EmailAddress"]?.ToString() ?? string.Empty,
                        Phone = reader["Phone"]?.ToString() ?? string.Empty,
                        Address = reader["Address"]?.ToString() ?? string.Empty
                    };
                }

                // Move to result set 3: Stay Information
                await reader.NextResultAsync();
                if (await reader.ReadAsync())
                {
                    result.StayInformation = new InvoiceStayDto
                    {
                        Accommodation = reader["Accommodation"]?.ToString() ?? string.Empty,
                        CheckInDateTime = reader["CheckInDateTime"]?.ToString() ?? string.Empty,
                        CheckOutDateTime = reader["CheckOutDateTime"]?.ToString() ?? string.Empty,
                        NightsStayed = reader["NightsStayed"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NightsStayed"]),
                        LateCheckout = reader["LateCheckout"]?.ToString() ?? string.Empty
                    };
                }

                // Move to result set 4: Charges Breakdown
                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    result.ChargesBreakdown.Add(new InvoiceChargeDto
                    {
                        ChargeType = reader["ChargeType"]?.ToString() ?? string.Empty,
                        Amount = reader["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Amount"])
                    });
                }

                // Move to result set 5: Service Charges
                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    result.ServiceCharges.Add(new InvoiceServiceChargeDto
                    {
                        ServiceChargeID = reader["ServiceChargeID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ServiceChargeID"]),
                        ServiceType = reader["ServiceType"]?.ToString() ?? string.Empty,
                        Amount = reader["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Amount"]),
                        ServiceDate = reader["ServiceDate"]?.ToString() ?? string.Empty,
                        CreatedBy = reader["CreatedBy"]?.ToString() ?? string.Empty,
                        IsFree = reader["IsFree"]?.ToString() ?? string.Empty
                    });
                }

                // Move to result set 6: Payment Information
                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    result.Payments.Add(new InvoicePaymentDto
                    {
                        PaymentID = reader["PaymentID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PaymentID"]),
                        AmountPaid = reader["AmountPaid"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AmountPaid"]),
                        PaymentDate = reader["PaymentDate"]?.ToString() ?? string.Empty,
                        PaymentStatus = reader["PaymentStatus"]?.ToString() ?? string.Empty,
                        PaymentMethod = reader["PaymentMethod"]?.ToString() ?? string.Empty
                    });
                }

                // Move to result set 7: Service Charges by Type
                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    result.ServiceChargesByType.Add(new InvoiceServiceChargeGroupDto
                    {
                        ServiceType = reader["ServiceType"]?.ToString() ?? string.Empty,
                        Count = reader["Count"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Count"]),
                        TotalAmount = reader["TotalAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalAmount"]),
                        AverageAmount = reader["AverageAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AverageAmount"]),
                        FreeItems = reader["FreeItems"] == DBNull.Value ? 0 : Convert.ToInt32(reader["FreeItems"]),
                        PercentageOfServiceCharges = reader["PercentageOfServiceCharges"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PercentageOfServiceCharges"])
                    });
                }

                // Move to result set 8: Service Charges by Date
                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    result.ServiceChargesByDate.Add(new InvoiceServiceChargeByDateDto
                    {
                        ServiceDay = reader["ServiceDay"]?.ToString() ?? string.Empty,
                        ServiceCount = reader["ServiceCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ServiceCount"]),
                        DailyTotal = reader["DailyTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DailyTotal"]),
                        ServicesUsed = reader["ServicesUsed"]?.ToString() ?? string.Empty
                    });
                }

                // Move to result set 9: Service Charge Statistics
                await reader.NextResultAsync();
                if (await reader.ReadAsync())
                {
                    result.ServiceChargeStatistics = new InvoiceServiceChargeStatsDto
                    {
                        TotalServiceItems = reader["TotalServiceItems"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalServiceItems"]),
                        PaidServiceItems = reader["PaidServiceItems"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PaidServiceItems"]),
                        FreeServiceItems = reader["FreeServiceItems"] == DBNull.Value ? 0 : Convert.ToInt32(reader["FreeServiceItems"]),
                        TotalServiceCharges = reader["TotalServiceCharges"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalServiceCharges"]),
                        AverageChargePerPaidItem = reader["AverageChargePerPaidItem"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AverageChargePerPaidItem"]),
                        HighestServiceCharge = reader["HighestServiceCharge"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["HighestServiceCharge"]),
                        LowestServiceCharge = reader["LowestServiceCharge"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["LowestServiceCharge"]),
                        PercentageOfBill = reader["PercentageOfBill"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PercentageOfBill"])
                    };
                }
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }

        private async Task ExecuteGenerateInvoiceDirectlyAsync(int billingId, GenerateCustomerInvoiceResponseDto result)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            
            using var command = new SqlCommand("[dbo].[GenerateCustomerInvoice]", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };
            
            command.Parameters.Add(new SqlParameter("@Billing_Id", SqlDbType.Int) { Value = billingId });
            
            // Use the same logic as ExecuteGenerateInvoiceAsync but with direct SqlConnection
            using var reader = await command.ExecuteReaderAsync();
            
            // Read all result sets using the same logic as above
            // (Implementation would be identical to ExecuteGenerateInvoiceAsync)
            await ReadAllResultSets(reader, result);
        }

        private async Task ReadAllResultSets(SqlDataReader reader, GenerateCustomerInvoiceResponseDto result)
        {
            // This method contains the same result set reading logic as in ExecuteGenerateInvoiceAsync
            // Extracted to avoid code duplication between EF and direct SQL approaches
            
            // Read result set 1: Invoice Header
            if (await reader.ReadAsync())
            {
                result.InvoiceHeader = new InvoiceHeaderDto
                {
                    InvoiceNumber = reader["InvoiceNumber"]?.ToString() ?? string.Empty,
                    InvoiceDate = reader["InvoiceDate"]?.ToString() ?? string.Empty,
                    BillingID = reader["BillingID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BillingID"]),
                    ReservationID = reader["ReservationID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ReservationID"]),
                    BillingAmount = reader["BillingAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BillingAmount"]),
                    PaymentStatus = reader["PaymentStatus"]?.ToString() ?? string.Empty,
                    PaymentMethod = reader["PaymentMethod"]?.ToString() ?? string.Empty,
                    BillingDate = reader["BillingDate"]?.ToString() ?? string.Empty
                };
            }

            // Move to result set 2: Customer Information
            await reader.NextResultAsync();
            if (await reader.ReadAsync())
            {
                result.Customer = new InvoiceCustomerDto
                {
                    CustomerID = reader["CustomerID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CustomerID"]),
                    FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                    LastName = reader["LastName"]?.ToString() ?? string.Empty,
                    CustomerName = reader["CustomerName"]?.ToString() ?? string.Empty,
                    EmailAddress = reader["EmailAddress"]?.ToString() ?? string.Empty,
                    Phone = reader["Phone"]?.ToString() ?? string.Empty,
                    Address = reader["Address"]?.ToString() ?? string.Empty
                };
            }

            // Move to result set 3: Stay Information
            await reader.NextResultAsync();
            if (await reader.ReadAsync())
            {
                result.StayInformation = new InvoiceStayDto
                {
                    Accommodation = reader["Accommodation"]?.ToString() ?? string.Empty,
                    CheckInDateTime = reader["CheckInDateTime"]?.ToString() ?? string.Empty,
                    CheckOutDateTime = reader["CheckOutDateTime"]?.ToString() ?? string.Empty,
                    NightsStayed = reader["NightsStayed"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NightsStayed"]),
                    LateCheckout = reader["LateCheckout"]?.ToString() ?? string.Empty
                };
            }

            // Move to result set 4: Charges Breakdown
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                result.ChargesBreakdown.Add(new InvoiceChargeDto
                {
                    ChargeType = reader["ChargeType"]?.ToString() ?? string.Empty,
                    Amount = reader["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Amount"])
                });
            }

            // Move to result set 5: Service Charges
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                result.ServiceCharges.Add(new InvoiceServiceChargeDto
                {
                    ServiceChargeID = reader["ServiceChargeID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ServiceChargeID"]),
                    ServiceType = reader["ServiceType"]?.ToString() ?? string.Empty,
                    Amount = reader["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Amount"]),
                    ServiceDate = reader["ServiceDate"]?.ToString() ?? string.Empty,
                    CreatedBy = reader["CreatedBy"]?.ToString() ?? string.Empty,
                    IsFree = reader["IsFree"]?.ToString() ?? string.Empty
                });
            }

            // Move to result set 6: Payment Information
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                result.Payments.Add(new InvoicePaymentDto
                {
                    PaymentID = reader["PaymentID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PaymentID"]),
                    AmountPaid = reader["AmountPaid"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AmountPaid"]),
                    PaymentDate = reader["PaymentDate"]?.ToString() ?? string.Empty,
                    PaymentStatus = reader["PaymentStatus"]?.ToString() ?? string.Empty,
                    PaymentMethod = reader["PaymentMethod"]?.ToString() ?? string.Empty
                });
            }

            // Move to result set 7: Service Charges by Type
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                result.ServiceChargesByType.Add(new InvoiceServiceChargeGroupDto
                {
                    ServiceType = reader["ServiceType"]?.ToString() ?? string.Empty,
                    Count = reader["Count"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Count"]),
                    TotalAmount = reader["TotalAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalAmount"]),
                    AverageAmount = reader["AverageAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AverageAmount"]),
                    FreeItems = reader["FreeItems"] == DBNull.Value ? 0 : Convert.ToInt32(reader["FreeItems"]),
                    PercentageOfServiceCharges = reader["PercentageOfServiceCharges"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PercentageOfServiceCharges"])
                });
            }

            // Move to result set 8: Service Charges by Date
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                result.ServiceChargesByDate.Add(new InvoiceServiceChargeByDateDto
                {
                    ServiceDay = reader["ServiceDay"]?.ToString() ?? string.Empty,
                    ServiceCount = reader["ServiceCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ServiceCount"]),
                    DailyTotal = reader["DailyTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DailyTotal"]),
                    ServicesUsed = reader["ServicesUsed"]?.ToString() ?? string.Empty
                });
            }

            // Move to result set 9: Service Charge Statistics
            await reader.NextResultAsync();
            if (await reader.ReadAsync())
            {
                result.ServiceChargeStatistics = new InvoiceServiceChargeStatsDto
                {
                    TotalServiceItems = reader["TotalServiceItems"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalServiceItems"]),
                    PaidServiceItems = reader["PaidServiceItems"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PaidServiceItems"]),
                    FreeServiceItems = reader["FreeServiceItems"] == DBNull.Value ? 0 : Convert.ToInt32(reader["FreeServiceItems"]),
                    TotalServiceCharges = reader["TotalServiceCharges"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalServiceCharges"]),
                    AverageChargePerPaidItem = reader["AverageChargePerPaidItem"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AverageChargePerPaidItem"]),
                    HighestServiceCharge = reader["HighestServiceCharge"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["HighestServiceCharge"]),
                    LowestServiceCharge = reader["LowestServiceCharge"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["LowestServiceCharge"]),
                    PercentageOfBill = reader["PercentageOfBill"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PercentageOfBill"])
                };
            }
        }

        private static bool IsConnectionError(SqlException sqlException)
        {
            // Common SQL Server connection error numbers
            var connectionErrorNumbers = new[] { 2, 53, 121, 232, 258, 1231, 1232, 11001, 18456, 4060 };
            return connectionErrorNumbers.Contains(sqlException.Number);
        }
    }
}