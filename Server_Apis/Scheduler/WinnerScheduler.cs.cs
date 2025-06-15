using Microsoft.Extensions.Hosting;
using Server_Apis.Interfaces;
namespace Server_Apis.Scheduler
{
    public class WinnerScheduler : BackgroundService
    {
        private readonly IServiceProvider _services;

        public WinnerScheduler(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = 60000 - DateTime.Now.Millisecond - DateTime.Now.Second * 1000;
                await Task.Delay(delay, stoppingToken);
                using var scope = _services.CreateScope();
                var winnerService = scope.ServiceProvider.GetRequiredService<IWinnerService>();
                await winnerService.DeclareWinnerAsync();
            }
        }
    }
}
