using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using QueensHotelAPI.Data;
using QueensHotelAPI.Models;
using System.Data;

namespace QueensHotelAPI.Repositories
{
    /// <summary>
    /// Repository implementation for MealPlan data operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-05-29 13:03:06
    /// Description: Executes GetMealPlanData stored procedure and maps results
    /// </summary>
    public class MealPlanRepository : IMealPlanRepository
    {
        private readonly QueensHotelDbContext _context;
        private readonly ILogger<MealPlanRepository> _logger;

        public MealPlanRepository(QueensHotelDbContext context, ILogger<MealPlanRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<MealPlan>> GetMealPlanDataAsync(string? mealPlanCode = null)
        {
            try
            {
                _logger.LogInformation("Executing GetMealPlanData stored procedure at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var results = new List<MealPlan>();

                await _context.Database.OpenConnectionAsync();

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "[dbo].[GetMealPlanData]";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;

                // Add parameters exactly matching the stored procedure
                command.Parameters.Add(new SqlParameter("@MealPlanCode", SqlDbType.VarChar, 45) { Value = (object?)mealPlanCode ?? DBNull.Value });

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(new MealPlan
                    {
                        Id = GetSafeInt(reader, "id"),
                        MealPlanCode = GetSafeString(reader, "MealPlanCode"),
                        Description = GetSafeString(reader, "Description"),
                        CostPerNight = GetSafeDecimal(reader, "CostPerNight"),
                        Status = GetSafeString(reader, "status"),
                        IsFree = GetSafeBool(reader, "IsFree")
                    });
                }

                _logger.LogInformation("Successfully retrieved {Count} meal plan records at {Timestamp} by user: {User}",
                    results.Count, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                return results;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Server error occurred while executing GetMealPlanData stored procedure at {Timestamp}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                throw new InvalidOperationException("Database error occurred while retrieving meal plan data", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while executing GetMealPlanData at {Timestamp} by user: {User}",
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

        private static int GetSafeInt(IDataReader reader, string columnName)
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

        private static decimal GetSafeDecimal(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0m : reader.GetDecimal(ordinal);
            }
            catch
            {
                return 0m;
            }
        }

        private static bool GetSafeBool(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? false : reader.GetBoolean(ordinal);
            }
            catch
            {
                return false;
            }
        }
    }
}