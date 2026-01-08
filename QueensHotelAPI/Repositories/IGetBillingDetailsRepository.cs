using System;
using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
    public interface IGetBillingDetailsRepository
    {
        Task<GetBillingDetailsResultDto> GetBillingDetailsAsync(int billingId);
    }
}

