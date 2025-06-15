namespace Server_Apis.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        public string Password { get; set; }

        public int Balance { get; set; }
    }
}
