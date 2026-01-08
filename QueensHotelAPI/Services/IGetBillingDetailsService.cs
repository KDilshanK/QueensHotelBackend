using System;
using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    public interface IGetBillingDetailsService
    {
        Task<GetBillingDetailsResultDto> GetBillingDetailsAsync(int billingId);
    }
}

