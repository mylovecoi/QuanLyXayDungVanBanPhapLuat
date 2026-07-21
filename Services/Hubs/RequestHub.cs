using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Services.Hubs
{
    public class RequestHub : Hub
    {
        public async Task SendDashboardUpdate()
        {
            await Clients.All.SendAsync("ReceiveUpdate");
        }
    }

}
