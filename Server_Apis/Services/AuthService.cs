
using DataAccessLayer.Data;
using Server_Apis.DTOs;
using DataAccessLayer.Models;
using Server_Apis.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Server_Apis.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(ApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

       

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username
                                        && u.Password == loginRequest.Password
                                        && u.Role.ToLower() == "User");

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials or role");

            var token = _jwtTokenGenerator.GenerateToken(user: user);

            return new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role,
                Password = user.Password
                
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.Password != dto.CurrentPassword)
                return false;

            user.Password = dto.NewPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
