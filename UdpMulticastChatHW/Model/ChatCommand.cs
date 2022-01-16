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
        public delegate object? CommandDelegate(params string[] args);
        public event CommandDelegate CommandEvent;
        public ChatCommand(string name, int argCount, CommandDelegate command)
        {
            CommandName = name;
            ArgumentCount = argCount;
            CommandEvent = command;
        }
        public object? Invoke(params string[] args)
        {
            return CommandEvent.Invoke(args);
        }
    }
}
