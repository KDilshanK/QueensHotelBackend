using System;
using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    public interface IInsertBillingServiceChargeService
    {
        Task<InsertBillingServiceChargeResultDto> InsertBillingServiceChargeAsync(int billingId, int serviceChargeId);
        Task<List<GetBillingServiceChargesResponseDto>> GetBillingServiceChargesAsync(int billingId);
    }
}

