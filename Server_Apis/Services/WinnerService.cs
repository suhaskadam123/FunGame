using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Server_Apis.Hubs;
using Server_Apis.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_Apis.Services
{
    public class WinnerService : IWinnerService
    {
        private readonly ApplicationDbContext _db;
        private readonly GlobalVarWinner _globalVar;
        private readonly IHubContext<TimerHub> _hubContext;

        public WinnerService(ApplicationDbContext db, GlobalVarWinner globalVar, IHubContext<TimerHub> hubContext)
        {
            _db = db;
            _globalVar = globalVar;
            _hubContext = hubContext;
        }

        public async Task DeclareWinnerAsync()
        {
            var drawTime = DateTime.Now.ToString("HH:mm");
            var drawDate = DateTime.Today;
            var gameId = 1;

            Console.WriteLine("✅ WinnerService Started");

            var game = await _db.GameSetting.FirstOrDefaultAsync(g => g.Id == gameId);
            if (game == null) return;

            var bets = await _db.BetData
                .Where(b => b.DrawDate == drawDate && b.DrawTime == drawTime && b.DeleteStatus == 0 && b.GameId == gameId)
                .ToListAsync();

            if (!bets.Any()) return;

            var (totalBetArray, totalPlayed) = CalculateTotalBets(bets);
            var bufferToBeGiven = (totalPlayed * game.Percentage / 100m) + game.Buffer;

            WinnerData winnerData;
            int winMode = 0;

            if (_globalVar.GreenChakriWinner != 999)
            {
                winnerData = HandleManualWinner(totalBetArray, (int)bufferToBeGiven);
                winMode = 1;
            }
            else
            {
                winnerData = SelectWinner(totalBetArray, (int)bufferToBeGiven);
            }

            // Emit via SignalR
            await _hubContext.Clients.All.SendAsync("GreenChakriWinner", new
            {
                winner = winnerData.Winner,
                xValue = _globalVar.GreenChakriXSelector
            });

            // Save to database
            game.Buffer = Math.Max(0, winnerData.NewBuffer);

            var win = new WinningNumber
            {
                //DrawDate = drawDate,
                //DrawTime = drawTime,
                //GameId = gameId,
                //Profit = (int)bufferToBeGiven - winnerData.TotalWin,
                //TotalWin = winnerData.TotalWin,
                //TotalPlayed = totalPlayed,
                //Winner = winnerData.Winner,
               // WinMod = winMode,
                XValue = _globalVar.GreenChakriXSelector
            };

            try
            {
                _db.WinningNumber.Add(win);
                await _db.SaveChangesAsync();
                Console.WriteLine("✅ Winner inserted successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inserting winner: {ex.Message}");
                return;
            }

            await UpdateBetDataAndBalances(bets, winnerData.Winner);

            // Reset global values
            _globalVar.GreenChakriWinner = 999;
            _globalVar.GreenChakriXSelector = 1;
        }

        public Task<List<WinningNumber>> GetAllWinnerData()
        {
            throw new NotImplementedException();
        }

        private (int[] totalBetArray, int totalPlayed) CalculateTotalBets(List<BetData> bets)
        {
            var totalBetArray = new int[11];
            int totalPlayed = 0;

            foreach (var bet in bets)
            {
                for (int i = 0; i <= 10; i++)
                {
                    var amount = (int?)bet.GetType().GetProperty($"Bet{i}")?.GetValue(bet) ?? 0;
                    totalBetArray[i] += amount;
                }
                totalPlayed += bet.BetTotal;
            }

            return (totalBetArray, totalPlayed);
        }

        private WinnerData HandleManualWinner(int[] totalBetArray, int buffer)
        {
            int winner = _globalVar.GreenChakriWinner;
            int totalWin = totalBetArray[winner] * 11 * _globalVar.GreenChakriXSelector;
            int newBuffer = buffer - totalWin;
            return new WinnerData { Winner = winner, TotalWin = totalWin, NewBuffer = newBuffer };
        }

        private WinnerData SelectWinner(int[] totalBetArray, int buffer)
        {
            var random = new Random();
            var range = Enumerable.Range(0, 11).OrderBy(x => random.Next()).ToList();

            var zeroBets = new List<int>();

            foreach (var i in range)
            {
                int potentialWin = totalBetArray[i] * 11;
                if (potentialWin <= buffer)
                {
                    if (totalBetArray[i] == 0)
                        zeroBets.Add(i);
                    else
                        return new WinnerData { Winner = i, TotalWin = potentialWin, NewBuffer = buffer - potentialWin };
                }
            }

            int fallbackWinner = zeroBets.Any()
                ? zeroBets[random.Next(zeroBets.Count)]
                : totalBetArray
                    .Select((val, idx) => new { val, idx })
                    .Where(x => x.val > 0)
                    .OrderBy(x => x.val)
                    .FirstOrDefault()?.idx ?? 0;

            int fallbackWin = totalBetArray[fallbackWinner] * 11;
            return new WinnerData { Winner = fallbackWinner, TotalWin = fallbackWin, NewBuffer = buffer - fallbackWin };
        }

        private async Task UpdateBetDataAndBalances(List<BetData> bets, int winner)
        {
            foreach (var bet in bets)
            {
                int winAmount = (int?)bet.GetType().GetProperty($"Bet{winner}")?.GetValue(bet) ?? 0;
                winAmount *= 11 * _globalVar.GreenChakriXSelector;

                bet.WinAmount = winAmount;
                bet.Winner = winner;
                bet.XValue = _globalVar.GreenChakriXSelector;

                var user = await _db.Users.FindAsync(bet.UserId);
                if (user != null)
                {
                    user.Balance += winAmount;
                }
            }

            await _db.SaveChangesAsync();
        }
    }

    public class WinnerData
    {
        public int Winner { get; set; }
        public int TotalWin { get; set; }
        public int NewBuffer { get; set; }
    }
}
