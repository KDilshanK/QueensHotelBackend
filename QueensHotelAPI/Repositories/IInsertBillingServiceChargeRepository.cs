using System;
using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
    public interface IInsertBillingServiceChargeRepository
    {
        Task<InsertBillingServiceChargeResultDto> InsertBillingServiceChargeAsync(int billingId, int serviceChargeId);
        Task<List<GetBillingServiceChargesResponseDto>> GetBillingServiceChargesAsync(int billingId);
    }
}

