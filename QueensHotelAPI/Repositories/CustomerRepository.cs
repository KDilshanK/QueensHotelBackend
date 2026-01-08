using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using QueensHotelAPI.Data;
using QueensHotelAPI.Models;
using System.Data;
using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
    /// <summary>
    /// Repository implementation for Customer data operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-05-29 12:39:46
    /// Description: Executes GetCustomerData stored procedure and maps results
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly QueensHotelDbContext _context;
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(QueensHotelDbContext context, ILogger<CustomerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerDetailsResult>> GetCustomerDataAsync(
            string? nic = null,
            string? passportId = null,
            string? emailAddress = null,
            string? phoneNo = null,
            string? firstName = null,
            string? lName = null)
        {
            try
            {
                _logger.LogInformation("Executing GetCustomerData stored procedure at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var results = new List<CustomerDetailsResult>();

                await _context.Database.OpenConnectionAsync();

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[GetCustomerData]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;

                // Add parameters exactly matching the stored procedure
                //command.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = DBNull.Value }); // Add the missing CustomerId parameter
                command.Parameters.Add(new SqlParameter("@NIC", SqlDbType.VarChar, 20) { Value = (object?)nic ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@PassportId", SqlDbType.VarChar, 20) { Value = (object?)passportId ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@EMailAddress", SqlDbType.VarChar, 50) { Value = (object?)emailAddress ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@PhoneNo", SqlDbType.VarChar, 20) { Value = (object?)phoneNo ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 20) { Value = (object?)firstName ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@LName", SqlDbType.VarChar, 20) { Value = (object?)lName ?? DBNull.Value });

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(new CustomerDetailsResult
                    {
                        CustomerId = GetSafeInt32(reader, "CustomerId"),
                        FirstName = GetSafeString(reader, "FirstName"),
                        LName = GetSafeString(reader, "LName"),
                        EmailAddress = GetSafeString(reader, "EmailAddress"),
                        Phone = GetSafeString(reader, "Phone"),
                        Address = GetSafeString(reader, "Address"),
                        PassportId = GetSafeString(reader, "PassportId"),
                        NIC = GetSafeString(reader, "NIC"),
                        DOB = GetSafeNullableDateTime(reader, "DOB"),
                        Gender = GetSafeString(reader, "Gender"),
                        CountryName = GetSafeString(reader, "CountryName")
                    });
                }

                _logger.LogInformation("Successfully retrieved {Count} customer records at {Timestamp} by user: {User}",
                    results.Count, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                return results;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Server error occurred while executing GetCustomerData stored procedure at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                throw new InvalidOperationException("Database error occurred while retrieving customer data", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while executing GetCustomerData at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                throw;
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }

        // Helper methods for safe data extraction
        private static string GetSafeString(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static int GetSafeInt32(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private static DateTime? GetSafeNullableDateTime(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
            }
            catch
            {
                return null;
            }
        }

        public async Task<CustomerDetailsResult?> GetCustomerDataByIdAsync(int customerId)
        {
            try
            {
                _logger.LogInformation("Executing GetCustomerData stored procedure for CustomerId: {CustomerId} at {Timestamp} by user: {User}",
                    customerId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                await _context.Database.OpenConnectionAsync();

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[GetCustomerData]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;

                // Add parameters exactly matching the stored procedure - only CustomerId is provided
                command.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customerId });
                command.Parameters.Add(new SqlParameter("@NIC", SqlDbType.VarChar, 20) { Value = DBNull.Value });
                command.Parameters.Add(new SqlParameter("@PassportId", SqlDbType.VarChar, 20) { Value = DBNull.Value });
                command.Parameters.Add(new SqlParameter("@EMailAddress", SqlDbType.VarChar, 50) { Value = DBNull.Value });
                command.Parameters.Add(new SqlParameter("@PhoneNo", SqlDbType.VarChar, 20) { Value = DBNull.Value });
                command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 20) { Value = DBNull.Value });
                command.Parameters.Add(new SqlParameter("@LName", SqlDbType.VarChar, 20) { Value = DBNull.Value });

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var result = new CustomerDetailsResult
                    {
                        CustomerId = GetSafeInt32(reader, "CustomerId"),
                        FirstName = GetSafeString(reader, "FirstName"),
                        LName = GetSafeString(reader, "LName"),
                        EmailAddress = GetSafeString(reader, "EmailAddress"),
                        Phone = GetSafeString(reader, "Phone"),
                        Address = GetSafeString(reader, "Address"),
                        PassportId = GetSafeString(reader, "PassportId"),
                        NIC = GetSafeString(reader, "NIC"),
                        DOB = GetSafeNullableDateTime(reader, "DOB"),
                        Gender = GetSafeString(reader, "Gender"),
                        CountryName = GetSafeString(reader, "CountryName")
                    };

                    _logger.LogInformation("Successfully retrieved customer record for CustomerId: {CustomerId} at {Timestamp} by user: {User}",
                        customerId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                    return result;
                }

                _logger.LogInformation("No customer found for CustomerId: {CustomerId} at {Timestamp} by user: {User}",
                    customerId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                return null;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Server error occurred while executing GetCustomerData stored procedure for CustomerId: {CustomerId} at {Timestamp}",
                    customerId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                throw new InvalidOperationException($"Database error occurred while retrieving customer data for ID {customerId}", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while executing GetCustomerData for CustomerId: {CustomerId} at {Timestamp} by user: {User}",
                    customerId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                throw;
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }

        public async Task<int?> InsertCustomerAsync(CreateCustomerDto dto)
        {
            try
            {
                await _context.Database.OpenConnectionAsync();

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[InsertCustomer]";
                command.CommandType = CommandType.StoredProcedure;

                // Update parameter names/types to match the new stored procedure
                command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 50) { Value = dto.FirstName });
                command.Parameters.Add(new SqlParameter("@LName", SqlDbType.VarChar, 50) { Value = dto.LName });
                command.Parameters.Add(new SqlParameter("@EmailAddress", SqlDbType.VarChar, 45) { Value = dto.EmailAddress });
                command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.VarChar, 15) { Value = dto.Phone });
                command.Parameters.Add(new SqlParameter("@Address", SqlDbType.VarChar, 45) { Value = dto.Address });
                command.Parameters.Add(new SqlParameter("@NIC", SqlDbType.VarChar, 45) { Value = (object?)dto.NIC ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@PassportID", SqlDbType.VarChar, 45) { Value = (object?)dto.PassportId ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@CountryOfResidence_id", SqlDbType.Int) { Value = dto.CountryOfResidence_id });
                command.Parameters.Add(new SqlParameter("@DOB", SqlDbType.VarChar, 45) { Value = (object?)dto.DOB ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Gender", SqlDbType.VarChar, 45) { Value = (object?)dto.Gender ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = (object?)dto.Password ?? DBNull.Value });

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    // Return the new customer ID from the result set
                    return reader["NewCustomerId"] == DBNull.Value ? null : Convert.ToInt32(reader["NewCustomerId"]);
                }
                return null;
            }
            catch
            {
                throw; // Let the controller handle/log the error
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }

        public async Task<UpdateCustomerResponseDto?> UpdateCustomerAsync(UpdateCustomerRequestDto dto)
        {
            await _context.Database.OpenConnectionAsync();
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[UpdateCustomerData]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 50) { Value = (object?)dto.FirstName ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@LName", SqlDbType.NVarChar, 50) { Value = (object?)dto.LName ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@EmailAddress", SqlDbType.NVarChar, 45) { Value = (object?)dto.EmailAddress ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar, 15) { Value = (object?)dto.Phone ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 45) { Value = (object?)dto.Address ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@status", SqlDbType.Int) { Value = (object?)dto.Status ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@CompanyMaster_Id", SqlDbType.Int) { Value = (object?)dto.CompanyMaster_Id ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@PassportId", SqlDbType.NVarChar, 45) { Value = (object?)dto.PassportId ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@NIC", SqlDbType.NVarChar, 45) { Value = (object?)dto.NIC ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@CountryOfResidence_id", SqlDbType.Int) { Value = (object?)dto.CountryOfResidence_id ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@DOB", SqlDbType.NVarChar, 45) { Value = (object?)dto.DOB ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar, 45) { Value = (object?)dto.Gender ?? DBNull.Value });

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new UpdateCustomerResponseDto
                    {
                        Status = reader["Status"]?.ToString() ?? "",
                        Message = reader["Message"]?.ToString() ?? "",
                        CustomerId = reader["CustomerId"] is int id ? id : Convert.ToInt32(reader["CustomerId"])
                    };
                }
                return null;
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                    await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task<CustomerLoginResponseDto> CustomerLoginAsync(CustomerLoginRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Executing Customer_LoginCheck stored procedure at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                await _context.Database.OpenConnectionAsync();

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[Customer_LoginCheck]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;

                // Add input parameters
                command.Parameters.Add(new SqlParameter("@UserName", SqlDbType.NVarChar, 256) { Value = dto.UserName });
                command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = dto.Password });

                // Add output parameters
                var successParam = new SqlParameter("@Success", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                var customerIdParam = new SqlParameter("@CustomerId", SqlDbType.Int) { Direction = ParameterDirection.Output };

                command.Parameters.Add(successParam);
                command.Parameters.Add(messageParam);
                command.Parameters.Add(customerIdParam);

                await command.ExecuteNonQueryAsync();

                var response = new CustomerLoginResponseDto
                {
                    Success = (bool)(successParam.Value ?? false),
                    Message = messageParam.Value?.ToString() ?? string.Empty,
                    CustomerId = customerIdParam.Value == DBNull.Value ? null : (int?)customerIdParam.Value
                };

                _logger.LogInformation("Customer login attempt completed - Success: {Success}, Message: {Message} at {Timestamp} by user: {User}",
                    response.Success, response.Message, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                return response;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Server error occurred while executing Customer_LoginCheck stored procedure at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                throw new InvalidOperationException("Database error occurred during customer login", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while executing Customer_LoginCheck at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                throw;
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }
    }
}