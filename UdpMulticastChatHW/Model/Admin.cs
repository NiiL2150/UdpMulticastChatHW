using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public class Admin : IUser, IAdmin
    {
        public string Name { get; set; }
        public IList<ChatCommand> Commands { get; set; }
        public IList<BanListItem> BanList { get; set; }

        public Admin()
        {
            Name = "admin";
            BanList = new List<BanListItem>();
            Commands = new List<ChatCommand>()
            {
                new ChatCommand("ban", 1, (string[] args) =>
                {
                    return null;
                })
            };
        }

        public async Task ReceiveAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync()
        {
            throw new NotImplementedException();
        }

        bool IAdmin.IsCommand(string command)
        {
            if (command.StartsWith('/'))
            {
                string command2 = command.Substring(1);
                string[] command3 = command2.Split(' ');

                string commandName = command3[0];
                string[] commandArgs = command3.Skip(1).ToArray();
                if(Commands.Any(x => x.CommandName == commandName && x.ArgumentCount == commandArgs.Count()))
                {
                    return true;
                }
            }
            return false;
        }

        void IAdmin.HandleCommand(string command)
        {
            string command2 = command.Substring(1);
            string[] command3 = command2.Split(' ');

            string commandName = command3[0];
            string[] commandArgs = command3.Skip(1).ToArray();
            Commands.First(x => x.CommandName == commandName).Invoke(commandArgs);
        }
    }
}
