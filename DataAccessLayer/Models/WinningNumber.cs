using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class WinningNumber
    {
        [Key]
        public int Id { get; set; }
        public int TotalWin { get; set; }
        public int Winner { get; set; }
        public DateTime DrawDate { get; set; }
        public string DrawTime { get; set; } = string.Empty;
        public int TotalPlayed { get; set; }
        public int Profit { get; set; }
        public int WinMod { get; set; }
        public int XValue { get; set; }
        public int GameId { get; set; }
    }
}
