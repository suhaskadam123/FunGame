using Microsoft.AspNetCore.SignalR;
using Server_Apis.Helpers;
using Server_Apis.Hubs;

namespace Server_Apis.Services
{
    public class MinuteTimerService : BackgroundService
    {
        private readonly IHubContext<TimerHub> _hubContext;

        public MinuteTimerService(IHubContext<TimerHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int countdown = 60;

            while (!stoppingToken.IsCancellationRequested)
            {
                string nextMinute = TimeHelper.GetNextMinuteTime();
                string currentIST = TimeHelper.GetIndiaTime().ToString("hh:mm:ss tt");

                string message = "";
                if (countdown == 10)
                    message = "⛔ No More Betting";
                else if (countdown == 1)
                    message = "🏆 Winner Declared";
                else if (countdown == 0)
                    message = "✅ Timer Restarted";

                // Send regular timer tick
                await _hubContext.Clients.All.SendAsync("TimerTick", new
                {
                    countdown,
                    nextMinute,
                    indiaTime = currentIST,
                    statusMessage = message
                });

                // Send winner when countdown = 1
                if (countdown == 1)
                {
                    // Simulated winner info – replace with actual logic if needed
                    int winner = new Random().Next(0, 10);
                    int xValue = 1;

                    await _hubContext.Clients.All.SendAsync("GreenChakriWinner", new
                    {
                        winner,
                        xValue
                    });
                }

                countdown--;
                if (countdown < 0)
                    countdown = 60;

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
