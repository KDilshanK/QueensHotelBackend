using System;
using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Repositories
{
	public interface IUserRepository
	{
        Task<UserLoginResponseDto?> UserLoginAsync(UserLoginRequestDto dto);
    }
}

