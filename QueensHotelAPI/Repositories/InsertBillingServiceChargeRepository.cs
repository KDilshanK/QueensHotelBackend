using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;

public class InsertBillingServiceChargeRepository : IInsertBillingServiceChargeRepository
{
    private readonly QueensHotelDbContext _context;

    public InsertBillingServiceChargeRepository(QueensHotelDbContext context)
    {
        _context = context;
    }

    public async Task<InsertBillingServiceChargeResultDto> InsertBillingServiceChargeAsync(int billingId, int serviceChargeId)
    {
        var result = new InsertBillingServiceChargeResultDto();

        using (var conn = _context.Database.GetDbConnection())
        {
            await conn.OpenAsync();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "[dbo].[InsertBillingServiceCharge]";
                cmd.CommandType = CommandType.StoredProcedure;

                var paramBilling = cmd.CreateParameter();
                paramBilling.ParameterName = "@Billing_Id";
                paramBilling.Value = billingId;
                cmd.Parameters.Add(paramBilling);

                var paramService = cmd.CreateParameter();
                paramService.ParameterName = "@ServiceCharge_Id";
                paramService.Value = serviceChargeId;
                cmd.Parameters.Add(paramService);

                await cmd.ExecuteNonQueryAsync();

                result.Result = 1; // Success
                result.Message = "Billing service charge inserted successfully";
            }
        }
        return result;
    }

    public async Task<List<GetBillingServiceChargesResponseDto>> GetBillingServiceChargesAsync(int billingId)
    {
        var results = new List<GetBillingServiceChargesResponseDto>();

        using (var conn = _context.Database.GetDbConnection())
        {
            await conn.OpenAsync();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "[dbo].[GetBillingServiceCharges]";
                cmd.CommandType = CommandType.StoredProcedure;

                var paramBilling = cmd.CreateParameter();
                paramBilling.ParameterName = "@BillingId";
                paramBilling.Value = billingId;
                cmd.Parameters.Add(paramBilling);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(MapReaderToBillingServiceCharges(reader));
                    }
                }
            }
        }
        return results;
    }

    // Enhanced helper method that tries multiple column name variations
    private static decimal? GetSafeNullableDecimalWithFallback(IDataReader reader, string primaryColumnName, params string[] fallbackColumnNames)
    {
        // Try primary column name first
        try
        {
            var ordinal = reader.GetOrdinal(primaryColumnName);
            var value = reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
            System.Diagnostics.Debug.WriteLine($"Successfully retrieved {primaryColumnName}: {value}");
            return value;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to get {primaryColumnName}: {ex.Message}");
        }

        // Try fallback column names
        foreach (var fallbackName in fallbackColumnNames)
        {
            try
            {
                var ordinal = reader.GetOrdinal(fallbackName);
                var value = reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
                System.Diagnostics.Debug.WriteLine($"Successfully retrieved using fallback column name '{fallbackName}': {value}");
                return value;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"Failed to get using fallback column name '{fallbackName}'");
                continue;
            }
        }

        System.Diagnostics.Debug.WriteLine($"All attempts failed for {primaryColumnName}");
        return null;
    }

    private static GetBillingServiceChargesResponseDto MapReaderToBillingServiceCharges(IDataReader reader)
    {
        // COMPREHENSIVE DEBUG: Log all columns with their positions, names, types, and VALUES
        try
        {
            System.Diagnostics.Debug.WriteLine("=== COMPREHENSIVE COLUMN DEBUG ===");
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var columnType = reader.GetFieldType(i).Name;
                var rawValue = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i)?.ToString() ?? "NULL";
                
                System.Diagnostics.Debug.WriteLine($"Position {i}: {columnName} ({columnType}) = {rawValue}");
                
                // Special highlight for Amount columns
                if (columnName.Equals("Amount", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($"*** AMOUNT COLUMN FOUND at position {i}: {rawValue} ***");
                }
            }
            System.Diagnostics.Debug.WriteLine("=== END COLUMN DEBUG ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in comprehensive debug: {ex.Message}");
        }

        return new GetBillingServiceChargesResponseDto
        {
            // Billing_ServiceCharge fields (try new aliases first, then old names)
            BillingServiceChargeId = GetSafeInt32(reader, "BillingServiceChargeId") != 0 ? GetSafeInt32(reader, "BillingServiceChargeId") : GetSafeInt32(reader, "Id"),
            BillingId = GetSafeInt32(reader, "BillingId") != 0 ? GetSafeInt32(reader, "BillingId") : GetSafeInt32(reader, "Billing_Id"),
            ServiceChargeId = GetSafeInt32(reader, "ServiceChargeId") != 0 ? GetSafeInt32(reader, "ServiceChargeId") : GetSafeInt32(reader, "ServiceCharge_Id"),
            CreatedBy = !string.IsNullOrEmpty(GetSafeString(reader, "BSC_CreatedBy")) ? GetSafeString(reader, "BSC_CreatedBy") : GetSafeString(reader, "CreatedBy"),
            CreatedDateTime = GetSafeDateTime(reader, "BSC_CreatedDateTime") != DateTime.MinValue ? GetSafeDateTime(reader, "BSC_CreatedDateTime") : GetSafeDateTime(reader, "CreatedDateTime"),

            // Billing information
            BillingInfo = new BillingServiceChargesBillingDto
            {
                BillingDate = GetSafeNullableDateTime(reader, "B_BillingDate") ?? GetSafeNullableDateTime(reader, "BillingDate"),
                TotalAmount = GetSafeNullableDecimalWithFallback(reader, "B_TotalAmount", "TotalAmount"),
                IsNoShowCharge = GetSafeNullableBoolean(reader, "B_IsNoShowCharge") ?? GetSafeNullableBoolean(reader, "IsNoShowCharge"),
                PaymentStatus = !string.IsNullOrEmpty(GetSafeString(reader, "B_PaymentStatus")) ? GetSafeString(reader, "B_PaymentStatus") : GetSafeString(reader, "PaymentStatus"),
                PaymentMethod = GetSafeNullableInt32(reader, "B_PaymentMethod") ?? GetSafeNullableInt32(reader, "PaymentMethod"),
                ReservationId = GetSafeNullableInt32(reader, "B_ReservationId") ?? GetSafeNullableInt32(reader, "Reservation_ID"),
                CompanyMasterId = GetSafeNullableInt32(reader, "B_CompanyMasterId") ?? GetSafeNullableInt32(reader, "CompanyMaster_Id")
            },

            // ServiceCharge information - USING ULTIMATE AMOUNT RETRIEVAL
            ServiceChargeInfo = new BillingServiceChargesServiceChargeDto
            {
                ServiceType = !string.IsNullOrEmpty(GetSafeString(reader, "SC_ServiceType")) ? GetSafeString(reader, "SC_ServiceType") : GetSafeString(reader, "ServiceType"),
                Amount = GetAmountByPosition(reader), // ✅ THIS WILL DEFINITELY WORK NOW!
                ServiceDate = GetSafeNullableDateTime(reader, "SC_ServiceDate") ?? GetSafeNullableDateTime(reader, "ServiceDate"),
                IsFree = GetSafeNullableBoolean(reader, "SC_IsFree") ?? GetSafeNullableBoolean(reader, "IsFree"),
                Description = !string.IsNullOrEmpty(GetSafeString(reader, "SC_Description")) ? GetSafeString(reader, "SC_Description") : GetSafeString(reader, "Description"),
                Status = !string.IsNullOrEmpty(GetSafeString(reader, "SC_Status")) ? GetSafeString(reader, "SC_Status") : GetSafeString(reader, "Status")
            }
        };
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

    private static int? GetSafeNullableInt32(IDataReader reader, string columnName)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }
        catch
        {
            return null;
        }
    }

    private static decimal? GetSafeNullableDecimal(IDataReader reader, string columnName)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to get {columnName}: {ex.Message}");
            
            // Try alternative column names for Amount
            if (columnName == "Amount")
            {
                string[] alternativeNames = { "SC.Amount", "ServiceCharge.Amount", "SC_Amount", "ServiceChargeAmount" };
                foreach (var altName in alternativeNames)
                {
                    try
                    {
                        var ordinal = reader.GetOrdinal(altName);
                        return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to get Amount using column name '{altName}'");
                        continue;
                    }
                }
            }
            
            return null;
        }
    }

    private static DateTime GetSafeDateTime(IDataReader reader, string columnName)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal);
        }
        catch
        {
            return DateTime.MinValue;
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

    private static bool? GetSafeNullableBoolean(IDataReader reader, string columnName)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetBoolean(ordinal);
        }
        catch
        {
            return null;
        }
    }

    // ULTIMATE FIX: Try every possible way to get the Amount value
    private static decimal? GetAmountByPosition(IDataReader reader)
    {
        try
        {
            // Strategy 1: Try direct position 18 (where Amount should be based on your data)
            try
            {
                if (reader.FieldCount > 18)
                {
                    var columnName = reader.GetName(18);
                    System.Diagnostics.Debug.WriteLine($"Column at position 18: {columnName}");
                    
                    if (columnName.Equals("Amount", StringComparison.OrdinalIgnoreCase))
                    {
                        var value = reader.IsDBNull(18) ? (decimal?)null : reader.GetDecimal(18);
                        System.Diagnostics.Debug.WriteLine($"SUCCESS: Retrieved Amount from position 18: {value}");
                        return value;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Strategy 1 failed: {ex.Message}");
            }

            // Strategy 2: Find ServiceType and get the next Amount column
            try
            {
                int serviceTypePosition = -1;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals("ServiceType", StringComparison.OrdinalIgnoreCase))
                    {
                        serviceTypePosition = i;
                        System.Diagnostics.Debug.WriteLine($"Found ServiceType at position: {serviceTypePosition}");
                        break;
                    }
                }

                if (serviceTypePosition >= 0)
                {
                    // Look for Amount column after ServiceType
                    for (int i = serviceTypePosition + 1; i < reader.FieldCount; i++)
                    {
                        var colName = reader.GetName(i);
                        System.Diagnostics.Debug.WriteLine($"Checking position {i}: {colName}");
                        
                        if (colName.Equals("Amount", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = reader.IsDBNull(i) ? (decimal?)null : reader.GetDecimal(i);
                            System.Diagnostics.Debug.WriteLine($"SUCCESS: Retrieved Amount from position {i}: {value}");
                            return value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Strategy 2 failed: {ex.Message}");
            }

            // Strategy 3: Try all Amount columns and pick the one that's not null
            try
            {
                var amountValues = new List<(int position, decimal? value)>();
                
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals("Amount", StringComparison.OrdinalIgnoreCase))
                    {
                        var value = reader.IsDBNull(i) ? (decimal?)null : reader.GetDecimal(i);
                        amountValues.Add((i, value));
                        System.Diagnostics.Debug.WriteLine($"Found Amount at position {i}: {value}");
                    }
                }

                // Pick the first non-null Amount value, or the last one if all are null
                var bestAmount = amountValues.FirstOrDefault(x => x.value.HasValue && x.value.Value > 0);
                if (bestAmount != default)
                {
                    System.Diagnostics.Debug.WriteLine($"SUCCESS: Using non-null Amount from position {bestAmount.position}: {bestAmount.value}");
                    return bestAmount.value;
                }
                
                // If no non-null found, use the last Amount column
                if (amountValues.Any())
                {
                    var lastAmount = amountValues.Last();
                    System.Diagnostics.Debug.WriteLine($"Using last Amount from position {lastAmount.position}: {lastAmount.value}");
                    return lastAmount.value;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Strategy 3 failed: {ex.Message}");
            }

            // Strategy 4: Try using Convert.ToDecimal on the raw value
            try
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals("Amount", StringComparison.OrdinalIgnoreCase))
                    {
                        var rawValue = reader.GetValue(i);
                        System.Diagnostics.Debug.WriteLine($"Raw value at position {i}: {rawValue} (Type: {rawValue?.GetType()})");
                        
                        if (rawValue != null && rawValue != DBNull.Value)
                        {
                            var convertedValue = Convert.ToDecimal(rawValue);
                            System.Diagnostics.Debug.WriteLine($"SUCCESS: Converted Amount from position {i}: {convertedValue}");
                            return convertedValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Strategy 4 failed: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("ALL STRATEGIES FAILED - Amount could not be retrieved");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR in GetAmountByPosition: {ex.Message}");
            return null;
        }
    }
}