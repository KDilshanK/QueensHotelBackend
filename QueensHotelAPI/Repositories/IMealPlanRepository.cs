using QueensHotelAPI.Models;

namespace QueensHotelAPI.Repositories
{
    /// <summary>
    /// Repository interface for MealPlan data operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-05-29 13:03:06
    /// Description: Interface for executing GetMealPlanData stored procedure
    /// </summary>
    public interface IMealPlanRepository
    {
        Task<IEnumerable<MealPlan>> GetMealPlanDataAsync(string? mealPlanCode = null);
    }
}