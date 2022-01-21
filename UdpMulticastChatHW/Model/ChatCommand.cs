using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public class ChatCommand
    {
        public string CommandName { get; set; }
        public int ArgumentCount { get; set; }
        public delegate void CommandDelegate(string[] args);
        public event CommandDelegate CommandEvent;
        public ChatCommand(string name, int argCount, CommandDelegate command)
        {
            CommandName = name;
            ArgumentCount = argCount;
            CommandEvent = command;
        }
        public void Invoke(string[] args)
        {
            CommandEvent.Invoke(args);
        }
    }
}
