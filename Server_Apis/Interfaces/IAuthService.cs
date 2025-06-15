using Server_Apis.DTOs;
using DataAccessLayer.Models;

namespace Server_Apis.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}
