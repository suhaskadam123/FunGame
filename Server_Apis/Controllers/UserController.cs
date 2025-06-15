using Microsoft.AspNetCore.Mvc;
using Server_Apis.Interfaces;

namespace Server_Apis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("GetUserBalance/{userId}")]
        public async Task<IActionResult> GetUserBalance(int userId)
        {
            var result = await _userRepository.GetUserBalanceAsync(userId);
            if (result == null)
                return NotFound(new { Message = "User not found or deleted" });

            return Ok(result);
        }
    }
}
