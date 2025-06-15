namespace Server_Apis.DTOs
{
    public class UserBalanceDto
    {
        public string Username { get; set; }
        public int Balance { get; set; }
        public int Status { get; set; }
        public int SuperDistributerId { get; set; }
        public int DistributerId { get; set; }
        public int RetailerId { get; set; }
    }
}
