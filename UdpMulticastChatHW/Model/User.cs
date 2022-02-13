using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.Model
{
    public class User : IUser
    {
        public string Name { get; set; }
        public bool IsKicked { get; set; } = false;

        public User(User user) : this(user.Name) { }

        public User(string name)
        {
            Name = name;
        }

        public User(string name, string pass, LogType logType = LogType.LogIn)
        {
            Name = name;
            if (logType == LogType.LogIn)
            {
                Task.Run(async () =>
                {
                    await this.SendSettingsAsync($"REQUEST CONNECTION {this.Name} {pass}");
                });
            }
            else if(logType == LogType.Register)
            {
                Task.Run(async () =>
                {
                    await this.SendSettingsAsync($"REGISTER {this.Name} {pass}");
                });
            }
        }

        public enum LogType
        {
            LogIn,
            Register
        }

        public async IAsyncEnumerable<string> ReceiveMessageAsync()
        {
            while (true)
            {
                using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
                {
                    IPEndPoint endPoint = IPStorage.ReceiveEndPoint;
                    client.Reuse();
                    client.Client.Bind(endPoint);
                    IPAddress address = IPStorage.MultiCastEndPoint.Address;
                    client.JoinMulticastGroup(address);
                    UdpReceiveResult result = await client.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(result.Buffer);
                    if (message.StartsWith("PRIVATE"))
                    {
                        string[] strs = message.Split(' ');
                        string sender = strs[1];
                        string receiver = strs[2];
                        if (this.Name == sender || this.Name == receiver) {
                            string msg = string.Join(' ', strs.Skip(3));
                            message = $"[{sender} -> {receiver}] {msg}";
                        }
                        else
                        {
                            continue;
                        }
                    }
                    yield return message;
                }
            }
        }

        public async Task SendMessageAsync(string text)
        {
            if(text.Length == 0) { return; }
            if (text.StartsWith('/'))
            {
                string[] strs = text.Split(' ');
                string str = strs[0] + ' ';
                str += this.Name;
                for (int i = 1; i < strs.Length; i++)
                {
                    str += ' ';
                    str += strs[i];
                }
                text = str;
            }
            else
            {
                text = $"{this.Name}: {text}{Environment.NewLine}";
            }
            using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
            {
                IPEndPoint endPoint = IPStorage.AdminEndPoint;
                client.Connect(endPoint);
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                await client.SendAsync(buffer, buffer.Length);
            }
        }

        public async Task ReceiveSettingsAsync()
        {
            while (true)
            {
                using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
                {
                    IPEndPoint endPoint = IPStorage.ReceiveEndPoint;
                    client.Reuse();
                    client.Client.Bind(endPoint);
                    IPAddress address = IPStorage.MultiCastSettings.Address;
                    client.JoinMulticastGroup(address);
                    UdpReceiveResult result = await client.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(result.Buffer);

                    if (message.Length > 0)
                    {
                        if (message.StartsWith("KICK"))
                        {
                            string name = message.Substring(5);
                            if(name == this.Name)
                            {
                                IsKicked = true;
                            }
                        }
                    }
                }
            }
        }

        public async Task SendSettingsAsync(string setting)
        {
            using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
            {
                IPEndPoint endPoint = IPStorage.AdminSettings;
                client.Connect(endPoint);
                byte[] buffer = Encoding.UTF8.GetBytes(setting);
                await client.SendAsync(buffer, buffer.Length);
            }
        }
    }
}
