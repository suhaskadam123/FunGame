namespace DataAccessLayer.Models
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int ExpiryInMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
