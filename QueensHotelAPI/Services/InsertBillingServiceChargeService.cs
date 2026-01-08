using System.Threading.Tasks;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;
using QueensHotelAPI.Services;

public class InsertBillingServiceChargeService : IInsertBillingServiceChargeService
{
    private readonly IInsertBillingServiceChargeRepository _repository;

    public InsertBillingServiceChargeService(IInsertBillingServiceChargeRepository repository)
    {
        _repository = repository;
    }

    public async Task<InsertBillingServiceChargeResultDto> InsertBillingServiceChargeAsync(int billingId, int serviceChargeId)
    {
        return await _repository.InsertBillingServiceChargeAsync(billingId, serviceChargeId);
    }

    public async Task<List<GetBillingServiceChargesResponseDto>> GetBillingServiceChargesAsync(int billingId)
    {
        return await _repository.GetBillingServiceChargesAsync(billingId);
    }
}