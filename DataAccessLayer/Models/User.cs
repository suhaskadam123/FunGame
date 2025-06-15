using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [Required]
        [StringLength(10)]
        public string Password { get; set; }

        public int Balance { get; set; } = 0;

        public int Percentage { get; set; }

        [Required]
        [StringLength(255)]
        public string Role { get; set; }

        [StringLength(10)]
        public string ReferName { get; set; }

        [Column("ReferId")]
        public int? ReferId { get; set; }  // Nullable to allow top-level (superadmin) user


        public int Status { get; set; } = 0;

        [Column("SDId")]
        public int SuperDistributerId { get; set; } = 0;

        [Column("DId")]
        public int DistributerId { get; set; } = 0;

        [Column("RId")]
        public int RetailerId { get; set; } = 0;

        [Column("SId")]
        public int SuperAdminId { get; set; } = 1;

        [StringLength(255)]
        public string UniqueId { get; set; }

        public DateTime? DateTime { get; set; }

        public int DeleteStatus { get; set; } = 1;

        [Required]
        [StringLength(255)]
        public string Note { get; set; }
    }
}
