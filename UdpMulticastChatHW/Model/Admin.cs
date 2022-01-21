using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public class Admin : IUser, IAdmin
    {
        public string Name { get; set; }
        public IList<ChatCommand> Commands { get; set; }
        public IList<BanListItem> BanList { get; set; }
        public IList<User> Users { get; set; }

        public Admin()
        {
            Name = "admin";

            Users = new List<User>();
            BanList = new List<BanListItem>();
            Commands = new List<ChatCommand>()
            {
                new ChatCommand("ban", 2, (string[] args) =>
                {
                    User user = new User(Users.First(x=>x.Name == args[0]));
                    if(!Int32.TryParse(args[1], out int seconds))
                    {
                        seconds = Int32.MaxValue;
                    }
                    
                    Users.Remove(user);
                    HandleCommand($"/kick {args[0]}");
                    BanList.Add(new BanListItem(user, seconds));
                }),
                new ChatCommand("ban", 1, (string[] args) =>
                {
                    HandleCommand($"/ban {args[0]} {Int32.MaxValue}");
                }),
                new ChatCommand("unban", 1, (string[] args) =>
                {
                    BanListItem user = BanList.First(x=>x.Name == args[0]);
                    BanList.Remove(user);
                }),
                new ChatCommand("kick", 1, (string[] args) =>
                {

                })
            };
        }

        public async Task SendAsync(string text)
        {
            if (IsCommand(text))
            {
                HandleCommand(text);
            }
            else
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
                {
                    IPAddress dest = IPAddress.Parse("127.0.0.1");
                    IPEndPoint endPoint = new IPEndPoint(dest, 51234);
                    client.Connect(endPoint);
                    await client.SendAsync(buffer, buffer.Length);
                    client.Close();

                }
                using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
                {
                    IPAddress dest = IPAddress.Parse("224.5.5.5");
                    client.JoinMulticastGroup(dest, 2);
                    IPEndPoint endPoint = new IPEndPoint(dest, 51234);
                    client.Connect(endPoint);
                    await client.SendAsync(buffer, buffer.Length);
                    client.Close();
                }
            }
        }

        public async IAsyncEnumerable<string> ReceiveAsync()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 51234);
            IPEndPoint rEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                using (UdpClient client = new UdpClient(endPoint))
                {
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    string message = "";

                    await Task.Run(() =>
                    {
                        byte[] result = client.Receive(ref rEndPoint);
                        message = Encoding.UTF8.GetString(result);
                    });
                    /*
                    client.Connect(rEndPoint);
                    UdpReceiveResult result = await client.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(result.Buffer);
                    */

                    while (true)
                    {
                        if (message.Length > 0)
                        {
                            if (message[0] == '/')
                            {
                                message = message.Substring(1);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    await SendAsync(message);
                    yield return message;
                }
            }
        }

        public bool IsCommand(string command)
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

        public void HandleCommand(string command)
        {
            string command2 = command.Substring(1);
            string[] command3 = command2.Split(' ');

            string commandName = command3[0];
            string[] commandArgs = command3.Skip(1).ToArray();
            Commands.First(x => x.CommandName == commandName).Invoke(commandArgs);
        }
    }
}
