using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public interface IAdmin
    {
        protected abstract bool IsCommand(string command);

        protected abstract void HandleCommand(string command);
    }
}
