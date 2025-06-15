using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Server_Apis.DTOs;
using Server_Apis.Interfaces;

namespace Server_Apis.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserBalanceDto?> GetUserBalanceAsync(int userId)
        {
            return await _context.Users
                .Where(u => u.UserId == userId)//delete status == 1 If you want to get wanly user record then use "u.Role == "User""
                .Select(u => new UserBalanceDto
                {
                    Username = u.Username,
                    Balance = u.Balance,
                    Status = u.Status,
                    SuperDistributerId = u.SuperDistributerId,
                    DistributerId = u.DistributerId,
                    RetailerId = u.RetailerId
                })
                .FirstOrDefaultAsync();
        }

    }
}
