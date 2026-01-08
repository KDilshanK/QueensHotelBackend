using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;

namespace QueensHotelAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserLoginResponseDto?> UserLoginAsync(UserLoginRequestDto dto)
        {
            return await _userRepository.UserLoginAsync(dto);
        }
    }
}