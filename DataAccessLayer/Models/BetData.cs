using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class BetData
    {
        [Key]
        public int BetId { get; set; }
        public int UserId { get; set; }
        public DateTime DrawDate { get; set; } 
        public string DrawTime { get; set; } = string.Empty;
        public int GameId { get; set; }
        public int BetTotal { get; set; }
        public int? WinAmount { get; set; }
        public int? Winner { get; set; }
        public int? XValue { get; set; }

        public int Bet0 { get; set; }
        public int Bet1 { get; set; }
        public int Bet2 { get; set; }
        public int Bet3 { get; set; }
        public int Bet4 { get; set; }
        public int Bet5 { get; set; }
        public int Bet6 { get; set; }
        public int Bet7 { get; set; }
        public int Bet8 { get; set; }
        public int Bet9 { get; set; }
        public int Bet10 { get; set; }
        public int DeleteStatus { get; set; } = 1;
        // This method fixes your error
    public int GetBetByIndex(int index)
    {
        return index switch
        {
            0 => Bet0,
            1 => Bet1,
            2 => Bet2,
            3 => Bet3,
            4 => Bet4,
            5 => Bet5,
            6 => Bet6,
            7 => Bet7,
            8 => Bet8,
            9 => Bet9,
            10 => Bet10,
            _ => 0
        };
    }
    }
}
