//using DataAccessLayer.Data;
//using DataAccessLayer.Models;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.EntityFrameworkCore;
//using Server_Apis.Hubs;
//using System;

//namespace Server_Apis.Services
//{
//    public class GreenChakriService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IHubContext<TimerHub> _hubContext;
//        private readonly GlobalVarWinner _globalVar;

//        public GreenChakriService(ApplicationDbContext context, IHubContext<TimerHub> hubContext, GlobalVarWinner globalVar)
//        {
//            _context = context;
//            _hubContext = hubContext;
//            _globalVar = globalVar;
//        }

//        public async Task DefineGreenChakriWinningAsync()
//        {
//            var drawTime = DateTime.Now.ToString("HH:mm");   // "14:05"
//            var drawDate = DateTime.Today;
//            var gameId = 1;

//            var setting = await _context.GameSetting.FirstOrDefaultAsync(x => x.Id == gameId);
//            if (setting == null) return;

//            var bets = await _context.BetData
//                .Where(b => b.DrawDate == drawDate && b.DrawTime == drawTime && b.DeleteStatus == 0 && b.GameId == gameId)
//                .ToListAsync();

//            var totalBetArray = new int[11];
//            var totalPlayed = 0;

//            foreach (var bet in bets)
//            {
//                for (int i = 0; i < 11; i++)
//                {
//                    totalBetArray[i] += bet.GetBetByIndex(i);
//                }
//                totalPlayed += bet.BetTotal;
//            }

//            var bufferToBeGiven = ((int)totalPlayed * setting.Percentage) / 100 + setting.Buffer;

//            WinnerData winnerData;
//            int winMode = 0;

//            if (_globalVar.GreenChakriWinner != 999)
//            {
//                winnerData = HandleManualWinner(totalBetArray, bufferToBeGiven);
//                winMode = 1;
//            }
//            else
//            {
//                winnerData = SelectWinner(totalBetArray, bufferToBeGiven);
//            }
//            // ✅ 3. Emit winner via SignalR
//            //await _hubContext.Clients.All.SendAsync("GreenChakriWinner", new
//            //{
//            //    winner = winnerData.Winner,
//            //    xValue = _globalVar.GreenChakriXSelector
//            //});

//            //setting.Buffer = winnerData.NewBuffer;
//            //await _context.SaveChangesAsync();

//            // Emit winner
//            await _hubContext.Clients.All.SendAsync("GreenChakriWinner", new
//            {
//                winner = winnerData.Winner,
//                xValue = _globalVar.GreenChakriXSelector
//            });

//            // Save to winningnumber table
//            var profit = bufferToBeGiven - winnerData.TotalWin;

//            var drawDateTime = DateTime.Parse($"{drawDate} {drawTime}");

//            var winEntry = new WinningNumber
//            {
//                Winner = winnerData.Winner,
//                TotalWin = winnerData.TotalWin,
//                DrawDate = drawDateTime,
//                TotalPlayed = totalPlayed,
//                Profit = profit,
//                WinMod = winMode,
//                XValue = _globalVar.GreenChakriXSelector,
//                GameId = gameId
//            };

//            _context.WinningNumber.Add(winEntry);
//            await _context.SaveChangesAsync();
//           ;
//            Console.WriteLine("Winner saved to DB successfully");

//            // Update bets and user balances
//            await UpdateBetData(bets, winnerData.Winner);

//            _globalVar.GreenChakriXSelector = 1;
//        }

//        private WinnerData HandleManualWinner(int[] totalBets, int buffer)
//        {
//            int winner = _globalVar.GreenChakriWinner;
//            int totalWin = totalBets[winner] * 11 * _globalVar.GreenChakriXSelector;
//            int newBuffer = buffer - totalWin;
//            _globalVar.GreenChakriWinner = 999;
//            return new WinnerData { Winner = winner, TotalWin = totalWin, NewBuffer = newBuffer };
//        }

//        private WinnerData SelectWinner(int[] totalBets, int buffer)
//        {
//            var zeroBets = new List<int>();
//            var random = new Random();
//            var candidates = Enumerable.Range(0, 11).OrderBy(x => random.Next()).ToList();

//            foreach (var i in candidates)
//            {
//                var potentialWin = totalBets[i] * 11;
//                if (potentialWin <= buffer)
//                {
//                    if (totalBets[i] == 0)
//                        zeroBets.Add(i);
//                    else
//                        return new WinnerData { Winner = i, TotalWin = potentialWin, NewBuffer = buffer - potentialWin };
//                }
//            }

//            int fallbackWinner = zeroBets.Any()
//                ? zeroBets[random.Next(zeroBets.Count)]
//                : totalBets.Select((val, idx) => new { val, idx })
//                           .Where(x => x.val > 0)
//                           .OrderBy(x => x.val)
//                           .FirstOrDefault()?.idx ?? 0;

//            var finalWin = totalBets[fallbackWinner] * 11;
//            return new WinnerData { Winner = fallbackWinner, TotalWin = finalWin, NewBuffer = buffer - finalWin };
//        }

//        private async Task UpdateBetData(List<BetData> bets, int winner)
//        {
//            var userMap = new Dictionary<int, int>();

//            foreach (var bet in bets)
//            {
//                var winAmt = bet.GetBetByIndex(winner) * 11 * _globalVar.GreenChakriXSelector;
//                bet.WinAmount = (int)winAmt;
//                bet.Winner = winner;
//                bet.XValue = _globalVar.GreenChakriXSelector;

//                if (!userMap.ContainsKey(bet.UserId))
//                {
//                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == bet.UserId);
//                    if (user != null)
//                    {
//                        userMap[bet.UserId] = user.Balance;
//                    }
//                }

//                userMap[bet.UserId] += winAmt;
//            }

//            foreach (var entry in userMap)
//            {
//                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == entry.Key);
//                if (user != null)
//                {
//                    user.Balance = (int)entry.Value;
//                }
//            }

//            await _context.SaveChangesAsync();
//        }


//    }


//}
