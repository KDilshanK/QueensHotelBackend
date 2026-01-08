using QueensHotelAPI.Models;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Service interface for MealPlan business logic
    /// Author: dilshan-jolanka
    /// Create date: 2025-05-29 13:03:06
    /// Description: Business layer interface for meal plan operations
    /// </summary>
    public interface IMealPlanService
    {
        Task<IEnumerable<MealPlan>> GetMealPlanDataAsync(string? mealPlanCode = null);
    }
}