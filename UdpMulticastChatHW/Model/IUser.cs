using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public interface IUser
    {
        public string Name { get; set; }

        public static IUser Admin(string name)
        {
            if (name == "admin")
            {
                return new Admin();
            }
            return new User(name);
        }

        public abstract Task ReceiveAsync();

        public abstract Task SendAsync();
    }
}
