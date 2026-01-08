using QueensHotelAPI.DTOs;

namespace QueensHotelAPI.Services
{
    public interface IUserService
    {
        Task<UserLoginResponseDto?> UserLoginAsync(UserLoginRequestDto dto);
    }
}