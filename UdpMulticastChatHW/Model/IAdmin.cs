using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public interface IAdmin
    {
        public abstract bool IsCommand(string command);

        public abstract void HandleCommand(string command);
    }
}
