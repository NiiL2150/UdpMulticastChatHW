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

        public User(string name)
        {
            Name = name;
        }
        public User(User user)
        {
            this.Name = user.Name;
        }

        public async Task SendAsync(string text)
        {
            using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
            {
                IPAddress dest = IPAddress.Parse("127.0.0.1");
                IPEndPoint endPoint = new IPEndPoint(dest, 51234);
                client.Connect(endPoint);
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                await client.SendAsync(buffer, buffer.Length);
                client.Close();
            }
        }

        public async IAsyncEnumerable<string> ReceiveAsync()
        {
            while (true)
            {
                using (UdpClient client = new UdpClient(AddressFamily.InterNetwork))
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 51234);
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    client.Client.Bind(endPoint);
                    IPAddress address = IPAddress.Parse("224.5.5.5");
                    client.JoinMulticastGroup(address);
                    UdpReceiveResult result = await client.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(result.Buffer);
                    yield return message;
                }
            }
        }
    }
}
