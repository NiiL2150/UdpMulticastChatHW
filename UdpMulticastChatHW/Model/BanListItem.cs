using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public class BanListItem
    {
        public string Name { get; set; }
        public DateTime BanExpireDateTime { get; set; }

        public BanListItem(string name, int seconds)
        {
            Name = name;
            BanExpireDateTime = DateTime.Now.AddSeconds(seconds);
        }
    }
}
