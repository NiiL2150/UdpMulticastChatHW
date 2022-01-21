using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public class BanListItem : User
    {
        public DateTime BanExpireDateTime { get; set; }

        public BanListItem(User user, int seconds) : base(user)
        {
            BanExpireDateTime = DateTime.Now.AddSeconds(seconds);
        }

        public bool BanStatus()
        {
            if(BanExpireDateTime <= DateTime.Now)
            {
                return false;
            }
            return true;
        }
    }
}
