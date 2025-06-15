using Microsoft.AspNetCore.SignalR;

namespace Server_Apis.Hubs
{
    public class TimerHub : Hub
    {
        public async Task BroadcastGreenChakriWinner(int winner, int xValue)
        {
            await Clients.All.SendAsync("GreenChakriWinner", new
            {
                winner,
                xValue
            });
        }

    }
}
