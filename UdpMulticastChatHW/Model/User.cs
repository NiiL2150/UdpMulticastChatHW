using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public class User : IUser
    {
        public string Name { get; set; }
        public IPAddress IPAddress { get; set; }

        public User(string name)
        {
            Name = name;
        }

        public async Task ReceiveAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync()
        {
            throw new NotImplementedException();
        }
    }
}
