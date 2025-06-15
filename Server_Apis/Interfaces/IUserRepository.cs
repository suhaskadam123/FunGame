using Server_Apis.DTOs;

namespace Server_Apis.Interfaces
{
    public interface IUserRepository
    {
        Task<UserBalanceDto> GetUserBalanceAsync(int id);
    }
}
