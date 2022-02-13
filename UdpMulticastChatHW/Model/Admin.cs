using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UdpMulticastChatHW.DBModel;

namespace UdpMulticastChatHW.Model
{
    public class Admin : IUser, IAdmin
    {
        public string Name { get; set; }
        public IList<ChatCommand> Commands { get; set; }
        public IList<BanListItem> BanList { get; set; }
        public bool IsKicked { get; set; } = false;

        public Admin(string name = "admin")
        {
            Name = name;

            BanList = new List<BanListItem>();
            Commands = new List<ChatCommand>()
            {
                new ChatCommand("ban", 2, (string[] args) =>
                {
                    User user = new User(args[0]);
                    if(!Int32.TryParse(args[1], out int seconds))
                    {
                        seconds = Int32.MaxValue;
                    }
                    
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
                    Task.Run(async () =>{
                        await SendSettingsAsync($"KICK {args[0]}");
                    });
                }),
                new ChatCommand("private", 3, (string[] args)=>{
                    Task.Run(async () =>
                    {
                        await ResendMessageAsync($"PRIVATE {args[0]} {args[1]} {args[2]}{Environment.NewLine}");
                    });
                })
            };
        }

        public async IAsyncEnumerable<string> ReceiveMessageAsync()
        {
            while (true)
            {
                using (UdpClient client = new UdpClient(IPStorage.AdminEndPoint))
                {
                    client.Reuse();
                    string message = "";

                    await Task.Run(() =>
                    {
                        var endPoint = IPStorage.ReceiveEndPoint.Clone();
                        byte[] result = client.Receive(ref endPoint);
                        message = Encoding.UTF8.GetString(result);
                    });

                    if (message.StartsWith("/private"))
                    {
                        message = message.Substring(1);
                        string[] strs = message.Split(' ');
                        HandleCommand(strs[0], strs[1], strs[2], string.Join(' ', strs.Skip(3)));
                        continue;
                    }

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
                    await ResendMessageAsync(message);
                    yield return message;
                }
            }
        }

        public async Task SendMessageAsync(string text)
        {
            if (IsCommand(text))
            {
                HandleCommand(text);
            }
            else
            {
                if(text.Length == 0) { return; }
                await ResendMessageAsync($"{this.Name}: {text}{Environment.NewLine}");
            }
        }

        public async Task ResendMessageAsync(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
            {
                IPEndPoint endPoint = IPStorage.AdminEndPoint;
                client.Connect(endPoint);
                await client.SendAsync(buffer, buffer.Length);
            }
            using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
            {
                IPEndPoint endPoint = IPStorage.MultiCastEndPoint;
                IPAddress dest = endPoint.Address;
                client.JoinMulticastGroup(dest, 2);
                client.Connect(endPoint);
                await client.SendAsync(buffer, buffer.Length);
            }
        }

        public async Task ReceiveSettingsAsync()
        {
            while (true)
            {
                using (UdpClient client = new UdpClient(IPStorage.AdminSettings))
                {
                    client.Reuse();
                    string message = "";

                    await Task.Run(() =>
                    {
                        var endPoint = IPStorage.ReceiveEndPoint.Clone();
                        byte[] result = client.Receive(ref endPoint);
                        message = Encoding.UTF8.GetString(result);
                    });

                    if (message.Length > 0)
                    {
                        if (message.StartsWith("REQUEST CONNECTION"))
                        {
                            string str = message.Substring(19);
                            string name = str.Split(' ')[0];
                            string password = str.Split(' ')[1];
                            if (BanList.Any(x => x.Name == name))
                            {
                                var user = BanList.First(x => x.Name == name);
                                if (!user.BanStatus())
                                {
                                    BanList.Remove(user);
                                }
                                else
                                {
                                    HandleCommand($"/kick {name}");
                                }
                            }
                            ChatDBContext db = new ChatDBContext();
                            DBUser? tmpUser = db.Users.FirstOrDefault(x => x.Name == name);
                            if (tmpUser != null)
                            {
                                if(tmpUser.Password != password)
                                {
                                    HandleCommand($"/kick {name}");
                                }
                            }
                            else
                            {
                                HandleCommand($"/kick {name}");
                            }
                        }

                        if (message.StartsWith("REGISTER"))
                        {
                            string str = message.Substring(9);
                            string name = str.Split(' ')[0];
                            string password = str.Split(' ')[1];
                            ChatDBContext db = new ChatDBContext();
                            if(db.Users.Any(x => x.Name == name))
                            {
                                HandleCommand($"/kick {name}");
                            }
                            else
                            {
                                if(Form1.IsValidUserName(name) && Form1.IsValidPassword(password))
                                {
                                    db.Users.Add(new DBUser { Name = name, Password = password });
                                    db.SaveChanges();
                                }
                                else
                                {
                                    HandleCommand($"/kick {name}");
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task SendSettingsAsync(string setting)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(setting);
            using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
            {
                IPEndPoint endPoint = IPStorage.MultiCastSettings;
                IPAddress dest = endPoint.Address;
                client.JoinMulticastGroup(dest, 2);
                client.Connect(endPoint);
                await client.SendAsync(buffer, buffer.Length);
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

            HandleCommand(command3[0], command3.Skip(1).ToArray());
        }

        public void HandleCommand(string commandName, params string[] commandArgs)
        {
            Commands.First(x => x.CommandName == commandName && x.ArgumentCount == commandArgs.Count()).Invoke(commandArgs);
        }
    }
}
