using QueensHotelAPI.Models;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    /// <summary>
    /// Service implementation for MealPlan business logic
    /// Author: dilshan-jolanka
    /// Create date: 2025-05-29 13:03:06
    /// Description: Business layer implementation for meal plan operations
    /// </summary>
    public class MealPlanService : IMealPlanService
    {
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly ILogger<MealPlanService> _logger;

        public MealPlanService(IMealPlanRepository mealPlanRepository, ILogger<MealPlanService> logger)
        {
            _mealPlanRepository = mealPlanRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<MealPlan>> GetMealPlanDataAsync(string? mealPlanCode = null)
        {
            try
            {
                _logger.LogInformation("MealPlan service GetMealPlanData called at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var mealPlans = await _mealPlanRepository.GetMealPlanDataAsync(mealPlanCode);

                _logger.LogInformation("MealPlan service GetMealPlanData completed - {Count} meal plans found at {Timestamp} by user: {User}",
                    mealPlans.Count(), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                return mealPlans;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MealPlan service GetMealPlanData failed at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                throw;
            }
        }
    }
}