using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    // Add this to your Models folder
    public class CreateSuperDistributorViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string Password { get; set; }

        [Required]
        [Range(0, 100)]
        [Display(Name = "Commission")]
        public int Commission { get; set; }

        [Required]
        public string Note { get; set; }

        [Required]
        public bool Status { get; set; } = true;

        public string ReferralId { get; set; }
    }
}
