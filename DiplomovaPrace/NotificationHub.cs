using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DiplomovaPrace
{
    public class NotificationHub : Hub
    {
        //public void Hello()
        //{
        //    Clients.All.hello();
        //}

        public void notify(string message)
        {
            Clients.All.notify(message);
        }
    }
}