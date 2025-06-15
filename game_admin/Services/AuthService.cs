using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace game_admin.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> AuthenticateSuperAdmin(string username, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username
                                      && u.Password == password
                                      && u.Role == "superadmin");
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> UsernameExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<string> GenerateUniqueUserIdAsync()
        {
            string id;
            Random rnd = new Random();
            do
            {
                id = rnd.Next(100000, 999999).ToString();
            } while (await _context.Users.AnyAsync(u => u.UniqueId == id));
            return id;
        }
        public async Task<List<User>> GetAllSuperDistributors()
        {
            return await _context.Users
                .Where(u => u.Role == "superDistributor")
                .OrderByDescending(u => u.UserId)
                .ToListAsync();
        }
        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
           
        }

        //public async Task UpdateSuperDistributerAsync(User user)
        //{
        //    _context.Users.Update(user);
        //    await _context.SaveChangesAsync();
        //}
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        //superadmin end

        public async Task<List<User>> GetAllDistributors()
        {
            return await _context.Users
                .Where(u => u.Role == "Distributer")
                .OrderByDescending(u => u.UserId)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllRetailer()
        {
            return await _context.Users
                .Where(u => u.Role == "Retailer")
                .OrderByDescending(u => u.UserId)
                .ToListAsync();
        }
        public async Task<List<User>> GetAllUser()
        {
            return await _context.Users
                .Where(u => u.Role == "User")
                .OrderByDescending(u => u.UserId)
                .ToListAsync();
        }



    }
}
