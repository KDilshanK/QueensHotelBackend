using Microsoft.AspNetCore.Mvc;
using QueensHotelAPI.Services;
using QueensHotelAPI.Models;

namespace QueensHotelAPI.Controllers
{
    /// <summary>
    /// Controller for MealPlan management operations
    /// Author: dilshan-jolanka
    /// Create date: 2025-05-29 13:03:06
    /// Description: API endpoints for meal plan data retrieval using GetMealPlanData stored procedure
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MealPlanController : ControllerBase
    {
        private readonly IMealPlanService _mealPlanService;
        private readonly ILogger<MealPlanController> _logger;

        public MealPlanController(IMealPlanService mealPlanService, ILogger<MealPlanController> logger)
        {
            _mealPlanService = mealPlanService;
            _logger = logger;
        }

        /// <summary>
        /// Get meal plan data using GetMealPlanData stored procedure
        /// Author: dilshan-jolanka
        /// Create date: 2025-05-29 13:03:06
        /// </summary>
        /// <param name="mealPlanCode">Meal plan code (supports partial matching)</param>
        /// <returns>Meal plan data matching the search criteria</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MealPlan>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MealPlan>>> GetMealPlanData(
            [FromQuery] string? mealPlanCode = null)
        {
            try
            {
                _logger.LogInformation("GetMealPlanData API called at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                var mealPlans = await _mealPlanService.GetMealPlanDataAsync(mealPlanCode);

                _logger.LogInformation("GetMealPlanData API completed successfully - {Count} meal plans found at {Timestamp} by user: {User}",
                    mealPlans.Count(), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");

                return Ok(mealPlans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMealPlanData API failed at {Timestamp} by user: {User}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "dilshan-jolanka");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}